CREATE  PROCEDURE [dbo].[Znode_GetBundleProductAssociatedChildQuantity]    
(     
    @SKU     VARCHAR(max) = NULL,  
 @versionid int =0,  
 @localeId int =0,  
 @publishCatalogId int =0,  
 @PortalId int =1  
)    
AS    
/*    
 Sample execute query  
 Exec Znode_GetBundleProductAssociatedChildQuantity @SKU='E4-48221', @versionid=12, @localeId=1, @publishCatalogId=3 ,@portalid=1
    
*/    
 BEGIN    
  BEGIN TRY    
         SET NOCOUNT ON;         
     
     
-- This sp is used to provide parent sku's child sku and bundlle quantity and remaining quantity  
   IF OBJECT_ID('tempdb..#sku') IS NOT NULL DROP TABLE #sku  
      
   SELECT * into #sku FROM DBO.Split(@SKU, ',') AS Sp  
  
   IF OBJECT_ID('tempdb..#parentdata') IS NOT NULL DROP TABLE #parentdata  
      
   select * into #parentdata from ZnodePublishProductEntity where sku in (select item from #sku)  
  
   IF OBJECT_ID('tempdb..#Parentchilddate') IS NOT NULL DROP TABLE #Parentchilddate  
     
   select p.ZnodeProductId,SKU Parentsku,AssociatedZnodeProductId,AssociatedProductBundleQuantity,ZPBPE.AssociatedProductDisplayOrder   
    into #Parentchilddate  
    from ZnodePublishBundleProductEntity  ZPBPE  
    inner join #parentdata p on p.ZnodeProductId=  ZPBPE.ZnodeProductId  
    where ZPBPE.VersionId=@versionid  
    and ZPBPE.ZnodeCatalogId = @publishCatalogId  
  
  select distinct PCD.ZnodeProductId ParentPublishProductId,PCD.Parentsku ParentBundleSKU, PCD.AssociatedZnodeProductId PublishProductId, ZPPE.SKU SKU, COALESCE(CAST(PCD.AssociatedProductBundleQuantity as DECIMAL(9,2)),0) AssociatedQuantity, ZPPE.[Name], 

 
    ZPPE.Attributes Attribute , ZI1.Quantity AvailableQuntity, PCD.AssociatedProductDisplayOrder  
    from #Parentchilddate PCD  
    inner join ZnodePublishProductEntity ZPPE  
     on ZPPE.ZnodeProductId = PCD.AssociatedZnodeProductId  
     left join (select ZI.* from  ZnodePortalWarehouse ZPW INNER JOIN ZnodeInventory ZI on ZPW.PortalId =@PortalId  and ZI.WarehouseId =ZPW.WarehouseId ) ZI1
			 on ZI1.SKU = ZPPE.SKU  
     where ZPPE.VersionId=@versionid  
     and ZPPE.LocaleId=@localeId  
     and ZPPE.ZnodeCatalogId=@publishCatalogId  
     order by PCD.AssociatedProductDisplayOrder
 END TRY    
 BEGIN CATCH    
  DECLARE @Status BIT ;    
  SET @Status = 0;    
  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetBundleProductAssociatedChildQuantity @SKU = '+ @SKU +' ';    
                      
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
        EXEC Znode_InsertProcedureErrorLog    
   @ProcedureName = 'Znode_GetBundleProductAssociatedChildQuantity',    
   @ErrorInProcedure = @Error_procedure,    
   @ErrorMessage = @ErrorMessage,    
   @ErrorLine = @ErrorLine,    
   @ErrorCall = @ErrorCall;    
 END CATCH    
     
  END;