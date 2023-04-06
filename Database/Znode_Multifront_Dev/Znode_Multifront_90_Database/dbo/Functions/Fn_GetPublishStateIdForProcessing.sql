-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
-- DROP FUNCTION  [dbo].[Fn_PublishStateIdForDraftState]

CREATE FUNCTION [dbo].[Fn_GetPublishStateIdForProcessing]()
RETURNS TinyINT
AS
BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForProcessing TinyINT=0;
         
		 SET @PublishStateIdForProcessing = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'PROCESSING' )

                   
         RETURN @PublishStateIdForProcessing;
END;