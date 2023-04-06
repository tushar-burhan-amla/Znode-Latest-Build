
CREATE PROCEDURE [dbo].[Znode_GetSearchQueryTypeWiseFeatureDetails_bak]
(
	@SearchProfileId int ,
	@SearchQueryTypeId int
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the catalog 
	 Unit Testig 
	 EXEC  Znode_GetSearchQueryTypeWiseFeatureDetails 2,2
*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 
				Declare @FeatureList as table (SearchFeatureId int,ParentSearchFeatureId int )
				
				
				Insert into @FeatureList
				(SearchFeatureId)
				Select SearchFeatureId
				from dbo.ZnodeSearchQueryTypeFeature dd 
				inner join dbo.ZnodeSearchQueryType  ss on ss.SearchQueryTypeId=dd.SearchQueryTypeId
				and ss.SearchQueryTypeId=@SearchQueryTypeId

				; WITH Objects --(Id, NextId,HLevel,SortPath/*,NewView1*/) 
				AS
				( -- This is the 'Anchor' or starting point of the recursive query
				SELECT null Id,
				SearchFeatureId NextId,
				1 AS HLevel,
				CAST(ParentSearchFeatureId  AS nvarchar(max)) AS SortPath 
				FROM @FeatureList rel
				UNION ALL -- This is the recursive portion of the query
				SELECT rel.ParentSearchFeatureId Id,
				rel.SearchFeatureId NextId,
				Objects.HLevel + 1 AS HLevel,
				null AS SortPath 
				FROM ZnodeSearchFeature rel
				INNER JOIN Objects -- Note the reference to CTE table name (Recursive Join)
				ON rel.ParentSearchFeatureId = Objects.NextId-- OR ( rel.NextId= @id AND rel.Id!= @id )
				)
				select aa.SearchFeatureId,aa.ParentSearchFeatureId,aa.FeatureCode,aa.FeatureName,aa.IsAdvanceFeature,aa.ControlType,aa.HelpDescription,
				ss.SearchFeatureValue
				from ZnodeSearchFeature aa
				inner join Objects o on o.NextId =aa.SearchFeatureId
				left join  ZnodeSearchProfileFeatureMapping ss on ss.SearchFeatureId=aa.SearchFeatureId
				and SearchProfileId=@SearchProfileId 
				order by o.NextId,o.id 

 
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