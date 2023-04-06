CREATE PROCEDURE [dbo].[Znode_DeletePimCategory](
       @PimCategoryIds VARCHAR(500)= '' ,
       @Status         BIT OUT,
	   @PimCategoryId TransferId READONLY  
	   )
AS
/*
Summary: This Procedure is used to delete PimCategory with their reference details from respective tables
Unit Testing:
	Declare @status bit 
	EXEC [Znode_DeletePimCategory]  16,@status =@status 
	Select @status 
 Alter table ZnodePublishCategory Drop Constraint FK_ZnodePublishCategory_ZnodePimCategory
*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @FinalCount INT = 0 
             DECLARE @CategoryIds TABLE (
                                        PimCategoryId INT
                                        );
             INSERT INTO @CategoryIds
                    SELECT item
                    FROM dbo.Split ( @PimCategoryIds , ','
                                   )
								   WHERE @PimCategoryIds <> '' ;
			 INSERT INTO @CategoryIds
			 SELECT  Id 
			 FROM @PimCategoryId


			 
			  
             DECLARE @V_CategorryCount INT;
             DECLARE @DeletePimCategoryId TABLE (
                                                PimCategoryId               INT ,
                                                PimCategoryAttributeValueId INT
                                                );
             INSERT INTO @DeletePimCategoryId
                    SELECT a.PimCategoryId , c.PimCategoryAttributeValueId
                    FROM [dbo].ZnodePimCategory AS a 
                                                     INNER JOIN @CategoryIds AS b ON ( a.PimCategoryId = b.PimCategoryId )
										   LEFT OUTER JOIN ZnodePimCategoryAttributeValue AS c ON ( a.PimCategoryId = c.PimCategoryId )

             SELECT @V_CategorryCount = COUNT(1)
             FROM ( SELECT PimCategoryId
                    FROM @DeletePimCategoryId
                    GROUP BY PimCategoryId
                  ) AS a;


			DELETE ZCSDL
				FROM ZnodeCMSSEODetaillocale ZCSDL
				where exists(select * FROM ZnodeCMSSEODetail ZCSD where ZCSD.CMSSEOTypeId = (select top 1 CMSSEOTypeId From ZnodeCMSSEOType where Name = 'Category')
				AND Exists( select * from ZnodePimCategoryAttributeValue ZPCA
				inner join @DeletePimCategoryId AS b on ZPCA. PimCategoryId = b.PimCategoryId
				inner join ZnodePimCategoryAttributeValueLocale ZPCAL on ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId
				where exists(select PimAttributeId from ZnodePimAttribute ZPA where IsCategory = 1 and AttributeCode ='CategoryCode' and ZPCA.PimAttributeId = ZPA.PimAttributeId)
				and ZCSD.SEOCode = ZPCAL.CategoryValue) and ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId)

			DELETE ZCSD
				FROM ZnodeCMSSEODetail ZCSD
				where ZCSD.CMSSEOTypeId = (select top 1 CMSSEOTypeId From ZnodeCMSSEOType where Name = 'Category')
				AND Exists( select * from ZnodePimCategoryAttributeValue ZPCA
				inner join @DeletePimCategoryId AS b on ZPCA. PimCategoryId = b.PimCategoryId
				inner join ZnodePimCategoryAttributeValueLocale ZPCAL on ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId
				where exists(select PimAttributeId from ZnodePimAttribute ZPA where IsCategory = 1 and AttributeCode ='CategoryCode' and ZPCA.PimAttributeId = ZPA.PimAttributeId)
				and ZCSD.SEOCode = ZPCAL.CategoryValue) 

		   DELETE FROM ZnodePimCategoryHierarchy WHERE EXISTS (SELECT TOP 1 1 FROM @DeletePimCategoryId DPCI  where DPCI.PimCategoryId = ZnodePimCategoryHierarchy.PimCategoryId )
		
             DELETE FROM ZnodePimCategoryAttributeValueLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletePimCategoryId AS b
                            WHERE b.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValueLocale.PimCategoryAttributeValueId
                          );
             DELETE FROM ZnodePimCategoryAttributeValue
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletePimCategoryId AS b
                            WHERE b.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValue.PimCategoryAttributeValueId
                          );
             DELETE FROM ZnodePimCategoryProduct
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletePimCategoryId AS b
                            WHERE b.PimCategoryId = ZnodePimCategoryProduct.PimCategoryId
                          );

             DELETE FROM ZnodePimCategory
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletePimCategoryId AS b
                            WHERE b.PimCategoryId = ZnodePimCategory.PimCategoryId
                          );

             SET @Status = 1;
             IF ( SELECT COUNT(1)
                  FROM @CategoryIds
                ) = @V_CategorryCount
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
                        
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimCategory @PimCategoryIds = '+@PimCategoryIds+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status,ERROR_MESSAGE();                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimCategory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;