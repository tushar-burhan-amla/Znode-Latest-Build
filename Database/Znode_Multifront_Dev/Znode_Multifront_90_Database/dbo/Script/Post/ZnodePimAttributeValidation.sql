UPDATE ZnodePimAttributeValidation SET Name = 1
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'DisplayOrderCategory')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber'
and AttributeTypeId = (select top 1  AttributeTypeId from ZnodeAttributeType WHERE AttributeTypeName = 'Number'))

UPDATE ZnodePimAttributeValidation SET Name = 99999
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'DisplayOrderCategory')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber'
and AttributeTypeId = (select top 1 AttributeTypeId from ZnodeAttributeType WHERE AttributeTypeName = 'Number'))
