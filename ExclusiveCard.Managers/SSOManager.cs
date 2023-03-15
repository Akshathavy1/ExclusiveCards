using AutoMapper;
using dk.nita.saml20;
using dk.nita.saml20.Bindings.SignatureProviders;
using dk.nita.saml20.config;
using dk.nita.saml20.Schema.Core;
using dk.nita.saml20.Schema.Metadata;
using dk.nita.saml20.Schema.Protocol;
using dk.nita.saml20.Utils;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using db = ExclusiveCard.Data.Models;
using NLog;

using dto = ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace ExclusiveCard.Managers
{
    public class SSOManager : ISSOManager
    {
        #region Private fields

        private IRepository<db.SSOConfiguration> _ssoRepo = null;
        private IKeyVaultManager _keyVaultManager = null;
        private IConfiguration _configuration;

        private readonly IMapper _mapper = null;
        private readonly ILogger _logger;

        #endregion Private fields

        #region Constructor

        public SSOManager(IRepository<db.SSOConfiguration> ssoRepo, IKeyVaultManager keyVaultManager, IMapper mapper, IConfiguration configuration)
        {
            _configuration = configuration;
            _mapper = mapper;
            _ssoRepo = ssoRepo;
            _keyVaultManager = keyVaultManager;

            _logger = LogManager.GetLogger(GetType().FullName);
            //_logger = LogManager.GetLogger("databaseLogger");

        }

        #endregion Constructor

        #region Public methods

        ///<see cref="ISSOManager.GetAllSSOConfigurations()"/>
        public async Task<List<SSOConfiguration>> GetAllSSOConfigurations()
        {
            var ssoList = _ssoRepo.Filter().ToList();
            var ssoConfigurations = _mapper.Map<List<dto.SSOConfiguration>>(ssoList);

            //Not sure why it was made async, but this will remove the warnings...
            await Task.CompletedTask;
            return ssoConfigurations;
        }

        ///<see cref="ISSOManager.GetSSOConfiguration(int)"/>
        public async Task<SSOConfiguration> GetSSOConfiguration(int ssoConfigId)
        {
            dto.SSOConfiguration ssoConfiguration = null;

            var dbSSOConfiguration = _ssoRepo.GetById(ssoConfigId);
            if (dbSSOConfiguration != null)
            {
                ssoConfiguration = _mapper.Map<dto.SSOConfiguration>(dbSSOConfiguration);
            }

            //Not sure why it was made async, but this will remove the warnings...
            await Task.CompletedTask;
            return ssoConfiguration;
        }

        public async Task<string> ProcessSSO(int ssoConfigId, Customer customer, string email, string productCode = null)
        {
            var ssoConfiguration = await GetSSOConfiguration(ssoConfigId);
            string certificateThumbprint = ssoConfiguration?.Certificate;
            var issuer = ssoConfiguration?.Issuer;
            var attributes = GetAttributes(ssoConfiguration.Id, customer, email, ssoConfiguration?.ClientId, productCode);
            var metadata = ssoConfiguration?.Metadata;
            var acsURL = ssoConfiguration?.DestinationUrl;

            _logger.Debug($"cert: {certificateThumbprint} config id: {ssoConfigId} has config: {(ssoConfiguration is null ? false : true)}");

            X509Certificate2 certificate = await GetCertificate(certificateThumbprint);

            if(certificate is null)
            { throw new Exception("Unable to find certificate"); }

            return await CreateAssertionAsync(issuer, metadata, acsURL, certificate, false, true, true, attributes);

        }

        public async Task<X509Certificate2> GetCertificate(string nameOrThumbprint, bool validOnly = true)
        {
            X509Certificate2 cert = null;

            //Try from Vault
            if (cert is null)
                cert = await GetCertFromKeyVault(nameOrThumbprint);

            //Try from key store
            //if (cert is null)
            //    foreach (StoreName store in Enum.GetValues(typeof(StoreName)))
            //    {
            //        foreach (StoreLocation location in Enum.GetValues(typeof(StoreLocation)))
            //        {
            //            cert = GetCertFromStore(store, location, nameOrThumbprint, validOnly);
            //            if (cert != null)
            //                return cert;
            //        }
            //    }

            //Try from file
            //if (cert is null)
            //    cert = GetCertFromFile();

            if (cert is null)
                throw new Exception($"Certificate {nameOrThumbprint} was not found");

            return cert;

            // Consider to call Dispose() on the certificate after it's being used, avaliable in .NET 4.6 and later
        }

        public async Task<string> GetIDPMetadata(int ssoConfigId)
        {
            var ssoConfiguration = await GetSSOConfiguration(ssoConfigId);
            string certificateThumbprint = ssoConfiguration?.Certificate;
            X509Certificate2 certificate = await GetCertificate(certificateThumbprint);
            //byte[] cert = certificate.Export(X509ContentType.Cert);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
            sb.AppendLine(@$"<md:EntityDescriptor entityID = ""{ssoConfiguration.Issuer}"" xmlns:md = ""urn:oasis:names:tc:SAML:2.0:metadata"">");
            sb.AppendLine(@"<md:IDPSSODescriptor WantAuthnRequestsSigned = ""false"" protocolSupportEnumeration = ""urn:oasis:names:tc:SAML:2.0:protocol"">");

            sb.AppendLine(@"<md:KeyDescriptor use=""signing"">");
            sb.AppendLine(@"<ds:KeyInfo xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">");
            sb.AppendLine(@"<ds:X509Data>");
            sb.AppendLine(@"<ds:X509Certificate>");

            //NEED TO ADD KEY HERE 
            if(certificate is null)
                sb.AppendLine(@"## NO CERTIFICATE FOUND ##");
            else
                sb.AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks));

            sb.AppendLine(@"</ds:X509Certificate>");
            sb.AppendLine(@"</ds:X509Data>");
            sb.AppendLine(@"</ds:KeyInfo>");
            sb.AppendLine(@"</md:KeyDescriptor>");

            sb.AppendLine(@"<md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</md:NameIDFormat>");
            sb.AppendLine(@"<md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress</md:NameIDFormat>");
            sb.AppendLine(@$"<md:SingleSignOnService Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"" Location=""{ssoConfiguration.Issuer}/sso/saml""/>");
            sb.AppendLine(@$"<md:SingleSignOnService Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"" Location=""{ssoConfiguration.Issuer}/sso/saml""");
            sb.AppendLine(@"</md:IDPSSODescriptor>");
            sb.AppendLine(@"</md:EntityDescriptor>");

            return sb.ToString();

        }

        #endregion Public methods

        #region Private methods

        private async Task<X509Certificate2> GetCertFromKeyVault(string CertificateName)
        {
            X509Certificate2 cert = null;
            try
            {
                cert = await _keyVaultManager.GetCertificate(CertificateName);
            }
            catch(System.Exception ex)
            {
                _logger.Debug(ex, $"Unable to find cert {CertificateName} in key vault");
            }
            return cert;
        }

        private X509Certificate2 GetCertFromStore(StoreName store, StoreLocation locaton, string thumbprint, bool validOnly = true)
        {
            X509Certificate2 cert = null;
            try
            {
                using (X509Store certStore = new X509Store(store, locaton))
                {
                    certStore.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                                X509FindType.FindByThumbprint,
                                                // Replace below with your certificate's thumbprint
                                                thumbprint,
                                                validOnly);
                    // Get the first cert with the thumbprint
                    cert = certCollection.OfType<X509Certificate2>().FirstOrDefault();
                    if (cert is null)
                    {
                        X509Certificate2Collection test = certStore.Certificates;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Unable to find Cert {thumbprint}");
                        sb.AppendLine(">>");
                        foreach (X509Certificate2 x in test)
                        {
                            sb.AppendLine(x.Thumbprint);
                        }
                        sb.AppendLine("<<");
                        _logger.Debug(new Exception(sb.ToString()), $"Unable to find cert {thumbprint} in key store");
                    }
                    // Consider to call Dispose() on the certificate after it's being used, avaliable in .NET 4.6 and later
                }
            }
            catch (System.Exception ex)
            {
                _logger.Debug(ex, $"Unable to find cert {thumbprint} in key vault");
            }

            return cert;
        }

        private X509Certificate2 GetCertFromFile()
        {
            X509Certificate2 certificate = null;

            //Certificate in appsettings
            var certificateFilePath = _configuration[$"SAML2:SigningCertificateFile"];
            var certificatePassword = _configuration[$"SAML2:SigningCertificatePassword"];
            try
            {
                if (!File.Exists(certificateFilePath))
                {
                    _logger.Error($"File not found {certificateFilePath}");
                    return null;
                }

                certificate = new X509Certificate2(certificateFilePath, certificatePassword);
            }
            catch (System.Exception ex)
            {
                _logger.Debug(ex, $"Unable to find cert {certificateFilePath}");
            }

            return certificate;
        }

        private Dictionary<string, string> GetAttributes(int SSOConfigurationId, Customer customer, string email, string clientId, string productCode = null)
        {
            //This is just Blackhawk, it needs changing to pick up the attributes from a table based on the SSOConfigurationId passed in
            var attributes = new Dictionary<string, string> {
                { "email", email},
                { "reference", customer?.AspNetUserId},
                { "firstname", customer?.Forename},
                { "lastname", customer?.Surname},
                { "addressLine1", "." },
                { "town", "." },
                { "postcode", "." },
                { "clientId", clientId},
                { "externalId", "192735a312a"},
                { "schemeCode", "GC"},
                { "productCode" , productCode ?? "" }
            };

            return attributes;
        }

        private async Task<string> CreateAssertionAsync(string issuer, string metadata, string acsUrl, X509Certificate2 idpCert, bool signResponse, bool signAssertion, bool base64Encode, IDictionary<string, string> attributes = null)
        {
            var metadataDocument = ParseFile(metadata);
            var response = new Response();
            response.Destination = acsUrl;
            response.IssueInstant = Now;
            response.Status = new dk.nita.saml20.Schema.Protocol.Status();
            response.Status.StatusCode = new StatusCode();
            response.Status.StatusCode.Value = Saml20Constants.StatusCodes.Success;
            response.Version = "2.0";

            if (signResponse)
            {
                response.ID = "id" + Guid.NewGuid();
                NameID nameId = new NameID();
                nameId.Value = issuer;
                response.Issuer = nameId;
            }

            Assertion assertion = CreateAssertion(issuer, metadataDocument.EntityId, attributes);

            response.Items = new object[] { assertion };

            // Serialize the response.
            XmlDocument assertionDoc = new XmlDocument();
            assertionDoc.XmlResolver = null;
            assertionDoc.PreserveWhitespace = true;

            var xmlString = Serialization.SerializeToXmlString(response);

            assertionDoc.LoadXml(xmlString);

            // Sign the assertion inside the response message.
            if (signAssertion)
            {
                var signatureProvider = SignatureProviderFactory.CreateFromShaHashingAlgorithmName(ShaHashingAlgorithm.SHA256);
                signatureProvider.SignAssertion(assertionDoc, assertion.ID, idpCert);
            }
            else
            {
                var signedXml = Sign(assertionDoc, response.ID, idpCert);
                XmlNodeList nodes = assertionDoc.DocumentElement.GetElementsByTagName("Issuer");
                nodes[0].ParentNode.InsertAfter(assertionDoc.ImportNode(signedXml.GetXml(), true), nodes[0]);
            }

            var responseXml = assertionDoc.OuterXml;
            if (base64Encode)
            {
                responseXml = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseXml));
                _logger.Debug($"SAML Response: {responseXml}");
            }

            await Task.CompletedTask;
            return responseXml;
        }

        private static Assertion CreateAssertion(string issuer, string receiver, IDictionary<string, string> attributes = null)
        {
            Assertion assertion = new Assertion();

            // Subject element
            assertion.Subject = new Subject();
            assertion.ID = "id" + Guid.NewGuid();
            assertion.IssueInstant = Now;

            assertion.Issuer = new NameID();
            assertion.Issuer.Value = issuer;

            SubjectConfirmation subjectConfirmation = new SubjectConfirmation();
            subjectConfirmation.SubjectConfirmationData = new SubjectConfirmationData();
            subjectConfirmation.SubjectConfirmationData.Recipient = receiver;
            subjectConfirmation.SubjectConfirmationData.NotBefore = Now.AddMinutes(-1);
            subjectConfirmation.SubjectConfirmationData.NotOnOrAfter = Now.AddMinutes(90);

            subjectConfirmation.Method = SubjectConfirmation.BEARER_METHOD;

            NameID nameId = new NameID();
            nameId.Format = Saml20Constants.NameIdentifierFormats.Unspecified;
            nameId.Value = Guid.NewGuid().ToString();

            assertion.Subject.Items = new object[] { nameId, subjectConfirmation };

            // Conditions element
            assertion.Conditions = new Conditions();
            assertion.Conditions.Items = new List<ConditionAbstract>();

            assertion.Conditions.NotBefore = Now.AddMinutes(-1);
            assertion.Conditions.NotOnOrAfter = Now.AddHours(1);

            AudienceRestriction audienceRestriction = new AudienceRestriction();
            audienceRestriction.Audience = new List<string>();
            audienceRestriction.Audience.Add(receiver);
            assertion.Conditions.Items.Add(audienceRestriction);

            var statements = new List<StatementAbstract>(2);
            {
                // AuthnStatement element
                var authnStatement = new AuthnStatement();
                authnStatement.AuthnInstant = Now;
                authnStatement.SessionIndex = Convert.ToString(new Random().Next());

                authnStatement.AuthnContext = new AuthnContext();

                authnStatement.AuthnContext.Items =
                    new object[] { "urn:oasis:names:tc:SAML:2.0:ac:classes:X509" };

                authnStatement.AuthnContext.ItemsElementName =
                    new[] { ItemsChoiceType5.AuthnContextClassRef };

                statements.Add(authnStatement);
            }

            // Generate attribute list.
            var attributeStatement = new AttributeStatement();

            var attrs = new List<SamlAttribute>();

            foreach (var attr in attributes)
            {
                attrs.Add(new SamlAttribute
                {
                    Name = attr.Key,
                    AttributeValue = new[] { attr.Value },
                    NameFormat = SamlAttribute.NAMEFORMAT_BASIC
                });
            }

            attributeStatement.Items = attrs.ToArray();

            statements.Add(attributeStatement);

            assertion.Items = statements.ToArray();

            return assertion;
        }

        private static SignedXml Sign(XmlDocument doc, string id, X509Certificate2 cert)
        {
            SignedXml signedXml = new SignedXml(doc);

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;
            signedXml.SigningKey = cert.GetRSAPrivateKey();

            // Retrieve the value of the "ID" attribute on the root assertion element.
            Reference reference = new Reference("#" + id);
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.WholeChain));

            signedXml.ComputeSignature();

            return signedXml;
        }

        private static DateTime Now => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

        private Saml20MetadataDocument ParseFile(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            var doc = LoadFileAsXmlDocument(xml);
            try
            {
                foreach (XmlNode child in doc.ChildNodes)
                {
                    if (child.NamespaceURI == Saml20Constants.METADATA)
                    {
                        if (child.LocalName == EntityDescriptor.ELEMENT_NAME)
                            return new Saml20MetadataDocument(doc);

                        // TODO Decide how to handle several entities in one metadata file.
                        if (child.LocalName == EntitiesDescriptor.ELEMENT_NAME)
                            throw new NotImplementedException();
                    }
                }

                // No entity descriptor found.
                throw new InvalidDataException();
            }
            catch (Exception e)
            {
                // Probably not a metadata file.
                _logger.Error(e, "File Parse Issue");
                return null;
            }
        }

        private static XmlDocument LoadFileAsXmlDocument(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.PreserveWhitespace = true;

            doc.LoadXml(xml);

            if (XmlSignatureUtils.IsSigned(doc) && !XmlSignatureUtils.CheckSignature(doc))
                throw new InvalidOperationException("Invalid file signature");

            return doc;
        }

        #endregion Private methods
    }
}