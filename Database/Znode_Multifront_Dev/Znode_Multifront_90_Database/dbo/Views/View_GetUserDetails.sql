CREATE View [dbo].[View_GetUserDetails] 
AS 
SELECT Id
,ANU.Email
,ANU.EmailConfirmed
,ANU.PasswordHash
,ANU.SecurityStamp
,ANU.PhoneNumber
,ANU.PhoneNumberConfirmed
,ANU.TwoFactorEnabled
,ANU.LockoutEndDateUtc
,ANU.LockoutEnabled
,ANU.AccessFailedCount
,ANU.PasswordChangedDate
,ZU.UserId, ANZU.UserName 
FROM AspNetZnodeUser ANZU 
INNER JOIN AspNetUsers ANU  ON (ANU.UserName = ANZU.AspNetZnodeUserId)
INNER JOIN ZnodeUSer ZU ON (ZU.AspNetUserId = ANU.Id)