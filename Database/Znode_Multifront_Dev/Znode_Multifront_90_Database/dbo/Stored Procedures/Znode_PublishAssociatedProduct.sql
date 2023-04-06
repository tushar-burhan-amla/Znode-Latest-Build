CREATE Procedure [dbo].[Znode_PublishAssociatedProduct]	
(
	@Status bit = 0 Out
)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
	BEGIN TRAN
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			delete TARGET from ZnodePublishAssociatedProduct TARGET
			where exists (select * from ZnodePublishAssociatedProductLog SOURCE1 where SOURCE1.PimCatalogId = TARGET.PimCatalogId )
			AND not exists(select * from ZnodePublishAssociatedProductLog SOURCE where (TARGET.PimProductId = SOURCE.PimProductId									   
				AND TARGET.PimCatalogId = SOURCE.PimCatalogId 									   
				AND TARGET.ParentPimProductId = SOURCE.ParentPimProductId						 
				AND TARGET.IsConfigurable = SOURCE.IsConfigurable 
				AND TARGET.IsBundle = SOURCE.IsBundle
				AND TARGET.IsGroup = SOURCE.IsGroup
				AND TARGET.IsAddOn = SOURCE.IsAddOn
				AND TARGET.IsLink = SOURCE.IsLink	
				AND TARGET.PublishStateId = SOURCE.PublishStateId))

			update TARGET 
			SET  TARGET.ModifiedBy = SOURCE.ModifiedBy 
				 ,TARGET.ModifiedDate = @GetDate,TARGET.IsDefault = SOURCE.IsDefault
			from ZnodePublishAssociatedProduct TARGET
			inner join ZnodePublishAssociatedProductLog SOURCE ON (TARGET.PimProductId = SOURCE.PimProductId									   
			AND TARGET.PimCatalogId = SOURCE.PimCatalogId 									   
			AND TARGET.ParentPimProductId = SOURCE.ParentPimProductId						 
			AND TARGET.IsConfigurable = SOURCE.IsConfigurable 
			AND TARGET.IsBundle = SOURCE.IsBundle
			AND TARGET.IsGroup = SOURCE.IsGroup
			AND TARGET.IsAddOn = SOURCE.IsAddOn
			AND TARGET.IsLink = SOURCE.IsLink	
			AND TARGET.PublishStateId = SOURCE.PublishStateId )

			insert into ZnodePublishAssociatedProduct(ParentPimProductId,PimProductId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
					,PimCatalogId,PublishStateId, DisplayOrder,IsDefault)
			select SOURCE.ParentPimProductId,SOURCE.PimProductId,SOURCE.IsConfigurable,SOURCE.IsBundle,SOURCE.IsGroup,SOURCE.IsAddOn,SOURCE.IsLink
					,SOURCE.CreatedBy ,@GetDate,SOURCE.ModifiedBy ,@GetDate ,SOURCE.PimCatalogId,SOURCE.PublishStateId, SOURCE.DisplayOrder, SOURCE.IsDefault
			from ZnodePublishAssociatedProductLog SOURCE
			where not exists(select * from ZnodePublishAssociatedProduct TARGET where (TARGET.PimProductId = SOURCE.PimProductId									   
			AND TARGET.PimCatalogId = SOURCE.PimCatalogId 									   
			AND TARGET.ParentPimProductId = SOURCE.ParentPimProductId						 
			AND TARGET.IsConfigurable = SOURCE.IsConfigurable 
			AND TARGET.IsBundle = SOURCE.IsBundle
			AND TARGET.IsGroup = SOURCE.IsGroup
			AND TARGET.IsAddOn = SOURCE.IsAddOn
			AND TARGET.IsLink = SOURCE.IsLink	
			AND TARGET.PublishStateId = SOURCE.PublishStateId
			AND SOURCE.PimCatalogId = TARGET.PimCatalogId  ))

			set @Status = 1
			select 1 Id,@Status [Status]

	COMMIT TRAN
	END TRY
	BEGIN CATCH
	set @Status = 0
	select 2 Id,@Status [Status]
		ROLLBACK TRAN

	END CATCH
end