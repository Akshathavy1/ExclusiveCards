CREATE TABLE [Exclusive].[Category] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)   NULL,
    [Code]         NVARCHAR (1024) NULL,
    [ParentId]     INT             NOT NULL,
    [IsActive]     BIT             NOT NULL,
    [DisplayOrder] INT             DEFAULT ((0)) NOT NULL,
    [UrlSlug]      NVARCHAR (60)   NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Category_UrlSlug]
    ON [Exclusive].[Category]([UrlSlug] ASC) WHERE ([UrlSlug] IS NOT NULL);

