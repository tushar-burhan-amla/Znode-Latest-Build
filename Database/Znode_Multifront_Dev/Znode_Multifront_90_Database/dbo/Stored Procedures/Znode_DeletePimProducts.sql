CREATE PROCEDURE [dbo].[Znode_DeletePimProducts]
(
       @PimProductId VARCHAR(MAX)= '' ,
       @Status       INT OUT,
	   @PimProductIds TransferId READONLY 
)
AS
	/* Summary :- This Procedures is used to hard delete the product details on the basis of product ids 
	 Unit Testing
	 begin tran 

	 rollback tran
	*/
    BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN DeletePimProducts;
			 DECLARE @FinalCount INT = 0 
             DECLARE @TBL_DeletdProductId TABLE ( PimProductId INT);
             INSERT INTO @TBL_DeletdProductId
                    SELECT Item
                    FROM dbo.split ( @PimProductId , ',' ) AS SP
					WHERE @PimProductId <> '' ; 
           	INSERT INTO @TBL_DeletdProductId
			SELECT ID 
			FROM @PimProductIds

			---Deleting product SEO
			Declare @SKU [dbo].[SelectColumnList]
			INSERT INTO @SKU
			SELECT c.AttributeValue
			FROM @TBL_DeletdProductId AS a INNER JOIN ZnodePimAttributeValue AS b ON a.PimProductId = b.PimProductId
			INNER JOIN ZnodePimAttributeValueLocale c ON b.PimAttributeValueId = c.PimAttributeValueId
			Where b.PimAttributeId = (select top 1 PimAttributeId From ZnodePimAttribute Where AttributeCode = 'SKU')

			DELETE ZCSDL FROM ZnodeCMSSEODetailLocale ZCSDL
			where exists(SELECT * FROM ZnodeCMSSEODetail ZCSD where ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId
						and Exists(Select * from @SKU S where ZCSD.SEOCode = S.StringColumn) 
						And CMSSeoTypeId = (Select top 1 CMSSeoTypeId from ZnodeCMSSEOType Where Name = 'Product') )

			DELETE ZCSD FROM ZnodeCMSSEODetail ZCSD  
			where Exists(Select * from @SKU S where ZCSD.SEOCode = S.StringColumn)
			And CMSSeoTypeId = (Select top 1 CMSSeoTypeId from ZnodeCMSSEOType Where Name = 'Product') 
			---Deleting product SEO

			 DELETE FROM ZnodePimConfigureProductAttribute WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletdProductId TDPI WHERE 
			   ZnodePimConfigureProductAttribute.PimProductId = PimProductId )

             DELETE FROM dbo.ZnodePimAttributeValueLocale
             WHERE PimAttributeValueId IN ( SELECT b.PimAttributeValueId
                                            FROM @TBL_DeletdProductId AS a INNER JOIN ZnodePimAttributeValue AS b ON a.PimProductId = b.PimProductId
                                          );

										  	 DELETE FROM dbo.ZnodePimProductAttributeDefaultValue
             WHERE PimAttributeValueId IN ( SELECT b.PimAttributeValueId
                                            FROM @TBL_DeletdProductId AS a INNER JOIN ZnodePimAttributeValue AS b ON a.PimProductId = b.PimProductId
                                          );

			 DELETE FROM dbo.ZnodePimProductAttributeTextAreaValue
             WHERE PimAttributeValueId IN ( SELECT b.PimAttributeValueId
                                            FROM @TBL_DeletdProductId AS a INNER JOIN ZnodePimAttributeValue AS b ON a.PimProductId = b.PimProductId
                                          );

			
			 DELETE FROM dbo.ZnodePimProductAttributeMedia
             WHERE PimAttributeValueId IN ( SELECT b.PimAttributeValueId
                                            FROM @TBL_DeletdProductId AS a INNER JOIN ZnodePimAttributeValue AS b ON a.PimProductId = b.PimProductId
                                          );

             DELETE FROM ZnodePimAttributeValue
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimAttributeValue.PimProductId
                          );
		   
		    DELETE FROM ZnodePimAddonGroupProduct  WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.pimProductId = ZnodePimAddonGroupProduct.PimChildProductId
                          )
		    DELETE FROM ZnodePimAddOnProductDetail
            WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.pimProductId = ZnodePimAddOnProductDetail.PimChildProductId
                          )
				OR EXISTS (SELECT TOP 1 1 FROM ZnodePimAddOnProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimAddOnProduct.PimProductId
                          ) AND ZnodePimAddOnProduct.PimAddOnProductId  = ZnodePimAddOnProductDetail.PimAddOnProductId );		  
						  
				
			 DELETE FROM ZnodePimAddOnProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimAddOnProduct.PimProductId
                          );
			DELETE FROM ZnodePimCustomFieldLocale
            WHERE EXISTS ( SELECT TOP 1 1
                            FROM ZnodePimCustomField AS zm
                            WHERE zm.PimCustomFieldId = ZnodePimCustomFieldLocale.PimCustomFieldId
                                  AND
                                  EXISTS ( SELECT TOP 1 1
                                           FROM @TBL_DeletdProductId AS a
                                           WHERE a.PimProductId = Zm.PimProductId
                                         )
                          );
             DELETE FROM ZnodePimCustomField
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimCustomField.PimProductId
                          );
             DELETE FROM ZnodePimLinkProductDetail
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimLinkProductDetail.PimProductId
                                  OR
                                  a.PimProductId = ZnodePimLinkProductDetail.PimParentProductId
                          );
             DELETE FROM ZnodePimProductImage
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimProductImage.PimProductId
                          );
             DELETE FROM ZnodePimProductTypeAssociation
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimProductTypeAssociation.PimProductId
                                  OR
                                  a.pimproductId = ZnodePimProductTypeAssociation.PimParentProductId
                          );


             DELETE FROM ZnodePimCategoryProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimCategoryProduct.PimProductId
                          );

	     DELETE FROM ZnodeBrandProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodeBrandProduct.PimProductId
                          );

             DELETE FROM ZnodePimProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_DeletdProductId AS a
                            WHERE a.PimProductId = ZnodePimProduct.PimProductId
                          );

	    --Delete price based on SKU--
	    DELETE ZPP FROM ZnodePrice ZPP  
	    where Exists(Select * from @SKU S where ZPP.SKU = S.StringColumn)

            --Delete Inventory based on SKU--
	    DELETE ZI FROM ZnodeInventory ZI  
	    where Exists(Select * from @SKU S where ZI.SKU = S.StringColumn)
		
			 SET @FinalCount = 	( SELECT COUNT(1) FROM dbo.split ( @PimProductId , ',') AS a WHERE @PimProductId <> '')
			 SET @FinalCount = 	CASE WHEN @FinalCount = 0 THEN  ( SELECT COUNT(1) FROM @TBL_DeletdProductId AS a ) ELSE   @FinalCount END 

             IF ( SELECT COUNT(1) FROM @TBL_DeletdProductId ) = @FinalCount
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN DeletePimProducts;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimProducts @PimProductId = '+@PimProductId+',@Status='+CAST(@Status AS VARCHAR(10));
              
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN DeletePimProducts;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimProducts',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;