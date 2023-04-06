
CREATE PROCEDURE [dbo].[Znode_GetTextWidgetConfiguration]
(
       @PortalId INT
	   ,@UserId INT =  0  	
	   ,@CMSMappingId INT =0,
	   @LocaleId INT = 0
)
AS
/*
Summary: This Procedure is used to get text widget configuration
Unit Testing :
 EXEC Znode_GetTextWidgetConfiguration 1,2

 exec Znode_GetTextWidgetConfiguration 1,2,213,1

 exec Znode_GetTextWidgetConfiguration @PortalId=1,@UserId=2,@CMSMappingId=213
*/
     BEGIN
         BEGIN TRY
		
             DECLARE @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId() ,@IncrementValue INT= 1;

             DECLARE @LocaleAll TABLE (
                                      RowId    INT IDENTITY(1 , 1) ,
                                      LocaleId INT ,
                                      Code     VARCHAR(300)
                                      );
             INSERT INTO @LocaleAll ( LocaleId , Code
                                    )
                    SELECT LocaleId , Code
                    FROM ZnodeLocale AS a
                    WHERE a.IsActive = 1 AND
					a.LocaleId IN (CASE WHEN  @LocaleId = 0  THEN LocaleId ELSE @LocaleId END)
					;

             DECLARE @ReturnXML TABLE (
                                      ReturnXMl XML
                                      );
             WHILE @IncrementValue <= ( SELECT MAX(RowId)
                                        FROM @LocaleAll
                                      )
                 BEGIN
                     DECLARE @CMSWidgetData TABLE (CMSTextWidgetConfigurationId INT ,LocaleId  INT ,CMSWidgetsId INT ,WidgetsKey NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping   NVARCHAR(100) ,[Text]  NVARCHAR(MAX));
                     
					 DECLARE @CMSWidgetDataFinal TABLE (CMSTextWidgetConfigurationId INT ,LocaleId    INT ,CMSWidgetsId INT ,WidgetsKey  NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping NVARCHAR(100) ,[Text]  NVARCHAR(MAX));

                     INSERT INTO @CMSWidgetDataFinal
                            SELECT CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM ZnodeCMSTextWidgetConfiguration AS a
                            WHERE (a.TypeOFMapping = 'ContentPageMapping'
                            AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
                            OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId )
							AND
                                 ( a.LocaleId IN 
								 ( ( SELECT LocaleId
                                                      FROM @LocaleAll
                                                      WHERE RowId = @IncrementValue
                                                    ) , @DefaultLocaleId 
                                                  ) ) )
										  
						   AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  )
						 	  


                     INSERT INTO @CMSWidgetData
                            SELECT CMSTextWidgetConfigurationId , (SELECT  LocaleId FROM @LocaleAll WHERE RowId = @IncrementValue)  AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM @CMSWidgetDataFinal
                            WHERE LocaleId = ( SELECT LocaleId
                                               FROM @LocaleAll
                                               WHERE RowId = @IncrementValue
                                             );



                     INSERT INTO @CMSWidgetData
                            SELECT CMSTextWidgetConfigurationId , ( SELECT LocaleId FROM @LocaleAll WHERE RowId = @IncrementValue) AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM @CMSWidgetDataFinal AS p
                            WHERE p.LocaleId = @DefaultLocaleId
                                  AND
                                  NOT EXISTS ( SELECT TOP 1 1
                                               FROM @CMSWidgetData AS q
                                               WHERE q.CMSWidgetsId = p.CMSWidgetsId
                                                     AND
                                                     q.WidgetsKey = p.WidgetsKey
                                                     AND
                                                     q.TypeOFMapping = p.TypeOFMapping
                                                     AND
                                                     q.CMSMappingId = p.CMSMappingId
                                             );

										

                     INSERT INTO @ReturnXML ( ReturnXMl
                                            )
                            SELECT ( SELECT CMSTextWidgetConfigurationId AS TextWidgetConfigurationId , LocaleId , CMSWidgetsId AS WidgetsId , WidgetsKey , CMSMappingId AS MappingId , TypeOFMapping , [Text] , @PortalId AS PortalId
                                     FROM @CMSWidgetData AS a
                                     WHERE a.CMSTextWidgetConfigurationId = w.CMSTextWidgetConfigurationId 
                                     FOR XML PATH('TextWidgetEntity')
                                   )
                            FROM @CMSWidgetData AS w
						
							;
                     SET @IncrementValue = @IncrementValue + 1;
                     DELETE FROM @CMSWidgetData;
                     DELETE FROM @CMSWidgetDataFinal;
                 END;
             SELECT *
             FROM @ReturnXML;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTextWidgetConfiguration @PortalId = '+CAST(@PortalId AS VARCHAR(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTextWidgetConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;