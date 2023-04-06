CREATE PROCEDURE [dbo].[Znode_GetPortalSearchProfile]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/* 
   SUMMARY : Stored Procedure to Get list of PortalsearchProfileid 
   Unit Testing:

   -- EXEC Znode_GetPortalSearchProfile @WhereClause = 'searchprofileid = 2',@RowsCount = 0,@UserId=2
*/
BEGIN
    BEGIN TRY

	SET NOCOUNT ON 

	DECLARE @SQL  NVARCHAR(max) 
		
	DECLARE @TBL_PortalSearchProfile TABLE 
		(PublishCatalogId INT,SearchProfileId INT,PortalName nvarchar(400),PortalId INT,ProfileName NVARCHAR(400), CatalogName nvarchar(400), RowId INT, CountNo INT)

	SET @SQL = '
		;With Cte_GetPortalSearchProfileList 
		AS  (
			SELECT DISTINCT	ZSCP.PublishCatalogId,ZSP.SearchProfileId,ZP.StoreName as PortalName,
				ZP.PortalId,ZSP.ProfileName,ZPC.CatalogName
			FROM ZnodeSearchProfile ZSP
			INNER JOIN ZnodePublishCatalogSearchProfile ZSCP ON (ZSP.SearchProfileId = ZSCP.SearchProfileId)
			INNER JOIN ZnodePortalCatalog ZPSP ON (ZSCP.PublishCatalogId = ZPSP.PublishCatalogId)
			INNER JOIN ZnodePortal ZP ON (ZP.PortalId = ZPSP.PortalId)
			INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZSCP.PublishCatalogId)
			)
			,Cte_GetFilterPortalSearchProfile
			AS (
			SELECT PublishCatalogId,SearchProfileId,PortalName,PortalId,ProfileName,CatalogName,
			'+dbo.Fn_GetPagingRowId(@Order_BY,'PublishCatalogId DESC')+',Count(*)Over() CountNo 
			FROM  Cte_GetPortalSearchProfileList CGPTL 
			WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'	
			)																			
			SELECT PublishCatalogId,SearchProfileId,PortalName,PortalId,ProfileName,CatalogName,RowId,CountNo
			FROM Cte_GetFilterPortalSearchProfile
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
		
			INSERT INTO @TBL_PortalSearchProfile(PublishCatalogId,SearchProfileId,PortalName,PortalId,ProfileName,CatalogName,RowId,CountNo)
			EXEC(@SQL)

			SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_PortalSearchProfile ),0)
			
			SELECT PublishCatalogId,SearchProfileId,PortalName,PortalId,ProfileName,CatalogName
			FROM @TBL_PortalSearchProfile
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalSearchProfile @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPortalSearchProfile',
			@ErrorInProcedure = 'Znode_GetPortalSearchProfile',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END