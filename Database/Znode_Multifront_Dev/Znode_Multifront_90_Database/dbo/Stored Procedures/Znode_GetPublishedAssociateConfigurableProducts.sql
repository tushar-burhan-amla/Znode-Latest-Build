
CREATE Procedure [dbo].[Znode_GetPublishedAssociateConfigurableProducts]
  (@CatalogVersionId INT,
  @PublishProductIds VARCHAR(Max),
  @PortalId INT,
  @LocaleId INT,
  @UserId INT)  
AS  
/* This stored procedure is used to get the data of associated configurable product of parent products
UNIT TESTING
EXEC [dbo].[Znode_GetPublishedAssociateConfigurableProducts] 16697,'904378,904379,904380,904381,904315,904383',1,1,2
*/
BEGIN    
 BEGIN TRY  
   SET NOCOUNT ON

       IF OBJECT_ID('tempdb..#TBL_PublishProductIds') IS NOT NULL
		DROP TABLE #TBL_PublishProductIds

	   CREATE TABLE [dbo].[#TBL_PublishProductIds]
	   ([PublishProductId] INT)

	   INSERT INTO [dbo].[#TBL_PublishProductIds] ([PublishProductId])
	   SELECT * FROM STRING_SPLIT(@PublishProductIds, ',')
   
       IF ISNULL(@CatalogVersionId, 0) > 0
        BEGIN

			SELECT ZPPE.ZnodeProductId AS PublishProductId,
			ZPCPE.ZnodeProductId AS ParentPublishProductId,
			ZPPE.SKU AS SKU
			FROM  ZnodePublishConfigurableProductEntity AS ZPCPE
			INNER JOIN ZnodePublishProductEntity AS ZPPE
			ON ZPCPE.AssociatedZnodeProductId = ZPPE.ZnodeProductId AND ZPCPE.VersionId = ZPPE.VersionId
			WHERE ZPPE.LocaleId = @LocaleId
			AND ZPPE.VersionId =  @CatalogVersionId
			AND ZPPE.IsActive = 1
			AND EXISTS (SELECT TOP 1 1  FROM [dbo].[#TBL_PublishProductIds] PP
			WHERE  PP.PublishProductId = ZPCPE.ZnodeProductId )
			 
	   END
      ELSE
        BEGIN

          SELECT ZPPE.ZnodeProductId AS PublishProductId,
          ZPCPE.ZnodeProductId AS ParentPublishProductId,
          ZPPE.SKU AS SKU
          FROM  ZnodePublishConfigurableProductEntity AS ZPCPE
          INNER JOIN ZnodePublishProductEntity AS ZPPE
          ON ZPCPE.AssociatedZnodeProductId = ZPPE.ZnodeProductId AND ZPCPE.VersionId = ZPPE.VersionId
          WHERE ZPPE.LocaleId = @LocaleId
          AND ZPPE.IsActive = 1
          AND EXISTS (SELECT TOP 1 1  FROM [dbo].[#TBL_PublishProductIds] PP
          WHERE PP.PublishProductId = ZPCPE.ZnodeProductId )
       END
 END TRY    

 BEGIN CATCH     
 
	  DECLARE @Status BIT ;
	  SET @Status = 0;
	  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
	  @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	  @ErrorCall NVARCHAR(MAX)= '
	  EXEC Znode_GetPublishedAssociateConfigurableProducts
	  @CatalogVersionId = '+CAST(@CatalogVersionId AS INT) +',
	  @PublishProductIds='+CAST(@PublishProductIds AS VARCHAR(Max))+',
	  @PortalId='+CAST(@PortalId AS INT)+',
	  @LocaleId='+ CAST(@LocaleId AS INT)+',
	  @UserId='+CAST(@UserId AS INT)
                               
      EXEC Znode_InsertProcedureErrorLog
	  @ProcedureName = 'Znode_GetPublishedAssociateConfigurableProducts',
	  @ErrorInProcedure = @Error_procedure,
	  @ErrorMessage = @ErrorMessage,
	  @ErrorLine = @ErrorLine,
	  @ErrorCall = @ErrorCall;  

 END CATCH    
END