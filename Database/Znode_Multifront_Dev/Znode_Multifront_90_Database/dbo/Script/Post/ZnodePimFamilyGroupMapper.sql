
delete from [ZnodePimFamilyGroupMapper]
where PimAttributeFamilyId = (select TOP 1 PimAttributeFamilyId from ZnodePimAttributeFamily WHERE FamilyCode = 'DefaultCategory') 
AND PimAttributeGroupId in (SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode IN ('ProductSetting','ShippingSettings'))

Delete from [ZnodePimFamilyGroupMapper]
where PimAttributeFamilyId IN (select PimAttributeFamilyId from ZnodePimAttributeFamily WHERE IsCategory = 1) 
AND PimAttributeGroupId in (SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode IN ('ProductSetting','ShippingSettings'))
