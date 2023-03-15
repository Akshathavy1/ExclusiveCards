CREATE TABLE [Marketing].[ContactLists]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[WhiteLabelId] INT NOT NULL,
	[ContactListReference] nvarchar(100),
	[ContactListName] NVARCHAR(270) NULL, 
    CONSTRAINT [PK_SendGridContactLists] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_SendGridContactLists_WhiteLabelSeetings_Id] FOREIGN KEY ([WhiteLabelId]) REFERENCES [CMS].[WhitelabelSettings]([Id])
)
