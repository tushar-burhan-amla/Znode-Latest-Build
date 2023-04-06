-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[FN_GetMediaPathHierarchy]
(	
-- Add the parameters for the function here
@MediaPathId INT
)
RETURNS TABLE
AS
     RETURN(
     WITH ASDA
          AS (
          -- Add the SELECT statement with parameter references here
          SELECT MediaPathId,
                 ParentMediaPathId
          FROM ZnodeMediaPath
          WHERE mediapathid = @MediaPathId
          UNION ALL
          SELECT a.MediaPathId,
                 a.ParentMediaPathId
          FROM ZnodeMediaPath a
               INNER JOIN ASDA b ON(a.ParentMediaPathId = b.MediaPathId))
          SELECT *
          FROM ASDA);