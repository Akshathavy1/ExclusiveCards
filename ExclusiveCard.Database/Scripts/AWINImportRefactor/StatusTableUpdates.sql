/*
	Script to adjust and add Import status records
	The id's in this table are linked to enum values
	hense the need to fix the identity values during the process
*/

DECLARE @StatusSettings table
(
	[Id] INT NOT NULL, 
	[Name] nvarchar(50) NULL,
	[Type] nvarchar(50) NULL,
	[IsActive] bit NOT NULL
)

INSERT @StatusSettings
  ([Id],[Name],[Type], [IsActive])
  VALUES
  (23,'Processed','Import',1)
 ,(66,'Uploaded','Import',1)
 ,(67,'Failed','Import',1)


SET IDENTITY_INSERT [Exclusive].[Status] ON

MERGE [Exclusive].[Status] AS target
USING @StatusSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Name] = source.[Name],
  target.[Type]=source.[Type],
  target.[IsActive]=source.[IsActive]
WHEN NOT MATCHED THEN
	INSERT ([Id],[Name],[Type], [IsActive])
	VALUES (source.[Id], source.[Name],source.[Type],source.[IsActive]) 
OUTPUT $action, Inserted.[Id], Inserted.[Name], Inserted.[Type],Inserted.[IsActive];                 

SET IDENTITY_INSERT [Exclusive].[Status] OFF
