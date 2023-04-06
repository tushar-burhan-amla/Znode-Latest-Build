-- =============================================
-- Create the Order by clase for dyanamic statement  
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetOrderByClause]
(
  @OrderBy NVARCHAR(MAX)
  ,@DefaultOrderBy NVARCHAR(1000)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Order  NVARCHAR(max)

	SET @Order = ' Order By '+CASE WHEN @OrderBy = '' OR @OrderBy IS NULL  THEN @DefaultOrderBy ELSE @OrderBy END 

    RETURN @Order
   

END