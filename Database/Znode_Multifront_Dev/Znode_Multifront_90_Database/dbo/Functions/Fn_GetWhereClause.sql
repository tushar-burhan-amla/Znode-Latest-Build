-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetWhereClause]
(
  @WhereClause NVARCHAR(MAX)
  ,@LogicalOprator NVARCHAR(500)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Where  NVARCHAR(max)=''

	SET @Where = CASE WHEN @WhereClause = '' OR @WhereClause IS NULL  THEN '' ELSE @LogicalOprator+' '+@WhereClause END 

    RETURN @Where
   

END