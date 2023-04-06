
CREATE PROCEDURE [dbo].[Znode_GetAssociatedPortalToShipping]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT            = 0)
AS
/*
 Summary : Associating Shipping to Portal
		   Result is fetched from view View_GetAssociatedPortalToShipping order by PortalId in descending order
 Unit Testing:
 begin tran
 EXEC Znode_GetAssociatedPortalToShipping ' shippingid = 7 and IsAssociated  = 1 ', @RowsCount = 0 
 rollback tran
*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AssociatedPortalToShipping TABLE (PortalId INT,StoreName NVARCHAR(MAX),CreatedDate DATE, ModifiedDate DATE,IsAssociated INT, ShippingId INT,ShippingPortalId INT,RowId INT,CountNo INT  )
           
			SET @SQL = '
						; WITH CTE_AssociatedPortalToShipping AS
						(
						SELECT  PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, ShippingId, ShippingPortalId
								,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo
						FROM View_GetAssociatedPortalToShipping
						WHERE 1=1 
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						
						)

						SELECT  PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, ShippingId, ShippingPortalId,RowId,CountNo
						FROM CTE_AssociatedPortalToShipping
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

						INSERT INTO @TBL_AssociatedPortalToShipping (PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, ShippingId, ShippingPortalId,RowId,CountNo)
						EXEC(@SQL)

						SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AssociatedPortalToShipping),0)
   
					    SELECT PortalId, StoreName, CreatedDate, ModifiedDate, IsAssociated, ShippingId, ShippingPortalId
					    FROM @TBL_AssociatedPortalToShipping 
            
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAssociatedPortalToShipping @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAssociatedPortalToShipping',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                     
         END CATCH;
     END;