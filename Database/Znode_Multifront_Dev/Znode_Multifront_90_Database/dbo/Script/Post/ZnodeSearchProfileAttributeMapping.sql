-- ZPD-16760 dt: 13Dec2021
update ZnodeSearchProfileAttributeMapping 
set IsUseInSearch=0 
where SearchProfileId=1 and IsFacets = 0 and AttributeCode not in ('ProductName','SKU')
and not exists(select * from ZnodeSearchProfileAttributeMapping where SearchProfileId=1 and IsFacets = 0 and AttributeCode not in ('ProductName','SKU') and IsUseInSearch <> 1)

-- ZPD-16937 dt: 20Dec2021
Update ZnodeSearchProfileAttributeMapping 
set IsFacets = 0 
where SearchProfileId = 1 and IsFacets = 1 and AttributeCode <> 'Brand'