CREATE PROCEDURE [dbo].[Znode_ManageCategoryList_XML](
      @WhereClause      XML ,
      @Rows             INT            = 100 ,
      @PageNo           INT            = 1 ,
      @Order_BY         NVARCHAR(1000) = '' ,
      @LocaleId         INT            = 1 ,
	  @PimCatalogId     INT            = 0,
	  @IsCatalogFilter   BIT            = 0)
AS
    /*
	   Summary: This Procedure is used to get all category list 
				The Result displays CategortName with PimCategoryId where CategoryName and CategoryImage are pivoted values
	   Unit Testing 	  
	   EXEC Znode_ManageCategoryList_XML '' ,@LocaleId= 1
	
    */
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
             DECLARE @TBL_PimcategoryDetails TABLE (PimCategoryId INT,CountId INT,RowId INT);
             DECLARE @TBL_CategoryIds TABLE (PimCategoryId INT ,ParentPimcategoryId INT);
             DECLARE @TBL_AttributeValue TABLE (PimCategoryAttributeValueId INT,PimCategoryId INT,CategoryValue NVARCHAR(MAX), AttributeCode VARCHAR(300), PimAttributeId  INT);
             DECLARE @TBL_FamilyDetails TABLE (PimCategoryId INT , PimAttributeFamilyId INT , AttributeFamilyName  NVARCHAR(MAX));
             DECLARE @TBL_DefaultAttributeValue TABLE (PimAttributeId INT ,AttributeDefaultValueCode VARCHAR(600) , IsEditable BIT ,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder int);
			 DECLARE @TBL_MediaAttribute TABLE (Id INT ,PimAttributeId INT ,AttributeCode VARCHAR(600) )
             DECLARE @TBL_ProfileCatalogCategory TABLE (ProfileCatalogId INT ,PimCategoryId INT);
             DECLARE @PimCategoryIds VARCHAR(MAX)= '' , @PimAttributeIds VARCHAR(MAX),@CategoryXML NVARCHAr(max);
			 DECLARE @RowsCount INT 

		 	IF @PimCatalogId = -1
			BEGIN
			SET @PimCategoryIds = SUBSTRING( ( SELECT DISTINCT ','+CAST(PimCategoryId AS VARCHAR(2000))
			FROM ZnodePimCategory ZPC 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryHierarchy ZPCC 
			WHERE ZPC.PimCategoryId = ZPCC.PimCategoryId)
			AND ZPC.PimCategoryId IS NOT NULL
			FOR XML PATH('') ) , 2 , 4000);

			END
			ELSE IF @PimCatalogId <> 0 AND @PimCatalogId <> -1
			BEGIN
			SET @PimCategoryIds = SUBSTRING( ( SELECT DISTINCT ','+CAST(PimCategoryId AS VARCHAR(2000)) 
			FROM ZnodePimCategory ZPC 
			WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryHierarchy ZPCC WHERE ZPC.PimCategoryId = ZPCC.PimCategoryId
			AND ZPCC.PimCatalogId = @PimCatalogId)
			AND ZPC.PimCategoryId IS NOT NULL
			FOR XML PATH('') ) , 2 , 4000);
			END
			ELSE IF @PimCatalogId = 0 AND @IsCatalogFilter = 1 -- filter for all catalog category except category which are not associated with any catalog
			BEGIN
			SET @PimCategoryIds = SUBSTRING( ( SELECT DISTINCT ','+CAST(PimCategoryId AS VARCHAR(2000)) 
			FROM ZnodePimCategory ZPC 
			WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryHierarchy ZPCC WHERE ZPC.PimCategoryId = ZPCC.PimCategoryId)
			AND ZPC.PimCategoryId IS NOT NULL
			FOR XML PATH('') ) , 2 , 4000);
			END

			

              INSERT INTO @TBL_PimcategoryDetails ( PimCategoryId , CountId , RowId)
             EXEC Znode_GetCategoryIdForPaging @WhereClause , @Rows , @PageNo , @Order_BY , @RowsCount  , @LocaleId , '' , @PimCategoryIds , 1; 


             SET @PimCategoryIds =  SUBSTRING( ( SELECT ','+ CAST(PimCategoryId AS VARCHAR(100)) FROM @TBL_PimcategoryDetails FOR XML PATH('')) , 2 , 4000);
             SET @PimAttributeIds = SUBSTRING( ( SELECT ','+ CAST(PimAttributeId AS VARCHAR(100)) FROM [dbo].[Fn_GetCategoryGridAttributeDetails]() FOR XML PATH('')) , 2 , 4000);

			 INSERT INTO @TBL_AttributeValue ( PimCategoryAttributeValueId , PimCategoryId , CategoryValue , AttributeCode , PimAttributeId)
             EXEC [dbo].[Znode_GetCategoryAttributeValue] @PimCategoryIds , @PimAttributeIds , @LocaleId;

             INSERT INTO @TBL_FamilyDetails ( PimAttributeFamilyId , AttributeFamilyName , PimCategoryId)
             EXEC Znode_GetCategoryFamilyDetails @PimCategoryIds , @LocaleId;
             
		     INSERT INTO @TBL_DefaultAttributeValue ( PimAttributeId , AttributeDefaultValueCode , IsEditable , AttributeDefaultValue,DisplayOrder)
             EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @PimAttributeIds , @LocaleId;
             
			 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_PimcategoryDetails ),0)
			 
			 INSERT INTO @TBL_MediaAttribute (Id,PimAttributeId,AttributeCode )
			 SELECT Id,PimAttributeId,AttributeCode 
			 FROM [dbo].[Fn_GetProductMediaAttributeId]()
			 		
		     ;WITH Cte_DefaultCategoryValue
              AS (SELECT PimCategoryId , PimAttributeId ,SUBSTRING( ( SELECT ','+AttributeDefaultValue FROM @TBL_DefaultAttributeValue AS TBDAV WHERE TBDAV.PimAttributeId = TBAV.PimAttributeId 
			     AND EXISTS ( SELECT TOP 1 1 FROM dbo.Split ( TBAV.CategoryValue , ',') AS SP WHERE sp.Item = TBDAV.AttributeDefaultValueCode)
                 FOR XML PATH('')) , 2 , 4000) AS AttributeValue FROM @TBL_AttributeValue AS TBAV							 
				 WHERE EXISTS ( SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryDefaultValueAttribute]() AS SP WHERE SP.PimAttributeId = TBAV.PimAttributeId))

             UPDATE TBAV SET TBAV.CategoryValue = CTDCV.AttributeValue
             FROM @TBL_AttributeValue TBAV 
			 INNER JOIN Cte_DefaultCategoryValue CTDCV ON ( CTDCV.PimCategoryId = TBAV.PimCategoryId AND CTDCV.PimAttributeId = TBAV.PimAttributeId );
                   
		    UPDATE  TBAV
			SET CategoryValue  = SUBSTRING ((SELECT ','+[dbo].FN_GetMediaThumbnailMediaPath(zm.Path) FROM ZnodeMedia ZM  WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(TBAV.CategoryValue ,',') SP  WHERE SP.Item = CAST(ZM.MediaId AS VARCHAR(50)) ) FOR XML PATH('')),2,4000)
			FROM @TBL_AttributeValue TBAV 
			INNER JOIN @TBL_MediaAttribute TBMA ON (TBMA.PimAttributeId = TBAV.PimAttributeId)	   
			
			INSERT INTO @TBL_AttributeValue ( PimCategoryId , CategoryValue , AttributeCode )

            SELECT PimCategoryId,AttributeFamilyName , 'AttributeFamily'
			FROM @TBL_FamilyDetails 				                           
		    

			INSERT INTO @TBL_AttributeValue             ( PimCategoryId , CategoryValue , AttributeCode     )
			--SELECT a.PimCategoryId  ,
			--CASE WHEN IsCategoryPublish = 1 THEN   'Published' WHEN IsCategoryPublish = 0 
			--THEN 'Draft'  ELSE 'Not Published' END, 'PublishStatus'
			--FROM @TBL_PimcategoryDetails a 
			--INNER JOIN ZnodePimCategory b ON (b.PimCategoryId = a.PimCategoryId)

			SELECT a.PimCategoryId PimCategoryId 
			,th.DisplayName, 'PublishStatus'
			FROM @TBL_PimcategoryDetails a 
			INNER JOIN ZnodePimCategory b ON (b.PimCategoryId = a.PimCategoryId)
			LEFT JOIN ZnodePublishState th ON (th.PublishStateId = b.PublishStateId)

			if (@PimCatalogId = -1)
			begin
				SET @CategoryXML =  '<MainCategory>'+ STUFF( (  SELECT '<Category>'+'<PimCategoryId>'+CAST(TBAD.PimCategoryId AS VARCHAR(50))+'</PimCategoryId>'+ STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST((SELECT ''+TBADI.CategoryValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'   
															FROM @TBL_AttributeValue TBADI      
																WHERE TBADI.PimCategoryId = TBAD.PimCategoryId 
																ORDER BY TBADI.PimCategoryId DESC
																FOR XML PATH (''), TYPE
																).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Category>'	   

				FROM @TBL_AttributeValue TBAD
				INNER JOIN @TBL_PimcategoryDetails TBPI ON (TBAD.PimCategoryId = TBPI.PimCategoryId )
				WHERE not exists(select * from ZnodePimCategoryHierarchy ZPCH Where TBAD.PimCategoryId = ZPCH.PimCategoryId)
				GROUP BY TBAD.PimCategoryId,TBPI.RowId 
				ORDER BY TBPI.RowId 
				FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainCategory>'
			end
			else if (@PimCatalogId = 0)
			begin
				SET @CategoryXML =  '<MainCategory>'+ STUFF( (  SELECT '<Category>'+'<PimCategoryId>'+CAST(TBAD.PimCategoryId AS VARCHAR(50))+'</PimCategoryId>'+ STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST((SELECT ''+TBADI.CategoryValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'   
														FROM @TBL_AttributeValue TBADI      
															WHERE TBADI.PimCategoryId = TBAD.PimCategoryId 
															ORDER BY TBADI.PimCategoryId DESC
															FOR XML PATH (''), TYPE
															).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Category>'	   

				FROM @TBL_AttributeValue TBAD
				INNER JOIN @TBL_PimcategoryDetails TBPI ON (TBAD.PimCategoryId = TBPI.PimCategoryId )
				GROUP BY TBAD.PimCategoryId,TBPI.RowId 
				ORDER BY TBPI.RowId 
				FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainCategory>'
			end
			else if (@PimCatalogId > 0)
			begin
				SET @CategoryXML =  '<MainCategory>'+ STUFF( (  SELECT '<Category>'+'<PimCategoryId>'+CAST(TBAD.PimCategoryId AS VARCHAR(50))+'</PimCategoryId>'+ STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST((SELECT ''+TBADI.CategoryValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'   
														FROM @TBL_AttributeValue TBADI      
															WHERE TBADI.PimCategoryId = TBAD.PimCategoryId 
															ORDER BY TBADI.PimCategoryId DESC
															FOR XML PATH (''), TYPE
															).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Category>'	   

				FROM @TBL_AttributeValue TBAD
				INNER JOIN @TBL_PimcategoryDetails TBPI ON (TBAD.PimCategoryId = TBPI.PimCategoryId )
				WHERE exists(select * from ZnodePimCategoryHierarchy ZPCH Where TBAD.PimCategoryId = ZPCH.PimCategoryId and ZPCH.PimCatalogId = @PimCatalogId)
				GROUP BY TBAD.PimCategoryId,TBPI.RowId 
				ORDER BY TBPI.RowId 
				FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainCategory>'
			end

			SELECT  @CategoryXML  CategoryXMl
		   
		     SELECT AttributeCode ,  ZPAL.AttributeName
			 FROM ZnodePimAttribute ZPA 
			 LEFT JOIN ZnodePiMAttributeLOcale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )
             WHERE LocaleId = 1 
			 AND IsCategory = 1  
			 AND ZPA.IsShowOnGrid = 1  
			 UNION ALL 
			 SELECT 'PublishStatus','Publish Status'
		    SELECT ISNULL(@RowsCount,0) AS RowsCount;



			
			 --SELECT  PIV.PimCategoryId , PIV.CategoryName , ZPC.IsActive AS [Status] , dbo.FN_GetMediaThumbnailMediaPath ( Zm.Path ) AS CategoryImage , @LocaleId AS LocaleId , ISNULL(TBFD.AttributeFamilyName , '') AS AttributeFamilyName
             
			 --FROM @TBL_PimcategoryDetails AS TBCD 
			 --INNER JOIN ( SELECT PimCategoryId , CategoryValue , AttributeCode  FROM @TBL_AttributeValue) AS TBAV PIVOT(MAX(CategoryValue)                                                             
			 --FOR AttributeCode IN([CategoryName] , [CategoryImage])) PIV ON ( PIV.PimCategoryId = TBCD.PimCategoryId )
			 --LEFT JOIN @TBL_FamilyDetails AS TBFD ON ( TBFD.PimCategoryId = PIV.PimCategoryId )
			 --LEFT JOIN ZnodeMedia AS ZM ON ( CAST(ZM.MediaId AS VARCHAR(50)) = PIV.[CategoryImage] )
			 --LEFT JOIN ZnodePimCategory AS ZPC ON ( ZPC.PimCategoryId = PIV.PimCategoryId ) 
			 --ORDER BY RowId;
             
             --SET @RowsCount = ISNULL( ( SELECT TOP 1 CountId FROM @TBL_PimcategoryDetails) , 0);
         END TRY
         BEGIN CATCH
                DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageCategoryList_XML @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageCategoryList_XML',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;