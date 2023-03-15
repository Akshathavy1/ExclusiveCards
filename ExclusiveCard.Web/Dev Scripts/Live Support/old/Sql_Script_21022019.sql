insert into Exclusive.Status values ('Processing', 'Import', 1)

INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190221095827_UrlSlug', N'2.1.4-rtm-31024')
GO

ALTER TABLE Exclusive.Category
ADD UrlSlug NVARCHAR(60) NULL

GO

/****** Object:  Index [IX_Category_UrlSlug]    Script Date: 21-Feb-19 5:34:51 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Category_UrlSlug] ON [Exclusive].[Category]
(
	[UrlSlug] ASC
)
WHERE ([UrlSlug] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

DECLARE @cateName NVARCHAR(50)
DECLARE @subCateName NVARCHAR(50)
DECLARE @urlSlug NVARCHAR(60)
DECLARE @tempSlug NVARCHAR(60)
DECLARE @index int = 1
DECLARE @count int
DECLARE @parentId INT
DECLARE @slugCount INT
DECLARE @counter INT

SELECT * INTO #TEMPTABLE FROM Exclusive.Category

SELECT @count = COUNT(*) FROM #TEMPTABLE

WHILE(@index <= @count)
	BEGIN
		--GET Parent Id
		SELECT @parentId = parentId from Exclusive.Category WHERE ID = @index

		--CHECK if parent
		IF(@parentId = 0)
			BEGIN
				SELECT @cateName = LOWER(RTRIM(LTRIM(name))) FROM Exclusive.Category WHERE ID = @index
				
				--REPLACE SPACE BY _
				SET @cateName = REPLACE(@cateName, ' ', '_')
				SET @cateName = REPLACE(@cateName, ' & ', '_')
				SET @cateName = REPLACE(@cateName, '&', '')
				SET @cateName = REPLACE(@cateName, ',', '')

				--UPDATE Slug
				UPDATE Exclusive.Category SET
				UrlSlug = REPLACE(@cateName, '__', '_') WHERE ID = @index
			END
		ELSE
			BEGIN
				SELECT @cateName = LOWER(RTRIM(LTRIM(name))) FROM Exclusive.Category WHERE ID = @parentId
				SELECT @subCateName = LOWER(RTRIM(LTRIM(name))) FROM Exclusive.Category WHERE ID = @index

				--REPLACE SPACE BY _
				SET @cateName = REPLACE(@cateName, ' ', '_')
				SET @cateName = REPLACE(@cateName, ' & ', '_')
				SET @cateName = REPLACE(@cateName, '&', '')
				SET @cateName = REPLACE(@cateName, ',', '')
				SET @subCateName = REPLACE(@subCateName, ' ', '_')
				SET @subCateName = REPLACE(@subCateName, ' & ', '_')
				SET @subCateName = REPLACE(@subCateName, '&', '')
				SET @subCateName = REPLACE(@subCateName, ',', '')

				--CHECK Slug name exists
				SET @urlSlug = CONCAT(SUBSTRING(@cateName, 1, 3), '_', @subCateName)
				SET @tempSlug = null

				--CHECK if slug exists
				SELECT @slugCount = COUNT(*) FROM Exclusive.Category WHERE UrlSlug = @urlSlug
				SET @counter = 1
				WHILE(@slugCount > 0)
					BEGIN
						SET @tempSlug = CONCAT(@urlSlug, '_', @counter)
						SELECT @slugCount = COUNT(*) FROM Exclusive.Category WHERE UrlSlug = @tempSlug
						SET @counter = @counter + 1
					END

				IF(@tempSlug IS NULL)
					BEGIN
						UPDATE Exclusive.Category SET
						UrlSlug = REPLACE(@urlSlug, '__', '_') WHERE ID = @index
					END
				ELSE
					BEGIN
						UPDATE Exclusive.Category SET
						UrlSlug = REPLACE(@tempSlug, '__', '_') WHERE ID = @index
					END
			END


		--increment counter
		SET @index = @index + 1
	END
	DROP TABLE #TEMPTABLE