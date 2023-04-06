-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
-- DROP FUNCTION  [dbo].[Fn_PublishStateIdForDraftState]

CREATE FUNCTION [dbo].[Fn_GetPublishStateIdForPreview]()
RETURNS TinyINT
AS
BEGIN
         -- Declare the return variable here
         DECLARE @PublishStateIdForPreview TinyINT=0;
         
		 SET @PublishStateIdForPreview = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'Preview' )

                   
         RETURN @PublishStateIdForPreview;
END;