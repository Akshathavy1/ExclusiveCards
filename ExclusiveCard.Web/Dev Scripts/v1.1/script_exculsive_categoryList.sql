
delete from [Exclusive].[Category]

 
 Declare @CategoryId int;

 Insert Into [Exclusive].[Category] VALUES ('Fashion',null,0,1,1)
 SELECT @CategoryId = Id FROM [Exclusive].[Category] WHERE Name = 'Fashion'


    Insert Into [Exclusive].[Category] VALUES ('Women',null,@CategoryId,1,1)
	Insert Into [Exclusive].[Category] VALUES ('Men',null,@CategoryId,1,2)
    Insert Into [Exclusive].[Category] VALUES ('Kids',null,@CategoryId,1,3)
	
    Insert Into [Exclusive].[Category] VALUES ('Shoes',null,@CategoryId,1,4)
	
    Insert Into [Exclusive].[Category] VALUES ('Lingerie',null,@CategoryId,1,5)
		
    Insert Into [Exclusive].[Category] VALUES ('Jewellery & Watches',null,@CategoryId,1,6)

    Insert Into [Exclusive].[Category] VALUES ('Handbags ',null,@CategoryId,1,7)
	
    Insert Into [Exclusive].[Category] VALUES ('Other Accessories',null,@CategoryId,1,8)

Insert Into .[Exclusive].[Category] VALUES ('Electricals',null,0,1,2)
SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Electricals'

    Insert Into [Exclusive].[Category] VALUES ('Computers',null,@CategoryId,1,1)
		
    Insert Into [Exclusive].[Category] VALUES ('Laptops & Tablets',null,@CategoryId,1,2)

    Insert Into [Exclusive].[Category] VALUES ('Television',null,@CategoryId,1,3)

    Insert Into [Exclusive].[Category] VALUES ('Music & entertainment',null,@CategoryId,1,4)
		
    Insert Into [Exclusive].[Category] VALUES ('Smart Home',null,@CategoryId,1,5)

    Insert Into [Exclusive].[Category] VALUES ('Video Games & Consoles',null,@CategoryId,1,6)

    Insert Into [Exclusive].[Category] VALUES ('Printers',null,@CategoryId,1,7)

    Insert Into [Exclusive].[Category] VALUES ('Mobile Phones & Devices',null,@CategoryId,1,8)

	Insert Into [Exclusive].[Category] VALUES ('Domestic Appliances',null,@CategoryId,1,9)
		
	Insert Into [Exclusive].[Category] VALUES ('Cameras & Photography',null,@CategoryId,1,10)

	Insert Into [Exclusive].[Category] VALUES ('Software',null,@CategoryId,1,11)
	
	Insert Into [Exclusive].[Category] VALUES ('Other Electricals',null,@CategoryId,1,12)
		
Insert Into [Exclusive].[Category] VALUES ('Health & Beauty',null,0,1,3)
SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Health & Beauty'

    Insert Into [Exclusive].[Category] VALUES ('Fragrances',null,@CategoryId,1,1)
		
    Insert Into [Exclusive].[Category] VALUES ('Cosmetics',null,@CategoryId,1,2)
		
    Insert Into [Exclusive].[Category] VALUES ('Hair Care',null,@CategoryId,1,3)
		
    Insert Into [Exclusive].[Category] VALUES ('Skin Care',null,@CategoryId,1,4)

    Insert Into [Exclusive].[Category] VALUES ('Mens Grooming',null,@CategoryId,1,5)

    Insert Into [Exclusive].[Category] VALUES ('Tanning Products',null,@CategoryId,1,6)

    Insert Into [Exclusive].[Category] VALUES ('Dietary & Nutrition',null,@CategoryId,1,7)
		
    Insert Into [Exclusive].[Category] VALUES ('Pharmacy & supplements',null,@CategoryId,1,8)

	Insert Into [Exclusive].[Category] VALUES ('Optical & Glasses',null,@CategoryId,1,9)
		
Insert Into .[Exclusive].[Category] VALUES ('Home & Garden',null,0,1,4)

SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Home & Garden'

    Insert Into [Exclusive].[Category] VALUES ('Appliances',null,@CategoryId,1,1)
		
    Insert Into [Exclusive].[Category] VALUES ('DIY',null,@CategoryId,1,2)
		
    Insert Into [Exclusive].[Category] VALUES ('Kitchen & Bathroom',null,@CategoryId,1,3)
		
    Insert Into [Exclusive].[Category] VALUES ('Garden',null,@CategoryId,1,4)
		
    Insert Into [Exclusive].[Category] VALUES ('Home Furnishings',null,@CategoryId,1,5)

    Insert Into [Exclusive].[Category] VALUES ('Lighting',null,@CategoryId,1,6)
			
    Insert Into [Exclusive].[Category] VALUES ('Kitchenwear',null,@CategoryId,1,7)
		
    Insert Into [Exclusive].[Category] VALUES ('Pet Products',null,@CategoryId,1,8)
		
	Insert Into [Exclusive].[Category] VALUES ('Trade & Professional',null,@CategoryId,1,9)
		
	Insert Into [Exclusive].[Category] VALUES ('Windows & doors',null,@CategoryId,1,10)

Insert Into .[Exclusive].[Category] VALUES ('Travel',null,0,1,5)
SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Travel'

    Insert Into [Exclusive].[Category] VALUES ('Accomodation',null,@CategoryId,1,1)
		
    Insert Into [Exclusive].[Category] VALUES ('Car Rental',null,@CategoryId,1,2)
		
    Insert Into [Exclusive].[Category] VALUES ('Holidays',null,@CategoryId,1,3)

    Insert Into [Exclusive].[Category] VALUES ('Train, Ferry & Other Travel',null,@CategoryId,1,4)

    Insert Into [Exclusive].[Category] VALUES ('Flights',null,@CategoryId,1,5)
				
    Insert Into [Exclusive].[Category] VALUES ('Airport Parking',null,@CategoryId,1,6)
		
    Insert Into [Exclusive].[Category] VALUES ('Airport Services',null,@CategoryId,1,7)

    Insert Into [Exclusive].[Category] VALUES ('Insurance',null,@CategoryId,1,8)
		
	Insert Into [Exclusive].[Category] VALUES ('Money & Exchange',null,@CategoryId,1,9)


Insert Into .[Exclusive].[Category] VALUES ('Leisure & Entertainment',null,0,1,6)
SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Leisure & Entertainment'

    Insert Into [Exclusive].[Category] VALUES ('Days Out',null,@CategoryId,1,1)

    Insert Into [Exclusive].[Category] VALUES ('Gaming',null,@CategoryId,1,2)
		
    Insert Into [Exclusive].[Category] VALUES ('Music, DVDs & Downloads',null,@CategoryId,1,3)

    Insert Into [Exclusive].[Category] VALUES ('Tickets & Events',null,@CategoryId,1,4)
		
    Insert Into [Exclusive].[Category] VALUES ('Hobbies',null,@CategoryId,1,5)
				
    Insert Into [Exclusive].[Category] VALUES ('TV Deals & Packages',null,@CategoryId,1,6)
		
    Insert Into [Exclusive].[Category] VALUES ('Streaming Services',null,@CategoryId,1,7)
		
    Insert Into [Exclusive].[Category] VALUES ('Dating',null,@CategoryId,1,8)
	Insert Into [Exclusive].[Category] VALUES ('Gambling',null,@CategoryId,1,9)
	Insert Into [Exclusive].[Category] VALUES ('Sports ',null,@CategoryId,1,10)
		
	Insert Into [Exclusive].[Category] VALUES ('Fitness & Gym',null,@CategoryId,1,11)
		
	Insert Into [Exclusive].[Category] VALUES ('Sportswear',null,@CategoryId,1,12)
		
Insert Into .[Exclusive].[Category] VALUES ('Insurance',null,0,1,7)

SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Insurance'

    Insert Into [Exclusive].[Category] VALUES ('Motor Insurance',null,@CategoryId,1,1)

    Insert Into [Exclusive].[Category] VALUES ('House Insurance',null,@CategoryId,1,2)


	Insert Into [Exclusive].[Category] VALUES ('Health Insurrance',null,@CategoryId,1,3)

    Insert Into [Exclusive].[Category] VALUES ('Life Insurance',null,@CategoryId,1,4)

    Insert Into [Exclusive].[Category] VALUES ('Travel Insurance',null,@CategoryId,1,5)

    Insert Into [Exclusive].[Category] VALUES ('Pet Insurance',null,@CategoryId,1,6)
    Insert Into [Exclusive].[Category] VALUES ('Landlord Insurance',null,@CategoryId,1,7)
    Insert Into [Exclusive].[Category] VALUES ('Other Insurance',null,@CategoryId,1,8)


Insert Into .[Exclusive].[Category] VALUES ('Shopping',null,0,1,8)
SELECT @CategoryId = Id FROM .[Exclusive].[Category] WHERE Name = 'Shopping'

    Insert Into [Exclusive].[Category] VALUES ('Food & Drink',null,@CategoryId,1,1)

    Insert Into [Exclusive].[Category] VALUES ('Supermarkets',null,@CategoryId,1,2)
				
    Insert Into [Exclusive].[Category] VALUES ('Mother & Baby',null,@CategoryId,1,3)
	
    Insert Into [Exclusive].[Category] VALUES ('Department Stores',null,@CategoryId,1,4)
			
    Insert Into [Exclusive].[Category] VALUES ('Jewellery',null,@CategoryId,1,5)

    Insert Into [Exclusive].[Category] VALUES ('Florists',null,@CategoryId,1,6)

    Insert Into [Exclusive].[Category] VALUES ('Charities',null,@CategoryId,1,7)

    Insert Into [Exclusive].[Category] VALUES ('Gifts',null,@CategoryId,1,8)
		
	Insert Into [Exclusive].[Category] VALUES ('Utilities',null,@CategoryId,1,9)

	Insert Into [Exclusive].[Category] VALUES ('Phone & Broadband',null,@CategoryId,1,10)
	
	Insert Into [Exclusive].[Category] VALUES ('TV Deals & Packages',null,@CategoryId,1,11)
	Insert Into [Exclusive].[Category] VALUES ('Comparison Sites',null,@CategoryId,1,12)
		
	Insert Into [Exclusive].[Category] VALUES ('Deal & Voucher Sites',null,@CategoryId,1,13)

	Insert Into [Exclusive].[Category] VALUES ('Finance',null,@CategoryId,1,14)
	
	Insert Into [Exclusive].[Category] VALUES ('Training & Recruitments',null,@CategoryId,1,15)
	Insert Into [Exclusive].[Category] VALUES ('Delivery Services',null,@CategoryId,1,16)
	Insert Into [Exclusive].[Category] VALUES ('Gadgets',null,@CategoryId,1,17)

	Insert Into [Exclusive].[Category] VALUES ('Car Accessories',null,@CategoryId,1,18)
		