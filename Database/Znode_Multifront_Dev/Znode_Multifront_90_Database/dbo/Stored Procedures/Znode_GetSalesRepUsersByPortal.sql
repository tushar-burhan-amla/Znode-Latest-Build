
Create PROCEDURE [dbo].[Znode_GetSalesRepUsersByPortal]  
(   
 @RoleName  VARCHAR(200)='',  
    @WhereClause VARCHAR(MAX)  = '',  
    @Rows   INT           = 100,  
    @PageNo   INT           = 1,  
    @Order_By  VARCHAR(1000) = '',  
    @RowCount  INT        = 0 OUT,  
 @PortalId int = 0  
)  
AS  
--[Znode_GetSalesRepUsers] @PortalId = 0  
begin  
  set nocount on  
  
  declare @SQL nvarchar(max)  
  declare @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId');  
  
  BEGIN TRY  
  
   if OBJECT_ID('tempdb..##CustomerUserAddDetails') is not null  
    drop table ##CustomerUserAddDetails  
  
   if OBJECT_ID('tempdb..##View_SalesRepUserAddDetails') is not null  
    drop table ##View_SalesRepUserAddDetails  
  
   -----Getting SalesRep users associated with Portals of @UserId (given user)  
   select a.UserId   
   into #SalesRepUser  
   from ZnodeUserPortal a  
   inner join ZnodeUser b on a.UserId = b.UserId   
   inner join AspNetUsers c on b.AspNetUserId = c.Id  
   inner join AspNetUserRoles d on c.Id = d.UserId  
   inner join AspNetRoles e on d.RoleId = e.Id  
   where e.Name = 'Sales Rep'  
   and (a.PortalId = @PortalId or a.PortalId is null)  
        
   SELECT a.userId,a.AspNetuserId,azu.UserName,a.FirstName,a.MiddleName,a.LastName,a.PhoneNumber,  
    a.Email,a.EmailOptIn,a.CreatedBy,CONVERT( DATE, a.CreatedDate) CreatedDate,A.ModifiedBy,  
    CONVERT( DATE, a.ModifiedDate) ModifiedDate,ur.RoleId,r.Name RoleName,  
    CASE  
     WHEN B.LockoutEndDateUtc IS NULL  
     THEN CAST(1 AS    BIT)  
     ELSE CAST(0 AS BIT)  
    END IsActive,  
    CAST(CASE WHEN ISNULL(LockoutEndDateUtc, 0) = 0 THEN  0 ELSE  1 END  AS    BIT) AS IsLock,  
    (ISNULL(RTRIM(LTRIM(a.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(a.MiddleName)), '')  
     +CASE  
      WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '') = ''  
      THEN ''  
      ELSE ' '  
     END+ISNULL(RTRIM(LTRIM(a.LastName)), '')) FullName,  
    e.Name AccountName,a.AccountId,a.ExternalId,  
    CASE  
     WHEN a.AccountId IS NULL  
     THEN 0  
     ELSE 1  
    END IsAccountCustomer,  
    a.BudgetAmount,r.TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser  
    ,a.CustomerPaymentGUID  
  into ##View_SalesRepUserAddDetails  
  FROM ZnodeUser a  
  left JOIN ASPNetUsers B ON(a.AspNetuserId = b.Id)  
  LEFT JOIN ZnodeAccount e ON(e.AccountId = a.AccountId)  
  LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)  
  LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)                                 
  LEFT JOIN AspNetZnodeUser azu ON(azu.AspNetZnodeUserId = b.UserName)  
  WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )  
  and exists(select * from #SalesRepUser SRU where a.UserId = SRU.UserId)  
  
 alter table ##View_SalesRepUserAddDetails   
 add DepartmentId int, PermissionsName varchar(200), PermissionCode varchar(200), DepartmentName varchar(300), AccountPermissionAccessId int,  
 AccountUserOrderApprovalId int, ApprovalName varchar(1000) , ApprovalUserId int, PortalId int , StoreName varchar(1000)  
 ,CountryName varchar(1000),CityName varchar(1000),StateName varchar(1000),PostalCode varchar(1000), CompanyName varchar(1000)  
  
 --To get data for DepartmentId  
 update CUD SET DepartmentId = i.DepartmentId  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeDepartmentUser i ON(i.UserId = cud.UserId)  
    
 --To get data for PermissionsName  
 update CUD SET PermissionsName = h.PermissionsName, PermissionCode = h.PermissionCode  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeAccountUserPermission f ON(f.UserId = cud.UserId)  
 INNER JOIN ZnodeAccountPermissionAccess g ON(g.AccountPermissionAccessId = f.AccountPermissionAccessId)  
 INNER JOIN ZnodeAccessPermission h ON(h.AccessPermissionId = g.AccessPermissionId)  
   
 --To get data for DepartmentName  
 update CUD SET DepartmentName = j.DepartmentName  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeDepartmentUser i ON(i.UserId = cud.UserId)  
 INNER JOIN ZnodeDepartment j ON(j.DepartmentId = i.DepartmentId)  
  
 --To get data for AccountPermissionAccessId  
 update CUD SET AccountPermissionAccessId = f.AccountPermissionAccessId  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeAccountUserPermission f ON(f.UserId = cud.UserId)  
  
 --To get data for AccountPermissionAccessId  
 update CUD SET AccountPermissionAccessId = f.AccountPermissionAccessId  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeAccountUserPermission f ON(f.UserId = cud.UserId)  
    
 --To get data for AccountUserOrderApprovalId  
 update CUD SET AccountUserOrderApprovalId = ZAUOA.AccountUserOrderApprovalId  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeAccountUserOrderApproval ZAUOA ON cud.UserId = ZAUOA.UserID  
    
 --To get data for ApprovalName,ApprovalUserId  
 update CUD SET ApprovalName = ISNULL(RTRIM(LTRIM(ZU.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '')  
         +CASE  
          WHEN ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '') = ''  
          THEN ''  
          ELSE ' '  
         END,  
     ApprovalUserId = ZAUOA.ApprovalUserId  
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeAccountUserOrderApproval ZAUOA ON cud.UserId = ZAUOA.UserID  
 INNER JOIN ZnodeUser ZU ON(ZU.UserId = ZAUOA.ApprovalUserId)  
    
 --To get data for PortalId  
 update CUD SET PortalId = CASE  
         WHEN cud.AccountId IS NULL  
         THEN up.PortalId  
         ELSE ZPA.PortalId  
        END   
 from ##View_SalesRepUserAddDetails cud  
 INNER JOIN ZnodeUserPortal up ON(up.UserId = cud.UserId)   
 INNER JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = cud.AccountId)   
    
 ----To get data for StoreName  
 create index Ind_##View_SalesRepUserAddDetails_UserId on ##View_SalesRepUserAddDetails(UserId)  
 update CUD SET StoreName = CASE WHEN zp.StoreName IS NULL THEN 'ALL' ELSE zp.StoreName END   
 from ##View_SalesRepUserAddDetails cud  
 LEFT JOIN ZnodeUserPortal up ON(up.UserId = cud.UserId)   
 LEFT JOIN ZnodePortal zp ON (up.PortalId = zp.PortalId)  
    
 ----To get data for StoreName  
 update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName  
 from ##View_SalesRepUserAddDetails a  
 inner join ZnodeAccountAddress ZAA on a.AccountId = ZAA.AccountId  
 inner  JOIN ZnodeAddress ZA on ZA.AddressId = ZAA.AddressId  
 where isnull(a.AccountId,0)<> 0-- is not null  
  
    
 ----To get data for CountryName, CityName, StateName, PostalCode, CompanyName  
 update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName  
 from ##View_SalesRepUserAddDetails a  
 inner join ZnodeAccountAddress ZAA on a.AccountId = ZAA.AccountId  
 inner  JOIN ZnodeAddress ZA on ZA.AddressId = ZAA.AddressId  
 where isnull(a.AccountId,0)<> 0-- is not null  
    
 update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName  
 from ##View_SalesRepUserAddDetails a  
 inner join ZnodeUserAddress ZUA on a.UserId = ZUA.UserId  
 inner  JOIN ZnodeAddress ZA on ZA.AddressId = zua.AddressId  
    
 SET @SQL = '     
   SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,  
   EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,IsActive,IsLock,FullName,  
   AccountName,PermissionsName,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId , ExternalId,  
   BudgetAmount,AccountUserOrderApprovalId,ApprovalName,ApprovalUserId,PermissionCode,CustomerPaymentGUID ,StoreName,PortalId,  
   CountryName, CityName, StateName, PostalCode, CompanyName  
   INTO #Cte_CustomerUserDetail  
   FROM ##View_SalesRepUserAddDetails a       
   WHERE 1=1 '+dbo.Fn_GetWhereClause(@WhereClause, ' AND ')+'   
       
  
   SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,  
   EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,IsActive,IsLock,FullName,  
   AccountName,PermissionsName,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId , ExternalId,  
   BudgetAmount,AccountUserOrderApprovalId,ApprovalName,ApprovalUserId,PermissionCode,CustomerPaymentGUID ,RANK()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC')+',UserId DESC) RowId ,StoreName,PortalId,  
   CountryName, CityName, StateName, PostalCode, CompanyName  
   into #AccountDetail  
   FROM #Cte_CustomerUserDetail -- genrate the unique rowid   
  
  
   SET @Count= ISNULL((SELECT  Count(1) FROM #AccountDetail    ),0)  
  
   SELECT DISTINCT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,  
   EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,IsActive,IsLock,FullName,  
   AccountName,PermissionsName,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId , ExternalId,  
   BudgetAmount,AccountUserOrderApprovalId,ApprovalName,ApprovalUserId,PermissionCode ,CustomerPaymentGUID,StoreName,PortalId,  
   CountryName, CityName, StateName, PostalCode, CompanyName  
   ,Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC')+',UserId DESC) RowNumber  
   into ##CustomerUserAddDetails  
   FROM #AccountDetail '+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC');  
   PRINT @SQL           
   EXEC SP_executesql @SQL,  
   N'@Count INT OUT',  
   @Count = @RowCount OUT;  
    
  SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,  
    EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,IsActive,IsLock,FullName,  
    AccountName,PermissionsName,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId , ExternalId,  
    BudgetAmount,AccountUserOrderApprovalId,ApprovalName,ApprovalUserId,PermissionCode ,CustomerPaymentGUID,StoreName,PortalId,  
    CountryName, CityName, StateName, PostalCode, CompanyName  
  from ##CustomerUserAddDetails  
  Order by RowNumber  
   
  if OBJECT_ID('tempdb..##CustomerUserAddDetails') is not null  
   drop table ##CustomerUserAddDetails  
  
  if OBJECT_ID('tempdb..##View_SalesRepUserAddDetails') is not null  
   drop table ##View_SalesRepUserAddDetails  
  
  END TRY  
         BEGIN CATCH  
            DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSalesRepUsers @RoleName = '+@RoleName+' ,@WhereClause='+@WhereClause+' ,@Rows= '+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_By='+@Order_By+',@RowCount='+CAST(@RowCount AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(50)) ;  
            EXEC Znode_InsertProcedureErrorLog  
            @ProcedureName    = 'Znode_GetSalesRepUsers',  
            @ErrorInProcedure = @ERROR_PROCEDURE,  
            @ErrorMessage     = @ErrorMessage,  
            @ErrorLine        = @ErrorLine,  
            @ErrorCall        = @ErrorCall;  
         END CATCH;  
end