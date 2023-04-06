CREATE PROCEDURE [dbo].[Znode_GetWebStoreSearchProfileTrigger]
(
	@Keyword NVARCHAR(100) = '',
    @ProfileId INT = '',
	@PublishCatalogId INT,
	@PortalId INT 
)
AS 
/*
	 Summary :- This Procedure is used to get the published search profile details. 
	 Unit Testig 
	 EXEC [Znode_GetWebStoreSearchProfileTrigger] 'Apple','1',3,1
*/
BEGIN 
	BEGIN TRY
	SET NOCOUNT ON 
		DECLARE @SearchProfileId INT , @DefaultSearchProfileId int  

		--Select @SearchProfileId=d.SearchProfileId 
		--from [ZnodeSearchProfileTrigger] d
		--inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=d.SearchProfileId
		--Where d.Keyword=@Keyword
		--and d.ProfileId=@ProfileId
		--and c.PublishCatalogId=@PublishCatalogId

		--If isnull(@SearchProfileId,0)=0
		--Begin
		--Select @SearchProfileId=d.SearchProfileId 
		--from [ZnodeSearchProfileTrigger] d
		--inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=d.SearchProfileId
		--Where d.Keyword=@Keyword
		--and c.PublishCatalogId=@PublishCatalogId
		--and d.ProfileId is null
		--eND

		--If isnull(@SearchProfileId,0)=0
		--Begin
		--Select @SearchProfileId=d.SearchProfileId 
		--from [ZnodeSearchProfileTrigger] d
		--inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=d.SearchProfileId
		--Where d.ProfileId=@ProfileId
		--and c.PublishCatalogId=@PublishCatalogId
		--and d.Keyword is null
		--eND

		IF ISNULL(@SearchProfileId,0)=0
		BEGIN
			SELECT @SearchProfileId=SearchProfileId 
			FROM ZnodePublishCatalogSearchProfile Where PublishCatalogId=@PublishCatalogId
		END 
		--If isnull(@SearchProfileId,0)=0
		-- Begin
		--	Select @SearchProfileId=min(a.SearchProfileId)
		--	from ZnodePortalSearchProfile a
		--	inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=a.SearchProfileId
		--	Where a.PortalId =@PortalId 
		--	and a.IsDefault=0
		--	and c.PublishCatalogId=@PublishCatalogId
		--End 

		
		SELECT @DefaultSearchProfileId = a.SearchProfileId
		FROM ZnodeSearchProfile a
		WHERE ProfileName = 'ZnodeSearchProfile'
		
		If Not exists (Select TOP 1 1 from ZnodePublishSearchProfileEntity where SearchProfileId = @SearchProfileId  )
			SELECT PublishSearchProfileEntityId,SearchProfileId,SearchProfileName,ZnodeCatalogId,FeaturesList,QueryTypeName,SearchQueryType,
				QueryBuilderClassName,SubQueryType,FieldValueFactor,[Operator],PublishStateId,SearchProfileAttributeMappingJson,CreatedBy,
				CreatedDate,ModifiedBy,ModifiedDate,SearchProfilePublishLogId
			FROM ZnodePublishSearchProfileEntity
			WHERE ( SearchProfileId = @DefaultSearchProfileId )  
		Else 
			SELECT PublishSearchProfileEntityId,SearchProfileId,SearchProfileName,ZnodeCatalogId,FeaturesList,QueryTypeName,SearchQueryType,
				QueryBuilderClassName,SubQueryType,FieldValueFactor,[Operator],PublishStateId,SearchProfileAttributeMappingJson,CreatedBy,
				CreatedDate,ModifiedBy,ModifiedDate,SearchProfilePublishLogId
			FROM ZnodePublishSearchProfileEntity
			WHERE ( SearchProfileId = @SearchProfileId  )  


		----To get SearchRule Item Details
		EXEC [Znode_GetSearchTriggerItemRuleDetails] @Keyword = @Keyword, @PublishCatalogId = @PublishCatalogId

	END TRY 
	BEGIN CATCH
		DECLARE @Status BIT;

		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 
					'EXEC Znode_GetWebStoreSearchProfileTrigger 
						@Keyword = '''+ISNULL(@Keyword,'''''')+''',
						@ProfileId='+ISNULL(CAST(@ProfileId AS VARCHAR(50)),'''''')+',
						@PublishCatalogId='+ISNULL(CAST(@PublishCatalogId AS	VARCHAR(50)),'''''')+',
						@PortalId='+ISNULL(CAST(@PortalId AS	VARCHAR(50)),'''''');
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;
		  
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetWebStoreSearchProfileTrigger',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END