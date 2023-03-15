DECLARE @Id INT;
DECLARE @FileMappingId INT;
IF((SELECT COUNT(*) FROM Exclusive.Affiliate WHERE [Name]='AWIN') = 0)
	BEGIN
		INSERT INTO Exclusive.Affiliate VALUES('AWIN')
	END

SELECT @Id = Id from Exclusive.Affiliate WHERE [Name] = 'AWIN'

IF((SELECT COUNT(*) FROM Exclusive.AffiliateFileMapping WHERE [Description] = 'FileMapping' AND AffiliateId = @Id) = 0)
BEGIN
INSERT INTO Exclusive.AffiliateFileMapping VALUES(@Id, 'FileMapping')
END

SELECT @FileMappingId = Id FROM Exclusive.AffiliateFileMapping WHERE [Description] = 'FileMapping' AND AffiliateId = @Id

IF((SELECT COUNT(*) FROM Exclusive.AffiliateFile WHERE [FileName] = 'Sales' AND AffiliateId = @Id) = 0)
BEGIN
	INSERT INTO Exclusive.AffiliateFile VALUES (@Id, 'Sales', 'Sales', 'OfferImportAwin', @FileMappingId)
END

IF((SELECT COUNT(*) FROM Exclusive.AffiliateFile WHERE [FileName] = 'PromoCodes' AND AffiliateId = @Id) = 0)
BEGIN
	INSERT INTO Exclusive.AffiliateFile VALUES (@Id, 'PromoCodes', 'Voucher Code', 'OfferImportAwin', @FileMappingId)
END
GO
