
CREATE PROCEDURE [dbo].[Znode_GetAssociatedPortalToTaxClass]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT            = 0)
AS

/*
  Summary: Associating TaxClass to Portal
		   Result is fetched from view View_GetAssociatedPortalToTaxClass order by PortalId in descending order
  Unit Testing:
  EXEC Znode_GetAssociatedPortalToTaxClass ' ', @RowsCount = 0 ,@PageNo = 2, @Rows = 10

*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AssociatedPortalToTaxClass TABLE (PortalId INT,StoreName NVARCHAR(MAX),CreatedDate DATE,ModifiedDate DATE,IsAssociated INT,TaxClassId INT,TaxClassPortalId INT,RowId INT,CountNo INT)
            
		 SET @SQL = '
					; WITH CTE_AssociatedPortalToTaxClass AS
					(
					 SELECT  PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, TaxClassId, PortalId TaxClassPortalId
								,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo
					 FROM View_GetAssociatedPortalToTaxClass
					 WHERE 1=1 
							     '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
					)

					SELECT PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, TaxClassId, TaxClassPortalId,RowId,CountNo
					FROM CTE_AssociatedPortalToTaxClass
								 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

					INSERT INTO @TBL_AssociatedPortalToTaxClass (PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, TaxClassId, TaxClassPortalId,RowId,CountNo)
					EXEC(@SQL)

					SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AssociatedPortalToTaxClass),0)

					SELECT  PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, TaxClassId, TaxClassPortalId
					FROM @TBL_AssociatedPortalToTaxClass
            
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAssociatedPortalToTaxClass @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status,@ErrorMessage;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAssociatedPortalToTaxClass',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                     
         END CATCH;
     END;