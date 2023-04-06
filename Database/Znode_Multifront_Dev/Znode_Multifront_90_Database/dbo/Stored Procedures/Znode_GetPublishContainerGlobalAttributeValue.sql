CREATE PROCEDURE [dbo].[Znode_GetPublishContainerGlobalAttributeValue]
(
    @CMSContainerProfileVariantId   INT = 0,
	@LocaleCode VARCHAR(100) = ''
)
AS
BEGIN
	BEGIN TRY
		DECLARE @LocaleId INT
		SET @LocaleId = ISNULL(( SELECT TOP 1 LocaleId FROM ZnodeLocale WHERE Code = @LocaleCode),0)

		IF OBJECT_ID('tempdb..#ContainerVariantEntity') IS NOT NULL
			DROP TABLE #ContainerVariantEntity;

		SELECT DISTINCT ZPCCVE.CMSContainerProfileVariantId,ZPCCVE.Name as ContentContainerName ,ZPCCVE.ContainerKey, 
			REPLACE(REPLACE(ZPCCVE.GlobalAttributes,'[',''),']','') AS GlobalAttributes, ZCCT.Name AS ContainerTemplateName
		INTO #ContainerVariantEntity
		FROM ZnodePublishContentContainerVariantEntity ZPCCVE
		LEFT JOIN ZnodeCMSContainerTemplate ZCCT ON ISNULL(ZPCCVE.CMSContainerTemplateId,0) = ZCCT.CMSContainerTemplateId
		WHERE ZPCCVE.CMSContainerProfileVariantId = @CMSContainerProfileVariantId AND (ISNULL(ZPCCVE.LocaleId,0) = @LocaleId OR ISNULL(@LocaleId,0) = 0)

		--SELECT t.CMSContainerProfileVariantId,t.ContentContainerName,t.ContainerKey,
		--	'['+STRING_AGG( t.GlobalAttributes,',') WITHIN GROUP (ORDER BY ContainerKey)+']'
		--	,t.ContainerTemplateName
		--FROM #ContainerVariantEntity t
		--GROUP BY t.CMSContainerProfileVariantId,t.ContentContainerName,t.ContainerKey,t.ContainerTemplateName;

		SELECT DISTINCT t2.CMSContainerProfileVariantId,t2.ContentContainerName,t2.ContainerKey,
			'['+STUFF((
			SELECT DISTINCT ',' + t1.GlobalAttributes
			FROM #ContainerVariantEntity t1
			WHERE t1.CMSContainerProfileVariantId=t2.CMSContainerProfileVariantId AND t1.ContentContainerName=t2.ContentContainerName
				AND t1.ContainerKey=t2.ContainerKey
			FOR XML PATH('')
			), 1, 1, '')+']' As GlobalAttributes
			, t2.ContainerTemplateName
		FROM #ContainerVariantEntity t2

		--SELECT ZPCCVE.Name as ContentContainerName ,ZPCCVE.ContainerKey, ZPCCVE.GlobalAttributes, ZCCT.Name AS ContainerTemplateName
		--FROM ZnodePublishContentContainerVariantEntity ZPCCVE
		--LEFT JOIN ZnodeCMSContainerTemplate ZCCT ON ISNULL(ZPCCVE.CMSContainerTemplateId,0) = ZCCT.CMSContainerTemplateId
		--WHERE ZPCCVE.CMSContainerProfileVariantId = @CMSContainerProfileVariantId AND (ISNULL(ZPCCVE.LocaleId,0) = @LocaleId OR ISNULL(@LocaleId,0) = 0)

		--SELECT 1 AS ID,CAST(1 AS BIT) AS Status;       
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		DECLARE @Status BIT ;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishContainerGlobalAttributeValue 					
					@CMSContainerProfileVariantId='+CAST(@CMSContainerProfileVariantId AS VARCHAR(10))
					+''',@LocaleCode='+ISNULL(@LocaleCode,'''')
					
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPublishContainerGlobalAttributeValue',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;