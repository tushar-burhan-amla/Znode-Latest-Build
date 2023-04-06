CREATE VIEW [dbo].[View_RoleUsers]
AS
     SELECT SWQ.Username,
            ANR.Name RoleName
     FROM AspNetUserRoles AS ANUR
          INNER JOIN AspNetRoles AS ANR ON ANUR.RoleId = ANR.Id
          INNER JOIN AspNetUsers AS ANU ON ANUR.userId = ANU.id
          INNER JOIN AspNetZnodeUser AS SWQ ON(swq.AspNetZnodeUserId = anu.UserName);