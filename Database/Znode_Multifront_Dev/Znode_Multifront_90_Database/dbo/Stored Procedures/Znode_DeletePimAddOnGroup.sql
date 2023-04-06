
CREATE PROCEDURE [dbo].[Znode_DeletePimAddOnGroup]
(  @PimAddOnGroupId VARCHAR(500),
   @Status          BIT OUT)
AS
/*
Summary: This Procedure is used to delete PimAddOn Group and Product with their reference details
Unit Testing:
begin tran
EXEC Znode_DeletePimAddOnGroup 1,0
rollback tran
*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @PimAddOnGroupIds TABLE(PimAddOnGroupId INT);
             INSERT INTO @PimAddOnGroupIds
                    SELECT item
                    FROM dbo.Split(@pimAddOnGroupId, ',');
             DECLARE @V_CategorryCount INT;
             DECLARE @DeletePimAddOnGroupId TABLE(PimAddOnGroupId INT);
             INSERT INTO @DeletePimAddOnGroupId
                    SELECT a.PimAddOnGroupId
                    FROM [dbo].ZnodePimAddOnGroup AS a
                         INNER JOIN @PimAddOnGroupIds AS b ON(a.PimAddOnGroupId = b.PimAddOnGroupId)
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimAddOnProduct AS q
                        WHERE  EXISTS
                        (
                            SELECT TOP 1 1
                            FROM ZnodePimAddOnProductDetail AS s
                            WHERE q.PimAddOnProductId = s.PimAddOnProductId
                        )
                              AND q.PimAddonGroupId = a.PimAddonGroupId
                    );
             DELETE FROM ZnodePimAddonGroupLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimAddOnGroupId AS q
                 WHERE q.PimAddonGroupId = ZnodePimAddonGroupLocale.pimaddongroupid
             );
             DELETE FROM ZnodePimAddOnProduct
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimAddOnGroupId AS q
                 WHERE q.PimAddonGroupId = ZnodePimAddOnProduct.pimaddongroupid
             );

			 DELETE FROM ZnodePimAddonGroupProduct
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimAddOnGroupId AS q
                 WHERE q.PimAddonGroupId = ZnodePimAddonGroupProduct.pimaddongroupid
             );

             DELETE FROM ZnodePimAddonGroup
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimAddOnGroupId AS q
                 WHERE q.PimAddonGroupId = ZnodePimAddonGroup.pimaddongroupid
             );
             SELECT @V_CategorryCount = COUNT(1)
             FROM
             (
                 SELECT PimAddOnGroupId
                 FROM @DeletePimAddOnGroupId
                 GROUP BY PimAddOnGroupId
             ) AS a;
             SET @Status = 1;
             IF
             (
                 SELECT COUNT(1)
                 FROM @PimAddOnGroupIds
             ) = @V_CategorryCount
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
              
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimAddOnGroup @PimAddOnGroupId = '+@PimAddOnGroupId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimAddOnGroup',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;