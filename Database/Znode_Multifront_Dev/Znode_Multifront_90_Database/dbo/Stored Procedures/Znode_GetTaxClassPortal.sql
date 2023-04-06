CREATE PROCEDURE [dbo].[Znode_GetTaxClassPortal](
      @WhereClause  NVARCHAR(MAX) = '' ,
      @Rows         INT           = 100 ,
      @PageNo       INT           = 1 ,
      @Order_BY     VARCHAR(1000) = '' ,
      @RowsCount    INT OUT ,
      @PortalId     INT           = 0 ,
      @IsAssociated INT           = 0)
AS

/*
  Summary: This Procedure is used to get TaxClass details According to associated portal.
		   
  Unit Testing: 
	EXEC [Znode_GetTaxClassPortal]  @PortalId = 0,@IsAssociated= 1, @RowsCount = 0
  
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_TaxClassTable TABLE (PortalId INT ,StoreName NVARCHAR(MAX) ,TaxClassId INT,Name NVARCHAR(200),IsActive BIT,DisplayOrder INT ,
             IsDefault BIT,ExternalId VARCHAR(50),CreatedDate  DATETIME ,ModifiedDate DATETIME ,RowId INT,CountNo INT);
             IF ( ISNULL(@PortalId , 0) = 0 )
                 BEGIN
                     SET @SQL = ' ;With Cte_GetTaxClassDetails AS 
				(
				 SELECT NULL PortalId,NULL StoreName,ZTC.TaxClassId,ZTC.Name,IsActive,DisplayOrder,0 IsDefault,ExternalId,ZTC.CreatedDate,ZTC.ModifiedDate  
						,'+dbo.Fn_GetPagingRowId ( @Order_BY , ' ZTC.TaxClassId '
                                                       ) +',Count(*)Over() CountNo
				 FROM ZnodeTaxClass ZTC 
				 LEFT JOIN ZnodeTaxClassSKU ZTCS ON (ZTCS.TaxClassId = ZTC.TaxClassId)
				 WHERE  1=1
				 '+dbo.Fn_GetFilterWhereClause ( @WhereClause
                                                   ) +'
				 GROUP BY ZTC.TaxClassId,ZTC.Name,IsActive,DisplayOrder,ExternalId,ZTC.CreatedDate,ZTC.ModifiedDate 
				 ) ';
                 END;
             ELSE
                 BEGIN
                     SET @SQL = '
		           ;With Cte_GetAssociatedTaxClass  AS 
				   (
					SELECT ZP.PortalId,ZP.StoreName,ZTC.TaxClassId,ZTC.Name,IsActive,ZTC.DisplayOrder DisplayOrder ,ISNULL(ZPTC.IsDefault,0) IsDefault,ZTC.ExternalId,ZTC.CreatedDate,ZTC.ModifiedDate, 
							 CASE WHEN ZPTC.TaxClassId IS NULL THEN 0 ELSE 1 END IsAssociated	
							
					FROM ZnodePortal ZP 
					CROSS APPLY ZnodeTaxClass ZTC 
					LEFT JOIN ZnodePortalTaxClass ZPTC ON(ZP.PortalId = ZPTC.PortalId AND ZTC.TaxClassId = ZPTC.TaxClassId)
					)
					, Cte_GetTaxClassDetails AS
					(
					   SELECT PortalId,StoreName,TaxClassId,Name,IsActive,DisplayOrder,IsDefault,ExternalId,CreatedDate,ModifiedDate	
							,'+dbo.Fn_GetPagingRowId ( @Order_BY , 'PortalId ,TaxClassId '
                                                            ) +',Count(*)Over() CountNo
					   FROM Cte_GetAssociatedTaxClass
					   WHERE PortalId = '+CAST(@PortalId AS VARCHAR(50))+'
					   AND IsAssociated = '+CAST(@IsAssociated AS VARCHAR(50))+'
					  '+dbo.Fn_GetFilterWhereClause ( @WhereClause
                                                         ) +'
					) ';
                 END;
             SET @SQL = @SQL+'	SELECT PortalId,StoreName,TaxClassId,Name,IsActive,DisplayOrder,IsDefault,ExternalId,CreatedDate,ModifiedDate,RowId,CountNo
			      FROM Cte_GetTaxClassDetails
				'+dbo.Fn_GetPaginationWhereClause ( @PageNo , @Rows);
                                                      
			
             INSERT INTO @TBL_TaxClassTable ( PortalId , StoreName , TaxClassId , Name , IsActive , DisplayOrder , IsDefault , ExternalId , CreatedDate , ModifiedDate , RowId , CountNo
                                            )
             EXEC (@SQL);
             SET @RowsCount = ISNULL( ( SELECT TOP 1 CountNo FROM @TBL_TaxClassTable) , 0);
             SELECT PortalId , StoreName , TaxClassId , Name , IsActive , DisplayOrder , IsDefault , ExternalId , CreatedDate , ModifiedDate
             FROM @TBL_TaxClassTable;
			
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTaxClassPortal @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@PortalId = '+CAST(@PortalId AS VARCHAR(50))+',@IsAssociated = '+CAST(@IsAssociated AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTaxClassPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;