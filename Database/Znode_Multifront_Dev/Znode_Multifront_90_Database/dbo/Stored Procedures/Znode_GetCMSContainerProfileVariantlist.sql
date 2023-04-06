CREATE PROCEDURE [dbo].[Znode_GetCMSContainerProfileVariantlist]  
(  
 @WhereClause NVARCHAR(MAX)  
 ,@ContainerKey NVARCHAR(MAX)  
 ,@Rows INT = 100     
 ,@PageNo INT = 1     
 ,@Order_BY VARCHAR(1000) = ''  
 ,@RowsCount INT OUT  
)  
AS  
-- EXEC Znode_GetCMSContainerProfileVariantlist '','',0,0,'',0  
BEGIN    
BEGIN TRY   
SET NOCOUNT ON  
 DECLARE @SQL NVARCHAR(MAX)  
 IF OBJECT_ID('tempdb..#TBL_ContentContainer') IS NOT NULL  
  DROP TABLE #TBL_ContentContainer;  
  
 IF OBJECT_ID('tempdb..#Cte_ContentContainerVarient') IS NOT NULL  
  DROP TABLE #Cte_ContentContainerVarient;  
  
 IF OBJECT_ID('tempdb..#Users') IS NOT NULL  
  DROP TABLE #Users;  
  
 CREATE TABLE #TBL_ContentContainer (CMSContainerProfileVariantId INT, ContainerKey NVARCHAR(MAX), PortalId INT, StoreName NVARCHAR(MAX), ProfileName NVARCHAR(200),CreatedByName VARCHAR(500), CreatedDate DATETIME, ModifiedByName VARCHAR(500),  
  ModifiedDate DATETIME, IsDefaultVarient BIT,StoreCode VARCHAR(200),ProfileCode Varchar(200),RowId INT,CountNo INT,IsActive BIT,PublishStatus VARCHAR(32))    
   
 SELECT CAST(UserName AS NVARCHAR(500)) AS UserName , ZU.UserId  
 INTO #Users  
 FROM ZnodeUser ZU WITH (NOLOCK)  
 WHERE EXISTS( SELECT * FROM ZnodeCMSContainerProfileVariant CWPV WHERE ZU.UserId = CWPV.CreatedBy )  
 UNION  
 SELECT UserName, ZU.UserId  
 FROM ZnodeUser ZU  
 WHERE EXISTS( SELECT * FROM ZnodeCMSContainerProfileVariant CWPV WHERE ZU.UserId = CWPV.ModifiedBy ) 
 
 SELECT DISTINCT WPV.CMSContainerProfileVariantId, ZCW.ContainerKey, WPV.PortalId, --ZCW.Tags,  
  CASE WHEN WPV.PortalId IS NULL THEN  'Any Store' ELSE ZP.StoreName END StoreName  ,  
  CASE WHEN WPV.ProfileId IS NULL THEN  'Any User Profile' ELSE ZPr.ProfileName END ProfileName,   
  U.UserName AS CreatedByName, WPV.CreatedDate, ISNULL(U1.UserName,U.UserName) AS ModifiedByName, WPV.ModifiedDate,  
  CAST(CASE WHEN WPV.ProfileId IS NULL AND WPV.PortalId IS NULL THEN 1 ELSE 0 END AS BIT) AS IsDefaultVarient,  
  CASE WHEN WPV.PortalId IS NULL THEN  'AnyStore' ELSE ZP.StoreCode END StoreCode ,  
  CASE WHEN WPV.ProfileId IS NULL THEN  'AnyUserProfile' ELSE ZPr.DefaultExternalAccountNo END ProfileCode,  
  CPVL.IsActive As Status,ZPS.DisplayName As PublishStatus  
 INTO #Cte_ContentContainerVarient  
 FROM ZnodeCMSContentContainer ZCW   
 INNER JOIN ZnodeCMSContainerProfileVariant WPV ON WPV.CMSContentContainerId = ZCW.CMSContentContainerId  
 LEFT JOIN ZnodeCMSContainerProfileVariantLocale CPVL ON WPV.CMSContainerProfileVariantId=CPVL.CMSContainerProfileVariantId  
 LEFT JOIN ZnodePublishState ZPS ON WPV.PublishStateId=ZPS.PublishStateId  
 LEFT JOIN ZnodePortal ZP ON (WPV.PortalId = ZP.PortalId )   
 LEFT JOIN ZnodeProfile ZPr ON WPV.ProfileId = ZPr.ProfileId  
 LEFT JOIN #Users U ON WPV.CreatedBy = U.UserId  
 LEFT JOIN #Users U1 ON WPV.ModifiedBy = U1.UserId  
 WHERE ZCW.ContainerKey = @ContainerKey
SET @SQL = '     
 ;With Cte_ContentContainerVarientList AS  
 (  
  SELECT CMSContainerProfileVariantId, ContainerKey, PortalId, StoreName, ProfileName,CreatedByName, CreatedDate, ModifiedByName, ModifiedDate, IsDefaultVarient,StoreCode,ProfileCode,  
  '+dbo.Fn_GetPagingRowId(@Order_BY,'IsDefaultVarient ASC,CMSContainerProfileVariantId DESC')+',Count(*)Over() CountNo,Status,PublishStatus  
  FROM #Cte_ContentContainerVarient  
  WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'  
 ) 
INSERT INTO #TBL_ContentContainer   
 SELECT *   
 FROM Cte_ContentContainerVarientList  
  
 SELECT CMSContainerProfileVariantId as ContainerProfileVariantId, ContainerKey, PortalId, StoreName, ProfileName,CreatedByName, CreatedDate, ModifiedByName, ModifiedDate, IsDefaultVarient ,StoreCode,ProfileCode,IsActive,PublishStatus  
 FROM #TBL_ContentContainer     
 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) +'  
  ORDER BY RowId'  
  
 EXEC (@SQL)  
 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM #TBL_ContentContainer ),0)    
  
END TRY  
BEGIN CATCH  
 DECLARE @Status BIT ;  
 SET @Status = 0;  
 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)=   
 ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)=   
 'EXEC Znode_GetCMSContainerProfileVariantlist @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',  
 @Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',  
 @RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
  
 SELECT 0 AS ID,CAST(0 AS BIT) AS Status;     
  
 EXEC Znode_InsertProcedureErrorLog  
 @ProcedureName = 'Znode_GetCMSContainerProfileVariantlist',  
 @ErrorInProcedure = @Error_procedure,  
 @ErrorMessage = @ErrorMessage,  
 @ErrorLine = @ErrorLine,  
 @ErrorCall = @ErrorCall;  
END CATCH;   
END;