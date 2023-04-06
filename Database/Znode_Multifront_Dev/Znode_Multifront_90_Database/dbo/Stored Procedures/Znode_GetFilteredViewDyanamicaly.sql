
 -- SELECT *  FROM View_GetMediaPathDetail 
-- DECLARE @FFF BIGINT  EXEC Znode_GetFilteredViewDyanamicaly @ViewName='View_GetMediaPathDetail',@WhereClause='mediaid = 162 and mediaid = 163 and mediaid = 164 and  CreatedBy = 2' ,@RowCount =@FFF OUT  SELECT @FFF
CREATE  Procedure [dbo].[Znode_GetFilteredViewDyanamicaly]
(
	 @ViewName  VARCHAR(1000),
	 @WhereClause Varchar(1000),
	 @Rows      int = 100,
	 @PageNo    int = 0,
	 @Order_By  Varchar(1000) = NULL ,
	 @RowCount  BIGInt = 10 OUT 

)
AS
	Begin 
		Begin Try 
   				SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END 
				DECLARE @V_SQL NVARCHAR(MAX) 
				DECLARE @TABLE_TOROWCOUNT TABLE(RowCountId   INT )
				
				
				SET @V_SQL = ' SELECT * INTO #temp_view FROM '+@ViewName+ ' WHERE 1=1 ' +case WHEN @WhereClause IS NOT NULL AND @WhereClause <> '' THEN  ' AND '+@WhereClause ELSE '' END
								+' SELECT  @tempo=Count(1) FROM #temp_view  SELECT * FROM #temp_view ' 
								+' Order BY '+ISNULL(@Order_BY,'1')+ ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '

                 --PRINT @V_SQL

				EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowCount out

				
		End Try 



		BeGin catch 
				Select ERROR_NUMBER(),ERROR_MESSAGE () , ERROR_SEVERITY () 
		End catch  




 END 

