-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetVoidRmaRequestStatusId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @RmaRequestStatusId INT;
         
		 SET @RmaRequestStatusId = ( SELECT TOP 1 RmaRequestStatusId FROM ZnodeRmaRequestStatus WHERE Name= 'Void'  )

                   
         RETURN @RmaRequestStatusId;
     END;