CREATE PROCEDURE [dbo].[Znode_GetPriceUserDetails]
(   @WhereClause VARCHAR(MAX),
    @AccountList INT           = 0,
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(1000) = '',
    @RowsCount   INT OUT)
AS
/*
 Summary: This Procedure is used to get Price details of user
         using conditions it checks whether user is having Account or not and result is fetched accordingly.
 Unit Testing:
 DECLARE @WE INT = 0 
 EXEC Znode_GetPriceUserDetails @WhereClause = ' IsAssociated = 1 and pricelistid = 16',@Order_BY =' PriceListId ASC' ,  @RowsCount =  @WE OUT ,@Rows = 10, @PageNo =1 

*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_PriceListAccount TABLE
             (PortalId int Null ,StoreName nvarchar(Max) Null,PriceListUserId INT,PriceListId INT,UserId INT,FullName VARCHAR(201),CreatedBy INT,CreatedDate DATETIME,
              ModifiedBy INT,ModifiedDate DATETIME,IsAssociated INT,AspNetUserId NVARCHAR(256),Precedence INT,AccountName NVARCHAR(400),EmailId VARCHAR(50),
			  UserName NVARCHAR(512),RowId INT,CountNo INT);

             IF @AccountList = 1 AND @WhereClause <> ''
                 BEGIN
                     SET @WhereClause = @WhereClause+' AND ';
                 END;
             ELSE
                 BEGIN
                     IF @AccountList = 1 AND @WhereClause = ''
                         BEGIN
                             SET @WhereClause = @WhereClause;
                         END;
                     ELSE
                         BEGIN
                             IF @AccountList = 0 AND @WhereClause = ''
                                 BEGIN
                                     SET @WhereClause = @WhereClause+' NOT ';
                                 END;
                             ELSE
                                 BEGIN
                                     IF @AccountList = 0 AND @WhereClause <> ''
                                         BEGIN
                                             SET @WhereClause = @WhereClause+' AND NOT ';
                                         END;
                                 END;
                         END;
                 END;
             SET @WhereClause = @WhereClause+'EXISTS (SELECT TOP 1 1 FROM AspNetUserRoles AUR INNER JOIN AspNetRoles AR ON (AUR.RoleId = AR.Id) WHERE AUR.UserId = VPLU.AspNetUserId AND (AR.Name IN (''Admin'') ))';
             SET @SQL = '
			 
			 ;With Cte_PriceListUser AS 
			 (
				SELECT  ZP.PortalId, Zp.StoreName, VPLU.PriceListUserId, VPLU.PriceListId,VPLU.UserId,VPLU.FullName ,VPLU.CreatedBy,VPLU.CreatedDate,VPLU.ModifiedBy,VPLU.ModifiedDate,VPLU.IsAssociated,VPLU.AspNetUserId
				,VPLU.Precedence,VPLU.AccountName,VPLU.EmailId,VPLU.UserName 
				FROM  View_GetPriceListUsers VPLU Left Outer Join ZnodeUserPortal Zup On VPLU.UserId = Zup.UserId  Left Outer Join  ZnodePortal ZP on ZP.PortalId  = Zup.PortalId
			 )
			 , Cte_PriceListUserFiltered AS 
			 (
				Select PortalId, StoreName, PriceListUserId, PriceListId,UserId,FullName ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsAssociated,AspNetUserId
				,Precedence,AccountName,EmailId,UserName ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PriceListId DESC,UserId DESC')+',Count(*)Over() CountNo
				FROM  Cte_PriceListUser VPLU 
				WHERE 1=1 
				  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			) 
 			
			  SELECT Distinct PortalId, StoreName ,PriceListUserId, PriceListId,UserId,FullName ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsAssociated,AspNetUserId
			 ,Precedence,AccountName,EmailId,UserName,RowId,CountNo FROM Cte_PriceListUserFiltered 
			 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
          
           
             INSERT INTO @TBL_PriceListAccount
             EXEC(@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_PriceListAccount ),0)

             SELECT PortalId, StoreName , PriceListId,PriceListUserId,UserId,FullName,IsAssociated,AspNetUserId,Precedence,AccountName,EmailId,UserName FROM @TBL_PriceListAccount;
  
		 END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPriceUserDetails @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+
			 ',@AccountList='+CAST(@AccountList AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+
			 ',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPriceUserDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;