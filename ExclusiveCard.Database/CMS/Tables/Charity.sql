CREATE TABLE [CMS].[Charity]
(
	[Id]     INT      IDENTITY (1, 1) NOT NULL, 
	[CharityName]  NVARCHAR (255)   NULL, 
	[CharityURL]   NVARCHAR (512)   NULL, 
	CONSTRAINT [PK_Charity] PRIMARY KEY CLUSTERED ([Id] ASC)
)
