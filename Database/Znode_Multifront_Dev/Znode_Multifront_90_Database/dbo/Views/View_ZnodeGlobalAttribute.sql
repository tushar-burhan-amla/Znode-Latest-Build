

CREATE View [dbo].[View_ZnodeGlobalAttribute] As
SELECT zga.AttributeTypeId,
Case When AttributeTypeName IN ('File','Image','Audio','Video') then 
'Media'
when AttributeTypeName IN ('Simple Select','Multi Select') then
'Select'
when ZAY.AttributeTypeName IN ('Text Area')  then
'TextArea' 
else 'Input'
end GroupAttributeType,GlobalAttributeId ,AttributeCode ,AttributeTypeName,
zga.IsRequired,zga.IsLocalizable,zga.IsActive,zga.DisplayOrder
FROM ZnodeGlobalAttribute zga 
INNER JOIN ZnodeAttributeType ZAY ON (ZAY.AttributeTypeId = zga.AttributeTypeId )