CREATE PROCEDURE [dbo].[Znode_DeleteContentWidget]
(
	@ContentWidgetIds  VARCHAR(2000),
	@status  BIT OUT
)
AS
BEGIN
		BEGIN TRY

			 BEGIN TRAN DeleteContentWidget
			
				 DECLARE @TBL_DeleteWidgetId TABLE(widgetId INT); 
             
					INSERT INTO @TBL_DeleteWidgetId(widgetId)
					SELECT Item FROM  dbo.Split(@ContentWidgetIds, ',')	
				 
					DELETE GWL  from ZnodeWidgetGlobalAttributeValueLocale GWL
					INNER JOIN ZnodeWidgetGlobalAttributeValue WGA ON 
					GWL.WidgetGlobalAttributeValueId = WGA.WidgetGlobalAttributeValueId
					INNER JOIN @TBL_DeleteWidgetId DC ON WGA.CMSContentContainerId = DC.widgetId
					
					
					DELETE GAV  FROM ZnodeWidgetGlobalAttributeValue GAV
					INNER JOIN @TBL_DeleteWidgetId DC ON  GAV.CMSContentContainerId = DC.widgetId	
					
					DELETE GEFM FROM ZnodeGlobalEntityFamilyMapper GEFM			
					INNER JOIN @TBL_DeleteWidgetId DC ON  GEFM.GlobalEntityValueId = DC.widgetId	
         
					DELETE CWL
					FROM ZnodeCMSContainerProfileVariantLocale CWL
					WHERE EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant CW
						INNER JOIN @TBL_DeleteWidgetId DC ON  CW.CMSContentContainerId = DC.widgetId
						WHERE CW.CMSContainerProfileVariantId = CWL.CMSContainerProfileVariantId)

					DELETE WPV FROM ZnodeCMSWidgetProfileVariant WPV
					INNER JOIN @TBL_DeleteWidgetId DC ON  WPV.CMSContentWidgetId = DC.widgetId

					DELETE CW FROM ZnodeCMSContentWidget CW
					INNER JOIN @TBL_DeleteWidgetId DC ON  CW.CMSContentWidgetId = DC.widgetId

				  SET @Status = 1;
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];

             COMMIT TRAN DeleteContentWidget;
		END TRY

		BEGIN CATCH

             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			 @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteContentWidget 
			 @ContentWidgetIds = '+@ContentWidgetIds+',@Status='+CAST(@Status AS VARCHAR(50));


			  	 SET @Status =0  
				 SELECT 1 AS ID,@Status AS Status;  
				 ROLLBACK TRAN DeleteContentWidget;
				 EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteContentWidget',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
       
		END CATCH

END
