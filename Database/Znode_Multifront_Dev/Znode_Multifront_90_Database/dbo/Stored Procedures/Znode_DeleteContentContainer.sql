CREATE PROCEDURE [dbo].[Znode_DeleteContentContainer]
(
	@ContentContainerIds  VARCHAR(2000),
	@status  BIT OUT
)
AS
BEGIN
		BEGIN TRY

			 BEGIN TRAN DeleteContentContainer
			
				 DECLARE @TBL_DeleteContainerId TABLE(ContainerId INT); 
             
					INSERT INTO @TBL_DeleteContainerId(ContainerId)
					SELECT Item FROM  dbo.Split(@ContentContainerIds, ',')	
				 
					DELETE GWL  from ZnodeWidgetGlobalAttributeValueLocale GWL
					INNER JOIN ZnodeWidgetGlobalAttributeValue WGA ON 
					GWL.WidgetGlobalAttributeValueId = WGA.WidgetGlobalAttributeValueId
					INNER JOIN @TBL_DeleteContainerId DC ON WGA.CMSContentContainerId = DC.ContainerId
					
					
					DELETE GAV  FROM ZnodeWidgetGlobalAttributeValue GAV
					INNER JOIN @TBL_DeleteContainerId DC ON  GAV.CMSContentContainerId = DC.ContainerId	
					
					DELETE GEFM FROM ZnodeGlobalEntityFamilyMapper GEFM			
					INNER JOIN @TBL_DeleteContainerId DC ON  GEFM.GlobalEntityValueId = DC.ContainerId	
         
					DELETE CWL
					FROM ZnodeCMSContainerProfileVariantLocale CWL
					WHERE EXISTS(SELECT * FROM ZnodeCMSContainerProfileVariant CW
						INNER JOIN @TBL_DeleteContainerId DC ON  CW.CMSContentContainerId = DC.ContainerId
						WHERE CW.CMSContainerProfileVariantId = CWL.CMSContainerProfileVariantId)

					DELETE WPV FROM ZnodeCMSContainerProfileVariant WPV
					INNER JOIN @TBL_DeleteContainerId DC ON  WPV.CMSContentContainerId = DC.ContainerId

					DELETE CW FROM ZnodeCMSContentContainer CW
					INNER JOIN @TBL_DeleteContainerId DC ON  CW.CMSContentContainerId = DC.ContainerId

				  SET @Status = 1;
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];

             COMMIT TRAN DeleteContentContainer;
		END TRY

		BEGIN CATCH
		ROLLBACK TRAN DeleteContentContainer;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			 @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteContentContainer 
			 @ContentContainerIds = '+@ContentContainerIds+',@Status='+CAST(@Status AS VARCHAR(50));


			  	 SET @Status =0  
				 SELECT 1 AS ID,@Status AS Status;  
				 
				 EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteContentContainer',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
       
		END CATCH

END