
CREATE FUNCTION [dbo].[Fn_GetRowsForPagination]
(
	 @PageNo INT 
	,@Rows  INT  
	,@ColumnName NVARCHAR(200)
)
RETURNS varchar(1000) 
AS
 ------------------------------------------------------------------------------------------------------------------------------------
 -- Summary 
 -- This Function is used to find the rows as per rows and page    
 -- here return the varchar use in dyanamic statement 
 -- 
------------------------------------------------------------------------------------------------------------------------------------
BEGIN
	-- Declare the return variable here
	DECLARE @Row varchar(300) 
	   
	  SET @Row = ' '+@ColumnName+' BETWEEN '+  
	  
	   CASE
                                   WHEN @Rows >= 1000000 -- if rows is gretter than 1000000 then this will return all the rows 
                                   THEN '0'
                                   ELSE CAST((@Rows * ( @PageNo - 1 )  )+ 1 AS VARCHAR(50))  END + ' AND '+
                              
		
      CASE
                                 WHEN @Rows >= 1000000 -- if rows is gretter than 1000000 then this will return all the rows 
                                 THEN CAST(@Rows AS VARCHAR(50))
                                 ELSE CAST(@Rows * ( @PageNo ) AS VARCHAR(50)) END
      
	-- Return the result of the function
	RETURN @Row

END