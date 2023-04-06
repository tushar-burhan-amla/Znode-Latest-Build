Create PROCEDURE [dbo].[Znode_DeleteStockNotifications]
AS
BEGIN
	BEGIN TRY
	DECLARE @Status BIT;
	declare @DeleteSentEmail int , @DeletePendingEmails int
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	select @DeleteSentEmail = (select Top 1  FeatureValues from ZnodeGlobalSetting where FeatureName = 'DeleteAlreadySentEmails')
	select @DeletePendingEmails = (select Top 1  FeatureValues from ZnodeGlobalSetting where FeatureName = 'DeletePendingEmails')

	IF OBJECT_ID('tempdb..#Tbl_StockNotice') IS NOT NULL
	DROP TABLE #Tbl_StockNotice

				Create TABLE #Tbl_StockNotice
				(
					  StockNoticeId  int,
					  EmailId    varchar(100),
					  DateDiffSent  int,
					  DateDiffPending int,
					  IsEmailSent bit
			    );

  insert into #Tbl_StockNotice(StockNoticeId, EmailId, DateDiffSent, DateDiffPending, IsEmailSent)
  select ZSN.StockNoticeId, ZSN.EmailId, (SELECT DATEDIFF(day,ZSN.ModifiedDate,  @GetDate)) as DateDiffSent ,
  (SELECT DATEDIFF(day, ZSN.CreatedDate, @GetDate )) as DateDiffPending,ZSN.IsEmailSent  from ZnodeStockNotice ZSN

	
	DELETE FROM ZnodeStockNotice 
    WHERE StockNoticeId IN (SELECT StockNoticeId FROM #Tbl_StockNotice WHERE (IsEmailSent = 0 AND DateDiffPending > @DeletePendingEmails)
                   OR (IsEmailSent = 1 AND DateDiffSent > @deleteSentEmail))
		 
		 SET @Status = 1  
			  SELECT 1 AS ID,@Status AS Status; 
	
	END TRY

	BEGIN CATCH
		  SET @Status =0  
			  SELECT 1 AS ID,@Status AS Status; 
		select @Status
		DECLARE @Error_procedure VARCHAR(1000) = ERROR_PROCEDURE()
			,@ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE()
			,@ErrorLine VARCHAR(100) = ERROR_LINE()
			,@ErrorCall NVARCHAR(MAX) = 'EXEC Znode_DeleteStockNotifications';
		SELECT 0 AS ID
			,CAST(0 AS BIT) AS STATUS;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_DeleteStockNotifications'
			,@ErrorInProcedure = ''
			,@ErrorMessage = @ErrorMessage
			,@ErrorLine = @ErrorLine
			,@ErrorCall = @ErrorCall;
	END CATCH;
END;