CREATE   PROCEDURE [dbo].[Znode_UpdateCategoryProductDisplayOrder]       
(      
 @JSONString NVARCHAR(max)= '[]'      
,@PimCategoryHierarchyId INT   
,@PageNo INT = 0 
,@ProductIds VARCHAR(max) ='' 
,@Status BIT = 0 out    
)      
      
AS      
BEGIN       
 BEGIN TRY       
  SET NOCOUNT ON       
    DECLARE @TBL_PimProductId TABLE (PimProductId INT, RowId INT IDENTITY(1,1)  )      
          
      
      
    INSERT INTO @TBL_PimProductId (PimProductId)      
    EXEC [dbo].[Znode_GetJSONTableData] @JSONString , 'Id'      
      
    SET  @PageNo = ISNULL(@PageNo,0)  
      
 UPDATE ZPCC      
 SET ZPCC.DisplayOrder =  CASE WHEN @PageNo = 0  THEN RowId ELSE     
   CASE WHEN LEN(RowId) > 1 THEN CAST(@PageNo-1 +LEFT(RowId , LEN(RowId)-1) AS VARCHAr(1000)) +RIGHT(CAST(RowId AS VARCHAr(1000)),1)   ELSE CAST(@PageNo-1 AS VARCHAr(1000)) + RIGHT(CAST(RowId AS VARCHAr(1000)),1) END  END     
 FROM ZnodePimCategoryProduct ZPCC 
 INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId      
 INNER JOIN @TBL_PimProductId TBLP ON (TBLP.PimProductId = ZPCC.PimProductId)      
 WHERE ZPCH.PimCategoryHierarchyId = @PimCategoryHierarchyId    

  SELECT 1 AS ID , CAST(1 AS BIT) AS [Status];    
 SET @Status = 1       
        
         
 END TRY       
 BEGIN CATCH       
  SELECT ERROR_MESSAGE()      
   SELECT 0 AS ID , CAST(0 AS BIT) AS [Status];    
  SET @Status = 0      
 END CATCH       
END