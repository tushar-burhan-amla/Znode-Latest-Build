-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetZnodeMessage]
(@MessageCode INT
)
RETURNS NVARCHAR(MAX)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @MessageName VARCHAR(500)= '';
         SELECT @MessageName = MessageName FROM ZnodeMessage  WHERE MessageCode = @MessageCode;
         RETURN @MessageName;
     END;