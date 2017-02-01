USE [VehicleContent]
GO

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[CCWeb].[GetRewriteForCarCom]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP	PROCEDURE [CCWeb].[GetRewriteForCarCom]
GO

CREATE PROCEDURE [CCWeb].[GetRewriteForCarCom]
	@hostname varchar(255),
	@urlPath varchar(256),
	@enableLogging varchar(1)
AS


BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
	DECLARE @newURL varchar(255), @NewHostname varchar(100), @RedirectHostname bit
	DECLARE @StatusCode int, @articleIdIdx int, @cfmIndex int
	DECLARE @UrlPathInfo varchar(255), @UrlParams varchar(255), @StaticMMPath varchar(255), @MakeSearchUrl varchar(255), @ChooserUrl varchar(255), @ResearchUrl varchar(255), @CatSearchUrl varchar(255), @ReviewsUrl varchar(255), @BuyNewCarUrl varchar(255), @BuyUsedUrl varchar(255), @FinanceUrl varchar(255)
	DECLARE @makeRowId int, @modelRowId int, @seriesRowId int, @urlMake varchar(255), @urlModel varchar(255), @urlSuperModel varchar(255),@researchMake varchar(255), @researchModel varchar(255), @MakeId int, @SuperModel varchar(255), @urlSeriesId int, @articleRowId int, @contentId int, @urlText varchar(255)
	
	SET	@StatusCode = 301
	SET @ResearchUrl = 'content/research/detail/'	
	SET @CatSearchUrl = 'content/research/catsearch/'
	SET @ReviewsUrl = 'content/research/reviews/'
	SET @BuyNewCarUrl = 'content/buy/lm/newcar/'
	SET @BuyUsedUrl = 'content/buy/used/'
	SET @ChooserUrl = 'content/research/chooser/'
	SET @MakeSearchUrl = 'content/research/makesearch/'
	SET @FinanceUrl = 'content/buy/finance/'
	
	SET @articleIdIdx = PATINDEX('%article_id_int%', @urlPath)
	SET @cfmIndex = PATINDEX('%.cfm%', @urlPath)+4
	SET	@UrlPathInfo = SUBSTRING(@urlPath, 0, @cfmIndex)
	SET @UrlParams = SUBSTRING(@urlPath, @cfmIndex, LEN(@urlPath))
	-- PRINT 'urlPath = ' + @urlPath
	-- PRINT 'UrlParams = ' + @UrlParams
	
	SET @NewHostname = 'www.car.com'
	SET @RedirectHostname = 0

	IF @hostname != 'www.car.com'			
	BEGIN
		SET @RedirectHostname = 1
		
		--If domain is part of the exclusion list, do not redirect to www.car.com
		SELECT	@NewHostname = Hostname, @RedirectHostname = 0
		FROM	CCWeb.UrlRewriteDomainException
		WHERE	Hostname = @hostname
				AND	
				Active = 1
	END
	
	-- Redirect Make/SuperModel page for static "/content/research/catsearch/" pages
	IF CHARINDEX(@CatSearchUrl, @urlPath) > 0 
	BEGIN
		IF (CHARINDEX('pricelist', @urlPath) > 0) OR (CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0)
		SET @newURl = ''
	END	
	
	
	-- Redirect Make/SuperModel page for static "/content/buy/lm/newcar/" pages
	IF CHARINDEX(@BuyNewCarUrl, @urlPath) > 0 AND CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0
	BEGIN
		SET @newURl = 'car-research/'
	END
		
	-- Redirect Make/SuperModel page for static "/content/buy/used/" pages
	IF CHARINDEX(@BuyUsedUrl, @urlPath) > 0 AND CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0
	BEGIN
		SET @newURl = 'cars-for-sale/'
	END
	
	-- Redirect Make/SuperModel page for static "/content/research/chooser/" pages
	IF CHARINDEX(@ChooserUrl, @urlPath) > 0 
	BEGIN
		IF CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0
			SET @newURl = ''
		ELSE IF CHARINDEX('make_vch/any', @urlPath) >= 1
			SET @newUrl = ''
	END

	
	-- Redirect Make/SuperModel page for static "/content/research/chooser/" pages
	IF CHARINDEX(@MakeSearchUrl, @urlPath) > 0 AND CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0
	BEGIN
		SET @newURl = 'car-research/'
	END
	
	-- Redirect Make/SuperModel page for static "/content/research/reviews/" pages
	IF CHARINDEX(@ReviewsUrl, @urlPath) > 0 AND CHARINDEX('make_vch', @urlPath) = 0 AND CHARINDEX('model_vch', @urlPath) = 0
		AND CHARINDEX('Kurrentrow', @urlPath) = 0
	BEGIN
		SET @newURl = 'buying-guides/'
	END
	
	-- Redirect Make/SuperModel page for static "/content/buy/finance/" pages
	IF CHARINDEX(@FinanceUrl, @urlPath) > 0 AND CHARINDEX('calculator', @urlPath) = 0 
	BEGIN
		SET @newURl = 'finance/'
	END
	
	
	
	IF @newURL is null
	BEGIN
	
		-- Redirect Make/SuperModel page for static "/content/research/detail/{make}/{make}-{model}.htm" pages
		IF CHARINDEX(@ResearchUrl, @urlPath) > 0 AND CHARINDEX('.htm', @urlPath) > 0
		BEGIN
		
			SET @StaticMMPath = RIGHT(@urlPath, LEN(@urlPath) - (CHARINDEX(@ResearchUrl, @urlPath) + LEN(@ResearchUrl) - 1))
			-- PRINT 'StaticMMPath = ' + @StaticMMPath

			IF Len(@StaticMMPath) > 0 
				IF CHARINDEX('/', @StaticMMPath) > 0
				BEGIN
					SET @researchMake = LEFT(@StaticMMPath, CHARINDEX('/', @StaticMMPath)-1)
					--PRINT 'researchMake = ' + @researchMake
					SET @researchModel = RIGHT(@StaticMMPath, LEN(@StaticMMPath) - (CHARINDEX('/', @StaticMMPath) + LEN(@researchMake) + 1))
					--PRINT 'researchModel = ' + @researchModel
					IF CHARINDEX('.htm', @researchModel) > 0
					BEGIN
						SET @researchModel = LEFT(@researchModel, CHARINDEX('.htm', @researchModel)-1)
						--PRINT 'researchModel = ' + @researchModel
						SELECT	top 1 @urlSuperModel = sm.SeoName, @urlMake = ma.SeoName
						FROM	pvc.Trim t Inner join 
								pvc.Model m on t.ModelId = m.ModelId and @researchModel = Replace(m.Name, ' ', '-') Inner Join 
								pvc.SuperModel sm on t.SuperModelId = sm.SuperModelId join 
								pvc.Make ma on t.MakeId = ma.MakeId and @researchMake = Replace(ma.Name, ' ', '-')
						WHERE	t.isActive = 1 and t.IsNew = 1
								AND NOT EXISTS (SELECT * FROM pvc.DeactivatedMakes dm where dm.MakeId = m.MakeId)
						Order By t.YearInt desc
						--PRINT 'SuperModel = ' + @urlSuperModel
						
						IF @urlSuperModel is not null
							BEGIN
								SET @newURl = lower(Replace(RTRIM(LTRIM(@urlMake)), ' ', '-')) + '/' + @urlSuperModel + '/'
							END					
						ELSE 
							BEGIN
								SELECT	top 1 @urlMake = m.SeoName
								FROM	pvc.Trim t Inner join 
										pvc.Make m on t.MakeId = m.MakeId 
								WHERE	m.name = @researchMake and t.isActive = 1 and t.IsNew = 1
										AND NOT EXISTS (SELECT * FROM pvc.DeactivatedMakes dm where dm.MakeId = m.MakeId)
								Order By t.YearInt desc
								
								IF @urlMake is not null
									SET @newURl = @urlMake + '/'
								ELSE
									SET @newURl = 'car-research/'
							END
					END
				END 
				ELSE
				BEGIN
					SET @researchMake = LEFT(@StaticMMPath, CHARINDEX('.', @StaticMMPath)-1)
					-- PRINT 'researchMake = ' + @urlmake
					
						--PRINT 'researchModel = ' + @researchModel
						SELECT	top 1 @urlMake = m.SeoName
						FROM	pvc.Trim t Inner join 
								pvc.Make m on t.MakeId = m.MakeId 
						WHERE	m.name = @urlmake and t.isActive = 1 and t.IsNew = 1
								AND NOT EXISTS (SELECT * FROM pvc.DeactivatedMakes dm where dm.MakeId = m.MakeId)
						Order By t.YearInt desc
						--PRINT 'SuperModel = ' + @SuperModel
						IF @urlMake is not null
							SET @newURl = @urlMake + '/'
						ELSE
							SET @newURl = 'car-research/'

				END	
		END
	END
	
	IF @newURL is null
	BEGIN
	
		DECLARE @tempVariables TABLE
		(
			rowId int identity,
			item nvarchar(4000)
		)
		
		-- splits all parameters (not from queryparam) into database records and puts it into @tempVariables
		IF CHARINDEX('?', @urlParams) = 0
			INSERT INTO @tempVariables (item)
			SELECT item
			FROM Split(@urlParams, '/')
		
		-- splits all parameters (from queryparams) into database records and puts it into @tempvariables
		IF CHARINDEX('?', @urlParams) > 0
		BEGIN
			SET @UrlParams = REPLACE(@urlParams, '?', '') -- removes the question mark
			
			
			INSERT INTO @tempVariables (item)
			SELECT item
			FROM SPLIT((
			SELECT item
				FROM Split(@urlParams, '&') where item like '%make_vch%') , '=') a 
			
			INSERT INTO @tempVariables (item)	
			SELECT item
			FROM SPLIT((
			SELECT item
				FROM Split(@urlParams, '&') where item like '%model_vch%') , '=') a 
			
			INSERT INTO @tempVariables (item)
			SELECT item
			FROM SPLIT((
			SELECT item
				FROM Split(@urlParams, '&') where item like '%series_vch%') , '=') a 
			
		END	
		
		--SELECT *
		--from @tempVariables
		
		---------------MODEL-----------------
		SELECT top 1 @modelRowId = rowId
		FROM @tempVariables
		WHERE Item = 'model_vch'
		ORDER by rowId
		
		SELECT top 1 @urlModel = item	
		FROM @tempVariables
		WHERE rowId = @modelRowId+1
		--PRINT 'urlModel = ' + @urlModel

		IF @urlModel is not null
		BEGIN
		
			SELECT	top 1 @urlSuperModel = sm.SeoName
			FROM	pvc.Trim t Inner join 
					pvc.Model m on t.ModelId = m.ModelId Inner Join 
					pvc.SuperModel sm on t.SuperModelId = sm.SuperModelId
			WHERE	m.name = @urlModel and t.isActive = 1 and t.IsNew = 1
			Order By t.YearInt desc

			--PRINT 'SuperModel = ' + @urlSuperModel
		END
		---------------MAKE-----------------
		
		SELECT top 1 @makeRowId = rowId
		FROM @tempVariables
		WHERE Item = 'make_vch'
		ORDER by rowId desc -- need to order by just in case there are two make_vch in URL ex:/content/research/catsearch/index.cfm/action/SelectTrim/newused/new/category/CONV/make_vch/Audi/model_vch/A5/virsection/make_vch
			
			
		SELECT top 1 @urlMake = m.SeoName, @MakeId = m.MakeId
		FROM @tempVariables tv join pvc.Make m
			on tv.Item = m.Name
		WHERE rowId = @makeRowId+1 and item <> 'any' 
				
		
		IF @MakeId is not null AND EXISTS (SELECT * FROM pvc.DeactivatedMakes dm where dm.MakeId = @MakeId)
			BEGIN
				SET @urlMake = null
				SET @newURl = 'car-research/'
			END
		ELSE IF EXISTS (
			SELECT *
			FROM @tempVariables tv 
			WHERE rowId = @makeRowId+1 and item = 'Any' 
		)
			BEGIN
				SET @urlMake = null
				SET @newURl ='car-research/'
			END
		/*
		if @urlMake is null
		BEGIN
			SET @newURl = 'car-research/'		
		END
		*/
		
		-- PRINT @urlMake

		---------------SERIES-----------------
		if @urlMake is null
		BEGIN
		
			SELECT top 1 @seriesRowId = rowId
			FROM @tempVariables
			WHERE Item = 'seriesid'
			ORDER by rowId desc -- need to order by just in case there are two make_vch in URL ex:/content/research/catsearch/index.cfm/action/SelectTrim/newused/new/category/CONV/make_vch/Audi/model_vch/A5/virsection/make_vch
				--PRINT @makeRowId
				
			SELECT top 1 @urlSeriesId = item
			FROM @tempVariables
			WHERE rowId = @seriesRowId+1
			--PRINT @urlMake
			
			select top 1 @SuperModel = sm.SeoName, @urlMake = m.SeoName
			from CCWeb.UrlRewrite_SeriesDetail mms join pvc.Make m
				on mms.MakeId = m.MakeID join pvc.SuperModel sm
				on mms.SuperModelId = sm.SuperModelId join pvc.Trim t
					on sm.SuperModelId = t.SuperModelId
			where mms.SeriesId = @urlSeriesId and not exists (select * from pvc.DeactivatedMakes dm where dm.MakeId = m.MakeId)
				and t.IsNew = 1
				order by YearInt desc
			
			if @seriesRowId is null
			begin
				SELECT top 1 @seriesRowId = rowId
				FROM @tempVariables
				WHERE Item = 'series_id_int'
				ORDER by rowId desc -- need to order by just in case there are two make_vch in URL ex:/content/research/catsearch/index.cfm/action/SelectTrim/newused/new/category/CONV/make_vch/Audi/model_vch/A5/virsection/make_vch
					--PRINT @makeRowId
					
				SELECT top 1 @urlSeriesId = item
				FROM @tempVariables
				WHERE rowId = @seriesRowId+1
				--PRINT @urlMake
				
				select top 1 @SuperModel = sm.SeoName, @urlMake = m.SeoName
				from CCWeb.UrlRewrite_SeriesDetail mms join pvc.Make m
					on mms.MakeId = m.MakeId join pvc.SuperModel sm
					on mms.SuperModelId = sm.SuperModelId
				where mms.SeriesId = @urlSeriesId and not exists (select * from pvc.DeactivatedMakes dm where dm.MakeId = m.MakeId)


			end
		END
		
		--------------ARTICLE----------------

	 
			SELECT top 1 @articleRowId = rowId
			FROM @tempVariables
			WHERE Item = 'article_id_int'
			ORDER by rowId
			
			SELECT top 1 @contentId = cast(item as int)
			FROM @tempVariables
			WHERE rowId = @articleRowId+1
			-- PRINT 'contentId = ' + cast(@contentId as varchar)
		
			IF @contentId is not null
				BEGIN
				
				-- validate to match a supermodel		
				SELECT	top 1 @urlText = cv.URL
				FROM pvc.Trim t join pvc.SuperModel sm
					on t.SuperModelId = sm.SuperModelId join CCWeb.UrlRewrite_ContentVehicles cv
					on cv.URL like '%' + sm.SeoName + '/%'
				WHERE	cv.ContentId = @contentId 
					and t.IsNew = 1 and t.IsActive = 1
				
				-- then validate to match a make
				IF @urlText is null
					SELECT	top 1 @urlText = '/' + ma.SeoName + '/'
					FROM pvc.Trim t join pvc.Make ma
						on t.MakeId = ma.MakeId join CCWeb.UrlRewrite_ContentVehicles cv 
						on cv.URL like '%' + ma.SeoName + '/%'
					WHERE	cv.ContentId = @contentId 
						and t.IsNew = 1 and t.IsActive = 1
						and not exists (select * from pvc.DeactivatedMakes dm where dm.MakeId = ma.MakeId)
				END

				-- then validate to match a category
				IF @UrlText is null
					SELECT top 1 @urlText = cv.URL
					FROM pvc.VehicleCategory vc join CCWeb.UrlRewrite_ContentVehicles cv
						on cv.URL like '%' + vc.SeoName + '/%'
					WHERE cv.ContentId = @contentId
				
				-- then validate to match an attribute
				IF @UrlText is null
					SELECT top 1 @urlText = cv.URL
					FROM pvc.VehicleAttribute va join CCWeb.UrlRewrite_ContentVehicles cv
						on cv.URL like '%' + va.SeoName + '/%'
					WHERE cv.ContentId = @contentId
		
				-- then validate to match an attribute
				IF @UrlText is null
					SELECT top 1 @urlText = cv.URL
					FROM CCWeb.UrlRewrite_ContentVehicles cv
					WHERE cv.ContentId = @contentId and cv.URL = 'car-research/'
		
		--------------------------------------

		IF @urlText is not null 
		
			SET @newURl = @urlText
		
		ELSE IF @contentId is not null
		
			SET @newUrl = ''
		
		ELSE IF @urlMake is not null AND @urlSuperModel is not null

			SET @newURl = @urlMake + '/' + @urlSuperModel + '/'

		ELSE IF @urlMake is not null
		
			SET @newURl = @urlMake + '/'

			

		-- Get value of model if model_vch is defined as /model_vch/
		/*IF PATINDEX('%model_vch/%', @urlPath) > 0
			BEGIN
				SELECT @modelRowId = rowId
				FROM @tempVariables
				WHERE Item = 'model_vch'
				
				SELECT @urlModel = item
				FROM @tempVariables
				WHERE rowId = @modelRowId+1
				--PRINT 'urlModel = ' + @urlModel

				IF @urlModel is not null
				BEGIN
					SELECT	top 1 @SuperModel = sm.Name
					FROM	pvc.Trim t Inner join 
							pvc.Model m on t.ModelId = m.ModelId Inner Join 
							pvc.SuperModel sm on t.SuperModelId = sm.SuperModelId
					WHERE	m.name = @urlModel and t.isActive = 1
					Order By t.YearInt desc

					--PRINT 'SuperModel = ' + @SuperModel
				END

			END*/
		
		-- Get value of model if model_vch is defined as model_vch=	
		/*ELSE IF PATINDEX ('%model_vch=%', @urlParams) > 0
			BEGIN
				SELECT * 
				FROM Split((
					SELECT Item
					FROM Split(@urlParams, '&')
					WHERE Item like 'model_vch=%' ), '=') a
			END	*/

		-- Get value of make if make_vch is defined
		
		
		/*IF PATINDEX('%make_vch/%', @urlPath) > 0
		BEGIN
			SELECT @makeRowId = rowId
			FROM Split(@urlParams, '/')
			WHERE Item = 'make_vch'
			ORDER by rowId desc -- need to order by just in case there are two make_vch in URL ex:/content/research/catsearch/index.cfm/action/SelectTrim/newused/new/category/CONV/make_vch/Audi/model_vch/A5/virsection/make_vch
			--PRINT @makeRowId
			
			SELECT @urlMake = item
			FROM Split(@urlParams, '/')
			WHERE rowId = @makeRowId+1 and item <> 'any'
			--PRINT @urlMake

			IF @urlMake is not null AND @SuperModel is not null
			BEGIN
				SET @newURl = lower(Replace(RTRIM(LTRIM(@urlMake)), ' ', '-')) + '/' + lower(Replace(RTRIM(LTRIM(@SuperModel)), ' ', '-')) + '/'
			END
			ELSE IF @urlMake is not null
			BEGIN
				SET @newURl = lower(Replace(RTRIM(LTRIM(@urlMake)), ' ', '-')) + '/'
			END
		END	*/
	END
	
	
	
	--print 'newurl = ' + @newURL
	--print 'urlpath = ' + @urlPath
	
	IF @newUrl is null
	BEGIN
		SELECT	@newURL = ResponsePath,@StatusCode = HTTPStatusCode
		FROM	CCWeb.UrlRewrite
		WHERE	RequestPath = @urlPath
				and
				Active = 1
	END		

	IF @newURl is not null
	BEGIN		

		IF @RedirectHostname = 1
		BEGIN
			SET @newURL = 'http://' + @NewHostName + '/' + @newURL
			--PRINT 'Hostname redirect: ' + @NewHostName
		END
		ELSE
		BEGIN
			--if @newUrl is the default index page - add hostname so url does not return a blank space
			IF RTRIM(@newURL) = ''
			BEGIN
				SET @newURL = 'http://' + @hostName + '/'
			END
		END
		BEGIN
		  IF @statusCode = 301
		  BEGIN
			  SELECT 'redirect:' + lower(@newURL) as newUrl
		  END
		  ELSE IF @statusCode = 300
		  BEGIN
			  SELECT 'rewrite:' + lower(@newURL) as newUrl
		  END
		END
	END
	ELSE
	BEGIN		
		IF @RedirectHostname = 1
		BEGIN
			SET @newURL = 'http://' + @NewHostName + '/' + @urlPath
		  IF @statusCode = 301
		  BEGIN
			  SELECT 'redirect:' + lower(@newURL) as newUrl
		  END
		  ELSE IF @statusCode = 300
		  BEGIN
			  SELECT 'rewrite:' + lower(@newURL) as newUrl
		  END
			--PRINT 'Hostname redirect - new url: ' + @newUrl			
		END
	END
	
	IF @enableLogging = 1
	BEGIN
		exec ccweb.UrlRewriteLogRequest 'GetRewriteForCarCom', @urlPath, @newURL, @statusCode, @hostname
	END
	
END
