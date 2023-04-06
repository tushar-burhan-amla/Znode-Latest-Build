 

CREATE   VIEW [dbo].[View_CustomerUserAddDetail]
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
			,
			Case when a.AccountId  is not null then ZAA.CountryName else ZA.CountryName end CountryName ,
			Case when a.AccountId  is not null then ZAA.CityName else ZA.CityName end CityName ,
			Case when a.AccountId  is not null then ZAA.StateName else ZA.StateName end StateName ,
			Case when a.AccountId  is not null then ZAA.PostalCode else ZA.PostalCode end PostalCode ,
			Case when a.AccountId  is not null then ZAA.CompanyName else ZA.CompanyName end CompanyName 
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
		  LEFT JOIN ZnodeAddress ZA on ZA.AddressId 
				in (Select AddressId from  ZnodeUserAddress ZUA where a.UserId = ZUA.UserId)  and ZA.IsDefaultBilling =  1
		  LEFT JOIN ZnodeAddress ZAA on ZAA.AddressId 
				in (Select AddressId from  ZnodeAccountAddress ZUAA where a.AccountId = ZUAA.AccountId) and ZAA.IsDefaultBilling = 1 
	
	  WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )
	  --AND a.AspNetUserId IS NOT NULL
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'View_CustomerUserAddDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'End
         Begin Table = "f"
            Begin Extent = 
               Top = 630
               Left = 38
               Bottom = 760
               Right = 276
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "g"
            Begin Extent = 
               Top = 762
               Left = 38
               Bottom = 892
               Right = 276
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "h"
            Begin Extent = 
               Top = 894
               Left = 38
               Bottom = 1024
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZAUOA"
            Begin Extent = 
               Top = 1026
               Left = 38
               Bottom = 1156
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZU"
            Begin Extent = 
               Top = 1158
               Left = 38
               Bottom = 1288
               Right = 270
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "up"
            Begin Extent = 
               Top = 894
               Left = 269
               Bottom = 1024
               Right = 439
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZPA"
            Begin Extent = 
               Top = 1290
               Left = 38
               Bottom = 1420
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "azu"
            Begin Extent = 
               Top = 1290
               Left = 251
               Bottom = 1403
               Right = 446
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "zp"
            Begin Extent = 
               Top = 1422
               Left = 38
               Bottom = 1552
               Right = 295
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZA"
            Begin Extent = 
               Top = 1554
               Left = 38
               Bottom = 1684
               Right = 256
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ZAA"
            Begin Extent = 
               Top = 1686
               Left = 38
               Bottom = 1816
               Right = 256
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'View_CustomerUserAddDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[36] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -768
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 270
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 262
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 270
               Left = 38
               Bottom = 400
               Right = 217
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ur"
            Begin Extent = 
               Top = 270
               Left = 255
               Bottom = 366
               Right = 425
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "r"
            Begin Extent = 
               Top = 366
               Left = 255
               Bottom = 496
               Right = 431
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "i"
            Begin Extent = 
               Top = 402
               Left = 38
               Bottom = 532
               Right = 223
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "j"
            Begin Extent = 
               Top = 498
               Left = 261
               Bottom = 628
               Right = 445
            End
            DisplayFlags = 280
            TopColumn = 0
         ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'View_CustomerUserAddDetail';

