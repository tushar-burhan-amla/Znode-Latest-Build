CREATE FUNCTION [dbo].[Fn_GetRecurciveContentPageGroup]
(@CMSContentPageGroupId VARCHAR(2000)
)
RETURNS @ConvertTableData TABLE
(CMSContentPageGroupId       INT,
 ParentCMSContentPageGroupId INT
)
AS
     BEGIN
         WITH Cte_RecursiveAccountId
              AS (
              SELECT CMSContentPageGroupId,
                     ParentCMSContentPageGroupId
              FROM ZnodeCMSContentPageGroup
              WHERE EXISTS ( SELECT TOP 1 1 FROM dbo.split(@CMSContentPageGroupId,',') SP WHERE  CMSContentPageGroupId = CAST(sp.Item AS INT ))
              UNION ALL
              SELECT ZA.CMSContentPageGroupId,
                     ZA.ParentCMSContentPageGroupId
              FROM ZnodeCMSContentPageGroup ZA
                   INNER JOIN Cte_RecursiveAccountId CTRA ON(ZA.ParentCMSContentPageGroupId = CTRA.CMSContentPageGroupId))
              INSERT INTO @ConvertTableData
                     SELECT *
                     FROM Cte_RecursiveAccountId;
         RETURN;
     END;