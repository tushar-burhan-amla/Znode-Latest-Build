
CREATE FUNCTION [dbo].[Fn_GetPublishStateIdForDraftState]()
RETURNS TinyINT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForDraft TinyINT;
         
		 SET @PublishStateIdForDraft = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'draft' )

                   
         RETURN @PublishStateIdForDraft;
     END;