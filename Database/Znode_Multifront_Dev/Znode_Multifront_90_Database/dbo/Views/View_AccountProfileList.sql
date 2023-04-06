CREATE VIEW [dbo].[View_AccountProfileList]
AS
SELECT  ZA.AccountId, ZAP.AccountProfileId, ZP.ProfileId, ZA.Name, ZP.ProfileName, ZAP.IsDefault, CASE WHEN ZAP.AccountProfileId IS NULL THEN 0 ELSE 1 END IsAssociated,ZP.ParentProfileId
FROM dbo.ZnodeAccount AS ZA CROSS APPLY dbo.ZnodeProfile AS ZP 
LEFT JOIN dbo.ZnodeAccountProfile AS ZAP ON (ZAP.AccountId = ZA.AccountId AND ZAP.ProfileId = ZP.ProfileId)