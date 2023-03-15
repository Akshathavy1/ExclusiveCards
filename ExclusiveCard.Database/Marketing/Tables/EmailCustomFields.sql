CREATE TABLE [Marketing].[EmailCustomFields]
(
	[Id]              INT            IDENTITY (1, 1) NOT NULL, 
    [ExclusiveField]  NVARCHAR(100)  NULL,   
    [CustomName]      NVARCHAR(100)  NULL,
    [FieldType]       NVARCHAR(6)    NOT NULL,
    [SubstitutionTag] NVARCHAR(100)  NULL,
    [SendGridId]      NVARCHAR(50)   NULL,    
)
