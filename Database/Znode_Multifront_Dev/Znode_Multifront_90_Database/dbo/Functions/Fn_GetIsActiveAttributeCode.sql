-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetIsActiveAttributeCode]()
RETURNS VARCHAR(600)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @IsActiveAttributeCode VARCHAR(600);
         
		 SET @IsActiveAttributeCode = 'IsActive'

		 -- SELECT * FROM ZnodePIMAttribute

                   
         RETURN @IsActiveAttributeCode;
     END;