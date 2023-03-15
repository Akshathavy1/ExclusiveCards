CREATE TABLE [Marketing].[Campaigns]
(
	[Id]                INT             IDENTITY (1, 1) NOT NULL, 
    [NewsletterId]      INT             NOT NULL, 
    [WhiteLabelId]      INT             NOT NULL, 
    [CampaignReference] NVARCHAR(100)   NULL, 
    [CampaignName]      NVARCHAR(100)   NOT NULL, 
    [SenderId]          INT             NULL, 
    [Enabled]           BIT             NULL, 
    CONSTRAINT [PK_Campaigns] PRIMARY KEY CLUSTERED ([Id] ASC),
    --CONSTRAINT [PK_Campaigns_Composite] PRIMARY KEY CLUSTERED  ([WhiteLabelId] ASC),
    CONSTRAINT [FK_Campaigns_WhiteLabelSettings_Id(Id)] FOREIGN KEY ([WhiteLabelId]) REFERENCES [CMS].[WhitelabelSettings]([Id]),
)
