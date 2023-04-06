-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
-- DROP FUNCTION  [dbo].[Fn_PublishStateIdForDraftState]

CREATE FUNCTION [dbo].[Fn_GetPublishStateIdForPublishFailed]()
RETURNS TinyINT
AS
BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForPublishFailed TinyINT;
         
		 SET @PublishStateIdForPublishFailed = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'PUBLISH_FAILED' )

                   
         RETURN @PublishStateIdForPublishFailed;
END;