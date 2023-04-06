
INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.dwg',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.dwg')

INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.bin',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.bin')

INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.file',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.file')

INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.tar',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.tar')

INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.gz',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.gz')

INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'Image'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.dwg'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'Image') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.dwg'))

INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.bin'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.bin'))

INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.file'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.file'))

INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.tar'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.tar'))

INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.gz'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.gz'))

INSERT INTO ZnodeAttributeInputValidationRule(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),null,'.rfa',9,null,
	2,getdate(),2,getdate()
where not exists(select * from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.rfa')


INSERT INTO ZnodeMediaAttributeValidation(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions'),
(select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.rfa'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaAttributeValidation where 
MediaAttributeId = (select top 1 MediaAttributeId from ZnodeMediaAttribute where AttributeCode = 'File') and
InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions') and 
InputValidationRuleId = (select top 1 InputValidationRuleId from ZnodeAttributeInputValidationRule where InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where name = 'Extensions')
and ValidationName = '.rfa'))

--dt 11/11/2022 --> ZPD-22870

INSERT INTO ZnodeMediaAttributeValidation
	(MediaAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 MediaAttributeId FROM [dbo].[ZnodeMediaAttribute] WHERE AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image')),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1)),
	(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'),
	'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM [dbo].[ZnodeMediaAttributeValidation] WHERE MediaAttributeId=(SELECT TOP 1 MediaAttributeId FROM [dbo].[ZnodeMediaAttribute] WHERE AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image'))
	AND InputValidationId=(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image'))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))