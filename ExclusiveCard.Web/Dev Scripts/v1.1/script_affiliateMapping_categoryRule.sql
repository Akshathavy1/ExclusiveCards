-- Affiliate Mappings for Category

delete from exclusive.AffiliateMapping where affiliateMappingRuleid = (select id from Exclusive.AffiliateMappingRule 
	where Description='AWIN Categories' and IsActive = 1 and AffiliateId = (select Id from Exclusive.Affiliate where Name='AWIN') )


DECLARE @affilateMappingRuleId INT;
DECLARE @categoryId INT = 0;

	SELECT @affilateMappingRuleId = Id from Exclusive.AffiliateMappingRule 
	where Description='AWIN Categories' and IsActive = 1 and AffiliateId = (select Id from Exclusive.Affiliate where Name='AWIN') 

	select @categoryId = Id from [Exclusive].[Category] Where Name = 'Women' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
	if(@categoryId <> 0)
	BEGIN
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bags',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bras',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Suspenders & Garters',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Dresses & Skirts',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Outerwear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Sportswear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Suits',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Swimwear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Tops',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Trousers',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Underwear',@categoryId)
	END

	SET @categoryId = 0

	select @categoryId = Id from [Exclusive].[Category] Where Name = 'Men' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
	if(@categoryId <> 0)
	BEGIN
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Outerwear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Socks',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Sportswear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Suits',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Swimwear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Tops',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Trousers',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Underwear',@categoryId)
	END

	SET @categoryId = 0

	select @categoryId = Id from [Exclusive].[Category] Where Name = 'Kids' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
	if (@categoryId <> 0)
	BEGIN
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Baby Clothes',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Boys'' Clothes',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Children''s Accessories',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Children''s Footwear',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Girls'' Clothes',@categoryId)
	END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Shoes' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Children''s Footwear',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Footwear',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Footwear',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Lingerie' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bras',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Suspenders & Garters',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Underwear',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Jewellery & Watches' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Jewellery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Watches',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Jewellery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Watches',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Handbags' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bags',@categoryId)
		END

		SET @categoryId = 0
		
		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Other Accessories' and ParentId  = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Fashion' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fancy Dress',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'General Clothing',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Nightwear',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Novelty T-Shirts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Socks & Hosiery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Swimming Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Accessories',@categoryId)
		END

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Computers' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CD-ROM Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CD Writers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CDs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Computer Cases',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Computer Speakers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Computers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CPUs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Writers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVDs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Floppy Disk Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Graphics Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hard Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HD DVD',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Memory Card Reader',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Memory Stick',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Mice',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microdrive',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Motherboards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Scanners',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sound Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'WebCams',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Laptops & Tablets' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CD-ROM Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CD Writers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Writers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVDs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Floppy Disk Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Graphics Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hard Drives',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HD DVD',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Laptop Bags',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Laptops',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Memory Card Reader',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Memory Stick',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Mice',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microdrive',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Motherboards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sound Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'WebCams',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Television' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'3D TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Combi TVs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Recorders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Rentals',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVDs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HD DVD',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HD DVD Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Home Cinema',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'LCD TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'LED TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Monitors',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Plasma TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Projection TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Standard (4:3) TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Television Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'TV Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Widescreen TV',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Music & Entertainment' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Audio Books',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CDs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microphones',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'MP3 Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Music Downloads',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Musical Instruments',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tuners',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Video Games & Consoles' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Console',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVDs',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Games',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Joysticks and Gaming',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Retro Games',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Video Games',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Printers' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'All-In-One Printers',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Printer Consumables',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Printers',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Mobile Phones & Devices' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microdrive',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Mobile Downloads',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Mobile Phone Accessories',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Prepay Mobile Phones',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Domestic Appliances' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blenders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'CD Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Coffee Makers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DVD Recorders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electric Kettles',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fans',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HD DVD Players',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Cameras & Photography' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Camcorders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cameras',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microdrive',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'SD Card',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Software' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Software',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Other Electricals' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Electricals' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Amplifiers & Receivers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Audio Systems',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Batteries',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bluetooth Adapters',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blu-Ray',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blu-Ray Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cables',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cassette Decks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cassettes & Vinyl',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'DJ Equipment',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electronic & RC Toys',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electronic Gadgets',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'GPS & Sat Nav',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Headsets',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'HiFi Speakers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Keyboards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Lighting',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microphones',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Monitors',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'MP3 Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Multimedia Card',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Networking',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Parts & Power Supplies',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'PDAs & Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Portable Cassette Players',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Portable MiniDisc',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Portable Radios',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Power Supplies',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Projector Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Projectors',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Radios',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Remote Controls',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Scanners',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'SmartMedia',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Stereos',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tumble Dryers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vacuum Cleaners',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Washer Dryers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Washing Machines',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'WebCams',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wireless Adapters',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wireless Routers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'XD Cards',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Fragrances' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fragrance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Cosmetics' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bodycare Appliances',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cosmetics',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Hair Care' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bodycare Appliances',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Haircare Appliances',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Haircare Products',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Skin Care' and ParentId =(SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bodycare Appliances',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Skincare',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Mens Grooming' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bodycare Appliances',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fragrance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Shaving',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Dietary & Nutrition' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Diet',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Nutrition',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Pharmacy & supplements' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vitamins & Supplements',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Optical & Glasses' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Health & Beauty' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Contact Lenses',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Glasses',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Glassware',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Optical Devices',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Appliances' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'3D TV',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathroom Scales',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blenders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Combi TVs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cookers & Ovens',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Dishwashers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electric Kettles',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Freezers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fridge Freezers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fridges',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Grills',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'DIY' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cleaning',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hand Tools',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Power Tools',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Kitchen & Bathroom' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathroom Scales',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathroom Sinks & Taps',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathrooms',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathtubs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bespoke Bathrooms',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bespoke Kitchens',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cleaning',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cooker Hoods',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cookers & Ovens',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Dishwashers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electric Kettles',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Freezers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fridge Freezers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fridges',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Grills',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Kitchen Sinks and Taps',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Kitchen Units',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microwaves',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Showers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sinks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Toasters',@categoryId)
		END

		SET @categoryId = 0
		
		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Garden' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Agricultural Products',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flowers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Garden & Leisure',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Garden Tools',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Plants & Seeds',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sheds & Garden Furniture',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Home Furnishings' and ParentId =  (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathtubs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Beds',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Chairs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Curtains & Blinds',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Decorations',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flooring & Carpeting',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Furniture Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Home Textiles',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'House Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Living Room',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Mattresses',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Painting & Decorating',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Radiators',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sofas',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tables',@categoryId)
		END

		SET @categoryId = 0
		
		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Lighting' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Lighting',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Kitchenwear' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cooker Hoods',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cookware & Utensils',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Crockery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cutlery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electric Kettles',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hobs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Microwaves',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Toasters',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Pet Products' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Pets',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Trade & Professional' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Home & Garden' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tyres',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Web Hosting',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wheels',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Accomodation' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Accommodation',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Buy Property',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Rent Property',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Car Rental' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Car Hire',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Car Loans',@categoryId)
		Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Driving',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Holidays' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cruises',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flights',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flying',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Holidays',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Short Breaks',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Train, Ferry & Other Travel' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cruises',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Ferries & Eurotunnel',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Flights' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flights',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flying',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Short Breaks',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Airport Parking' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Airport Parking',@categoryId)
		END

		SET @categoryId = 0
    
		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Travel' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Driving',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Days Out' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Concerts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cricket',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Outdoor Adventure',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sporting Events',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Theatre',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tourist Attractions',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Water Experiences',@categoryId)
	    END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Gaming' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Joysticks and Gaming',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Retro Games',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Music, DVDs & Downloads' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Audio Books',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blu-Ray',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blu-Ray Players',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Tickets & Events' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Concerts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sporting Events',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Theatre',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tourist Attractions',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Hobbies' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Badminton',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Basketball',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Books',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Boxing',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cycling',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Darts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Driving',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Equestrian',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Extreme Sports',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fishing',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Football',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Golf',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Magazine Subscriptions',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Pool & Billiards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Snooker',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Squash',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Table Tennis',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tennis',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Theatre',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Streaming Services' and ParentId =(SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Streaming Media Devices',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Sports' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Badminton',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Basketball',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Boxing',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cricket',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cycling',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Darts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Equestrian',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Football',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Golf',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hockey',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Motorsport',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Pool & Billiards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Snooker',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Squash',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Table Tennis',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tennis',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Water Sports',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Winter Sports',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Fitness & Gym' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fitness Equipment',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Sportswear' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Leisure & Entertainment' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Sportswear',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Sportswear',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Motor Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vehicle Insurance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'House Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Home Insurance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Health Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Health Insurance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Life Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Life Insurance',@categoryId)
		END

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Travel Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Travel Insurance',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Other Insurance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Insurance' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Specialist Insurance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Food & Drink' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Alcoholic Drinks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cakes',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Chocolate',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Condiments',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Drinks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fruit & Vegetables',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hampers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Meat',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Organic Food',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Poultry & Fish',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Ready Meals',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Snacks & Sweets',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tinned Food',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wine',@categoryId)
		END
		
		SET @categoryId = 0
				
		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Supermarkets' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Alcoholic Drinks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bags',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Barbecues & Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bras',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cakes',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Chocolate',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cleaning',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Dairy',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Drinks',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fruit & Vegetables',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Greetings Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Meat',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Oral Health',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Organic Food',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Party Food',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Poultry & Fish',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Ready Meals',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Snacks & Sweets',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tinned Food',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wine',@categoryId)
	    END
	
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Mother & Baby' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Baby Clothes',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Baby Products',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Baby Toys',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Maternity',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Department Stores' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping'  and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Action Figures',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bags',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Barbecues & Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathroom Scales',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Boys'' Clothes',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bras',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Cables',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Calendars',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Chairs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Coffee Makers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Combi TVs',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Fancy Dress',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Musical Toys',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Office Supplies',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Outdoor Toys',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Puzzles & Learning',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Soft Toys',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Toy Models',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Trampolines',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Tumble Dryers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Washer Dryers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Washing Machines',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Jewellery' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping'  and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Men''s Jewellery',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Women''s Jewellery',@categoryId)
	    END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Florists' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping'  and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Flowers',@categoryId)

		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Gifts' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping'  and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Anniversary Gifts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Birthday Gifts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Christmas Gifts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Greeting Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Novelty Gifts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Personalised Gifts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Sports Memorabilia',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Valentine''s Day',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Wedding Gifts',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Utilities' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Gas & Electric Bills',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Phone & Broadband' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Broadband',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Home Telephone Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'House Telephones',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Hubs & Switches',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Comparison Sites' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Broadband',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Car Hire',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Car Loans',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Buy Property',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Credit Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Gas & Electric Bills',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Home Insurance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Health Insurance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Life Insurance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'New & Used Cars',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Rent Property',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Telephone Bills',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Unsecured Loans',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vehicle Finance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vehicle Leasing',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] Where Name = 'Deal & Voucher Sites' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Concerts',@categoryId)
		END
		
		select @categoryId = Id from [Exclusive].[Category] where Name = 'Finance' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Credit Cards',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Finance',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vehicle Finance',@categoryId)
		END

		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] where Name = 'Gadgets' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Bathroom Scales',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Blenders',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Coffee Makers',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Console Accessories',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Electronic Gadgets',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Headphones',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Headsets',@categoryId)
		END
		
		SET @categoryId = 0

		select @categoryId = Id from [Exclusive].[Category] where Name = 'Car Accessories' and ParentId = (SELECT Id FROM [Exclusive].[Category] WHERE Name = 'Shopping' and ParentId = 0)
		if (@categoryId <> 0)
		BEGIN
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Auto Care',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Car Parts',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Custom Number Plates',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'In-Car Entertainment',@categoryId)
			Insert into Exclusive.AffiliateMapping values(@affilateMappingRuleId,'Vehicle Servicing',@categoryId)
		END