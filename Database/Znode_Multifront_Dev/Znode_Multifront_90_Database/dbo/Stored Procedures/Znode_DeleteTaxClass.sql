


CREATE PROCEDURE [dbo].[Znode_DeleteTaxClass]
( @TaxClassId VARCHAR(max),
  @Status     INT OUT,
  @IsForceFullyDelete BIT = 0 )
AS
/*
Summary: This Procedure is used to delete taxclass details or an order
Unit Testing:
EXEC Znode_DeleteTaxClass 

*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
			  DECLARE @StatusOutINT Table (Id INT ,Message NVARCHAR(max), Status BIT )
			  DECLARE @DeletedIdsIN TransferId 
             DECLARE @DeletedClassId TABLE(TaxClassId INT);


            INSERT INTO @DeletedClassId			   
			SELECT Item
			FROM dbo.split(@TaxClassId, ',') AS a
			WHERE (NOT EXISTS (SELECT TOP 1 1 FROM ZnodeTaxClassSKU  asa  
			INNER JOIN ZnodeOmsOrderLineItems vf ON (vf.Sku = asa.SKU)   
			WHERE asa.TaxClassId = a.Item ) OR @IsForceFullyDelete =1 )

			INSERT INTO @DeletedIdsIN 
			SELECT DISTINCT a.OmsOrderId 
			FROM ZnodeOmsOrder A 
			INNER JOIN ZnodeOMsOrderDetails b  ON (b.OmsOrderId = a.OmsOrderId )
			INNER JOIN 	ZnodeOmsOrderLineItems c ON (c.OmsOrderDetailsId = b.OmsOrderDetailsId)
			INNER JOIN ZnodeTaxClassSKU  vf ON (vf.Sku = c.SKU)
			WHERE  EXISTS (SELECT TOP 1 1 FROM @DeletedClassId DA WHERE DA.TaxClassId = VF.TaxClassId)
			
		    INSERT INTO @StatusOutINT (Id ,Status) 
			EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIdsIN , @status = 0 

             DELETE FROM ZnodeTaxRule
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletedClassId AS a
                 WHERE a.TaxClassId = ZnodeTaxRule.TaxClassId
             );
             --DELETE FROM ZnodeTaxClassSKU
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM @DeletedClassId AS a
             --    WHERE a.TaxClassId = ZnodeTaxClassSKU.TaxClassId
             --);	 
             DELETE FROM ZnodePortalTaxClass
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletedClassId AS a
                 WHERE a.TaxClassId = ZnodePortalTaxClass.TaxClassId
             );
			
			
			DELETE FROM ZnodeTaxClassSKU WHERE  EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletedClassId AS a
                 WHERE a.TaxClassId = ZnodeTaxClassSKU.TaxClassId
             );  
			 
             DELETE FROM ZnodeTaxClass
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletedClassId AS a
                 WHERE a.TaxClassId = ZnodeTaxClass.TaxClassId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @DeletedClassId
             ) =
             (
                 SELECT COUNT(1)
                 FROM dbo.split(@TaxClassId, ',') AS a
             )
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
							SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
							SET @Status = 0;
                 END;
             
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
		
            DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteTaxClass  @TaxClassId = '+@TaxClassId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteTaxClass ',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;