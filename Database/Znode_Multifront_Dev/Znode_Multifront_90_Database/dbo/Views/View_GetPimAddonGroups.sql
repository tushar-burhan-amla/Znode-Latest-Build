



CREATE VIEW [dbo].[View_GetPimAddonGroups] 
AS 
SELECT ag.PimAddonGroupId as PimAddonGroupId,ag.DisplayType, agl.AddonGroupName as AddOnGroupName FROM znodepimaddongroup ag INNER JOIN ZnodePimAddonGroupLocale agl ON ag.PimAddonGroupId=agl.PimAddonGroupId