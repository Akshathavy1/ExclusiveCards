/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
declare @whiteLabelSettings table(
	[Id]			INT NOT NULL,
	[Name]			NVARCHAR (255) NULL,
	[DisplayName]   NVARCHAR (255) NULL,
    [URL]           NVARCHAR (100) NULL,
    [Slug]          NVARCHAR (255) NULL,
    [CompanyNumber] NVARCHAR (10)  NULL,
    [CSSFile]       NVARCHAR (255) NULL,
    [Logo]          NVARCHAR (255) NULL,
    [ClaimsEmail]   NVARCHAR (100) NULL,
    [HelpEmail]     NVARCHAR (100) NULL,
    [MainEmail]     NVARCHAR (100) NULL,
    [Address]       NVARCHAR (MAX) NULL,
    [CardName]      NVARCHAR (255) NULL
)

INSERT @whiteLabelSettings ([Id], [Name], [DisplayName], [URL], [Slug], [CompanyNumber], [CSSFile], [Logo], [ClaimsEmail], [HelpEmail], [MainEmail], [Address], [CardName]) 
	VALUES(1,'True Potential', 'True Potential', 'https://localhost:44325/tp/','tp','1245326','\tp.css','images\Homepage\GB\aa.png','claims@tp.com','help@tp.com','info@tp.com','TP, UK',NULL)
INSERT @whiteLabelSettings ([Id], [Name], [DisplayName], [URL], [Slug], [CompanyNumber], [CSSFile], [Logo], [ClaimsEmail], [HelpEmail], [MainEmail], [Address], [CardName]) 
	VALUES(2,'True Potential Rewards', 'True Potential Rewards Ltd', 'https://localhost:44365','true-potential','11616720','true-potential.css','logo.svg','claims@tpinvestor.co.uk','help-me@tpinvestor.co.uk','enquiries@tpinvestor.co.uk','27 Example Street, Someville, United Kingdom, PR1 1RP','True Potential Card')
INSERT @whiteLabelSettings ([Id], [Name], [DisplayName], [URL], [Slug], [CompanyNumber], [CSSFile], [Logo], [ClaimsEmail], [HelpEmail], [MainEmail], [Address], [CardName]) 
	VALUES(3,'Exclusive Rewards', 'Exclusive Media Ltd', 'https://localhost:44325/','exclusive','11616720','exclusive.css','logo.svg','claims@exclusiverewards.co.uk','help-me@exclusiverewards.co.uk','enquiries@exclusiverewards.co.uk','15 Hoghton Street, Southport, United Kingdom, PR9 0NS','Exclusive Card')
INSERT @whiteLabelSettings ([Id], [Name], [DisplayName], [URL], [Slug], [CompanyNumber], [CSSFile], [Logo], [ClaimsEmail], [HelpEmail], [MainEmail], [Address], [CardName]) 
	VALUES(4,'Exclusive Consumer Rights', 'Consumer Rights', 'https://localhost:44325','consumer-rights','11616123','consumer-rights.css','logo.svg','claims@consumerrights.co.uk','hekp-me@consumer-rights.co.uk','enquiries@consumer-rights.co.uk','22 Test Street, Southport, United Kingdom, PR9 0NS','Exclusive Card')

SET IDENTITY_INSERT [CMS].[WhiteLabelSettings] ON 

MERGE [CMS].[WhiteLabelSettings] AS target
USING @whiteLabelSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN

  UPDATE SET
  target.[Name] = source.[Name],
  target.[DisplayName] = source.[DisplayName],
  target.[URL] = source.[URL],
  target.[Slug] = source.[Slug],
  target.[CompanyNumber] = source.[CompanyNumber],
  target.[CSSFile] = source.[CSSFile],
  target.[Logo] = source.[Logo],
  target.[ClaimsEmail] = source.[ClaimsEmail],
  target.[HelpEmail] = source.[HelpEmail],  
  target.[MainEmail] = source.[MainEmail],
  target.[Address] = source.[Address],
  target.[CardName] = source.[CardName]
WHEN NOT MATCHED THEN
  INSERT ([Id], [Name], [DisplayName], [URL], [Slug], [CompanyNumber], [CSSFile], [Logo], [ClaimsEmail], [HelpEmail], [MainEmail], [Address], [CardName]) 
  VALUES (source.[Id], source.[Name], source.[DisplayName], source.[URL], source.[Slug], source.[CompanyNumber], source.[CSSFile], source.[Logo], source.[ClaimsEmail], source.[HelpEmail], source.[MainEmail], source.[Address], source.[CardName]) 
OUTPUT $action, Inserted.[Id], Inserted.[Name];                 

SET IDENTITY_INSERT [CMS].[WhiteLabelSettings] OFF