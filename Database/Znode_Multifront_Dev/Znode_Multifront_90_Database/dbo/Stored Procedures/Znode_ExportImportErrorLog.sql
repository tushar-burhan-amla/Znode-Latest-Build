CREATE PROCEDURE [dbo].[Znode_ExportImportErrorLog]
(
    @WhereClause NVARCHAR(MAX),
	@FileType NVARCHAR(20),
	@ImportProcessLogId INT,
	@Status	BIT = 0 OUT
)
/*
	Summary: 
		This Procedure is used to export Import Error Log data based on input values.

	Unit Testing :
		EXEC [dbo].[Znode_ExportImportErrorLog]  @WhereClause='',@FileType=N'CSV',@ImportProcessLogId=1034,@Status=1
*/
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @Count INT, 
				@Table NVARCHAR(MAX),
				@SSQLString NVARCHAR(MAX),
				@ExportProcessLogId INT=0,
				@ImportType NVARCHAR(50);

		DECLARE @Fn_GetFilterWhereClause NVARCHAR(MAX) = ''
		SET @Fn_GetFilterWhereClause=dbo.Fn_GetFilterWhereClause(@WhereClause);

		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'RowNumber','zil.RowNumber')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'ColumnName','zil.ColumnName')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'Data','zil.Data')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'ErrorDescription','zil.ErrorDescription')
		
		SET @ImportType =
                         (
						 SELECT TOP 1 NAME AS IMPORTTYPE FROM ZnodeImportProcessLog zip
                         inner join  ZnodeImportTemplate zit WITH (NOLOCK) ON zip.ImportTemplateId=zit.ImportTemplateId
                         inner join ZnodeImportHead zih WITH (NOLOCK) on zit.ImportHeadId=zih.ImportHeadId
                         Where zip.ImportProcessLogId=@ImportProcessLogId
						 );

		INSERT INTO ZnodeExportProcessLog (ExportType,FileType,[Status],ProcessStartedDate,ProcessCompletedDate,TableName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @ImportType,@FileType,'Started',@GetDate,NULL,NULL,3,@GetDate,3,@GetDate

		SET @ExportProcessLogId = SCOPE_IDENTITY()

		SET @Table = 'ImportErrorLog'+ @ImportType +'_'+ CAST(@ExportProcessLogId As VARCHAR(20));

		UPDATE ZnodeExportProcessLog
		SET TableName = @Table
		WHERE ExportProcessLogId = @ExportProcessLogId;

		SET @SSQLString='    	
				SELECT zil.ImportLogId ID, zil.ImportProcessLogId, ISNULL(zil.RowNumber, 0) [Row Number], ISNULL(zil.ColumnName, '''') [Column Name],
					ISNULL(zil.Data, '''') [Column Data], zm.MessageName + 
					CASE 
						WHEN zm.MessageCode IN (17)	AND '''+@ImportType+''' in (''Pricing'') AND ISNULL(zil.ColumnName, '''') NOT like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultPriceRoundOff(isnull(DefaultErrorValue,''0000000.00'') - 1)
						WHEN zm.MessageCode IN (17)	AND '''+@ImportType+''' in (''Pricing'') AND ISNULL(zil.ColumnName, '''') like ''%Quantity%''  THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'') -1) 
						WHEN zm.MessageCode IN (17)	AND '''+@ImportType+''' in (''Inventory'')  THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'') - 1) 
						WHEN zm.MessageCode IN (41) AND '''+@ImportType+''' in (''Pricing'') AND ISNULL(zil.ColumnName, '''') NOT like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultPriceRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
						WHEN zm.MessageCode IN (41) AND '''+@ImportType+''' in (''Pricing'') AND ISNULL(zil.ColumnName, '''')  like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
						WHEN zm.MessageCode IN (41) AND '''+@ImportType+''' in (''Inventory'') THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
						WHEN zm.MessageCode IN (44) AND '''+@ImportType+''' in (''Pricing'') THEN +''  ''+ isnull(DefaultErrorValue,''0000000.00'' )
						WHEN zm.MessageCode IN (129) AND '''+@ImportType+''' NOT IN (''Product'') THEN +'' ''+ isnull(DefaultErrorValue,''0000000.00'')+''.''
						ELSE ''''END ''Error Description'' 
				INTO '+@Table+' 
				FROM ZnodeImportLog AS zil WITH (NOLOCK) 
				INNER JOIN ZnodeMessage AS zm WITH (NOLOCK) ON zil.ErrorDescription = CONVERT(VARCHAR(50) , zm.MessageCode)
				WHERE zil.ImportProcessLogId='+CAST(@ImportProcessLogId AS NVARCHAR(20))+' '+ @Fn_GetFilterWhereClause+'  			
				'
	    EXEC (@SSQLString);
		
		SET @Count = @@ROWCOUNT;
				
		SET @GetDate = dbo.Fn_GetDate();

		UPDATE ZnodeExportProcessLog 
		SET [Status]= 'In Progress'
		WHERE ExportProcessLogId =@ExportProcessLogId;

		SELECT TableName
		FROM ZnodeExportProcessLog
		WHERE ExportProcessLogId = @ExportProcessLogId;

		SELECT @Count As [COUNT];

		SET @Status = 1;
	END TRY
 
	BEGIN CATCH
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'Znode_ExportImportErrorLog 
					@WhereClause = '+CAST(@WhereClause AS NVARCHAR(MAX))+',
					@FileType='+CAST(@FileType AS VARCHAR(20))+',
					@ImportProcessLogId='+CAST(@ImportProcessLogId AS NVARCHAR(20))+',
					@Status='+CAST(@Status AS VARCHAR(10));
		
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ExportImportErrorLog',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END