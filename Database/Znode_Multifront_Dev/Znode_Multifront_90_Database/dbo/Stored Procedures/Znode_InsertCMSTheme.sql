CREATE PROCEDURE [dbo].[Znode_InsertCMSTheme]
( @Name   VARCHAR(200),
  @UserId INT)
AS
    /*-----------------------------------------------------------------------------
    --Summary:  Insert default theme with their asset into the table ZNodeCMSThemeAsset
    --			Insert all default asset except PDP type asset with PIMProductTypeId is null
    --          PIMProductTypeId will insert only with PDP type Asset for other will be null entry 
    --          Insert Widgets default data with new theme 
    --Unit Testing   
    --			Begin 
    --				Begin Transaction 
    --					Exec Znode_InsertCMSTheme    @Name = 'y',  @UserId  =1       
    --					SELECT * FROM ZnodeCMSTheme 
    --					SELECT * FROM ZNodeCMSThemeAsset
    --				Rollback Transaction 
    --			ENd  
    ----------------------------------------------------------------------------- 
	*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @CMSTheme TABLE
             (CMSThemeId INT,
              Name       VARCHAR(200)
             );
             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSTheme AS s
                 WHERE Name = @Name
             )
                 BEGIN
                     INSERT INTO @CMSTheme
                            SELECT 0,
                                   @Name;
                 END;
             ELSE
                 BEGIN
                     INSERT INTO ZnodeCMSTheme
                     (Name,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                     OUTPUT INSERTED.CMSThemeId,
                            inserted.Name
                            INTO @CMSTheme
                            SELECT @Name,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate;

                     --INSERT INTO ZNodeCMSThemeAsset 
                     --(
                     --	CMSThemeId,CMSAssetId,ProductTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
                     --)
                     --SELECT (SELECT TOP 1 CMSThemeId FROM @CMSTheme),ZCA.CMSAssetId,NULL,@UserId,@GetDate,@UserId,@GetDate
                     --FROM ZnodeCMSAsset ZCA inner join ZnodeCMSAssetType ZCAT ON   ZCAT.CMSAssetTypeId = ZCA.CMSAssetTypeId
                     --WHERE ZCA.IsDefault = 1   and ZCAT.AssetType <> 'PDP'
                     -- Retrive all product type id from ZnodePimAttributeDefaultValue table be using AttributeCode = 'ProductType'
                     -- insert it in table variable for cross join with Assettype PDP
                     --
                     DECLARE @TBL_ProductType TABLE(PimAttributeDefaultValueId INT);
                     INSERT INTO @TBL_ProductType
                            SELECT ZPADV.PimAttributeDefaultValueId
                            FROM ZnodePimAttributeDefaultValue AS ZPADV
                                 INNER JOIN ZnodePimAttribute AS ZPA ON ZPADV.PimAttributeId = ZPA.PimAttributeId
                            WHERE ZPA.AttributeCode = 'ProductType';

                     --INSERT INTO ZNodeCMSThemeAsset 
                     --(
                     --	CMSThemeId,CMSAssetId,ProductTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
                     --)
                     --SELECT (SELECT TOP 1 CMSThemeId FROM @CMSTheme),CMSAssetId,TPT.PimAttributeDefaultValueId,@UserId,@GetDate,@UserId,@GetDate
                     --FROM ZnodeCMSAssetType ZCAT INNER JOIN ZnodeCMSAsset ZCA on ZCAT.CMSAssetTypeId = ZCA.CMSAssetTypeId
                     --Cross join  @TBL_ProductType TPT
                     --WHERE ZCA.IsDefault = 1  and ZCAT.AssetType = 'PDP'
                     ---- Insert Widgets default data with new theme 
                     --Insert into ZnodeCMSThemeWidgets
                     --(
                     --	CMSThemeId,CMSWidgetsId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
                     --)
                     --Select (SELECT TOP 1 CMSThemeId FROM @CMSTheme), CMSWidgetsId ,@UserId,@GetDate,@UserId,@GetDate from ZnodeCMSWidgets


                 END;
             SELECT CMSThemeId,
                    NAME
             FROM @CMSTheme;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
             ROLLBACK TRAN;
         END CATCH;
     END;