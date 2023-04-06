
CREATE PROCEDURE [dbo].[Znode_GetMediaWidgetConfiguration]
(
       @PortalId INT
	   ,@UserId INT =  0  	
	   ,@CMSMappingId INT =0
)
AS
/*
Summary: This Procedure is used to get Media widget configuration
Unit Testing :
 EXEC Znode_GetMediaWidgetConfiguration 1,2

*/
     BEGIN
		 SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @ReturnXML TABLE (
                                      ReturnXMl XML
                                      );
                      
                     DECLARE @CMSWidgetData TABLE (CMSMediaConfigurationId INT ,CMSWidgetsId INT ,WidgetsKey NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping   NVARCHAR(100) ,[MediaPath]  NVARCHAR(1000));
                     
					 DECLARE @CMSWidgetDataFinal TABLE (CMSMediaConfigurationId INT ,CMSWidgetsId INT ,WidgetsKey  NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping NVARCHAR(100) ,[MediaPath]  NVARCHAR(1000));

                     INSERT INTO @CMSWidgetDataFinal
                     SELECT CMSMediaConfigurationId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , ZM.Path as MediaPath
                     FROM ZnodeCMSMediaConfiguration AS a
					 inner join ZnodeMedia ZM on a.MediaId = ZM.MediaId
                     WHERE (a.TypeOFMapping = 'ContentPageMapping'
                     AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
                     OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId ))
					 AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  )
					
                     INSERT INTO @ReturnXML ( ReturnXMl
                                            )
                            SELECT ( SELECT CMSMediaConfigurationId AS MediaWidgetConfigurationId , CMSWidgetsId AS WidgetsId , WidgetsKey , CMSMappingId AS MappingId , TypeOFMapping , MediaPath , @PortalId AS PortalId
                                     FROM @CMSWidgetDataFinal AS a
                                     WHERE a.CMSMediaConfigurationId = w.CMSMediaConfigurationId 
                                     FOR XML PATH('MediaWidgetEntity')
                                   )
                            FROM @CMSWidgetDataFinal AS w

					 SELECT * FROM @ReturnXML;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaWidgetConfiguration @PortalId = '+CAST(@PortalId AS VARCHAR(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaWidgetConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;