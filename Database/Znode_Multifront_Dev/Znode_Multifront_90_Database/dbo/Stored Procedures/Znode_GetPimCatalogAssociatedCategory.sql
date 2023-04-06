CREATE PROCEDURE [dbo].[Znode_GetPimCatalogAssociatedCategory]
(	@WhereClause      XML,
	@Rows             INT           = 100,
	@PageNo           INT           = 1,
	@Order_BY         VARCHAR(1000) = '',
	@RowsCount        INT OUT,
	@LocaleId         INT           = 1,
	@PimCatalogId     INT           = 0,
	@IsAssociated     BIT           = 0,
	@PimCategoryId    INT           = -1,
	@PimCategoryHierarchyId INT = 0 )
AS
/*
     Summary :- This procedure is used to get the attribute values as per changes 
     Unit Testing 
	 begin tran
     EXEC [dbo].[Znode_GetPimCatalogAssociatedCategory] @WhereClause = '',@RowsCount = 0 ,@PimCatalogId = 5 ,@IsAssociated = 1,@PimCategoryId = -1
     rollback tran
	 SELECT * FROM ZnodePimCategoryHierarchy
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_PimcategoryDetails TABLE
             (PimCategoryId INT,
              CountId       INT,
              RowId         INT
             );
             DECLARE @TBL_CategoryIds TABLE
             (PimCategoryId       INT,
              ParentPimcategoryId INT
             );
             DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             DECLARE @TBL_FamilyDetails TABLE
             (PimCategoryId        INT,
              PimAttributeFamilyId INT,
              AttributeFamilyName  NVARCHAR(MAX)
             );
             DECLARE @TBL_DefaultAttributeValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(600),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder             INT 
             );

             DECLARE @PimCategoryIds VARCHAR(MAX), @PimAttributeIds VARCHAR(MAX);
     
						INSERT INTO @TBL_CategoryIds(PimCategoryId,ParentPimcategoryId )
						SELECT PimCategoryId,ParentPimcategoryId 
						FROM  [dbo].[Fn_GetRecurciveCategoryIds_new](@PimCategoryHierarchyId, @PimCatalogId) AS FNGTRCT 
						UNION ALL 
					    SELECT ass.PimCategoryId,null
						FROM  ZnodePimCategoryHierarchy ass 
						WHERE (ass.ParentPimCategoryHierarchyId =  @PimCategoryHierarchyId   OR ( @PimCategoryHierarchyId = -1 AND ass.ParentPimCategoryHierarchyId IS NULL ))
						AND ass.PimCatalogId =@PimCatalogId
						 SET @IsAssociated = 0                                                                                                                               

						SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(100)) 
						FROM @TBL_CategoryIds  FOR XML PATH('') ), 2, 4000);
                        
                        INSERT INTO @TBL_PimcategoryDetails(PimCategoryId, CountId,RowId  )                      
                        EXEC Znode_GetCategoryIdForPaging @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount,@LocaleId,'',@PimCategoryIds,@IsAssociated;
						
						SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_PimcategoryDetails), 0);
																																														
						SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(100)) FROM @TBL_PimcategoryDetails FOR XML PATH('') ), 2, 4000);
																																																																				
						SET @PimAttributeIds = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(100)) FROM [dbo].[Fn_GetCategoryGridAttributeDetails]()
											   FOR XML PATH('')	), 2, 4000);	
						DECLARE @TBL_MediaAttribute TABLE (Id INT ,PimAttributeId INT ,AttributeCode VARCHAR(600) )

						 INSERT INTO @TBL_MediaAttribute (Id,PimAttributeId,AttributeCode )
						 SELECT Id,PimAttributeId,AttributeCode 
						 FROM [dbo].[Fn_GetProductMediaAttributeId]()
																																																															
						INSERT INTO @TBL_AttributeValue(PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
						EXEC [dbo].[Znode_GetCategoryAttributeValue]@PimCategoryIds,@PimAttributeIds,@LocaleId;

						INSERT INTO @TBL_FamilyDetails(PimAttributeFamilyId,AttributeFamilyName,PimCategoryId)
						EXEC Znode_GetCategoryFamilyDetails @PimCategoryIds,@LocaleId;
							
							
						INSERT INTO @TBL_DefaultAttributeValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder )
						EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @PimAttributeIds,@LocaleId;
						
						INSERT INTO @TBL_AttributeValue ( PimCategoryId , CategoryValue , AttributeCode )

						SELECT PimCategoryId,AttributeFamilyName , 'AttributeFamily'
						FROM @TBL_FamilyDetails 		

						UPDATE  TBAV
						SET CategoryValue  = SUBSTRING ((SELECT ','+[dbo].FN_GetMediaThumbnailMediaPath(zm.Path) FROM ZnodeMedia ZM  WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(TBAV.CategoryValue ,',') SP  WHERE SP.Item = CAST(ZM.MediaId AS VARCHAR(50)) ) FOR XML PATH('')),2,4000)
						FROM @TBL_AttributeValue TBAV 
						INNER JOIN @TBL_MediaAttribute TBMA ON (TBMA.PimAttributeId = TBAV.PimAttributeId)	

						DECLARE @CategoryXML XML 

						SET @CategoryXML =  '<MainCategory>'+ STUFF( ( SELECT '<Category>'+'<PimCategoryId>'+CAST(TBAD.PimCategoryId AS VARCHAR(50))+'</PimCategoryId>'+ STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST((SELECT ''+TBADI.CategoryValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'   
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


			SELECT  @CategoryXML  CategoryXMl
		   
		     SELECT AttributeCode ,  ZPAL.AttributeName
			 FROM ZnodePimAttribute ZPA 
			 LEFT JOIN ZnodePiMAttributeLOcale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )
             WHERE LocaleId = 1 
			 AND IsCategory = 1  
			 AND ZPA.IsShowOnGrid = 1  

		    SELECT ISNULL(@RowsCount,0) AS RowsCount;
						

         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE()
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimCatalogAssociatedCategory @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+			 ',@PimCategoryId='+CAST(@PimCategoryId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimCatalogAssociatedCategory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
