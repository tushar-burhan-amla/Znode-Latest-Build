-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetFilterWhereClause]
(
  @WhereClause NVARCHAR(MAX)
 )
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Where  NVARCHAR(max)=''

	SET @Where = CASE WHEN @WhereClause = '' OR @WhereClause IS NULL  THEN '' ELSE ' AND '+' '+@WhereClause END 

    RETURN @Where
   

END