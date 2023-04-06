
-- TASKid -ZPD1149

	DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
        INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
			,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
			CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
		OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  
		
		SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Text')
		,'CategoryCode',1,0,1,1,0,0,1,1,null,1,0,null,2,GETDATE(),2,GETDATE()
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'CategoryCode')
		
		INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT 1 ,IPAS.PimAttributeId, 'Category Code', null, 2,GETDATE(),2,GETDATE()   
		FROM @InsertedPimAttributeIds IPAS 
		


		IF OBJECT_ID('#AttributeValidation', 'U') IS NOT NULL
		BEGIN 
			DROP TABLE #AttributeValidation
		END
		
		CREATE TABLE #AttributeValidation (InputValidationId INT,InputValidationRuleId INT,RegExp NVARCHAR (1000))
		INSERT INTO #AttributeValidation VALUES (5,8,null)
		INSERT INTO #AttributeValidation VALUES(6,null,'^[a-zA-Z0-9][a-zA-Z0-9]*$')
		INSERT INTO #AttributeValidation VALUES(10,null,100)
		INSERT INTO #AttributeValidation VALUES(21,null,'true')
	
		
		INSERT INTO ZnodePimAttributeValidation
		(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT DISTINCT IPA.PimAttributeId,ZAIV.InputValidationId,ZAIVR.InputValidationRuleId,ZAIVR.RegExp , 2,GETDATE(),2,GETDATE()   
		FROM @InsertedPimAttributeIds IPA
		INNER JOIN ZnodeAttributeInputValidation ZAIV ON IPA.AttributeTypeId = ZAIV.AttributeTypeId
		INNER JOIN #AttributeValidation ZAIVR ON (ZAIVR.InputValidationId = ZAIV.InputValidationId) 
		WHERE NOT EXISTS  (SELECT TOP 1 1 FROM ZnodePimAttributeValidation pv WHERE pv.PimAttributeId = IPA.PimAttributeId
							AND pv.InputValidationId = ZAIV.InputValidationId)
	    AND IPA.AttributeTypeId  IN (SELECT AttributeTypeId FROM ZnodeAttributeType AT  WHERE  AT.AttributeTypeName IN ('Text'))
		
		
		insert into ZnodePimFrontendProperties 
	    (PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select IPA.PimAttributeId,0 IsComparable, 0 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
		from @InsertedPimAttributeIds IPA

		
			INSERT INTO ZnodePimAttributeGroupMapper
		(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		select (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'GeneralInfo'),(select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode'),null,1,2,getdate(),2,getdate()
		WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'GeneralInfo') AND
		PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode') )
		


		insert into ZnodePimFamilyGroupMapper(PimAttributeFamilyId	,PimAttributeGroupId	,PimAttributeId	,GroupDisplayOrder	,IsSystemDefined	,CreatedBy	,
		CreatedDate	,ModifiedBy	,ModifiedDate)
		select (select PimAttributeFamilyId from ZnodePimAttributeFamily where FamilyCode = 'DefaultCategory'),(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'GeneralInfo'),
		(select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode'),5.000000,1,2,getdate(),2,getdate()
		WHERE NOT EXISTS (select * from ZnodePimFamilyGroupMapper where PimAttributeFamilyId = (select PimAttributeFamilyId from ZnodePimAttributeFamily where FamilyCode = 'DefaultCategory') AND 
		PimAttributeGroupId = (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'GeneralInfo') and PimAttributeId=  (select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode') )

		

DECLARE @tbl_attributevalue TABLE (PimAttributeValueId INT,PimCategoryId INT)

INSERT INTO ZnodePimCategoryAttributeValue(PimCategoryId,PimAttributeFamilyId,PimAttributeId,PimAttributeDefaultValueId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
OUTPUT inserted.PimCategoryAttributeValueId,inserted.PimCategoryId into @tbl_attributevalue
SELECT  distinct pc.PimCategoryId,pc.PimAttributeFamilyId,(select PimAttributeId from ZnodePimattribute where attributecode = 'CategoryCode'),null, 2 ,getdate(),2,getdate()
FROM ZnodePimCategory pc
INNER JOIN ZnodePimCategoryAttributeValue b on (pc.PimCategoryId = b.PimCategoryId)
INNER JOIN ZnodePimCategoryAttributeValuelocale c on (b.PimCategoryAttributeValueId = c.PimCategoryAttributeValueId)
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValuelocale pam 
INNER JOIN ZnodePimCategoryAttributeValue pav on  (pav.PimCategoryAttributeValueId = pam.PimCategoryAttributeValueId)
INNER JOIN ZnodePimCategory pp on (pp.PimCategoryId = pav.PimCategoryId)
WHERE pav.PimCategoryId = pc.PimCategoryId and pav.PimAttributeId = (select PimAttributeId from ZnodePimattribute where attributecode = 'CategoryCode')
)
AND b.PimAttributeId = (select PimAttributeId from ZnodePimattribute where attributecode = 'CategoryName')
AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValue x WHERE x.PimCategoryId = pc.PimCategoryId AND x.PimAttributeId = (select PimAttributeId from ZnodePimattribute where attributecode = 'CategoryCode'))




INSERT INTO ZnodePimCategoryAttributeValuelocale (LocaleId,PimCategoryAttributeValueId,CategoryValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT distinct 1,PimAttributeValueId,'C'+cast(PimCategoryId as nvarchar(400)), 2,getdate(),2,getdate()
FROM @tbl_attributevalue a 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValuelocale b WHERE b.PimCategoryAttributeValueId = a.PimAttributeValueId)



IF OBJECT_ID('#AttributeValidation', 'U') IS NOT NULL
		BEGIN 
			DROP TABLE #AttributeValidation
		END

GO

--ZPD-22363 Dt.26-Sept-2022
DELETE A FROM
(
SELECT PimCategoryProductId, PimCategoryId, PimProductId, ROW_NUMBER() OVER (PARTITION BY PimCategoryId, PimProductId ORDER BY PimCategoryProductId) As Rn
FROM ZnodePimCategoryProduct
) A WHERE A.Rn>1
