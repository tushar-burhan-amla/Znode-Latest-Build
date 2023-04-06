CREATE PROCEDURE [dbo].[Znode_DeletePimDownloadableProductKey]
( @PimDownloadableProductKeyId VARCHAR(2000),
  @Status           BIT OUT)
AS 
/*  
     Summary : Remove content page details with their referance data 
			   Here complete delete the PimDownloadableProductKeys and their references without any check  
			   If passed @CMSPimDownloadableProductKeyIds are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma separeted data in table rows 
			   1 ZnodeCMSPimDownloadableProductKeysProfile
			   2 ZnodeCMSPimDownloadableProductKeysLocale
			   3 ZnodeCMSPimDownloadableProductKeyGroupMapping
			   4 ZnodeCMSSEODetail
			   5 ZnodeCMSPimDownloadableProductKeys
     Unit Testing 
	 begin tran
     DEclare @Status bit 
     EXEC Znode_DeletePimDownloadableProductKey  29 ,@Status =@Status OUT 
	 rollback tran
       
    */
	 BEGIN
         BEGIN TRAN DeletePimDownloadableProductKey;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_DeletePimDownloadableProductKey TABLE(PimDownloadableProductKeysId INT);  -- table holds the PimDownloadableProductKeysId id 
             DECLARE @IsUsed BIT =0
			 INSERT INTO @TBL_DeletePimDownloadableProductKey
                    SELECT a.PimDownloadableProductKeyId
                    FROM [dbo].[ZnodePimDownloadableProductKey] AS a
                         INNER JOIN dbo.Split(@PimDownloadableProductKeyId, ',') AS b ON(a.PimDownloadableProductKeyId = b.Item) -- dbo.Split function use to make ',' separeted data in table rows 
					WHERE a.IsUsed=@IsUsed

			 DELETE FROM [ZnodePimDownloadableProductKey]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletePimDownloadableProductKey AS TBDCP
                 WHERE TBDCP.PimDownloadableProductKeysId = [ZnodePimDownloadableProductKey].PimDownloadableProductKeyId
             );
			 IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeletePimDownloadableProductKey
             ) =
             (   -- if count are equal then  dataset status are return true other wise false 
                 SELECT COUNT(1)
                 FROM dbo.Split(@PimDownloadableProductKeyId, ',')
             ) 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeletePimDownloadableProductKey;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimDownloadableProductKey @CMSPimDownloadableProductKeyId = '+@PimDownloadableProductKeyId+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             ROLLBACK TRAN DeletePimDownloadableProductKey;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePimDownloadableProductKey',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;