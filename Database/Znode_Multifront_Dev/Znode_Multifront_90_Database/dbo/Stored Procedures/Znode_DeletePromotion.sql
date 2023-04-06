
CREATE PROCEDURE [dbo].[Znode_DeletePromotion]
      ( @PromotionId VARCHAR(300) ,
        @Status      INT OUT)
AS
/*
Summary: This Procedure is used to delete promotion associated to user
Unit Testing:
EXEC Znode_DeletePromotion @PromotionId =,0
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeletdAttributeId TABLE (
                                              PromotionId INT
                                              );
             INSERT INTO @DeletdAttributeId
                    SELECT Item
                    FROM dbo.split ( @PromotionId , ','
                                   ) AS a; 

             DELETE FROM ZnodeAccountPromotion
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletdAttributeId AS a
                            WHERE a.PromotionId = ZnodeAccountPromotion.PromotionId
                          );
             DELETE FROM ZnodeUserPromotion
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletdAttributeId AS a
                            WHERE a.PromotionId = ZnodeUserPromotion.PromotionId
                          );
             DELETE FROM ZnodePromotionCoupon
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletdAttributeId AS a
                            WHERE a.PromotionId = ZnodePromotionCoupon.PromotionId
                          ); 

             DELETE FROM ZnodePromotionCatalogs WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId a WHERE a.PromotionId = ZnodePromotionCatalogs.PromotionId) 

             DELETE FROM ZnodePromotionCategory WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId a WHERE a.PromotionId = ZnodePromotionCategory.PromotionId)

             DELETE FROM ZnodePromotionProduct WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId a WHERE a.PromotionId = ZnodePromotionProduct.PromotionId) 

			 DELETE FROM ZnodePromotionBrand WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId a WHERE a.PromotionId = ZnodePromotionBrand.PromotionId) 

			 DELETE FROM ZnodePromotionShipping WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId a WHERE a.PromotionId = ZnodePromotionShipping.PromotionId) 

			 Delete From ZnodePromotionProfileMapper where exists (SELECT TOP 1 1
                            FROM @DeletdAttributeId AS a
                            WHERE a.PromotionId = ZnodePromotionProfileMapper.PromotionId
                          );
             DELETE FROM ZnodePromotion
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletdAttributeId AS a
                            WHERE a.PromotionId = ZnodePromotion.PromotionId
                          );
             IF ( SELECT COUNT(1)
                  FROM @DeletdAttributeId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @PromotionId , ','
                                     ) AS a
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
            DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePromotion @PromotionId = '+@PromotionId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePromotion',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;