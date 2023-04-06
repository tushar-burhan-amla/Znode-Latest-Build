CREATE PROCEDURE [dbo].[Znode_GetAllMedia]
( @WhereClause VARCHAR(1000), 
  @Order_BY    VARCHAR(1000)  = '',  
  @Rows        INT           = 100,
  @PageNo      INT           = 1, 
  @RowsCount   INT OUT
)
AS
/*  
    Summary: This procedure is used to get Media list   
    Unit Testing:   
    EXEC Znode_GetAllMedia @WhereClause='' ,@Order_BY ='',  @Rows= 10, @PageNo = 1, @RowsCount=0 
*/  
  
BEGIN
	BEGIN TRY
            SET NOCOUNT ON;
            DECLARE @SQL NVARCHAR(MAX);
			DECLARE @TBL_AllMedia TABLE (MediaId INT,MediaConfigurationId INT,Path VARCHAR(300),
										 FileName VARCHAR(300) ,Type NVARCHAR(3000),RowId INT,CountNo INT)  
			
	  SET @SQL = '
	 ;WITH CTE_GetMediaList AS  
      (  
		  SELECT MediaId ,MediaConfigurationId ,Path ,FileName ,Type ,
		  ' + dbo.Fn_GetPagingRowId(@Order_BY,'MediaId ASC')+',Count(*)Over() CountNo   
		  FROM ZnodeMedia  
		  WHERE 1=1 '+ dbo.Fn_GetFilterWhereClause(@WhereClause)+'       
      )  
  
      SELECT MediaId ,MediaConfigurationId ,Path ,FileName ,Type,RowId,CountNo
	  FROM CTE_GetMediaList' + dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  

   INSERT INTO @TBL_AllMedia  
   EXEC(@SQL)  
  
   SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_AllMedia ),0)  
     
   SELECT * FROM @TBL_AllMedia  

   END TRY
   BEGIN CATCH
	       DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				    @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				    @ErrorLine VARCHAR(100)= ERROR_LINE(),
				    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAllMedia @WhereClause = '+ cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status; 
	  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAllMedia',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END