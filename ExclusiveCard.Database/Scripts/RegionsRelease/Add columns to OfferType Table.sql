/*
ALTER TABLE [CMS].[OfferList]
    ADD [WhitelabelId] INT NULL,
        [DisplayType]  INT NULL;


GO
PRINT N'Creating Foreign Key [CMS].[FK_OfferList_WhitelabelSettings_Id]...';


GO
ALTER TABLE [CMS].[OfferList] WITH NOCHECK
    ADD CONSTRAINT [FK_OfferList_WhitelabelSettings_Id] FOREIGN KEY ([WhitelabelId]) REFERENCES [CMS].[WhitelabelSettings] ([Id]);

*/