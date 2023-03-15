CREATE TABLE [CMS].[League]
(
	[Id] INT       IDENTITY (1, 1) NOT NULL, 
	[Description]  NVARCHAR (50)   NOT NULL, 
	[ImagePath]    NVARCHAR (512)  NULL,
	[SiteCategoryId]  INT          DEFAULT 1 NOT NULL, 
	CONSTRAINT [PK_League] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_League_SiteCategory_SiteCategoryId] FOREIGN KEY ([SiteCategoryId]) REFERENCES [CMS].[SiteCategory] ([Id]),

)
