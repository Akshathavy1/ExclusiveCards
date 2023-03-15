DECLARE @Description nvarchar(128)='Local Offer'

IF NOT EXISTS

(SELECT [Id] FROM [Exclusive].[OfferType] WHERE [Description] = @Description)
BEGIN
	INSERT [Exclusive].[OfferType]
	([Description],[IsActive])
	VALUES
	 (@Description,1)

END