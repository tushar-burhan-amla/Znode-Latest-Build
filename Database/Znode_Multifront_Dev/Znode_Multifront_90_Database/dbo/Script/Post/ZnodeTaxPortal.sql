
UPDATE ZnodeTaxPortal SET AvataxIsTaxIncluded = 0 WHERE AvataxIsTaxIncluded IS NULL

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'AvataxIsTaxIncluded' AND TABLE_NAME = 'ZnodeTaxPortal')
BEGIN
	IF NOT EXISTS(SELECT * FROM SYS.default_constraints WHERE NAME = 'DF_ZnodeTaxPortal_AvataxIsTaxIncluded')
	BEGIN
		ALTER TABLE ZnodeTaxPortal ADD CONSTRAINT DF_ZnodeTaxPortal_AvataxIsTaxIncluded DEFAULT 0 FOR AvataxIsTaxIncluded
	END
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'AvataxIsTaxIncluded' AND TABLE_NAME = 'ZnodeTaxPortal')
BEGIN
	ALTER TABLE ZnodeTaxPortal ALTER COLUMN AvataxIsTaxIncluded BIT NOT NULL
END


UPDATE ZnodeTaxRuleTypes SET ClassName = 'AvataxClient' WHERE ClassName = 'AvataxTaxSales' AND NAME = 'Avatax Tax Class'

-- ZPD-23166 Dt.11-Nov-2022
DELETE TRT
FROM ZnodeTaxRuleTypes TRT
WHERE ClassName='STOCCHTax'
	AND NOT EXISTS (SELECT * FROM ZnodeTaxPortal A WHERE A.TaxRuleTypeId=TRT.TaxRuleTypeId)
	AND NOT EXISTS (SELECT * FROM ZnodeTaxRule B WHERE B.TaxRuleTypeId=TRT.TaxRuleTypeId)
	AND NOT EXISTS (SELECT * FROM ZnodeOmsQuoteTaxRule C WHERE C.TaxRuleTypeId=TRT.TaxRuleTypeId)
	AND NOT EXISTS (SELECT * FROM ZnodeOmsTaxRule D WHERE D.TaxRuleTypeId=TRT.TaxRuleTypeId);
