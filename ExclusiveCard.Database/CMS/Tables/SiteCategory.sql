CREATE TABLE [CMS].[SiteCategory]
(
	[Id]           INT      IDENTITY (1, 1) NOT NULL, 
	[Description]  NVARCHAR (20)            NOT NULL, 
	CONSTRAINT [PK_SiteCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
)
