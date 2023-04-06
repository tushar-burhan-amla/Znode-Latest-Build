--EXEC Znode_DeletePublishCatalogData '3,4,5,6,7'

CREATE PROCEDURE [dbo].[Znode_DeletePublishCatalogData](
       @PublishCatalogId  VARCHAR(500)= '')
AS 
    -----------------------------------------------------------------------------
    --Summary:  Remove publish catalog data from publish table when delete the catalog 
    --		   	
    --          
    --Unit Testing   

    ----------------------------------------------------------------------------- 
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;

             BEGIN TRAN A;

				   Delete From ZnodePublishCatalogEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))

				   Delete From ZnodePublishCategoryEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   
				   Delete From ZnodePublishProductEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))

				   Delete From ZnodePublishAddonEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   
				   Delete From ZnodePublishGroupProductEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   
				   Delete From ZnodePublishConfigurableProductEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   
				   Delete From ZnodePublishBundleProductEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   
				   Delete ZPSE from ZnodePublishSeoEntity ZPSE join
				   ZnodePublishVersionEntity ZPVE on ZPSE.VersionId = ZPSE.VersionId
				   where ZPVE.ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))
				   and ZPSE.SEOTypeName  in ('Product', 'Category', 'Brand')
                                                                            				   
				   Delete From ZnodePublishVersionEntity
				   where  ZnodeCatalogId in (select Cast(Id as int) from  dbo.Split(@PublishCatalogId,','))	      
           
                   SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
             SELECT ERROR_MESSAGE() , ERROR_LINE() , ERROR_PROCEDURE();
             ROLLBACK TRAN A;
         END CATCH;
     END;