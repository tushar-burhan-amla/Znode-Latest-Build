
CREATE PROCEDURE [dbo].[Znode_GetSearchProfileTriggerList]
(   @Keyword nvarchar(100) = '',
    @UserProfileId int = '',
	@PublishCatalogId int,
	@PortalId int 
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the catalog 
	 Unit Testig 
	 EXEC  Znode_GetSearchProfileTriggerList 'Apple',1,1,1
*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 
				 Declare @SearchProfileId int 

				 Select @SearchProfileId=d.SearchProfileId 
				 from [ZnodeSearchProfileTrigger] d
				 inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=d.SearchProfileId
				 Where d.Keyword=@Keyword
				 and d.UserProfileId=@UserProfileId
				 and c.PublishCatalogId=@PublishCatalogId

				 If isnull(@SearchProfileId,0)=0
				 Begin
					Select @SearchProfileId=a.SearchProfileId 
					from ZnodePortalSearchProfile a
					inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=a.SearchProfileId
					Where a.PortalId =@PortalId 
					and a.IsDefault=1
					and c.PublishCatalogId=@PublishCatalogId
				End 

				If isnull(@SearchProfileId,0)=0
				 Begin
					Select @SearchProfileId=min(a.SearchProfileId)
					from ZnodePortalSearchProfile a
					inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=a.SearchProfileId
					Where a.PortalId =@PortalId 
					and a.IsDefault=0
					and c.PublishCatalogId=@PublishCatalogId
				End 

				If isnull(@SearchProfileId,0)=0
				 Begin
					Select @SearchProfileId=a.SearchProfileId
					from ZnodeSearchProfile a
					inner join ZnodePublishCatalogSearchProfile c on c.SearchProfileId=a.SearchProfileId
					Where c.IsDefault=1
					and c.PublishCatalogId=@PublishCatalogId
				End 

				If Isnull(@SearchProfileId,0)>0
				exec [dbo].[Znode_GetSearchProfileDetails] @SearchProfileId=@SearchProfileId
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