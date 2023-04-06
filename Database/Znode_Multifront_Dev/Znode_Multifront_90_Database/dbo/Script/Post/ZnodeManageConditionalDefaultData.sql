-- ZPD-19785 Dt: 15-June-2022
IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeManageConditionalDefaultData WHERE ConditionalCode = 'DeleteOldSearchProfileDuringDatabaseUpgradeOneTime' AND IsActive=1)
BEGIN
	IF OBJECT_ID('tempdb..#SearchProfileId') IS NOT NULL
		DROP TABLE #SearchProfileId;

	SELECT SearchProfileId 
	INTO #SearchProfileId
	FROM ZnodeSearchProfile WHERE ProfileName<>'ZnodeSearchProfile';

	DELETE FROM ZnodePortalSearchProfile WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	--DELETE FROM ZnodeSearchActivity WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodePublishCatalogSearchProfile WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfileAttributeMapping WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfileFeatureMapping WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfileFieldValueFactor WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfileProductMapping WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfileTrigger WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	UPDATE ZnodeSearchActivity SET SearchProfileId = NULL WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
	DELETE FROM ZnodeSearchProfile WHERE SearchProfileId IN (SELECT SearchProfileId FROM #SearchProfileId)
END

GO

INSERT INTO ZnodeManageConditionalDefaultData (ConditionalCode,ConditionalName,DataSource,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'DeleteOldSearchProfileDuringDatabaseUpgradeOneTime',
	'This is only for delete old search profile and its references data from respective tables during upgrade of existing database & this will execute only one time. This record will used to ignore delete records during fresh database creation.'
		,'Upgrade Database',1,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeManageConditionalDefaultData WHERE ConditionalCode = 'DeleteOldSearchProfileDuringDatabaseUpgradeOneTime' AND IsActive=1);
