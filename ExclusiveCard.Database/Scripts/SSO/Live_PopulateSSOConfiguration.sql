--## Script to add SSO Configuration value for blackhawk SAML##

--NOTE: At time of writing, we do not have all the details from blackhawk to complete this script
--The missing details are:
--@DestinationUrl
--@ClientId
--the consumer service location url (part of @metadata)
--the signout service response url's (part of @metadata)


--DECLARE @Id int
DECLARE @Name nvarchar(128)
DECLARE @DestinationUrl nvarchar(max)
DECLARE @ClientId nvarchar(max)
DECLARE @Metadata nvarchar(max)
DECLARE @Certificate nvarchar(max)
DECLARE @Issuer nvarchar(max)

--SELECT @Id = 1
SELECT @Name ='Blackhawk'
SELECT @DestinationUrl = 'https://app.employeebenefitschoice.com/saml2/login' --<< [BLACKHAWK'S LIVE SITE LOGIN URL]
SELECT @ClientId = 'C0F1938D-45B2-404F-803F-4830C9BD0F7D' --<< [BLACKHAWK'S LIVE CLIENTID FOR EXCLUSIVE]

SELECT @Metadata ='<?xml version="1.0" encoding="utf-8"?>
<q1:EntityDescriptor entityID="https://employeebenefitschoice.com" validUntil="2021-10-08T10:36:55.4886355Z" ID="id466a1d1252394b77835a70cc6b6cc0e0" xmlns:q1="urn:oasis:names:tc:SAML:2.0:metadata"><Signature xmlns="http://www.w3.org/2000/09/xmldsig#"><SignedInfo><CanonicalizationMethod Algorithm="http://www.w3.org/2001/10/xml-exc-c14n#" /><SignatureMethod Algorithm="http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" /><Reference URI="#id466a1d1252394b77835a70cc6b6cc0e0"><Transforms><Transform Algorithm="http://www.w3.org/2000/09/xmldsig#enveloped-signature" /><Transform Algorithm="http://www.w3.org/2001/10/xml-exc-c14n#" /></Transforms><DigestMethod Algorithm="http://www.w3.org/2001/04/xmlenc#sha256" /><DigestValue>dZQogq5lyhzmvWToEP9MbNIwqEKVqnozu5ypOfc+nFw=</DigestValue></Reference></SignedInfo><SignatureValue>cvrv823i3dTYlBjJyo/EseAWuhreIDLtQzbUEA7sUYXFZbBhQ0aV9jTMbblWAS7QMk5UsqhZ2/fFHizH5snISKIPY2/HSY+bMkEnV9vumQ5+33A2JwpNaIKBUj4EPkZpn2CxPORP+9fBhvcdRgjSkSROiji49f0LtdtuWYuhIgi2OebMOvlCS/o0/fOlP6/tI1AxKa7fGMStoTGqLvjFq+JdW4VkifruDWRY8jt1BxOITf3jzsfRnVoLaumjf2eP2bNZTPHzbisXGJX9baZnUOiIwdzeAlzOW5vQorB5mgpRDFG/xSEi7jugSTD6pR9LpXoQQqncOLqs9XeTVG7GTnn5Tl7ACn1itCUvVV46kShiLVX3P1kZ5WetO7/P0/s6cYvwmyYgbjEM1+R1XlJLnkDabnHFneTNZoFF8eDJqcYI5xJX09Rb6idNw/Rr8iKttgz6UdZmK0WkWBQ75Sp2qq5jXCt6522SEIhWDBd5rN3s3rv8jhMARtCLtsT0IsXOwFwL6hnTg0AUtahM4H6KmOANLlVNZGS4FCCDzTSNrK8pZveErUYqDzvy+0Hp/blO8rgPwRoc0E9q3svCyF0dcvjVWc36lE+iiafIA6aOxwNRdGXxSq3Ycys1NMbq62qfW6xAOcc+XyZMDhCflrbECHl77AlMrSqkU+nwT6nN58o=</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIEyDCCArCgAwIBAgIQVRBFDKP9bZpHcUSSTGxz0TANBgkqhkiG9w0BAQUFADAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwHhcNMTgwNTA5MjMwMDAwWhcNMjgwNTE2MjMwMDAwWjAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDGAWwYVC4ycx+Y8ltkin0QNISqYA9tkxOl15YpGm7gcgmkQaoe1cALV9S7E/xDRwfThfV75qQEe6miB63p1Grb591062yvUbu2wQfPJiBaHLDPWfgtN7kRwO2cZ8H9lIAr8vp6r6dtryLDqMjKoG53/mrB4JL87fa2GSsgji7NZAJAgr0ScrnDOp2r54FRpv4glZd1lt8jZr4JTN9MpBB/VZ7g6eGJmwyq+uM4O+xJXWRE5utCbg5EdMsMraT6h8vIM8r68GPr7F1AJfHhwqCxleT9U+Rus7ZyDFtYnId9wTUeyue1Ep/HktcWzT+ZbOYe4m3y/fIHKG/+P4qZhJPbeHna5BxLpWL//JcIoPoTgu/yzTXSHiNHY6KP5sUlG8sH5VIILJ8FbVrcRDvBGg8NLXG3ezozFy+9IS7flWRKa1b3rYUnSacv61B7nhWncLzuyqYiQXUQv1DbJcdIOZ+LdBDGKHYt38SxC6sgVQKVum21d9EpD9sVHLYZAWiDe+LPaeFo6dcDRS0QPdmLDd+HS65tdkgzmDLzWkjN0zJ++rD1DNxvDH+FyDQsF85mrzU2aeZqKJdolCe5wP5SGTOl1br4BWoHhQwqUYFacV5gweJOBnImc07dnHkl6BkIIblLgdzmFhUWnRFtc+1zscM3VBPdC4YqwYZopW9gkTn6YQIDAQABMA0GCSqGSIb3DQEBBQUAA4ICAQCj4cdlLNbVxrW6gNUQiMCAwmUtY4tRWjFqNUD29bhGSJpiqgo0+DvHn9hMi6/H3Qekkbv9AuFTwqxuvrL64asG4ahFQegyqQxzsK81fg0ShVU5i2BUFBMlizEoAnDyrFzDmOU6cW0g1z1/zbnbNAABkeJ0EM6+cJ9tKYKOlP1r2iYjPQjLPBAP6TIRoJEECbNdcKe7LrRpr0eN+5yIlvHBZWmPuIanU/aDJMjuwr104PYQs85FSoyOAYF4DqThYmAqru7POQLqE7waSqcEYbPdQBGdr2pkqC7ugpNBUYBBGYG+LrLQFP/jj6sqnqdh/o+njmxF660OUVWtTzyXlxL+Cu1cBU0j2hpJgO4aNF89FQAwsWyYHXq5xYwvXgsfmifib0prp/UzzS/eSQ7so8Q4FUmnsjZ7LedtJsuIZsA7CTuRx4p1ogJQBtDvQfcHdFgwCL00n8Qxyr5dceBZAyQ4gFErNylAMTSJaZ05lY+vxxiv8w+8tN/2r7+MA6zP5x9a5Skk54hEwJkgACjTByVHp/DmuVTpdJEjfaLYQdoMCvyhN4wALy10VL9f0WMb90xLImXI5TkWlnnY8fLcRiA9t3D8fIulUeuqa9akFkpzPPDdqzMfEXe+xsaR8zR2Ee7TyH/nfoTXPGLDTDWOS7YaLtSVP8yJqN0qBE0JCkn3ug==</X509Certificate></X509Data></KeyInfo></Signature>
  <q1:SPSSODescriptor protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol" AuthnRequestsSigned="true" WantAssertionsSigned="true">
    <q1:KeyDescriptor use="signing">
      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
        <X509Data>
          <X509Certificate>MIIEyDCCArCgAwIBAgIQVRBFDKP9bZpHcUSSTGxz0TANBgkqhkiG9w0BAQUFADAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwHhcNMTgwNTA5MjMwMDAwWhcNMjgwNTE2MjMwMDAwWjAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDGAWwYVC4ycx+Y8ltkin0QNISqYA9tkxOl15YpGm7gcgmkQaoe1cALV9S7E/xDRwfThfV75qQEe6miB63p1Grb591062yvUbu2wQfPJiBaHLDPWfgtN7kRwO2cZ8H9lIAr8vp6r6dtryLDqMjKoG53/mrB4JL87fa2GSsgji7NZAJAgr0ScrnDOp2r54FRpv4glZd1lt8jZr4JTN9MpBB/VZ7g6eGJmwyq+uM4O+xJXWRE5utCbg5EdMsMraT6h8vIM8r68GPr7F1AJfHhwqCxleT9U+Rus7ZyDFtYnId9wTUeyue1Ep/HktcWzT+ZbOYe4m3y/fIHKG/+P4qZhJPbeHna5BxLpWL//JcIoPoTgu/yzTXSHiNHY6KP5sUlG8sH5VIILJ8FbVrcRDvBGg8NLXG3ezozFy+9IS7flWRKa1b3rYUnSacv61B7nhWncLzuyqYiQXUQv1DbJcdIOZ+LdBDGKHYt38SxC6sgVQKVum21d9EpD9sVHLYZAWiDe+LPaeFo6dcDRS0QPdmLDd+HS65tdkgzmDLzWkjN0zJ++rD1DNxvDH+FyDQsF85mrzU2aeZqKJdolCe5wP5SGTOl1br4BWoHhQwqUYFacV5gweJOBnImc07dnHkl6BkIIblLgdzmFhUWnRFtc+1zscM3VBPdC4YqwYZopW9gkTn6YQIDAQABMA0GCSqGSIb3DQEBBQUAA4ICAQCj4cdlLNbVxrW6gNUQiMCAwmUtY4tRWjFqNUD29bhGSJpiqgo0+DvHn9hMi6/H3Qekkbv9AuFTwqxuvrL64asG4ahFQegyqQxzsK81fg0ShVU5i2BUFBMlizEoAnDyrFzDmOU6cW0g1z1/zbnbNAABkeJ0EM6+cJ9tKYKOlP1r2iYjPQjLPBAP6TIRoJEECbNdcKe7LrRpr0eN+5yIlvHBZWmPuIanU/aDJMjuwr104PYQs85FSoyOAYF4DqThYmAqru7POQLqE7waSqcEYbPdQBGdr2pkqC7ugpNBUYBBGYG+LrLQFP/jj6sqnqdh/o+njmxF660OUVWtTzyXlxL+Cu1cBU0j2hpJgO4aNF89FQAwsWyYHXq5xYwvXgsfmifib0prp/UzzS/eSQ7so8Q4FUmnsjZ7LedtJsuIZsA7CTuRx4p1ogJQBtDvQfcHdFgwCL00n8Qxyr5dceBZAyQ4gFErNylAMTSJaZ05lY+vxxiv8w+8tN/2r7+MA6zP5x9a5Skk54hEwJkgACjTByVHp/DmuVTpdJEjfaLYQdoMCvyhN4wALy10VL9f0WMb90xLImXI5TkWlnnY8fLcRiA9t3D8fIulUeuqa9akFkpzPPDdqzMfEXe+xsaR8zR2Ee7TyH/nfoTXPGLDTDWOS7YaLtSVP8yJqN0qBE0JCkn3ug==</X509Certificate>
        </X509Data>
      </KeyInfo>
    </q1:KeyDescriptor>
    <q1:KeyDescriptor use="encryption">
      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
        <X509Data>
          <X509Certificate>MIIEyDCCArCgAwIBAgIQVRBFDKP9bZpHcUSSTGxz0TANBgkqhkiG9w0BAQUFADAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwHhcNMTgwNTA5MjMwMDAwWhcNMjgwNTE2MjMwMDAwWjAgMR4wHAYDVQQDExVlbXBsb3llZWJlbmVmaXRzLWxpdmUwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDGAWwYVC4ycx+Y8ltkin0QNISqYA9tkxOl15YpGm7gcgmkQaoe1cALV9S7E/xDRwfThfV75qQEe6miB63p1Grb591062yvUbu2wQfPJiBaHLDPWfgtN7kRwO2cZ8H9lIAr8vp6r6dtryLDqMjKoG53/mrB4JL87fa2GSsgji7NZAJAgr0ScrnDOp2r54FRpv4glZd1lt8jZr4JTN9MpBB/VZ7g6eGJmwyq+uM4O+xJXWRE5utCbg5EdMsMraT6h8vIM8r68GPr7F1AJfHhwqCxleT9U+Rus7ZyDFtYnId9wTUeyue1Ep/HktcWzT+ZbOYe4m3y/fIHKG/+P4qZhJPbeHna5BxLpWL//JcIoPoTgu/yzTXSHiNHY6KP5sUlG8sH5VIILJ8FbVrcRDvBGg8NLXG3ezozFy+9IS7flWRKa1b3rYUnSacv61B7nhWncLzuyqYiQXUQv1DbJcdIOZ+LdBDGKHYt38SxC6sgVQKVum21d9EpD9sVHLYZAWiDe+LPaeFo6dcDRS0QPdmLDd+HS65tdkgzmDLzWkjN0zJ++rD1DNxvDH+FyDQsF85mrzU2aeZqKJdolCe5wP5SGTOl1br4BWoHhQwqUYFacV5gweJOBnImc07dnHkl6BkIIblLgdzmFhUWnRFtc+1zscM3VBPdC4YqwYZopW9gkTn6YQIDAQABMA0GCSqGSIb3DQEBBQUAA4ICAQCj4cdlLNbVxrW6gNUQiMCAwmUtY4tRWjFqNUD29bhGSJpiqgo0+DvHn9hMi6/H3Qekkbv9AuFTwqxuvrL64asG4ahFQegyqQxzsK81fg0ShVU5i2BUFBMlizEoAnDyrFzDmOU6cW0g1z1/zbnbNAABkeJ0EM6+cJ9tKYKOlP1r2iYjPQjLPBAP6TIRoJEECbNdcKe7LrRpr0eN+5yIlvHBZWmPuIanU/aDJMjuwr104PYQs85FSoyOAYF4DqThYmAqru7POQLqE7waSqcEYbPdQBGdr2pkqC7ugpNBUYBBGYG+LrLQFP/jj6sqnqdh/o+njmxF660OUVWtTzyXlxL+Cu1cBU0j2hpJgO4aNF89FQAwsWyYHXq5xYwvXgsfmifib0prp/UzzS/eSQ7so8Q4FUmnsjZ7LedtJsuIZsA7CTuRx4p1ogJQBtDvQfcHdFgwCL00n8Qxyr5dceBZAyQ4gFErNylAMTSJaZ05lY+vxxiv8w+8tN/2r7+MA6zP5x9a5Skk54hEwJkgACjTByVHp/DmuVTpdJEjfaLYQdoMCvyhN4wALy10VL9f0WMb90xLImXI5TkWlnnY8fLcRiA9t3D8fIulUeuqa9akFkpzPPDdqzMfEXe+xsaR8zR2Ee7TyH/nfoTXPGLDTDWOS7YaLtSVP8yJqN0qBE0JCkn3ug==</X509Certificate>
        </X509Data>
      </KeyInfo>
    </q1:KeyDescriptor>
    <q1:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://app.employeebenefitschoice.com/saml2/logout" ResponseLocation="https://app.employeebenefitschoice.com/saml2/logout" />
    <q1:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" Location="https://app.employeebenefitschoice.com/saml2/logout" ResponseLocation="https://app.employeebenefitschoice.com/saml2/logout" />
    <q1:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</q1:NameIDFormat>
    <q1:AssertionConsumerService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://app.employeebenefitschoice.com/saml2/login" index="0" isDefault="true" />
    <q1:AttributeConsumingService index="0" isDefault="true">
      <q1:ServiceName xml:lang="da">SP</q1:ServiceName>
      <q1:RequestedAttribute Name="email" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="reference" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="title" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="firstname" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="lastname" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="addressLine1" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="addressLine2" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="addressLine3" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="town" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="county" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="postcode" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="clientId" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="externalId" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="schemeCode" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="productCode" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
      <q1:RequestedAttribute Name="externalClientId" NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:basic" />
    </q1:AttributeConsumingService>
  </q1:SPSSODescriptor>
</q1:EntityDescriptor>'
SELECT @Certificate ='ExclusiveMedia-Live'
SELECT @Issuer = 'https://exclusiverewards.co.uk/'

INSERT INTO [Exclusive].[SSOConfiguration]
           ([Name] ,[DestinationUrl],[ClientId] ,[Metadata] ,[Certificate],[Issuer])
     VALUES
           (@Name, @DestinationUrl, @ClientId, @Metadata, @Certificate, @Issuer)
