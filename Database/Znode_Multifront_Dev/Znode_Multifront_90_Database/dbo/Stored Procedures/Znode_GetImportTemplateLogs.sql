  
CREATE  PROCEDURE [dbo].[Znode_GetImportTemplateLogs]
( @WhereClause NVARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
    /*
	
    Summary : Get Import Template Log details and errorlog associated to it
     Unit Testing 
	 begin tran
    DECLARE @RowsCount INT;
    EXEC Znode_GetImportTemplateLogs @WhereClause = '',@Rows = 1000,@PageNo = 1,@Order_BY = NULL,@RowsCount = @RowsCount OUT;
	rollback tran
   
    */
	 BEGIN
        BEGIN TRY
          SET NOCOUNT ON;

			 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

		     DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_ErrorLog TABLE(ImportHeadId INT, ImportName NVARCHAR(100),TemplateName VARCHAR(300) ,[Status] VARCHAR(50) ,ProcessStartedDate DATETIME ,ProcessCompletedDate DATETIME
										,ImportTemplateId INT,ImportProcessLogId INT ,RowId INT,CountNo  INT)


			IF Object_id('Tempdb..[#ErrorLog]') IS NOT NULL
			DROP TABLE Tempdb..[#ErrorLog]

			CREATE TABLE #ErrorLog(ImportHeadId INT, ImportName NVARCHAR(100), TemplateName NVARCHAR(600),Status NVARCHAR(100),
			ProcessStartedDate DATETIME,ProcessCompletedDate DATETIME,ImportTemplateId INT,ImportProcessLogId INT,Createdby INT)

					   INSERT INTO #ErrorLog(ImportHeadId,ImportName,TemplateName,Status,ProcessStartedDate,ProcessCompletedDate,ImportTemplateId,ImportProcessLogId,Createdby)
						select zih.ImportHeadId, zih.NAME ImportName, zit.TemplateName,zipl.Status ,
						zipl.ProcessStartedDate,
						zipl.ProcessCompletedDate,zipl.ImportTemplateId ,zipl.ImportProcessLogId,zipl.Createdby
						from ZnodeImportHead zih inner join  ZnodeImportTemplate zit ON zih.ImportHeadId = zit.ImportHeadId  
						Inner join ZnodeImportProcessLog zipl on zit.ImportTemplateId = zipl.ImportTemplateId
						
			            SET @SQL = ';With Cte_ErrorlogFilter AS
						(

					   SELECT ImportHeadId,ImportName,TemplateName,Status,ProcessStartedDate,ProcessCompletedDate,ImportTemplateId
								,ImportProcessLogId,'+dbo.Fn_GetPagingRowId(@Order_BY,'ImportProcessLogId DESC')+', Count(*)Over() CountNo
					   FROM #ErrorLog
					   WHERE 1 = 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						) 
					   SELECT ImportHeadId, ImportName,TemplateName ,Status,ProcessStartedDate,ProcessCompletedDate,ImportTemplateId ,ImportProcessLogId,RowId,CountNo 
					   FROM Cte_ErrorlogFilter 
					   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	       
			 INSERT INTO @TBL_ErrorLog (ImportHeadId, ImportName,TemplateName ,[Status],ProcessStartedDate,ProcessCompletedDate,ImportTemplateId ,ImportProcessLogId,RowId,CountNo )
			 EXEC(@SQl)												
             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ErrorLog ), 0);

			 SELECT ImportHeadId, ImportName,TemplateName ,Status,ProcessStartedDate,ProcessCompletedDate,ImportTemplateId ,ImportProcessLogId
			 FROM @TBL_ErrorLog

			 IF Object_id('Tempdb..[#ErrorLog]') IS NOT NULL DROP TABLE Tempdb..[#ErrorLog]
           
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportTemplateLogs @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetImportTemplateLogs',
				@ErrorInProcedure = 'Znode_GetImportTemplateLogs',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                   
         END CATCH;
     END;