ALTER TABLE [Exclusive].[MerchantImage] ADD [TimeStamp] datetime2 NOT NULL DEFAULT '2019-02-09T00:00:00.0000000Z';

GO

ALTER TABLE [CMS].[AdvertSlots] ADD [TimeStamp] datetime2 NOT NULL DEFAULT '2019-02-09T00:00:00.0000000Z';

GO

ALTER TABLE [CMS].[Adverts] ADD [TimeStamp] datetime2 NOT NULL DEFAULT '2019-02-09T00:00:00.0000000Z';

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190430095341_ImageTimestamp', N'2.1.8-servicing-32085');

GO

CREATE TABLE [Exclusive].[EmailTemplates] (
    [Id] int NOT NULL IDENTITY,
    [EmailName] nvarchar(100) NULL,
    [Subject] nvarchar(512) NULL,
    [BodyText] nvarchar(MAX) NULL,
    [BodyHtml] nvarchar(MAX) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmailTemplates] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Exclusive].[EmailsSent] (
    [Id] int NOT NULL IDENTITY,
    [EmailTemplateId] int NOT NULL,
    [CustomerId] int NOT NULL,
    [EmailTo] nvarchar(512) NULL,
    [DateSent] datetime2 NOT NULL,
    CONSTRAINT [PK_EmailsSent] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailsSent_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmailsSent_EmailTemplates_EmailTemplateId] FOREIGN KEY ([EmailTemplateId]) REFERENCES [Exclusive].[EmailTemplates] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_EmailsSent_CustomerId] ON [Exclusive].[EmailsSent] ([CustomerId]);

GO

CREATE INDEX [IX_EmailsSent_EmailTemplateId] ON [Exclusive].[EmailsSent] ([EmailTemplateId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190507075611_Emails', N'2.1.8-servicing-32085');

GO

ALTER TABLE [Exclusive].[MerchantImage] ADD [ImageType] nvarchar(max) NULL;

GO

ALTER TABLE [CMS].[AdvertSlots] ADD [ImageType] nvarchar(max) NULL;

GO

ALTER TABLE [CMS].[Adverts] ADD [ImageType] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190507133312_FileType', N'2.1.8-servicing-32085');

GO

ALTER TABLE [Exclusive].[EmailTemplates] ADD [TemplateTypeId] int NOT NULL DEFAULT 0;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190508003038_EmailTemplateIdAdd', N'2.1.8-servicing-32085');

GO

ALTER TABLE [CMS].[PanelAdverts] DROP CONSTRAINT [PK_PanelAdverts];

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CMS].[PanelAdverts]') AND [c].[name] = N'CountryCode');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CMS].[PanelAdverts] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [CMS].[PanelAdverts] ALTER COLUMN [CountryCode] nvarchar(3) NOT NULL;

GO

ALTER TABLE [CMS].[PanelAdverts] ADD CONSTRAINT [PK_PanelAdverts] PRIMARY KEY ([PanelId], [AdvertId], [CountryCode]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190508092613_ConstraintPanelAdvert', N'2.1.8-servicing-32085');

GO

IF NOT EXISTS (SELECT * FROM ExclusiveCard.Exclusive.EmailTemplates WHERE [TemplateTypeId] = 0)
BEGIN
--do what needs to be done if not
INSERT INTO ExclusiveCard.Exclusive.EmailTemplates
VALUES ('Welcome Email','Welcome to Exclusive Card',null,'<!DOCTYPE html>
<html>
<head></head>
<body>
<p>Dear [Name]</p>

<p>Welcome to Exclusive Card.</p>

<p>Your account is now open and you can enjoy all the benefits of being an Exclusive member.</p>
<p>Please, always remember to make sure you are logged in to your account each time you use the <br>
	Exclusive website to take advantage of all the features and offers available. You will also not be able <br>
to receive cashback unless you are logged into your account.</p>
<p><b>You should now download our free mobile application (Exclusive Card) which is available on the <br>
App Store for iPhone and Google Play Store for Android devices, in order to access the electronic <br>
version of your membership card.</b>This only needs to be shown whenever you use high street or restaurant deals.</p>
<p>If you encounter any problems or would like to ask any further questions regarding your new Exclusive <br>
	account please email us on :<a href="mailto: contact@exclusivecard.co.uk">contact@exclusivecard.co.uk</a></p> 

<p>The Exclusive Team</p>
</body>
</html>',0,0)
END



GO

IF NOT EXISTS (SELECT * FROM ExclusiveCard.Exclusive.EmailTemplates WHERE [TemplateTypeId] = 1)
BEGIN
--do what needs to be done if not
INSERT INTO ExclusiveCard.Exclusive.EmailTemplates
VALUES ('Welcome Email','Welcome to Exclusive Card',null,'<!DOCTYPE html>
<html>
<head></head>
<body>
<p>Dear [Name]</p>
<h1>Renewal</h1>
<p>Welcome to Exclusive Card.</p>

<p>Your account is now open and you can enjoy all the benefits of being an Exclusive member.</p>
<p>Please, always remember to make sure you are logged in to your account each time you use the <br>
	Exclusive website to take advantage of all the features and offers available. You will also not be able <br>
to receive cashback unless you are logged into your account.</p>
<p><b>You should now download our free mobile application (Exclusive Card) which is available on the <br>
App Store for iPhone and Google Play Store for Android devices, in order to access the electronic <br>
version of your membership card.</b>This only needs to be shown whenever you use high street or restaurant deals.</p>
<p>If you encounter any problems or would like to ask any further questions regarding your new Exclusive <br>
	account please email us on :<a href="mailto: contact@exclusivecard.co.uk">contact@exclusivecard.co.uk</a></p> 

<p>The Exclusive Team</p>
</body>
</html>',0,1)
END