-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetIsActiveTrue]()
RETURNS BIT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @IsActive BIT;
         
		 SET @IsActive = 1

                   
         RETURN @IsActive;
     END;