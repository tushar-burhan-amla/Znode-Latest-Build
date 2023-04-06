	CREATE Procedure [dbo].[Znode_DeleteAllCatalog] (@IsAllCatalog bit = 0 ,@DeleteCatalogId NVARCHAR(MAX) = '',@IsDeleteFromPublish int)
	AS
	Begin
	      Declare @Status Bit
		  IF @IsAllCatalog  =1 
		  BEGIN
		     	SET @DeleteCatalogId = SUBSTRING((SELECT TOP 100 ','+CAST(PimCatalogId AS VARCHAR(50)) FROM ZnodePimCatalog FOR XML PATH('')), 2, 4000);
				--Remove extra products from catalog
				Select @DeleteCatalogId
				EXEC Znode_DeletePimCatalog @PimCatalogIds = @DeleteCatalogId, @IsDeleteFromPublish = @IsDeleteFromPublish;
		  END
		  Else
		  Begin
			EXEC Znode_DeletePimCatalog @PimCatalogIds = @DeleteCatalogId, @IsDeleteFromPublish = @IsDeleteFromPublish;
		  END			 
	End