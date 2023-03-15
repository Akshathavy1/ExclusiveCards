CREATE TABLE [CMS].[SiteOwner]
(
	[Id]           INT      IDENTITY (1, 1) NOT NULL, 
	[Description]  NVARCHAR (20)            NOT NULL, 
	[ClanDescription] NVARCHAR(MAX) NULL, 
    [BeneficiaryConfirmation] NVARCHAR(MAX) NULL, 
    [StarndardRewardConfirmation] NVARCHAR(MAX) NULL, 
    [ClanHeading] NVARCHAR(MAX) NULL, 
    [BeneficiaryHeading] NVARCHAR(MAX) NULL, 
    [StarndardHeading] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_SiteOwner] PRIMARY KEY CLUSTERED ([Id] ASC)
)
