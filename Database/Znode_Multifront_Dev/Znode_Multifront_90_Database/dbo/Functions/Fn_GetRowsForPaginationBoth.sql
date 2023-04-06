
CREATE FUNCTION [dbo].[Fn_GetRowsForPaginationBoth]
(
	 @PageNo   INT 
	,@Rows     INT 
	,@OrderBY  VARCHAR(1000) 
	,@PkColumn VARCHAR(100)
	,@IsRowId  INT 
)
RETURNS varchar(2000) 
AS
 ------------------------------------------------------------------------------------------------------------------------------------
 -- Summary 
 -- This Function is used to find the rows as per rows and page    
 -- here return the varchar use in dyanamic statement 
 -- 
------------------------------------------------------------------------------------------------------------------------------------
BEGIN
	-- Declare the return variable here
	DECLARE @Row varchar(2000) 
	   
	  IF @IsRowId = 1 
	  BEGIN 
	    
		SET @Row = ' DENSE_RANK()Over('+dbo.Fn_GetOrderByClause(@OrderBY,@PkColumn)+','+@PkColumn+' DESC) RowId '

      END 
	  ELSE 
	  IF @IsRowId = 2
	  BEGIN 


	  SET @Row = ' WHERE RowId BETWEEN '+  
	  
	   CASE
                                   WHEN @Rows >= 1000000 -- if rows is gretter than 1000000 then this will return all the rows 
                                   THEN '0'
                                   ELSE CAST((@Rows * ( @PageNo - 1 )  )+ 1 AS VARCHAR(50))  END + ' AND '+
                              
		
      CASE
                                 WHEN @Rows >= 1000000 -- if rows is gretter than 1000000 then this will return all the rows 
                                 THEN CAST(@Rows AS VARCHAR(50))
                                 ELSE CAST(@Rows * ( @PageNo ) AS VARCHAR(50)) END
      END 
	-- Return the result of the function
	RETURN @Row

END