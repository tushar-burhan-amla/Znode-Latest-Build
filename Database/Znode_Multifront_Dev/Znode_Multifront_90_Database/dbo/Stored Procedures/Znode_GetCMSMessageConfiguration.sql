


CREATE PROCEDURE [dbo].[Znode_GetCMSMessageConfiguration]
( 
  @PortalId INT,
  @UserId   INT = 0,
  @LocaleId INT = 0 
  )
AS 
   /* 
    Summary :
     Sp To get Znode CMS messages configuration in XML format
     INPUT: PortalId
     Get all Znode CMS messages associated with Portal ID. messages will repeat according to MessageKey  and  number of active locale as already done in Product Publish 
     LocaleId from ZnodeCMSmessage
     PortalId Input(Present in ZnodeCMSPortalMessage)
     MessageKey  from znodecmsmessagekey
     Message from ZnodeCMSmessage
     Keep the filed name in XML as mentioned above
    unit testing : 
     EXEC Znode_GetCMSMessageConfiguration 7 ,1
	
 */
     BEGIN
         BEGIN TRY
             DECLARE @SetLocaleId INT=
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
             INSERT INTO @TBL_LocaleAll(LocaleId,Code)
                    SELECT LocaleId,Code
                    FROM ZnodeLocale AS a
                    WHERE a.IsActive = 1 
					AND (a.LocaleId = @LocaleId  OR  @LocaleId = 0 );

             DECLARE @ReturnXML TABLE(ReturnXMl XML);
             WHILE @IncrementValue <=
             (
                 SELECT MAX(RowId)
                 FROM @TBL_LocaleAll
             )
                 BEGIN
                     DECLARE @TBL_CMSMessageData TABLE
                     (CMSMessageId INT,
                      LocaleId     INT,
                      Message      NVARCHAR(MAX),
                      MessageKey   NVARCHAR(100),
                      AreaName     VARCHAR(100)
                     );
                     DECLARE @TBL_CMSMessageDataFinal TABLE
                     (CMSMessageId INT,
                      LocaleId     INT,
                      Message      NVARCHAR(MAX),
                      MessageKey   NVARCHAR(100),
                      AreaName     VARCHAR(100)
                     );
                     INSERT INTO @TBL_CMSMessageDataFinal
                            SELECT DISTINCT ZCM.CMSMessageId,ZCM.LocaleId,ZCM.Message,ZCMK.MessageKey,''
                            FROM ZnodeCMSmessage AS ZCM
                            INNER JOIN ZnodeCMSPortalMessage AS ZCPM ON ZCM.CMSMessageId = ZCPM.CMSMessageId
                            INNER JOIN znodecmsmessagekey AS ZCMK ON ZCPM.CMSMessageKeyId = ZCMK.CMSMessageKeyId
                            WHERE ZCPM.PortalId = @PortalId
                                  AND (ZCM.LocaleId IN
                                      (
                                      (
                                          SELECT LocaleId
                                          FROM @TBL_LocaleAll
                                          WHERE RowId = @IncrementValue
                                      ), @SetLocaleId
                                      )); 

                     INSERT INTO @TBL_CMSMessageData
                            SELECT CMSMessageId,
                            (
                                SELECT LocaleId
                                FROM @TBL_LocaleAll
                                WHERE RowId = @IncrementValue
                            ) AS LocaleId,Message,MessageKey,AreaName
                            FROM @TBL_CMSMessageDataFinal
                            WHERE LocaleId =
                            (
                                SELECT LocaleId
                                FROM @TBL_LocaleAll
                                WHERE RowId = @IncrementValue
                            ); 

                     INSERT INTO @TBL_CMSMessageData
                            SELECT CMSMessageId,
                            (
                                SELECT LocaleId
            FROM @TBL_LocaleAll
                                WHERE RowId = @IncrementValue
                            ) AS LocaleId,Message,MessageKey,AreaName
                            FROM @TBL_CMSMessageDataFinal AS CMDF
                            WHERE CMDF.LocaleId = @SetLocaleId
                                  AND NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @TBL_CMSMessageData AS CMD
                                WHERE CMD.MessageKey = CMDF.MessageKey
                                      AND CMDF.AreaName = CMD.AreaName
                            );

                     INSERT INTO @ReturnXML(ReturnXMl)
                            SELECT
                            (
                                SELECT MessageKey,LocaleId,@PortalId AS PortalId,Message,AreaName AS Area,CMSMessageId 
                                FROM @TBL_CMSMessageData AS TCMD
                                WHERE TCMD.CMSMessageId = TCC.CMSMessageId
                                      AND TCC.AreaName = TCMD.AreaName
                                FOR XML PATH('MessageEntity')
                            )
                            FROM @TBL_CMSMessageData AS TCC;

                     SET @IncrementValue = @IncrementValue + 1;
                     DELETE FROM @TBL_CMSMessageData;
                     DELETE FROM @TBL_CMSMessageDataFinal;
                 END;
             SELECT *
             FROM @ReturnXML;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSMessageConfiguration @PortalId = '+CAST(@PortalId AS VARCHAR
(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSMessageConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;