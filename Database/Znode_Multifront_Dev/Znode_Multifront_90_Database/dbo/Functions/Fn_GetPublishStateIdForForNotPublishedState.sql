-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
--DROP FUNCTION  [dbo].[Fn_PublishStateIdForDraftState]

CREATE FUNCTION [dbo].[Fn_GetPublishStateIdForForNotPublishedState]()
RETURNS TinyINT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForDraft TinyINT;
         
		 SET @PublishStateIdForDraft = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'not_published' )

                   
         RETURN @PublishStateIdForDraft;
     END;