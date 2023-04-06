CREATE PROCEDURE [dbo].[Znode_GetTouchPointConfigurationList]
(   @TouchPointConfigurationXML XML,
	@WhereClause                VARCHAR(1000) = NULL,
	@Rows                       INT           = 1000,
	@PageNo                     INT           = 1,
	@Order_BY                   VARCHAR(100)  = 'ConnectorTouchPoints',
	@IsAssigned					bit			  = 0,
	@RowsCount                  INT OUT)  

AS
/*
Summary: This Procedure is used to get
Unit Testing :
 EXEC [dbo].[Znode_GetTouchPointConfigurationList]
*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		  DECLARE  @TBL_TouchPointConfiguration  TABLE  (ERPTaskSchedulerId INT ,ERPConfiguratorId int,Interface Nvarchar(200) ,SchedulerName Nvarchar(200), SchedulerType varchar(20),ConnectorTouchPoints nvarchar(500)
											   ,IsEnabled bit,Triggers VArchar(max),NextRunTime VArchar(50),SchedulerCreatedDate Varchar(50),EventID int,LastRunResult VArchar(200),LastRunTime VArchar(50), SchedulerCallFor varchar(200),RowId INT  , CountId INT)
											 

		 IF @Order_BY LIKE '%ConnectorTouchPoints%'
		 BEGIN 
		   SET @Order_BY = @Order_BY+',Interface'
		 END 
		 ELSE IF @Order_BY LIKE '%Interface%' 
		 BEGIN 
		 SET @Order_BY = @Order_BY+',ConnectorTouchPoints' 
		 END 
		 ELSE IF @Order_BY LIKE '%schedulername%' 
		 BEGIN 
		 SET @Order_BY = +''+@Order_BY+',ConnectorTouchPoints' 
		 END 
		 ELSE IF @Order_BY LIKE '%schedulertype%' 
		 BEGIN 
		 SET @Order_BY = +''+@Order_BY+',ConnectorTouchPoints' 
		 END 
		 ELSE IF @Order_BY LIKE '%IsEnabled%' 
		 BEGIN 
		 SET @Order_BY = +''+@Order_BY+',ConnectorTouchPoints' 
		 END 
		 ELSE IF @Order_BY = '' OR  @Order_BY IS NULL 
		 BEGIN 
		  SET @Order_BY ='IsEnabled desc,Interface,ConnectorTouchPoints' 
		 END 
		 ELSE 
		 BEGIN 
		  SET  @Order_BY = @Order_BY+',ConnectorTouchPoints,Interface' 
		 END 



             DECLARE @SQL NVARCHAR(MAX);
             SET @SQL = ' 
   
     DECLARE  @TBL_TouchPointConfiguration  TABLE  (ERPTaskSchedulerId INT ,ERPConfiguratorId int,Interface Nvarchar(200) ,SchedulerName Nvarchar(200),SchedulerType varchar(20),ConnectorTouchPoints nvarchar(500)
											   ,IsEnabled bit,Triggers VArchar(max),NextRunTime VArchar(50),SchedulerCreatedDate Varchar(50),EventID int,LastRunResult VArchar(200),LastRunTime VArchar(50), SchedulerCallFor varchar(200)
											 )
   
    INSERT INTO @TBL_TouchPointConfiguration
		SELECT 
				 Tbl.Col.value(''ERPTaskSchedulerId[1]'', ''NVARCHAR(max)'') ERPTaskSchedulerId
				,Tbl.Col.value(''ERPConfiguratorId[1]'', ''NVARCHAR(max)'') ERPConfiguratorId
				,Tbl.Col.value(''Interface[1]'', ''NVARCHAR(max)'') Interface
				,Tbl.Col.value(''SchedulerName[1]'', ''NVARCHAR(max)'') SchedulerName
				,Tbl.Col.value(''SchedulerType[1]'', ''VARCHAR(max)'') SchedulerType
				,Tbl.Col.value(''ConnectorTouchPoints[1]'', ''NVARCHAR(max)'') ConnectorTouchPoints
				,Tbl.Col.value(''IsEnabled[1]'', ''NVARCHAR(max)'') IsEnabled
				,Tbl.Col.value(''Triggers[1]'', ''NVARCHAR(max)'') Triggers
				,Tbl.Col.value(''NextRunTime[1]'', ''NVARCHAR(max)'') NextRunTime
				,Tbl.Col.value(''SchedulerCreatedDate[1]'', ''NVARCHAR(max)'') SchedulerCreatedDate
				,Tbl.Col.value(''EventID[1]'', ''NVARCHAR(max)'') EventID
				,Tbl.Col.value(''LastRunResult[1]'', ''NVARCHAR(max)'') LastRunResult
				,Tbl.Col.value(''LastRunTime[1]'', ''NVARCHAR(max)'') LastRunTime
				,Tbl.Col.value(''SchedulerCallFor[1]'', ''NVARCHAR(max)'') SchedulerCallFor
		 FROM   @TouchPointConfigurationXML.nodes(''//ArrayOfTouchPointConfigurationModel//TouchPointConfigurationModel'') Tbl(Col)




  ;With Cte_GetTouchPointList AS
   (
	   SELECT  a.ERPTaskSchedulerId ,a.ERPConfiguratorId,Interface ,ISNULL(a.SchedulerName,'''') as SchedulerName,ISNULL(a.SchedulerType,'''') as SchedulerType,ConnectorTouchPoints ,a.IsEnabled  ,ISNULL(Triggers,'''') as Triggers ,ISNULL(NextRunTime,'''') as NextRunTime
					,SchedulerCreatedDate ,EventID,ISNULL(LastRunResult,'''') as LastRunResult, ISNULL(LastRunTime,'''') as LastRunTime
					,ISNULL(a.SchedulerCallFor,'''') as SchedulerCallFor
	   FROM @TBL_TouchPointConfiguration a
	   left outer join ZnodeERPTaskScheduler b on(a.ERPConfiguratorId =b.ERPConfiguratorId and a.ConnectorTouchPoints = b.TouchPointName and '+cast(@IsAssigned as varchar(1))+' = 1 )
	   WHERE b.TouchPointName IS NULL
    ), 
   Cte_GetTouchPointListDetails as 
   (
   SELECT *,'+[dbo].[Fn_GetPagingRowId](@Order_BY,'IsEnabled desc,Interface,ConnectorTouchPoints')+' ,Count(*)Over() CountId
   FROM Cte_GetTouchPointList 
   where 1=1  '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)
   +'
   ) SELECT * 
   FROM Cte_GetTouchPointListDetails 
   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+'
   '  
             PRINT @SQL
         
			INSERT INTO @TBL_TouchPointConfiguration 
		
		     EXEC SP_executesql
                  @SQL,
                  N'@TouchPointConfigurationXML XML ',
                  @TouchPointConfigurationXML = @TouchPointConfigurationXML;
           SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_TouchPointConfiguration ),0)

		   SELECT a.ERPTaskSchedulerId ,a.ERPConfiguratorId,Interface , a.SchedulerName,a.SchedulerType,ConnectorTouchPoints ,a.IsEnabled  , Triggers ,NextRunTime
				,SchedulerCreatedDate ,EventID, LastRunResult ,LastRunTime, a.SchedulerCallFor
		   FROM @TBL_TouchPointConfiguration  a
		
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTouchPointConfigurationList @TouchPointConfigurationXML='+CAST(@TouchPointConfigurationXML AS VARCHAR(max))+', @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTouchPointConfigurationList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;