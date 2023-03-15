CREATE TABLE [Marketing].[Contacts]
(
	[Id] INT  IDENTITY (1, 1) NOT NULL,
	[ExclusiveCustomerId] INT NOT NULL,
	[ContactReference] nvarchar(50) NULL,
	CONSTRAINT [PK_SendGridContact] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_SendGridContact_Customer_Id] FOREIGN KEY (ExclusiveCustomerId) REFERENCES [Exclusive].[Customer](Id) 
)
