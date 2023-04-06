


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
-- DROP FUNCTION  [dbo].[Fn_PublishStateIdForDraftState]

create FUNCTION [dbo].[Fn_GetPublishStateIdForPublish]()
RETURNS TinyINT
AS
BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForPublish TinyINT;
         
		 SET @PublishStateIdForPublish = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'PRODUCTION' )

                   
         RETURN @PublishStateIdForPublish;
END;