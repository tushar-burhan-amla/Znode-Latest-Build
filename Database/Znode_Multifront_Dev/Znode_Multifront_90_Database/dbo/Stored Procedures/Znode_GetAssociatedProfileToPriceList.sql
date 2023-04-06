CREATE PROCEDURE [dbo].[Znode_GetAssociatedProfileToPriceList]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT            = 0)
AS
/*
  Summary : Associating PriceList to Profile
            Result is fetched from view View_GetAssociatedProfileToPriceList order by ProfileId in descending order
  Unit Testing
  EXEC [Znode_GetAssociatedProfileToPriceList] 'PriceListId = 1 AND  IsAssociated = 0 ', @RowsCount = 0 
 
*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AssociatedProfileToPriceList TABLE (PortalId int , StoreName Nvarchar(max), ProfileId INT, ProfileName NVARCHAR(200),CreatedDate DATE,ModifiedDate DATE,IsAssociated INT,PriceListId INT,PriceListProfileId INT,Precedence INT,RowId INT,CountNo INT)
          
			SET @SQL = '
						; WITH CTE_AssociatedProfileToPriceList AS
						(
						 SELECT  PortalId, StoreName,ProfileId, ProfileName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListProfileId,Precedence
												,'+dbo.Fn_GetPagingRowId(@Order_BY,'ProfileId DESC')+',Count(*)Over() CountNo
						 FROM View_GetAssociatedProfileToPriceList
						 WHERE 1=1
												'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						)

						SELECT PortalId, StoreName,ProfileId, ProfileName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListProfileId,Precedence,RowId,CountNo
						FROM CTE_AssociatedProfileToPriceList
												'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

			INSERT INTO @TBL_AssociatedProfileToPriceList (PortalId, StoreName,ProfileId, ProfileName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListProfileId,Precedence,RowId,CountNo)
			EXEC(@SQL)

			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AssociatedProfileToPriceList),0)

			SELECT  PortalId, StoreName,ProfileId, ProfileName , CreatedDate , ModifiedDate,IsAssociated,PriceListId,PriceListProfileId,Precedence
			FROM @TBL_AssociatedProfileToPriceList
           
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAssociatedProfileToPriceList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAssociatedProfileToPriceList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                     
         END CATCH;
     END;