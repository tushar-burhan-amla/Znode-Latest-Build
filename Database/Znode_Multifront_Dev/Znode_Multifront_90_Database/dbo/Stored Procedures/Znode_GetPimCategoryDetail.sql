CREATE PROCEDURE [dbo].[Znode_GetPimCategoryDetail]
(
      @WhereClause      XML ,
      @Rows             INT            = 100 ,
      @PageNo           INT            = 1 ,
      @Order_BY         NVARCHAR(1000) = '' ,
      @RowsCount        INT OUT ,
      @LocaleId         INT            = 1 

)
AS
    /*
	   Summary: This Procedure is used to get all category list 
				The Result displays CategortName with PimCategoryId where CategoryName and CategoryImage are pivoted values
	   Unit Testing 
	   begin tran
	   EXEC Znode_GetPimCategoryDetail '' , @RowsCount = 0 ,@LocaleId= 1
	   rollback tran
    */
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @TBL_PimcategoryDetails TABLE (PimCategoryId INT,CountId INT,RowId INT);
             DECLARE @TBL_CategoryIds TABLE (PimCategoryId INT ,ParentPimcategoryId INT);
             DECLARE @TBL_AttributeValue TABLE (PimCategoryAttributeValueId INT,PimCategoryId INT,CategoryValue NVARCHAR(MAX), AttributeCode VARCHAR(300), PimAttributeId  INT);
             DECLARE @TBL_FamilyDetails TABLE (PimCategoryId INT , PimAttributeFamilyId INT , AttributeFamilyName  NVARCHAR(MAX));
             DECLARE @TBL_DefaultAttributeValue TABLE (PimAttributeId INT ,AttributeDefaultValueCode VARCHAR(600) , IsEditable BIT ,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder int);
             --DECLARE @TBL_ProfileCatalogCategory TABLE (ProfileCatalogId INT ,PimCategoryId INT);
             DECLARE @TBL_ProfileCatalogCategory TABLE (PimCategoryId INT);
             DECLARE @PimCategoryIds VARCHAR(MAX)= '' , @PimAttributeIds VARCHAR(MAX);


			 INSERT INTO @TBL_ProfileCatalogCategory (PimCategoryId)
			 SELECT distinct ZCC.PimCategoryId
			 FROM ZnodePimCategoryProduct AS ZCC 
			 INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZCC.PimCategoryId = ZPCH.PimCategoryId
			 INNER JOIN ZnodeProfile AS ZP ON ( ZP.PimCatalogId = ZPCH.PimCatalogId )
			        
			 SET @PimCategoryIds = SUBSTRING( ( SELECT DISTINCT ','+CAST(PimCategoryId AS VARCHAR(20)) FROM @TBL_ProfileCatalogCategory FOR XML PATH('') ) , 2 , 4000);
			 --END;

             INSERT INTO @TBL_PimcategoryDetails ( PimCategoryId , CountId , RowId)
             EXEC Znode_GetCategoryIdForPaging @WhereClause , @Rows , @PageNo , @Order_BY , @RowsCount , @LocaleId , '' , @PimCategoryIds , 1; 

             SET @PimCategoryIds =  SUBSTRING( ( SELECT ','+ CAST(PimCategoryId AS VARCHAR(100)) FROM @TBL_PimcategoryDetails FOR XML PATH('')) , 2 , 4000);
             SET @PimAttributeIds = SUBSTRING( ( SELECT ','+ CAST(PimAttributeId AS VARCHAR(100)) FROM [dbo].[Fn_GetCategoryGridAttributeDetails]() FOR XML PATH('')) , 2 , 4000);

			 INSERT INTO @TBL_AttributeValue ( PimCategoryAttributeValueId , PimCategoryId , CategoryValue , AttributeCode , PimAttributeId)
             EXEC [dbo].[Znode_GetCategoryAttributeValue] @PimCategoryIds , @PimAttributeIds , @LocaleId;

             INSERT INTO @TBL_FamilyDetails ( PimAttributeFamilyId , AttributeFamilyName , PimCategoryId)
             EXEC Znode_GetCategoryFamilyDetails @PimCategoryIds , @LocaleId;
             
		     INSERT INTO @TBL_DefaultAttributeValue ( PimAttributeId , AttributeDefaultValueCode , IsEditable , AttributeDefaultValue,DisplayOrder)
             EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @PimAttributeIds , @LocaleId;
             		
		     ;WITH Cte_DefaultCategoryValue
              AS (SELECT PimCategoryId , PimAttributeId ,SUBSTRING( ( SELECT ','+AttributeDefaultValue FROM @TBL_DefaultAttributeValue AS TBDAV WHERE TBDAV.PimAttributeId = TBAV.PimAttributeId 
			     AND EXISTS ( SELECT TOP 1 1 FROM dbo.Split ( TBAV.CategoryValue , ',') AS SP WHERE sp.Item = TBDAV.AttributeDefaultValueCode)
                 FOR XML PATH('')) , 2 , 4000) AS AttributeValue FROM @TBL_AttributeValue AS TBAV							 
				 WHERE EXISTS ( SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryDefaultValueAttribute]() AS SP WHERE SP.PimAttributeId = TBAV.PimAttributeId))

             UPDATE TBAV SET TBAV.CategoryValue = CTDCV.AttributeValue
             FROM @TBL_AttributeValue TBAV INNER JOIN Cte_DefaultCategoryValue CTDCV ON ( CTDCV.PimCategoryId = TBAV.PimCategoryId AND CTDCV.PimAttributeId = TBAV.PimAttributeId );
                                           
		     SELECT  PIV.PimCategoryId , PIV.CategoryName , ZPC.IsActive AS [Status] , dbo.FN_GetMediaThumbnailMediaPath ( Zm.Path ) AS CategoryImage , @LocaleId AS LocaleId , ISNULL(TBFD.AttributeFamilyName , '') AS AttributeFamilyName
             FROM @TBL_PimcategoryDetails AS TBCD INNER JOIN ( SELECT PimCategoryId , CategoryValue , AttributeCode  FROM @TBL_AttributeValue) AS TBAV PIVOT(MAX(CategoryValue)                                                             
			 FOR AttributeCode IN([CategoryName] , [CategoryImage])) PIV ON ( PIV.PimCategoryId = TBCD.PimCategoryId )
			 LEFT JOIN @TBL_FamilyDetails AS TBFD ON ( TBFD.PimCategoryId = PIV.PimCategoryId )
			 LEFT JOIN ZnodeMedia AS ZM ON ( CAST(ZM.MediaId AS VARCHAR(50)) = PIV.[CategoryImage] )
			 LEFT JOIN ZnodePimCategory AS ZPC ON ( ZPC.PimCategoryId = PIV.PimCategoryId ) ORDER BY RowId;
             
             SET @RowsCount = ISNULL( ( SELECT TOP 1 CountId FROM @TBL_PimcategoryDetails) , 0);
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimCategoryDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimCategoryDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;