DECLARE @PaymentproviderSettings table(
	[Id]		INT NOT NULL,
	[Name]		NVARCHAR (100) NULL,
	[IsActive]  bit NOT NULL
)

INSERT @PaymentproviderSettings
  ([Id], [Name], [IsActive])
  VALUES
	(1,'Cashback', 1),
    (2,'PayPal', 1),
    (3,'GoogleIAP', 1 ),
    (4,'AppleIAP', 1)

SET IDENTITY_INSERT [Exclusive].[Paymentprovider] ON

MERGE [Exclusive].[Paymentprovider] AS target
USING @PaymentproviderSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN

  UPDATE SET
  target.[Name] = source.[Name],
  target.[IsActive] = source.[IsActive]
WHEN NOT MATCHED THEN
INSERT ([Id], [Name], [IsActive])
  VALUES (source.[Id], source.[Name], source.[IsActive]) 
OUTPUT $action, Inserted.[Id], Inserted.[Name];                 

SET IDENTITY_INSERT [Exclusive].[Paymentprovider] OFF
