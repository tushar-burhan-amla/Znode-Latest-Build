-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetXmlWhereClauseForAttribute]
(@WhereClauseChanges NVARCHAR(MAX),
 @WherClause    NVARCHAR(max) ,  
 @LocaleId           INT
)
RETURNS NVARCHAR(MAX)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @Where NVARCHAR(MAX)= '';
         SET @Where = CASE
                          WHEN @WhereClauseChanges = ''
                               OR @WhereClauseChanges IS NULL
                          THEN '<ArrayOfWhereClauseModel> <WhereClauseModel>'+dbo.Fn_GetDefaultFilter(@Localeid)+'</WhereClauseModel> </ArrayOfWhereClauseModel> '
                          ELSE REPLACE(@WhereClauseChanges, '</WhereClauseModel> </ArrayOfWhereClauseModel>', '</WhereClauseModel> <WhereClauseModel>'+dbo.Fn_GetDefaultFilter(@Localeid)+'</WhereClauseModel> </ArrayOfWhereClauseModel> ')
                      END;

		 SET @Where = CASE WHEN @WherClause = '' THEN @Where ELSE REPLACE(@Where, '</ArrayOfWhereClauseModel>', @wherClause+'</ArrayOfWhereClauseModel>') END 
         RETURN @Where;
     END;