
CREATE PROCEDURE [dbo].[Znode_GetCMSContentPages]
(@PortalId INT)

AS
/*
 Summary : Get CMS Content Pages in XML format
 EXEC Znode_GetCMSContentPages 7 

*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @LocaleId INT=
             (
                 SELECT FeatureValues
                 FROM ZnodeGlobalSetting
                 WHERE FeatureName = 'Locale'
             ),  @IncrementValue INT= 1;

             DECLARE @TBL_LocaleAll TABLE
             (RowId    INT IDENTITY(1, 1),
              LocaleId INT,
              Code     VARCHAR(300)
             );

             INSERT INTO @TBL_LocaleAll
             (LocaleId,
              Code
             )
                    SELECT LocaleId,Code
                    FROM ZnodeLocale AS a
                    WHERE a.IsActive = 1;
             DECLARE @ReturnXML TABLE(ReturnXMl XML);
             WHILE @IncrementValue <=
             (
                 SELECT MAX(RowId)
                 FROM @TBL_LocaleAll
             )
                 BEGIN
                     DECLARE @TBL_ContentPageDetail TABLE
                     (ContentPageId   INT,
                      [FileName]      NVARCHAR(4000),
                      ProfileId       VARCHAR(4000),
                      LocaleId        INT,
                      ContentPageHtml NVARCHAR(MAX),
                      PageTitle       NVARCHAR(200),
                      PageName        NVARCHAR(200),
                      ActivationDate  DATETIME,
                      ExpirationDate  DATETIME
                     );

                     DECLARE @TBL_ContentPageDetailFinal TABLE
                     (ContentPageId   INT,
                      [FileName]      NVARCHAR(4000),
                      ProfileId       VARCHAR(4000),
                      LocaleId        INT,
                      ContentPageHtml NVARCHAR(MAX),
                      PageTitle       NVARCHAR(200),
                      PageName        NVARCHAR(200),
                      ActivationDate  DATETIME,
                      ExpirationDate  DATETIME
                     );
                    
                     INSERT INTO @TBL_ContentPageDetail
                            SELECT *
                            FROM @TBL_ContentPageDetailFinal AS a
                            WHERE a.LocaleId =
                            (
                                SELECT LocaleId
                                FROM @TBL_LocaleAll
                                WHERE RowId = @IncrementValue
                            );
                     INSERT INTO @TBL_ContentPageDetail
                            SELECT *
                            FROM @TBL_ContentPageDetailFinal AS a
                            WHERE a.LocaleId = @LocaleId;
                     INSERT INTO @ReturnXML
                            SELECT
                            (
                                SELECT *
                                FROM @TBL_ContentPageDetail AS q
                                WHERE q.ContentPageId = n.ContentPageId
                                FOR XML PATH('ContentPageConfigEntity')
                            )
                            FROM @TBL_ContentPageDetail AS n;
                     SET @IncrementValue = @IncrementValue + 1;
                     DELETE FROM @TBL_ContentPageDetail;
                     DELETE FROM @TBL_ContentPageDetailFinal;
                 END;
             SELECT *
             FROM @ReturnXML;
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSContentPages @PortalId = '+CAST(@PortalId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSContentPages',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;