CREATE PROCEDURE [dbo].[Znode_ExportFormSubmissions] 
(
    @WhereClause NVARCHAR(MAX),
	@FileType NVARCHAR(20),
	@Status	BIT = 0 OUT
)
/*
	Summary: 
		This Procedure is used to export FormSubmissions data based on input values.

	Unit Testing :
		EXEC Znode_ExportFormSubmissions @WhereClause='',@FileType=N'CSV',@Status=1
		EXEC Znode_ExportFormSubmissions @WhereClause=N'formcode like ''%BottleSuggestionForm%''',@FileType=N'CSV',@Status=1
		EXEC Znode_ExportFormSubmissions @WhereClause=N'storename like ''%QA Znode Bottles%''',@FileType=N'CSV',@Status=1
		EXEC Znode_ExportFormSubmissions @WhereClause=N'username like ''%shubham.yende@yopmail.com%''',@FileType=N'CSV',@Status=1
		EXEC Znode_ExportFormSubmissions @WhereClause=N'fullname like ''%Tapesh D%''',@FileType=N'CSV',@Status=1
		EXEC Znode_ExportFormSubmissions @WhereClause=N'CreatedDate between ''05/19/2022 12:00:00 am'' and ''05/19/2022 11:59:59 pm''',@FileType=N'CSV',@Status=1

*/
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @ExportProcessLogId INT = 0, @Count INT, @Table NVARCHAR(MAX), @GLobalAttributeCode NVARCHAR(MAX), @SSQLString NVARCHAR(MAX);
		DECLARE @Fn_GetFilterWhereClause NVARCHAR(MAX) = ''
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

		SET @Fn_GetFilterWhereClause=dbo.Fn_GetFilterWhereClause(@WhereClause);

		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'FormCode','ZFB.FormCode')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'StoreName','ZP.StoreName')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'UserName','ZU.UserName')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'FullName','ZU.FullName')
		SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'CreatedDate','SS.CreatedDate')
			
		INSERT INTO ZnodeExportProcessLog (ExportType,FileType,[Status],ProcessStartedDate,ProcessCompletedDate,TableName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT 'FormSubmissions',@FileType,'Started',@GetDate,NULL,NULL,3,@GetDate,3,@GetDate

		SET @ExportProcessLogId = SCOPE_IDENTITY()

		SET @Table = 'ExportFormSubmissions'+'_'+ CAST(@ExportProcessLogId As VARCHAR(20));

		UPDATE ZnodeExportProcessLog
		SET TableName = @Table
		WHERE ExportProcessLogId = @ExportProcessLogId;

		IF OBJECT_ID('tempdb..[#GlobalAttribute]') IS NOT NULL
			DROP TABLE tempdb..[#GlobalAttribute];

		IF OBJECT_ID('tempdb..[##GLobalAttributeData]') IS NOT NULL
			DROP TABLE tempdb..[##GLobalAttributeData];
					
		SELECT A.FormBuilderSubmitId,C.AttributeCode
			,CASE WHEN B.MediaPath IS NOT NULL THEN RIGHT(B.MediaPath,LEN(B.MediaPath)-(CHARINDEX('--',B.MediaPath,1)+1)) ELSE B.AttributeValue END As AttributeValue
			, B.GlobalAttributeDefaultValueId, B.LocaleId
		INTO #GlobalAttribute
		FROM ZnodeFormBuilderGlobalAttributeValue A WITH (NOLOCK)
		INNER JOIN ZnodeFormBuilderGlobalAttributeValueLocale B WITH (NOLOCK) ON B.FormBuilderGlobalAttributeValueId=A.FormBuilderGlobalAttributeValueId
		INNER JOIN ZnodeGlobalAttribute C WITH (NOLOCK) ON C.GlobalAttributeId=A.GlobalAttributeId;

		UPDATE aa
		SET AttributeValue=CASE WHEN aa.AttributeValue IS NULL THEN RIGHT(h.AttributeDefaultValueCode,LEN(h.AttributeDefaultValueCode)-(CHARINDEX('--',h.AttributeDefaultValueCode,1)+1)) ELSE aa.AttributeValue END
		FROM #GlobalAttribute aa
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId
		WHERE ISNULL(aa.AttributeValue,'')='';

		UPDATE h
		SET h.AttributeValue=RIGHT(g.AttributeDefaultValue,LEN(g.AttributeDefaultValue)-(CHARINDEX('--',g.AttributeDefaultValue,1)+1))
		FROM #GlobalAttribute h
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
        WHERE ISNULL(h.AttributeValue,'')='' AND g.LocaleId=h.LocaleId;

		ALTER TABLE #GlobalAttribute DROP COLUMN IF EXISTS GlobalAttributeDefaultValueId;
		ALTER TABLE #GlobalAttribute DROP COLUMN IF EXISTS LocaleId;
		
		SELECT @GLobalAttributeCode = STUFF((
		SELECT DISTINCT ',' + AttributeCode
		FROM #GlobalAttribute
		FOR XML PATH('') 
		), 1, 1, '');

		SET @SSQLString ='SELECT * INTO ##GLobalAttributeData
		FROM (
		SELECT FormBuilderSubmitId,AttributeCode, AttributeValue FROM #GlobalAttribute
		)T
		PIVOT(MAX(Attributevalue) FOR AttributeCode IN ('+@GLobalAttributeCode+'))PIV'

		EXEC (@SSQLString);
		
		SET @SSQLString='SELECT ZFB.FormCode,ZP.StoreName,ZU.UserName,ZU.FullName,SS.CreatedDate,V.*
		INTO '+@Table+'
		FROM ZnodeFormBuilderSubmit ss WITH (NOLOCK) 
		INNER JOIN ZnodeFormBuilder ZFB WITH (NOLOCK) on SS.FormBuilderId=ZFB.FormBuilderId
		INNER JOIN ZnodePortal ZP WITH (NOLOCK) ON ZP.PortalId=SS.PortalId 
		LEFT JOIN View_CustomerUserDetail ZU WITH (NOLOCK) ON ZU.UserId=SS.UserId												
		LEFT JOIN ##GLobalAttributeData V ON (V.FormBuilderSubmitId=SS.FormBuilderSubmitId)
		WHERE 1=1 '+ @Fn_GetFilterWhereClause+'
		'		
		EXEC (@SSQLString);
		
		SET @Count = @@ROWCOUNT;
				
		IF OBJECT_ID('tempdb..[#GlobalAttribute]') IS NOT NULL
			DROP TABLE tempdb..[#GlobalAttribute];

		IF OBJECT_ID('tempdb..[##GLobalAttributeData]') IS NOT NULL
			DROP TABLE tempdb..[##GLobalAttributeData];
		
		SET @GetDate = dbo.Fn_GetDate();

		UPDATE ZnodeExportProcessLog 
		SET [Status]= 'In Progress',
			ProcessCompletedDate = dbo.Fn_GetDate()
		WHERE ExportProcessLogId =@ExportProcessLogId;

		SELECT TableName
		FROM ZnodeExportProcessLog
		WHERE ExportProcessLogId = @ExportProcessLogId;

		SELECT @Count As [Count];

		SET @Status = 1;
	END TRY
 
	BEGIN CATCH
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'Znode_ExportFormSubmissions 
					@WhereClause = '+CAST(@WhereClause AS NVARCHAR(MAX))+',
					@FileType='+CAST(@FileType AS VARCHAR(20))+',
					@Status='+CAST(@Status AS VARCHAR(10));
		
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ExportFormSubmissions',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END