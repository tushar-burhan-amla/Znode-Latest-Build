CREATE PROCEDURE [dbo].[Znode_GetSearchWidgetConfiguration]
(
       @PortalId INT
	   ,@UserId INT =  0  	
	   ,@CMSMappingId INT =0 ,
	   @LocaleId INT = 0
)
AS
/*
Summary: This Procedure is used to get text widget configuration
Unit Testing :
 EXEC [dbo].[Znode_GetSearchWidgetConfiguration] 1,2,213,4
*/
     BEGIN
         BEGIN TRY
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId() , @IncrementValue INT= 1;
             DECLARE @LocaleAll TABLE (
                                      RowId    INT IDENTITY(1 , 1) ,
                                      LocaleId INT ,
                                      Code     VARCHAR(300)
                                      );
             INSERT INTO @LocaleAll ( LocaleId , Code
                                    )
                    SELECT LocaleId , Code
                    FROM ZnodeLocale AS a
                    WHERE a.IsActive = 1
					AND
					a.LocaleId IN (CASE WHEN  @LocaleId = 0  THEN LocaleId ELSE @LocaleId END);


             DECLARE @ReturnXML TABLE (
                                      ReturnXMl XML
                                      );
             WHILE @IncrementValue <= ( SELECT MAX(RowId)
                                        FROM @LocaleAll
                                      )
                 BEGIN
                     DECLARE @CMSWidgetData TABLE (
                                                  CMSSearchWidgetId INT ,
                                                  LocaleId                     INT ,
                                                  CMSWidgetsId                 INT ,
                                                  WidgetsKey                   NVARCHAR(256) ,
                                                  CMSMappingId                 INT ,
                                                  TypeOFMapping                NVARCHAR(100) ,
												  [AttributeCode]   varchar(300),
                                                  [SearchKeyword]             varchar(300)
                                                  );
                     DECLARE @CMSWidgetDataFinal TABLE (
                                                       CMSSearchWidgetId INT ,
                                                       LocaleId                     INT ,
                                                       CMSWidgetsId                 INT ,
                                                       WidgetsKey                   NVARCHAR(256) ,
                                                       CMSMappingId                 INT ,
                                                       TypeOFMapping                NVARCHAR(100) ,
                                                      [AttributeCode]   varchar(300),
                                                       [SearchKeyword]             varchar(300)
                                                       );
                     INSERT INTO @CMSWidgetDataFinal
                            SELECT CMSSearchWidgetId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , AttributeCode,SearchKeyword
                            FROM ZnodeCMSSearchWidget AS a
                            WHERE ( a.TypeOFMapping = 'ContentPageMapping'
                                  AND EXISTS ( SELECT TOP 1 1
                                           FROM ZnodeCMSContentPages
                                           WHERE a.CMSMappingId = CMSContentPagesId
                                                 AND PortalId = @PortalId  )
                                              OR ( a.TypeOFMapping = 'PortalMapping'
													AND
													a.CMSMappingId = @PortalId )
												AND
                                  ( a.LocaleId IN ( ( SELECT LocaleId
                                                      FROM @LocaleAll
                                                      WHERE RowId = @IncrementValue
                                                    ) , @DefaultLocaleId
                                                  ) )	  )
							     AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  );
							--	SELECT * fROM @CMSWidgetDataFinal
                     INSERT INTO @CMSWidgetData
                            SELECT CMSSearchWidgetId , ( SELECT LocaleId
                                                                    FROM @LocaleAll
                                                                    WHERE RowId = @IncrementValue
                                                                  ) AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping ,AttributeCode,SearchKeyword
                            FROM @CMSWidgetDataFinal
                            WHERE LocaleId = ( SELECT LocaleId
                                               FROM @LocaleAll
                                               WHERE RowId = @IncrementValue
                                             );
                     INSERT INTO @CMSWidgetData
                            SELECT CMSSearchWidgetId , ( SELECT LocaleId
                                                                    FROM @LocaleAll
                                                                    WHERE RowId = @IncrementValue
                                                                  ) AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , AttributeCode,SearchKeyword
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
                            SELECT ( SELECT CMSSearchWidgetId AS CMSSearchWidgetId , LocaleId , CMSWidgetsId AS WidgetsId , WidgetsKey , CMSMappingId AS MappingId , TypeOFMapping , ISNULL(AttributeCode,'') AttributeCode,SearchKeyword , @PortalId AS PortalId
                                     FROM @CMSWidgetData AS a
                                     WHERE a.CMSSearchWidgetId = w.CMSSearchWidgetId
                                     FOR XML PATH('SearchWidgetEntity')
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
				@ProcedureName = 'Znode_GetSearchWidgetConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;