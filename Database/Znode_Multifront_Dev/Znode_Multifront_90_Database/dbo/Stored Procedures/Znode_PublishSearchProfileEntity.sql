CREATE PROCEDURE [dbo].[Znode_PublishSearchProfileEntity] 
(
	@SearchProfileId INT,
	@UserId INT
)
AS
BEGIN
	BEGIN TRY			
		SET NOCOUNT ON
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @SearchProfilePublishLogId INT, @ProfileName NVARCHAR(200);

		SELECT TOP 1 @ProfileName=ProfileName FROM ZnodeSearchProfile WHERE SearchProfileId=@SearchProfileId;

		IF NOT EXISTS 
		(SELECT TOP 1 1 FROM ZnodePimCatalog 
		WHERE PimCatalogId =(SELECT TOP 1 PublishCatalogId FROM ZnodePublishCatalogSearchProfile 
													WHERE SearchProfileId = @SearchProfileId
													)
							
		) AND @ProfileName<>'ZnodeSearchProfile'
		BEGIN
			SELECT CAST(0 AS BIT) Status 
		END
		ELSE
		BEGIN
			INSERT INTO ZnodeSearchProfilePublishLog
				(SearchProfileId,PublishstateId,PublishStartDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
			SELECT @SearchProfileId, dbo.Fn_GetPublishStateIdForProcessing(),@GetDate,@UserId , @GetDate,@UserId , @GetDate

			SET @SearchProfilePublishLogId = @@IDENTITY

			SELECT a.SearchProfileId,A.SearchFeatureValue,B.FeatureCode,b.FeatureName,b.IsAdvanceFeature,B.HelpDescription  
			INTO #ZnodeSearchFeature
			FROM ZnodeSearchProfileFeatureMapping A 
			INNER JOIN ZnodeSearchFeature B ON A.SearchFeatureId = B.SearchFeatureId
			WHERE A.SearchProfileId = @SearchProfileId
				
			SELECT B.SearchProfileId,
				'['+(SELECT A.SearchFeatureValue,A.FeatureCode,A.FeatureName,A.IsAdvanceFeature
			From #ZnodeSearchFeature A
			WHERE A.SearchProfileId = b.SearchProfileId 
			For Json path, Without_Array_Wrapper)+']' as FeatureValueJson
			INTO #ZnodeSearchFeatureJson
			FROM #ZnodeSearchFeature B 
			GROUP BY B.SearchProfileId

			SELECT A.SearchProfileId,
				'['+(	SELECT B.AttributeCode, B.IsFacets,B.IsUseInSearch,B.BoostValue,B.IsNgramEnabled
						FROM ZnodeSearchProfileAttributeMapping B  
						WHERE A.SearchProfileId = B.SearchProfileId
						For Json Path, Without_Array_Wrapper)
				+']' as SearchProfileAttributeMappingJson
			INTO #ZnodeSearchProfileAttributeMappingJson 
			FROM ZnodeSearchProfileAttributeMapping A
			WHERE A.SearchProfileId = @SearchProfileId
			GROUP BY A.SearchProfileId

			SELECT A.SearchProfileId,
				'['+(	select B.FieldName, B.FieldValueFactor
						from ZnodeSearchProfileFieldValueFactor B  
						Where A.SearchProfileId = B.SearchProfileId
						For Json Path, Without_Array_Wrapper)
				+']' as FieldValueFactor
			INTO #ZnodeSearchProfileFieldValueFactor 
			FROM ZnodeSearchProfileFieldValueFactor A
			WHERE A.SearchProfileId = @SearchProfileId
			GROUP BY A.SearchProfileId

			SELECT @SearchProfilePublishLogId SearchProfilePublishLogId,  b.SearchProfileId,
				b.ProfileName,pc.PimCatalogId PublishCatalogId,ZSFJ.FeatureValueJson FeaturesList
				,c.QueryTypeName,b.SearchQueryTypeId  
				,c.QueryBuilderClassName,d.QueryTypeName SubQueryTypeName,vf.FieldValueFactor,
				b.Operator,ZSPAMJ.SearchProfileAttributeMappingJson SearchProfileAttributeMappingJson,
				@UserId CreatedBy, @GetDate CreatedDate,@UserId Modifiedby, @GetDate ModifiedDate
			INTO #ZnodeSearchProfile
			FROM  ZnodeSearchProfile B 
			LEFT JOIN dbo.ZnodePublishCatalogSearchProfile cc on cc.SearchProfileId=b.SearchProfileId
			LEFT JOIN dbo.ZnodePImCatalog pc on pc.PimCatalogId=cc.PublishCatalogId
			INNER JOIN ZnodeSearchQueryType C on b.SearchQueryTypeId=C.SearchQueryTypeId 
			--LEFT JOIN ZnodeSearchProfileFieldValueFactor ZSPF on ZSPF.SearchProfileId = b.SearchProfileId
			LEFT JOIN #ZnodeSearchFeatureJson ZSFJ on ZSFJ.SearchProfileId = b.SearchProfileId
			LEFT JOIN  #ZnodeSearchProfileAttributeMappingJson ZSPAMJ on  ZSPAMJ.SearchProfileId = b.SearchProfileId
			LEFT JOIN ZnodeSearchQueryType d on  d.SearchQueryTypeId =b.SearchSubQueryTypeId
			LEFT JOIN #ZnodeSearchProfileFieldValueFactor vf on vf.SearchProfileId =b.SearchProfileId
			WHERE b.SearchProfileId=@SearchProfileId

			IF EXISTS (SELECT TOP 1 1 FROM #ZnodeSearchProfile)
			BEGIN 
				INSERT INTO ZnodePublishSearchProfileEntity(
					SearchProfilePublishLogId,SearchProfileId,SearchProfileName,ZnodeCatalogId,FeaturesList,
					QueryTypeName,SearchQueryType,
					QueryBuilderClassName,SubQueryType,FieldValueFactor,Operator,SearchProfileAttributeMappingJson,
					CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				SELECT SearchProfilePublishLogId,SearchProfileId,ProfileName,PublishCatalogId,FeaturesList,
					QueryTypeName,SearchQueryTypeId  ,
					QueryBuilderClassName,SubQueryTypeName,FieldValueFactor,Operator,SearchProfileAttributeMappingJson,
					CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
				FROM #ZnodeSearchProfile 
		
		     

				UPDATE ZnodeSearchProfilePublishLog
				SET PublishstateId = dbo.Fn_GetPublishStateIdForPublish()
				WHERE SearchProfilePublishLogId = @SearchProfilePublishLogId

				UPDATE ZnodeSearchProfile
				SET PublishStateId = dbo.Fn_GetPublishStateIdForPublish() 
				WHERE SearchProfileId = @SearchProfileId

				UPDATE ZnodePublishSearchProfileEntity 
				SET PublishstateId = dbo.Fn_GetPublishStateIdForPublish() 
				WHERE SearchProfileId = @SearchProfileId 

				DELETE
				FROM ZnodePublishSearchProfileEntity
				WHERE SearchProfilePublishLogId  < @SearchProfilePublishLogId AND SearchProfileId = @SearchProfileId
				SELECT CAST(1 AS BIT) Status
			END
			ELSE
			BEGIN
				UPDATE ZnodeSearchProfilePublishLog 
				SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
				WHERE SearchProfilePublishLogId = @SearchProfilePublishLogId 

				SELECT CAST(0 AS BIT) Status 

				UPDATE ZnodeSearchProfile 
				SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
				WHERE SearchProfileId = @SearchProfileId 
		
	     		UPDATE ZnodePublishSearchProfileEntity 
				SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
				WHERE SearchProfileId = @SearchProfileId
			END
		END
	END TRY

	BEGIN CATCH
		UPDATE ZnodeSearchProfile 
		SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
		WHERE SearchProfileId = @SearchProfileId 

		UPDATE ZnodeSearchProfilePublishLog 
		SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
		WHERE SearchProfilePublishLogId = @SearchProfilePublishLogId AND SearchProfileId = @SearchProfileId
		
		UPDATE ZnodePublishSearchProfileEntity 
		SET PublishstateId = dbo.Fn_GetPublishStateIdForPublishFailed() 
		WHERE SearchProfileId = @SearchProfileId 

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishSearchProfileEntity @SearchProfileId = '+CAST(@SearchProfileId AS VARCHAR(10))+',@UserId='+CAST(@UserId AS VARCHAR(10));

		SELECT CAST(0 AS BIT) Status
		 
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_PublishSearchProfileEntity',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END

