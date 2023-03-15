CREATE TABLE [CMS].[OfferList] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [ListName]           NVARCHAR (50)  NULL,
    [Description]        NVARCHAR (500) NULL,
    [MaxSize]            INT            NOT NULL,
    [IsActive]           BIT            NOT NULL,
    [IncludeShowAllLink] BIT            DEFAULT ((0)) NOT NULL,
    [ShowAllLinkCaption] NVARCHAR (100) NULL,
    [PermissionLevel]    SMALLINT       DEFAULT (CONVERT([smallint],(0))) NOT NULL,
    [WhitelabelId] INT NULL, 
    [DisplayType] INT NULL , 
    CONSTRAINT [PK_OfferList] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OfferList_WhitelabelSettings_Id] FOREIGN KEY ([WhitelabelId]) REFERENCES [CMS].[WhitelabelSettings] ([Id]),
);

