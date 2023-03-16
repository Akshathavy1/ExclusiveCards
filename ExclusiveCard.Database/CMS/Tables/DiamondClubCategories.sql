CREATE TABLE [CMS].[DiamondClubCategories]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Description] NVARCHAR(20) NOT NULL, 
    CONSTRAINT [PK_DiamondClubCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
)
