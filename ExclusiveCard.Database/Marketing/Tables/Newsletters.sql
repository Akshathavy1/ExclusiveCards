CREATE TABLE [Marketing].[Newsletter]
(
	[Id]                INT             IDENTITY (1, 1) NOT NULL , 
    [Name]              NVARCHAR(255)   NULL, 
    [Description]       NVARCHAR(512)   NULL,
    [Schedule]          NVARCHAR(MAX)   NULL,
    [Enabled]           BIT             NULL, 
    [EmailTemplateId]   INT             NOT NULL, 
    [OfferListId]       INT             NOT NULL, 
    CONSTRAINT [PK_Newsletters] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Newsletters_EmailTemplates_EmailTemplateId(Id)] FOREIGN KEY ([EmailTemplateId]) REFERENCES [Exclusive].[EmailTemplates]([Id]),
    CONSTRAINT [FK_Newsletters_OfferList_OfferListId(Id)] FOREIGN KEY ([OfferListId]) REFERENCES [CMS].[OfferList]([Id]),
)
