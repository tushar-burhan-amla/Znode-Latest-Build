
CREATE View [dbo].[View_PimAttributeGroupbyFamily]
   
   AS SELECT DISTINCT qq.PimAttributeFamilyId,GroupCode,AttributeGroupName,c.PimAttributeGroupId,CAST('Group' AS VARCHAR(100)) GroupType

	FROM  dbo.ZnodePimAttributeFamily qq
	INNER JOIN dbo.ZnodePimFamilyGroupMapper w ON (qq.PimAttributeFamilyId = w.PimAttributeFamilyId )
	INNER JOIN ZnodePimAttributeGroup q ON (w.PimAttributeGroupId = q.PimAttributeGroupId)
	INNER JOIN ZnodePimAttributeGroupLocale c ON (q.PimAttributeGroupId = c.PimAttributeGroupId )