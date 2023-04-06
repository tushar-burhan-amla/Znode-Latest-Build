CREATE PROCEDURE [dbo].[Znode_DeletePublishCatalogProduct]
(   
   	@PublishProductIds	Transferid READONLY 
	,@PublishCatalogId   INT = 0 
	,@PimCategoryHierarchyId int = 0 
	,@PimCatalogId int = 0 
	
)
AS
   /*
    Summary:Delete the publish product from all related references 
    Unit Testing   
	begin tran
	SELECT * FROM ZnodePublishCatalog 
    EXEC Znode_DeletePublishCatalog @PublishCatalogIds =5 ,@IsDeleteCatalogId = 1 , @PublishCategoryIds = ''
	rollback tran
   */ 
     BEGIN
         BEGIN TRANSACTION A;
         BEGIN TRY
             SET NOCOUNT ON;
	
	         DECLARE @TBL_DeleteIds TABLE (PublishCatalogId INT ,PublishCategoryId INT ,PublishProductId INT )  
			 DECLARE @TBL_DeleteCategoryProduct TABLE (PublishCategoryProductId INT )			 
			 DECLARE @TBL_CategoryCategoryHierarchyIds TABLE (CategoryId int , ParentCategoryId int )
			 
		
			If @PimCategoryHierarchyId <> 0 AND @PimCatalogId <>  0 
			Begin
				INSERT INTO @TBL_CategoryCategoryHierarchyIds(CategoryId , ParentCategoryId )
				Select Distinct PimCategoryId , Null FROM (
				SELECT PimCategoryId,ParentPimCategoryId from DBO.[Fn_GetRecurciveCategoryIds](@PimCategoryHierarchyId,@PimCatalogId)
				Union 
				Select PimCategoryId , null  from ZnodePimCategoryHierarchy where PimCategoryHierarchyId = @PimCategoryHierarchyId 
				Union 
				Select PimCategoryId , null  from [Fn_GetRecurciveCategoryIds_new] (@PimCategoryHierarchyId,@PimCatalogId) ) Category  


				INSERT INTO @TBL_DeleteIds (PublishCatalogId,PublishProductId,PublishCategoryId  )
				 SELECT ZPP.PublishCatalogId,ZPP.PublishProductId ,ZPCP.PublishCategoryId
				 FROM ZnodePublishProduct  ZPP 
				 INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId) 
				 LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND ZPCP.PublishProductId = ZPP.PublishProductId )
				 LEFT JOIN ZnodePublishCategory  ZPPC ON (ZPPC.PublishCatalogId = ZPC.PublishCatalogId AND ZPPC.PublishCategoryId = ZPCP.PublishCategoryId)
				 AND ZPPC.PimCategoryId in 
							(
								Select CategoryId from @TBL_CategoryCategoryHierarchyIds
							) 
				 WHERE EXISTS (SELECT TOP 1 1 FROM @PublishProductIds ZPCC WHERE ZPCC.Id = ZPP.PublishProductId )

				 INSERT INTO @TBL_DeleteCategoryProduct 
				 SELECT PublishCategoryProductId 
				 FROM ZnodePublishCategoryProduct a 
				 INNER JOIN ZnodePublishProduct ZPP ON (ZPP.PublishProductId = a.PublishProductId AND ZPP.PublishCatalogId = a.PublishCatalogId)
				 INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = a.PublishCatalogId )
				 INNER JOIN ZnodePublishCategory ZPCC ON (ZPCC.PublishCategoryId = a.PublishCategoryId)
				 WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPDF 
				       INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPDF.PimCategoryId = ZPCH.PimCategoryId
					   WHERE ZPCH.PimCatalogId = ZPC.PimCatalogId AND ZPDF.PimCategoryId = ZPCC.PimCategoryId AND ZPDF.PimProductId = ZPP.PimProductId  )
				 AND ZPCC.PimCategoryId in 
							(
								Select CategoryId from @TBL_CategoryCategoryHierarchyIds
							) 


				 AND a.PublishCatalogId = @PublishCatalogId
			END 
			Else 
			Begin
				 INSERT INTO @TBL_DeleteIds (PublishCatalogId,PublishProductId,PublishCategoryId  )
				 SELECT ZPP.PublishCatalogId,ZPP.PublishProductId ,ZPCP.PublishCategoryId
				 FROM ZnodePublishProduct  ZPP 
				 INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId) 
				 LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND ZPCP.PublishProductId = ZPP.PublishProductId )
				 LEFT JOIN ZnodePublishCategory  ZPPC ON (ZPPC.PublishCatalogId = ZPC.PublishCatalogId AND ZPPC.PublishCategoryId = ZPCP.PublishCategoryId)
				 WHERE EXISTS (SELECT TOP 1 1 FROM @PublishProductIds ZPCC WHERE ZPCC.Id = ZPP.PublishProductId )

				 INSERT INTO @TBL_DeleteCategoryProduct 
				 SELECT PublishCategoryProductId 
				 FROM ZnodePublishCategoryProduct a 
				 INNER JOIN ZnodePublishProduct ZPP ON (ZPP.PublishProductId = a.PublishProductId AND ZPP.PublishCatalogId = a.PublishCatalogId)
				 INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = a.PublishCatalogId )
				 INNER JOIN ZnodePublishCategory ZPCC ON (ZPCC.PublishCategoryId = a.PublishCategoryId)
				 WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPDF 
				       INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPDF.PimCategoryId = ZPCH.PimCategoryId
					   WHERE ZPCH.PimCatalogId = ZPC.PimCatalogId AND ZPDF.PimCategoryId = ZPCC.PimCategoryId AND ZPDF.PimProductId = ZPP.PimProductId  )
				 AND a.PublishCatalogId = @PublishCatalogId
			 END 

			delete del from @TBL_DeleteIds del
			where exists(select * from ZnodePimCategoryProduct ZPCC 
				INNER JOIN ZnodePublishProduct ZPP on ZPCC.PimProductId = ZPP.PimproductId	
			       and ZPP.PublishProductId = del.PublishProductId)		
				
		     UPDATE 	TBL 
			 SET PublishCategoryId = NULL 
			 FROM @TBL_DeleteIds TBL 
			 WHERE  EXISTS  (SELECT TOP 1 1 FROM ZnodePublishCategoryProduct ZPCC 
			 WHERE  TBL.PublishCatalogId = ZPCC.PublishCatalogId AND ZPCC.PublishCategoryId = TBL.PublishCategoryId   AND TBL.PublishProductId <> ZPCC.PublishProductId ) 			 

             DELETE FROM ZnodeUserWishList
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM ZnodePublishProductDetail ZPPD
                     WHERE ZPPD.SKU = ZnodeUserWishList.SKU
                           AND qw.[PublishProductId] = ZPPD.[PublishProductId]
                 )
             );
            

             DELETE FROM [ZnodePromotionProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishProductId] = [ZnodePromotionProduct].[PublishProductId]
             );
             DELETE FROM [ZnodePromotionCategory]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS w
                 WHERE w.[PublishCategoryId] = [ZnodePromotionCategory].[PublishCategoryId]
             );
             DELETE FROM [ZnodePublishCategoryDetail]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS w
                 WHERE w.[PublishCategoryId] = [ZnodePublishCategoryDetail].[PublishCategoryId]
             );	
            
       
             DELETE FROM [ZnodePublishCategoryProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishProductId] = [ZnodePublishCategoryProduct].[PublishProductId]
             )
			 OR 
			 EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteCategoryProduct AS qw
                 WHERE qw.PublishCategoryProductId = [ZnodePublishCategoryProduct].PublishCategoryProductId
             )
			 ;
             DELETE FROM [ZnodePublishCategoryProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS w
                 WHERE w.[PublishCategoryId] = [ZnodePublishCategoryProduct].[PublishCategoryId]
             );
			  DELETE FROM dbo.ZnodeSearchGlobalProductBoost WHERE 
			 EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishProductId] = ZnodeSearchGlobalProductBoost.[PublishProductId]
              );
             DELETE FROM [ZnodePublishProductDetail]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishProductId] = [ZnodePublishProductDetail].[PublishProductId]
             );
             DELETE FROM [ZNodePublishProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishProductId] = [ZNodePublishProduct].[PublishProductId]
             
			 );
		
			 DELETE FROM ZnodePublishCategoryDetail 
			 WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishCategoryId] = ZnodePublishCategoryDetail.[PublishCategoryId]
              );
			
			DELETE FROM ZnodeSearchGlobalProductCategoryBoost 
			 WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishCategoryId] = ZnodeSearchGlobalProductCategoryBoost.[PublishCategoryId]
             
			 );
			 DELETE FROM ZnodePublishCategory
			 WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteIds AS qw
                 WHERE qw.[PublishCategoryId] = ZnodePublishCategory.[PublishCategoryId]
             
			 );

			 			 				
             COMMIT TRANSACTION A;
			 
         END TRY
         BEGIN CATCH
		      SELECT ERROR_MESSAGE() 
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePublishCatalogProduct @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(200))+',@PimCategoryHierarchyId='+CAST(@PimCategoryHierarchyId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(200));


             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePublishCatalogProduct',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;

         END CATCH;
     END;