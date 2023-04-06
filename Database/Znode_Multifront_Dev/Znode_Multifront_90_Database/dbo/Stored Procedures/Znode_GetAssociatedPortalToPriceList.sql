
CREATE PROCEDURE [dbo].[Znode_GetAssociatedPortalToPriceList]
( @WhereClause NVarchar(Max) = '',
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY VARCHAR(1000)	 =  '',
  @RowsCount   INT OUT,
  @LocaleId    INT           = 0)
AS

/*
 Summary: Associating PriceList with Portal 
		  The result is displayed from view View_GetAssociatedPortalToPriceList order by PortalId in descending order
 Unit Testing:
 EXEC Znode_GetAssociatedPortalToPriceList ' pricelistid = 7 and IsAssociated  = 1 ', @RowsCount = 0 

*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AssociatedPortalToPriceList TABLE (PortalId INT,StoreName NVARCHAR(MAX),CreatedDate DATE,ModifiedDate DATE,IsAssociated INT,PriceListId INT,PriceListPortalId INT,Precedence INT,RowId INT,CountNo INT,CatalogName NVARCHAR(MAX))
             

		     SET @SQL = '
					; WITH CTE_AssociatedPortalToPriceList AS
					(
					SELECT  PortalId,StoreName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListPortalId,Precedence
								,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo,CatalogName
					FROM  View_GetAssociatedPortalToPriceList 
					WHERE 1=1
					'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				
					)

					SELECT PortalId,StoreName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListPortalId,Precedence,RowId,CountNo,CatalogName
					FROM CTE_AssociatedPortalToPriceList
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

			 INSERT INTO @TBL_AssociatedPortalToPriceList (PortalId,StoreName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListPortalId,Precedence,RowId,CountNo,CatalogName)
			 EXEC (@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AssociatedPortalToPriceList),0)

			 SELECT PortalId,StoreName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListPortalId,Precedence,CatalogName
             FROM @TBL_AssociatedPortalToPriceList 
          
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAssociatedPortalToPriceList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAssociatedPortalToPriceList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;