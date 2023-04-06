


CREATE VIEW [dbo].[View_CustomerUserDetail]
AS
     SELECT a.userId,
            a.AspNetuserId,
            azu.UserName,
            a.FirstName,
            a.MiddleName,
            a.LastName,
            a.PhoneNumber,
            a.Email,
            a.EmailOptIn,
            a.CreatedBy,
            CONVERT( DATE, a.CreatedDate) CreatedDate,
            A.ModifiedBy,
            CONVERT( DATE, a.ModifiedDate) ModifiedDate,
            ur.RoleId,
            r.Name RoleName,
            CASE
                WHEN B.LockoutEndDateUtc IS NULL
                THEN CAST(1 AS    BIT)
                ELSE CAST(0 AS BIT)
            END IsActive,
            CAST(CASE WHEN ISNULL(LockoutEndDateUtc, 0) = 0 THEN  0 ELSE  1 END  AS    BIT) AS IsLock,
            (ISNULL(RTRIM(LTRIM(a.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(a.MiddleName)), '')+CASE
                                                                                                  WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '') = ''
                                                                                                  THEN ''
                                                                                                  ELSE ' '
                                                                                              END+ISNULL(RTRIM(LTRIM(a.LastName)), '')) FullName,
            e.Name AccountName,
            h.PermissionsName,
            j.DepartmentName,
            i.DepartmentId,
            a.AccountId,
            f.AccountPermissionAccessId,
            a.ExternalId,
            CASE
                WHEN a.AccountId IS NULL
                THEN 0
                ELSE 1
            END IsAccountCustomer,
            a.BudgetAmount,
            ZAUOA.AccountUserOrderApprovalId,
            (ISNULL(RTRIM(LTRIM(ZU.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '')+CASE
                                                                                                    WHEN ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '') = ''
                                                                                                    THEN ''
                                                                                                    ELSE ' '
                                                                                                END+ISNULL(RTRIM(LTRIM(ZU.LastName)), '')) ApprovalName,
            ZAUOA.ApprovalUserId,
            h.PermissionCode,
            CASE
                WHEN a.AccountId IS NULL
                THEN up.PortalId
                ELSE ZPA.PortalId
            END PortalId
			,r.TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser
			,a.CustomerPaymentGUID
			,CASE WHEN zp.StoreName IS NULL THEN 'ALL' ELSE zp.StoreName END StoreName
     FROM ZnodeUser a
          left JOIN ASPNetUsers B ON(a.AspNetuserId = b.Id)
          LEFT JOIN ZnodeAccount e ON(e.AccountId = a.AccountId)
          LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)
          LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)
          LEFT JOIN ZnodeDepartmentUser i ON(i.UserId = a.UserId)
          LEFT JOIN ZnodeDepartment j ON(j.DepartmentId = i.DepartmentId)
          LEFT JOIN ZnodeAccountUserPermission f ON(f.UserId = a.UserId)
          LEFT JOIN ZnodeAccountPermissionAccess g ON(g.AccountPermissionAccessId = f.AccountPermissionAccessId)
          LEFT JOIN ZnodeAccessPermission h ON(h.AccessPermissionId = g.AccessPermissionId)
          LEFT JOIN ZnodeAccountUserOrderApproval ZAUOA ON a.UserId = ZAUOA.UserID
          LEFT JOIN ZnodeUser ZU ON(ZU.UserId = ZAUOA.ApprovalUserId)
          LEFT JOIN ZnodeUserPortal up ON(up.UserId = a.UserId)
                                          
          LEFT JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = a.AccountId)
                                            
          LEFT JOIN AspNetZnodeUser azu ON(azu.AspNetZnodeUserId = b.UserName)
	      LEFT JOIN ZnodePortal zp ON (up.PortalId = zp.PortalId)
	  WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )
	
