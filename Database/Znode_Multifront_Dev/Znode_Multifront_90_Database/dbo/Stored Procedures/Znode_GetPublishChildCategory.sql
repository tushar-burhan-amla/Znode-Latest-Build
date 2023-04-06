CREATE PROC Znode_GetPublishChildCategory 
(
 @VersionId INT = 0
,@ZnodeCategoryId int  = 0 
) 
AS 
BEGIN 
BEGIN TRY
SET NOCOUNT ON 
;with cte as 
(
SELECT a.ZnodeCategoryId, CAST(TRANSLATE(a.ZnodeParentCategoryIds ,'[]','  ') AS INT ) ZnodeParentCategoryIds
FROM ZnodePublishCategoryEntity a with(nolock) 
WHERE a.VersionId = @VersionId AND ZnodeCategoryId = @ZnodeCategoryId 
UNION ALL 
SELECT a.ZnodeCategoryId, CAST(TRANSLATE(a.ZnodeParentCategoryIds ,'[]','  ') AS INT )
FROM ZnodePublishCategoryEntity a with(nolock) 
INNER JOIN cte v ON (CAST(TRANSLATE(a.ZnodeParentCategoryIds ,'[]','  ') AS INT )  =  v.ZnodeCategoryId)
WHERE a.VersionId = @VersionId 


) 

SELECT cast(1 as int) as Id , STRING_AGG( ZnodeCategoryId,',') as MessageDetails, cast(1 as bit) as Status 
FROM cte 
 
END TRY 

BEGIN CATCH  
 SELECT cast(0 as int) as Id , ERROR_MESSAGE() as MessageDetails, cast(0 as bit) as Status
END CATCH 
END 
