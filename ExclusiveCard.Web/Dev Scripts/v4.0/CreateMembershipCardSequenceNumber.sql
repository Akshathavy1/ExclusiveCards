DECLARE @cardnumber  INT

SELECT  @cardNumber = [value] FROM [Exclusive].[SequenceNumbers] where [description] = 'MembershipCardNumber'

if @CardNumber IS NULL
BEGIN
	INSERT [Exclusive].[SequenceNumbers]([description],[Value] ) Values ('MembershipCardNumber', 20000)
END