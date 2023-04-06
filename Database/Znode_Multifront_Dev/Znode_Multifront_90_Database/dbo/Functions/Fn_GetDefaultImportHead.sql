-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetDefaultImportHead](
               @ImportHeadId int)
RETURNS NVARCHAR(MAX)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultValue NVARCHAR(MAX)= '';
                 SELECT @DefaultValue = NAME from ZnodeImportHead WHERE ImportHeadId =  @ImportHeadId 
         RETURN @DefaultValue;
     END;