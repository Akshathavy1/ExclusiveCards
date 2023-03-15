CREATE TABLE [Exclusive].[SSOConfiguration]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [DestinationUrl] NVARCHAR(MAX) NOT NULL, 
    [ClientId] NVARCHAR(MAX) NOT NULL, 
    [Metadata] NVARCHAR(MAX) NOT NULL, 
    [Certificate] NVARCHAR(MAX) NOT NULL,
    [Issuer] NVARCHAR(MAX) NOT NULL DEFAULT 'https://exclusiverewards.co.uk/',
    CONSTRAINT [PK_SSOConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
)
