CREATE PROCEDURE [dbo].[Znode_GetProfileCatalog]
(   @WhereClause  NVARCHAR(MAX),
    @Rows         INT           = 100,
    @PageNo       INT           = 1,
    @Order_BY     VARCHAR(1000) = '',
    @RowsCount    INT OUT,
    @ProfileId    INT           = 0,
    @IsAssociated BIT           = 0)
AS
  /* Summary :- This procedure is used to find catalog associated to the profile
				if  @IsAssociated =0 then profile are fetched which are not attached to catalog
				else profile are fetched which are attached to catalog
     Unit Testing 
     EXEC Znode_GetProfileCatalog @WhereClause = '',@ProfileId =1, @RowsCount =0, @IsAssociated =1
   */
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX)= '';
             DECLARE @TBL_ProfileCatalog TABLE
             (
              ProfileId        INT,
              PimCatalogId     INT,
              CatalogName      NVARCHAR(MAX),
              ROWID            INT,
              CountId          INT
             );
             IF @IsAssociated = 0
                 BEGIN
                     SET @SQL = '
					 ;With Cte_GetProfile AS 
					 (SELECT  '+CAST(@ProfileId AS VARCHAR(20))+' ProfileId ,ZPC.PimCatalogId , ZPC.CatalogName,
					 '+[dbo].[Fn_GetPagingRowId](@Order_BY, 'ZPC.PimCatalogId')+',Count(*)Over() CountId FROM ZnodePimCatalog ZPC 
					 WHERE NOT EXISTS (SELECT TOP 1 1 FROM  ZnodeProfile  ZP WHERE ZP.ProfileId = '+CAST(@ProfileId AS VARCHAR(20))+' AND ZP.PimCatalogId = ZPC.PimCatalogId)'+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' ) 
					 
					 SELECT  ProfileId ,PimCatalogId , CatalogName,CountId FROM Cte_GetProfile CT
					 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
                 END;
             ELSE
                 BEGIN
                     SET @SQL = ' 
					 ;With Cte_GetProfile AS 
					 (SELECT  ZP.ProfileId ,ZP.PimCatalogId , ZC.CatalogName ,'+[dbo].[Fn_GetPagingRowId](@Order_BY, 'ZP.PimCatalogId')+',Count(*)Over() CountId
					 FROM ZnodeProfile ZP INNER JOIN ZnodePimCatalog ZC ON (ZC.PimCatalogId = ZP.PimCatalogId)
					 WHERE ZP.ProfileId = '+CAST(@ProfileId AS VARCHAR(20))+' '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' ) 
		 
					 SELECT  ProfileId ,PimCatalogId , CatalogName ,CountId FROM  Cte_GetProfile
					 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
                 END;

             INSERT INTO @TBL_ProfileCatalog (ProfileId,PimCatalogId,CatalogName,CountId)
			 EXEC (@SQL);

			 SELECT ProfileId,PimCatalogId,CatalogName FROM @TBL_ProfileCatalog;
			 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_ProfileCatalog), 0);
 
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		SET @Status = 0;
		 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC  @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@ProfileId = '+CAST(@ProfileId AS VARCHAR(50))+',@IsAssociated = '+CAST(@IsAssociated AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetProfileCatalog',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
         END CATCH;
     END;