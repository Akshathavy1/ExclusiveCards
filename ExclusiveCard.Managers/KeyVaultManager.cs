using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private IConfiguration _configuration;
        private readonly ILogger _logger;

        public KeyVaultManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = LogManager.GetLogger(GetType().FullName);
        }

        public async Task<X509Certificate2> GetCertificate(string certificateName)
        {

            string vaultUrl = _configuration["KeyVault:VaultUri"] ?? "";
            Uri vaultUri;
            if (!Uri.TryCreate(vaultUrl, UriKind.Absolute, out vaultUri))
                Uri.TryCreate($"https://{vaultUrl}.vault.azure.net/", UriKind.Absolute, out vaultUri);

            var o = new DefaultAzureCredentialOptions();
            o.VisualStudioTenantId = _configuration["KeyVault:AzureTenantId"] ?? "";
            DefaultAzureCredential credential = new DefaultAzureCredential(o);

            // Get the certificate to use for encrypting and decrypting.
            CertificateClient certificateClient = new CertificateClient(vaultUri, credential);
            KeyVaultCertificateWithPolicy certificate = await certificateClient.GetCertificateAsync(certificateName);

            if(certificate is null)
            {
                _logger.Error($@"Error: certificate ""{certificateName}"" not found");
                return null;
            }
            // Make sure the private key is exportable.
            else if (certificate.Policy?.Exportable != true)
            {
                _logger.Error($@"Error: certificate ""{certificateName}"" is not exportable.");
                return null;
            }
            else if (certificate.Policy?.KeyType != CertificateKeyType.Rsa)
            {
                _logger.Error($@"Error: certificate type ""{certificate.Policy?.KeyType}"" cannot be used to locally encrypt and decrypt.");
                return null;
            }
            //var certificate = await _certificateClient.DownloadCertificateAsync(certificateName);

            // Get the managed secret which contains the public and private key (if exportable).
            string secretName = ParseSecretName(certificate.SecretId);
            if(string.IsNullOrWhiteSpace(secretName))
            {
                _logger.Error($@"Error: certificate ""{certificateName}"" secret name not available.");
            }
            SecretClient secretClient = new SecretClient(vaultUri, credential);
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
            if (secret is null)
            {
                _logger.Error($@"Error: certificate ""{certificateName}"" secret not returned");
            }

            // Get a certificate pair from the secret value.
            X509Certificate2 pfx = ParseCertificate(secret);

            return pfx;
        }

        private string ParseSecretName(Uri secretId)
        {
            if (secretId.Segments.Length < 3)
            {
                throw new InvalidOperationException($@"The secret ""{secretId}"" does not contain a valid name.");
            }

            return secretId.Segments[2].TrimEnd('/');
        }

        private X509Certificate2 ParseCertificate(KeyVaultSecret secret)
        {
            if (string.Equals(secret.Properties.ContentType, CertificateContentType.Pkcs12.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                byte[] pfx = Convert.FromBase64String(secret.Value);
                return new X509Certificate2(pfx);
            }

            // For PEM, you'll need to extract the base64-encoded message body.
            // .NET 5.0 introduces the System.Security.Cryptography.PemEncoding class to make this easier.
            if (string.Equals(secret.Properties.ContentType, CertificateContentType.Pem.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                StringBuilder privateKeyBuilder = new StringBuilder();
                StringBuilder publicKeyBuilder = new StringBuilder();

                using StringReader reader = new StringReader(secret.Value);
                StringBuilder currentKeyBuilder = null;

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Equals("-----BEGIN PRIVATE KEY-----", StringComparison.OrdinalIgnoreCase))
                    {
                        currentKeyBuilder = privateKeyBuilder;
                    }
                    else if (line.Equals("-----BEGIN CERTIFICATE-----", StringComparison.OrdinalIgnoreCase))
                    {
                        currentKeyBuilder = publicKeyBuilder;
                    }
                    else if (line.StartsWith("-----", StringComparison.Ordinal))
                    {
                        currentKeyBuilder = null;
                    }
                    else if (currentKeyBuilder is null)
                    {
                        throw new InvalidOperationException("Invalid PEM-encoded certificate.");
                    }
                    else
                    {
                        currentKeyBuilder.Append(line);
                    }

                    line = reader.ReadLine();
                }

                string privateKeyBase64 = privateKeyBuilder?.ToString() ?? throw new InvalidOperationException("No private key found in certificate.");
                string publicKeyBase64 = publicKeyBuilder?.ToString() ?? throw new InvalidOperationException("No public key found in certificate.");

                byte[] privateKey = Convert.FromBase64String(privateKeyBase64);
                byte[] publicKey = Convert.FromBase64String(publicKeyBase64);

                X509Certificate2 certificate = new X509Certificate2(publicKey);

                using RSA rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(privateKey, out _);

                return certificate.CopyWithPrivateKey(rsa);
            }

            throw new NotSupportedException($@"Certificate encoding ""{secret.Properties.ContentType}"" is not supported.");
        }
    }
}
