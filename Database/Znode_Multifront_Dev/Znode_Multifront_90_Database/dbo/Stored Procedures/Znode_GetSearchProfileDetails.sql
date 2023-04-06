CREATE PROCEDURE [dbo].[Znode_GetSearchProfileDetails]
(
	@SearchProfileId int 
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the catalog 
	 Unit Testig 
	 EXEC  Znode_GetCatalogList '',100,1,'',0
*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 

		   Declare @SearchQueryTypeId int 

				Select @SearchQueryTypeId=SearchQueryTypeId 
				from ZnodeSearchProfile
				Where  SearchProfileId=@SearchProfileId	

				Exec [dbo].[Znode_GetSearchQueryTypeWiseFeatureDetails] @SearchProfileId=@SearchProfileId,@SearchQueryTypeId=@SearchQueryTypeId
						

				Select b.SearchProfileId,b.AttributeCode,b.BoostValue,b.IsNgramEnabled,b.IsUseInSearch,b.IsFacets
				from ZnodeSearchProfileAttributeMapping b
				Where  b.SearchProfileId=@SearchProfileId AND IsUseInSearch=1

				Select b.SearchProfileId,b.AttributeCode,b.BoostValue,b.IsNgramEnabled,b.IsFacets,b.IsUseInSearch
				from ZnodeSearchProfileAttributeMapping b
				Where  b.SearchProfileId=@SearchProfileId AND IsFacets=1

				Select b.SearchQueryTypeId , b.SearchQueryTypeId ,b.ProfileName,b.Operator,
				c.QueryTypeName,c.QueryBuilderClassName,
				b.SearchSubQueryTypeId,d.QueryTypeName SubQueryTypeName,d.QueryBuilderClassName SubQueryBuilderClassName,
				cc.PublishCatalogId,pc.CatalogName, b.SearchProfileId
				from  ZnodeSearchProfile B 
				left join dbo.ZnodePublishCatalogSearchProfile cc on cc.SearchProfileId=b.SearchProfileId
				left join dbo.ZnodePimCatalog pc on pc.PimcatalogId=cc.PublishCatalogId
				inner join ZnodeSearchQueryType C on b.SearchQueryTypeId=C.SearchQueryTypeId 
				Left join ZnodeSearchQueryType d on  d.SearchQueryTypeId =b.SearchSubQueryTypeId
				Where  b.SearchProfileId=@SearchProfileId	 

				select pfv.FieldName,pfv.FieldValueFactor 
				from ZnodeSearchProfileFieldValueFactor pfv
				WHERE pfv.SearchProfileId = @SearchProfileId

		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			@ErrorLine VARCHAR(100)= ERROR_LINE(), 
			@ErrorCall NVARCHAR(MAX)
	--		= 'EXEC Znode_GetCatalogList @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS
 --VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetZnodeSearchProfileList',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END