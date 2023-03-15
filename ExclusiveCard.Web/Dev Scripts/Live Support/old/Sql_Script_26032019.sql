UPDATE A SET
A.ImagePath = REPLACE(ImagePath COLLATE Latin1_General_BIN, 'Adverts', 'images') 
FROM [ExclusiveCard].[CMS].[Adverts] A