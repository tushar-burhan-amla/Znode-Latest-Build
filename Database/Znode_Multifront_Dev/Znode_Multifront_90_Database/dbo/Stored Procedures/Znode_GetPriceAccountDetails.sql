--Alter View View_GetPriceListAccounts As
--Select a.PriceListId, a.PriceListAccountId, a.AccountId, b.FullName from  ZnodePriceListAccount a inner join [dbo].[View_AccountRoles] b 
--on a.AccountId = b.Accountid 

   
     -- exec Znode_GetPriceAccountDetails @WhereClause='',@RowsCount=null,@Rows = 10,@PageNo=1,@Order_BY = Null
CREATE     Procedure [dbo].[Znode_GetPriceAccountDetails]  
  
(  
 @WhereClause Varchar(1000)     
,@Rows INT = 1000     
,@PageNo INT = 0     
,@Order_BY VARCHAR(100) =  NULL  
,@RowsCount int out  
)  
AS  
BEGIN    
 SET NOCOUNT ON   
 BEGIN TRY   

   DECLARE @V_SQL NVARCHAR(MAX)  
     
   SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END   
              
   SET @V_SQL = ' SELECT PriceListId, PriceListAccountId,AccountId,FullName ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsAssociated INTO #TEMP_Account11 FROM '+ ' View_GetPriceListAccounts ' + 'WHERE 1=1 '
   --+CAST (@LocaleId AS VARCHAR(300))  
        + case WHEN @WhereClause IS NOT NULL and @WhereClause <> '' and @WhereClause <> 'PriceListAccountId  = -1'   THEN  ' AND '+REPLACE(@WhereClause,'PriceListAccountId  = -1 AND','') ELSE '' END  
        +' SELECT  @tempo=Count(1) FROM  #TEMP_Account11  SELECT * FROM #TEMP_Account11 '   
        +' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')+ ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '  
  
  -- print @V_SQL  
   EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowsCount out  
   
   
 END TRY   
  
 BEGIN CATCH   
     
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
  
 END CATCH   
  
END