
  
CREATE PROCEDURE [dbo].[Znode_GetTouchPointSchedulerHistory]
(   @TouchPointSchedulerHistoryXML XML,
	@WhereClause                VARCHAR(1000) = NULL,
	@Rows                       INT           = 1000,
	@PageNo                     INT           = 1,
	@Order_BY                   VARCHAR(100)  = 'DateTime desc',
	@RowsCount                  INT OUT)  

AS
/*
Summary: This Procedure is used to get
Unit Testing :
 EXEC [dbo].[Znode_GetTouchPointSchedulerHistory]
*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		  DECLARE  @TBL_TouchPointSchedulerHistory  TABLE  (SchedulerName Nvarchar(200),Level varchar(200), TaskCategory varchar(200),DateTime Varchar(50),RecordId varchar(max),EventID Varchar(50),OperationalCode varchar(max),LogName varchar(max),MachineName varchar(max),CorrelationId varchar(200) ,LastRunTime varchar(200),RowId INT , CountId INT)

		 --IF @Order_BY LIKE '%SchedulerName%'
		 --BEGIN 
		 --  SET @Order_BY = @Order_BY+',Interface'
		 --END 
		 --ELSE IF @Order_BY LIKE '%Interface%' 
		 --BEGIN 
		 --SET @Order_BY = @Order_BY+',ConnectorTouchPoints' 
		 --END 
		 --ELSE IF @Order_BY = '' OR  @Order_BY IS NULL 
		 --BEGIN 
		 -- SET @Order_BY ='IsEnabled desc,Interface,ConnectorTouchPoints' 
		 --END 
		 --ELSE 
		 --BEGIN 
		 -- SET  @Order_BY = @Order_BY+',ConnectorTouchPoints,Interface' 
		 --END 

             DECLARE @SQL NVARCHAR(MAX);
             SET @SQL = ' 
   
			 DECLARE  @TBL_TouchPointSchedulerHistory  TABLE  (Id int identity(1,1) ,SchedulerName Nvarchar(200),Level varchar(200),TaskCategory varchar(200),DateTime Varchar(50),RecordId varchar(max),EventID Varchar(50),OperationalCode varchar(max),LogName varchar(max),MachineName varchar(max),CorrelationId varchar(200),LastRunTime varchar(200))
   
			INSERT INTO @TBL_TouchPointSchedulerHistory (SchedulerName ,Level,TaskCategory, DateTime,RecordId,EventID , OperationalCode,LogName,MachineName,CorrelationId,LastRunTime)
				SELECT 
						 Tbl.Col.value(''SchedulerName[1]'', ''NVARCHAR(max)'') SchedulerName
						,Tbl.Col.value(''Level[1]'', ''VARCHAR(max)'') Level
						,Tbl.Col.value(''TaskCategory[1]'', ''VARCHAR(max)'') TaskCategory
						,Tbl.Col.value(''DateTime[1]'', ''NVARCHAR(max)'') DateTime
						,Tbl.Col.value(''RecordId[1]'', ''VARCHAR(max)'') RecordId
						,Tbl.Col.value(''EventID[1]'', ''VARCHAR(max)'') EventID
						,Tbl.Col.value(''OperationalCode[1]'', ''VARCHAR(max)'') OperationalCode
						,Tbl.Col.value(''LogName[1]'', ''VARCHAR(max)'') LogName
						,Tbl.Col.value(''MachineName[1]'', ''VARCHAR(max)'') MachineName
						,Tbl.Col.value(''CorrelationId[1]'', ''VARCHAR(max)'') CorrelationId
						,Tbl.Col.value(''LastRunTime[1]'', ''VARCHAR(max)'') LastRunTime
				 FROM   @TouchPointSchedulerHistoryXML.nodes(''//ArrayOfTouchPointConfigurationModel//TouchPointConfigurationModel'') Tbl(Col)
			  ;With Cte_GetTouchPointList AS
			   (
			   SELECT  ISNULL(SchedulerName,'''') as SchedulerName,ISNULL(Level,'''') as Level,ISNULL(TaskCategory,'''') as TaskCategory,DateTime,ISNULL(RecordId,'''') as RecordId,ISNULL(EventID,'''') as EventID,ISNULL(OperationalCode,'''') as OperationalCode,ISNULL(LogName,'''') as LogName,ISNULL(MachineName,'''') as MachineName,
			   ISNULL(CorrelationId,'''') as CorrelationId,ISNULL(LastRunTime,'''') as LastRunTime, '+[dbo].[Fn_GetPagingRowId](@Order_BY,'Id desc')+',Count(*)Over() CountId 
			   FROM @TBL_TouchPointSchedulerHistory  WHERE 1=1 
			   '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)
			   +'
			   )
			   SELECT * 
			   FROM Cte_GetTouchPointList 
			   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+'
			   '  
			INSERT INTO @TBL_TouchPointSchedulerHistory
		     EXEC SP_executesql
                  @SQL,
                  N'@TouchPointSchedulerHistoryXML XML ',
                  @TouchPointSchedulerHistoryXML = @TouchPointSchedulerHistoryXML;
           SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_TouchPointSchedulerHistory),0)

		   SELECT a.SchedulerName,a.TaskCategory, a.Level, DateTime,a.RecordId,EventID ,OperationalCode,LogName,MachineName,CorrelationId,LastRunTime
		   FROM @TBL_TouchPointSchedulerHistory a
		 END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTouchPointSchedulerHistory @TouchPointSchedulerHistoryXML='+CAST(@TouchPointSchedulerHistoryXML AS VARCHAR(max))+', @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTouchPointSchedulerHistory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;