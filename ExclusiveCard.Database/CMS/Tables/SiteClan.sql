CREATE TABLE [CMS].[SiteClan]
(
	[Id]                 INT            IDENTITY (1, 1) NOT NULL, 
	[LeagueId]             INT            NULL, 
	[Description]        NVARCHAR (50)    NOT NULL, 
	[ImagePath]          NVARCHAR (512)   NULL, 
	[PrimaryColour]      NVARCHAR (10)    NULL, 
	[SecondaryColour]    NVARCHAR (10)    NULL, 
	[CharityId]            INT            NULL, 
	[SiteOwnerId]          INT            NOT NULL, 
	[SiteCategoryId]       INT            NOT NULL, 
	[WhiteLabelId]         INT            NULL,
	[MembershipPlanId]     INT            NOT NULL,
	CONSTRAINT [PK_SiteClan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SiteClan_League_LeagueId] FOREIGN KEY ([LeagueId]) REFERENCES [CMS].[League] ([Id]),
    CONSTRAINT [FK_SiteClan_Charity_CharityId] FOREIGN KEY ([CharityId]) REFERENCES [CMS].[Charity] ([Id]),
    CONSTRAINT [FK_SiteClan_SiteOwner_SiteOwnerId] FOREIGN KEY ([SiteOwnerId]) REFERENCES [CMS].[SiteOwner] ([Id]),
    CONSTRAINT [FK_SiteClan_SiteCategory_SiteCategoryId] FOREIGN KEY ([SiteCategoryId]) REFERENCES [CMS].[SiteCategory] ([Id]),
    CONSTRAINT [FK_SiteClan_WhitelabelSettings_WhiteLabelId] FOREIGN KEY ([WhiteLabelId]) REFERENCES [CMS].[WhitelabelSettings] ([Id]),
    CONSTRAINT [FK_SiteClan_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id]),

)
