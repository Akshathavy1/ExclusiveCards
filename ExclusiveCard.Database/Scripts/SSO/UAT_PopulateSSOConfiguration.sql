--## Script to add SSO Configuration value for blackhawk SAML##
--DECLARE @Id int
DECLARE @Name nvarchar(128)
DECLARE @DestinationUrl nvarchar(max)
DECLARE @ClientId nvarchar(max)
DECLARE @Metadata nvarchar(max)
DECLARE @Certificate nvarchar(max)
DECLARE @Issuer nvarchar(max)

--SELECT @Id = 1
SELECT @Name ='Blackhawk'
SELECT @DestinationUrl ='https://benefits-ebc-uat.grgcloud.net/saml2/login'
SELECT @ClientId ='934914BD-39BF-4503-9CB6-AD7091802A93'
SELECT @Metadata ='<?xml version="1.0" encoding="utf-8"?>
<q1:EntityDescriptor xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
                     xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                     entityID="https://benefits-ebc-uat.grgcloud.net" 
                     ID="_a9d4482e-798d-4389-b3b3-d7eebf316caf" 
                     xmlns:q1="urn:oasis:names:tc:SAML:2.0:metadata">
  <q1:SPSSODescriptor protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol" AuthnRequestsSigned="true" WantAssertionsSigned="true">
    <q1:KeyDescriptor use="signing">
      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
        <X509Data>
          <X509Certificate>MIIEfTCCA2WgAwIBAgIHFkRANXGitDANBgkqhkiG9w0BAQsFADCBqjELMAkGA1UEBhMCR0IxEzARBgNVBAgMCk1lcnNleXNpZGUxEjAQBgNVBAcMCVNvdXRocG9ydDEaMBgGA1UECgwRRXhjbHVzaXZlIFJld2FyZHMxDzANBgNVBAsMBkRldk9wczEeMBwGA1UEAwwVZXhjbHVzaXZlcmV3YXJkc2NvLnVrMSUwIwYJKoZIhvcNAQkBFhZzdXBwb3J0QHJlYWNoYXdhcmUuY29tMB4XDTIyMDIwOTEwNDYxMVoXDTIzMDIxMDEwNDYxMVowgasxCzAJBgNVBAYTAkdCMRMwEQYDVQQIDApNZXJzZXlzaWRlMRIwEAYDVQQHDAlTb3V0aHBvcnQxGjAYBgNVBAoMEUV4Y2x1c2l2ZSBSZXdhcmRzMQ8wDQYDVQQLDAZEZXZPcHMxHzAdBgNVBAMMFmV4Y2x1c2l2ZXJld2FyZHMuY28udWsxJTAjBgkqhkiG9w0BCQEWFnN1cHBvcnRAcmVhY2hhd2FyZS5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDQIu8oqBfSv0CSTJku6ceAJcFw0Dq1xEBZRB10wdmCfMcOaii538odCNsAEn5K7Z5g2MwzCb1p6jeNZ8qsRblWwFCsubrt50F5LNv1q3EbHWD0e8Y6rbbHtkqB1jtMgcM5yqLk/Yln+egFwevBPKKdDHrjzci+maZ0qINqUgz5dBA4XjFQZQsb1KZZLeiuwnN1KhHxDpfsKQyfeFyHofoVYjV4Oi5E/Kf5kxCtGc3SFiaEqMAbLPbpF6A779kUvJMN64gdJYzYc5n+vIWj4Im8vhPxL09ksl5o2TISwn5+HXLm+yTzLw2CtbkBUdpzOqTsV1SPkH2FJ4ARlwsDzHttAgMBAAGjgaQwgaEwOgYJYIZIAYb4QgENBC0WK21vZF9zc2wgZ2VuZXJhdGVkIGN1c3RvbSBzZXJ2ZXIgY2VydGlmaWNhdGUwEQYJYIZIAYb4QgEBBAQDAgZAMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwIQYDVR0RBBowGIIWZXhjbHVzaXZlcmV3YXJkcy5jby51azANBgkqhkiG9w0BAQsFAAOCAQEAN/0kXVetFG6MKqrUZLh8wbLwUp6I5hh7UzwayZrJPZe1rBswhZEBM1Liy8n8GvRWlWtuD04M0Ev+s7OJKorqFG3CGQwO1NW+DPyVyerJFyMdXAzQzzIXJRyX3iwo+ydZSONlRkWwl3wTSU8C83lrTqdr6Ib4GAjTZ8FSIGgIw2rY+QT2QdMcfY3wp6e7+493EqkhedIzADMHoujvxKmpAQEAc9DaTdi1lSZ2BjV9EFvW6kPFh7zoVGvhHnyKjAVa4bvLVdDjUZzaRz6RnSJknI+eALBiS1MxVG7xH/gUqcP0rExOvPz3ZmsLfRsldu0cYy+VKngdnxOqAUvlHOGXog==</X509Certificate>
        </X509Data>
      </KeyInfo>
    </q1:KeyDescriptor>
    <q1:KeyDescriptor use="encryption">
      <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
        <X509Data>
          <X509Certificate>MIIEfTCCA2WgAwIBAgIHFkRANXGitDANBgkqhkiG9w0BAQsFADCBqjELMAkGA1UEBhMCR0IxEzARBgNVBAgMCk1lcnNleXNpZGUxEjAQBgNVBAcMCVNvdXRocG9ydDEaMBgGA1UECgwRRXhjbHVzaXZlIFJld2FyZHMxDzANBgNVBAsMBkRldk9wczEeMBwGA1UEAwwVZXhjbHVzaXZlcmV3YXJkc2NvLnVrMSUwIwYJKoZIhvcNAQkBFhZzdXBwb3J0QHJlYWNoYXdhcmUuY29tMB4XDTIyMDIwOTEwNDYxMVoXDTIzMDIxMDEwNDYxMVowgasxCzAJBgNVBAYTAkdCMRMwEQYDVQQIDApNZXJzZXlzaWRlMRIwEAYDVQQHDAlTb3V0aHBvcnQxGjAYBgNVBAoMEUV4Y2x1c2l2ZSBSZXdhcmRzMQ8wDQYDVQQLDAZEZXZPcHMxHzAdBgNVBAMMFmV4Y2x1c2l2ZXJld2FyZHMuY28udWsxJTAjBgkqhkiG9w0BCQEWFnN1cHBvcnRAcmVhY2hhd2FyZS5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDQIu8oqBfSv0CSTJku6ceAJcFw0Dq1xEBZRB10wdmCfMcOaii538odCNsAEn5K7Z5g2MwzCb1p6jeNZ8qsRblWwFCsubrt50F5LNv1q3EbHWD0e8Y6rbbHtkqB1jtMgcM5yqLk/Yln+egFwevBPKKdDHrjzci+maZ0qINqUgz5dBA4XjFQZQsb1KZZLeiuwnN1KhHxDpfsKQyfeFyHofoVYjV4Oi5E/Kf5kxCtGc3SFiaEqMAbLPbpF6A779kUvJMN64gdJYzYc5n+vIWj4Im8vhPxL09ksl5o2TISwn5+HXLm+yTzLw2CtbkBUdpzOqTsV1SPkH2FJ4ARlwsDzHttAgMBAAGjgaQwgaEwOgYJYIZIAYb4QgENBC0WK21vZF9zc2wgZ2VuZXJhdGVkIGN1c3RvbSBzZXJ2ZXIgY2VydGlmaWNhdGUwEQYJYIZIAYb4QgEBBAQDAgZAMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwIQYDVR0RBBowGIIWZXhjbHVzaXZlcmV3YXJkcy5jby51azANBgkqhkiG9w0BAQsFAAOCAQEAN/0kXVetFG6MKqrUZLh8wbLwUp6I5hh7UzwayZrJPZe1rBswhZEBM1Liy8n8GvRWlWtuD04M0Ev+s7OJKorqFG3CGQwO1NW+DPyVyerJFyMdXAzQzzIXJRyX3iwo+ydZSONlRkWwl3wTSU8C83lrTqdr6Ib4GAjTZ8FSIGgIw2rY+QT2QdMcfY3wp6e7+493EqkhedIzADMHoujvxKmpAQEAc9DaTdi1lSZ2BjV9EFvW6kPFh7zoVGvhHnyKjAVa4bvLVdDjUZzaRz6RnSJknI+eALBiS1MxVG7xH/gUqcP0rExOvPz3ZmsLfRsldu0cYy+VKngdnxOqAUvlHOGXog==</X509Certificate>
        </X509Data>
      </KeyInfo>
    </q1:KeyDescriptor>
    <q1:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://exclusivecards-test.azurewebsites.net/saml2/logout" ResponseLocation="https://benefits-ebc-uat.grgcloud.net/saml2/logout" />
    <q1:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" Location="https://exclusivecards-test.azurewebsites.net/saml2/logout" ResponseLocation="https://benefits-ebc-uat.grgcloud.net/saml2/logout" />
    <q1:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</q1:NameIDFormat>
    <q1:AssertionConsumerService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://benefits-ebc-uat.grgcloud.net/saml2/login" index="0" isDefault="true" />
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
    </q1:AttributeConsumingService>
  </q1:SPSSODescriptor>
</q1:EntityDescriptor>'
SELECT @Certificate ='ExclusiveMedia-UAT'
SELECT @Issuer = 'https://exclusivecards-test.azurewebsites.net/'

INSERT INTO [Exclusive].[SSOConfiguration]
           ([Name] ,[DestinationUrl],[ClientId] ,[Metadata] ,[Certificate],[Issuer])
     VALUES
           (@Name, @DestinationUrl, @ClientId, @Metadata, @Certificate, @Issuer)
