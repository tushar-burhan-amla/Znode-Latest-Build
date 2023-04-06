CREATE PROCEDURE [dbo].[Znode_DeletePimCatalog]
( 
  @PimCatalogIds       VARCHAR(500)= '',
  @IsDeleteFromPublish BIT          = 0,
  @PimCatalogId		   TransferId READONLY, 
  @IsForceFullyDelete  BIT = 0 
)
AS
/*
Summary: This Procedure is used to delete PimCatalog with their respective details
Unit Testing:
EXEC Znode_DeletePimCatalog 1

*/
        BEGIN
         BEGIN TRAN;
         BEGIN TRY

             SET NOCOUNT ON;
             DECLARE @PublishCatalogIdsGen TABLE(PublishCatalogId INT);
			 DECLARE @DeletedCategoryHirachyId  TABLE(PimCategoryHierarchyId INT);
             DECLARE @PublishCatalogIds VARCHAR(3000);
             DECLARE @CatalogIds TABLE(PimCatalogId INT);
             INSERT INTO @CatalogIds
                    SELECT item
                    FROM dbo.Split(@PimCatalogIds, ',') AS zs
					WHERE @PimCatalogIds <> '';
					
			 INSERT INTO @CatalogIds 
			 SELECT id 
			 FROM  @PimCatalogId
             DECLARE @DeletePimCatalogId TABLE(PimCatalogId INT);

             INSERT INTO @DeletePimCatalogId
                    SELECT a.PimCatalogId
                    FROM [dbo].ZnodePimCatalog AS a
                         INNER JOIN @CatalogIds AS b ON(a.PimCatalogId = b.PimCatalogId)
                    WHERE(NOT EXISTS
                         (
                             SELECT TOP 1 1
                             FROM ZnodePublishCatalog AS zpc
                                  INNER JOIN ZnodePortalCatalog AS de ON(de.PublishCatalogId = zpc.PublishCatalogId)
                             WHERE CASE 
							 WHEN @IsDeleteFromPublish = 1 
							 THEN zpc.PimCatalogId 
							 ELSE 0
                                   END = a.PimCatalogId
                         ) OR @IsForceFullyDelete = 1 );

             INSERT INTO @PublishCatalogIdsGen
                    SELECT PublishCatalogId
                    FROM ZnodePublishCatalog AS a
                         INNER JOIN @DeletePimCatalogId AS b ON(a.PimCatalogId = b.PimCatalogId)
                    WHERE(NOT EXISTS
                         (
                             SELECT TOP 1 1
                             FROM ZnodePublishCatalog AS zpc
                                  INNER JOIN ZnodePortalCatalog AS de ON(de.PublishCatalogId = zpc.PublishCatalogId)
                             WHERE zpc.PimCatalogId = a.PimCatalogId
                         ));

			
			 INSERT INTO @DeletedCategoryHirachyId
			 SELECT PimCategoryHierarchyId
			 FROM ZnodePimCategoryHierarchy
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimCatalogId AS b
                 WHERE b.Pimcatalogid = ZnodePimCategoryHierarchy.Pimcatalogid
             );

			 
             DELETE FROM ZnodePimVersioning
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimCatalogId AS b
                 WHERE b.Pimcatalogid = ZnodePimVersioning.Pimcatalogid
             );
             
           
             DELETE FROM ZnodePimCategoryHierarchy
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimCatalogId AS b
                 WHERE b.Pimcatalogid = ZnodePimCategoryHierarchy.Pimcatalogid
             );
			
			Update a set a.PimCatalogId = null  from ZnodeProfile a Inner join @DeletePimCatalogId  b on 
			b.Pimcatalogid =b.Pimcatalogid

			 DELETE FROM ZnodePimCatalog
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletePimCatalogId AS b
                 WHERE b.Pimcatalogid = ZnodePimCatalog.Pimcatalogid
             );
             IF @IsDeleteFromPublish = 1
                 BEGIN
				   DECLARE Cur_DeleteCatalog CURSOR FOR
				    SELECT PublishCatalogId 
                                                           FROM @PublishCatalogIdsGen
				    OPEN Cur_DeleteCatalog ;
					FETCH NEXT FROM Cur_DeleteCatalog INTO @PublishCatalogIds 
					WHILE @@FETCH_STATUS = 0 
					BEGIN 
             
                     EXEC Znode_DeletePublishCatalog
                          @PublishCatalogIds=@PublishCatalogIds,
                          @IsDeleteCatalogId = 1;

				   FETCH NEXT FROM Cur_DeleteCatalog INTO @PublishCatalogIds 
                    END 
				CLOSE Cur_DeleteCatalog; 
				DEALLOCATE Cur_DeleteCatalog;

                 END;
             IF
             (
                 SELECT COUNT(1)
                 FROM @DeletePimCatalogId
             ) =
             (
                 SELECT COUNT(1)
                 FROM @CatalogIds
             )
                 BEGIN
                     SELECT 1 AS ID,
                            @PublishCatalogIds AS [MessageDetails],
                            CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            @PublishCatalogIds AS [MessageDetails],
                            CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimCatalog @PimCatalogIds = '+@PimCatalogIds+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             --SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK  TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimCatalog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;