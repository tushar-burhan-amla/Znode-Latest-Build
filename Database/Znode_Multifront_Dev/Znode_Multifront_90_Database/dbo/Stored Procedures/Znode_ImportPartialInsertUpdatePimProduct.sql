CREATE PROCEDURE [dbo].[Znode_ImportPartialInsertUpdatePimProduct]
(
    @PimProductDetail  PIMPRODUCTDETAIL READONLY,
    @UserId            INT       ,
    @status            BIT    OUT,
    @IsNotReturnOutput BIT    = 0,
	@CopyPimProductId  INT	  = 0 
)
AS
/*
     Summary : To Insert / Update single Product with multiple attribute values 
     Update Logic: 
*/
BEGIN
    BEGIN TRAN A;
    BEGIN TRY
		DECLARE @PimProductId INT;
		DECLARE @TBL_PimProductId TABLE(PimAttributeValueId INT,ZnodePimAttributeValueLocaleId INT );
		DECLARE @TBL_CopyPimProductId TABLE(PimAttributeValueId INT,OldPimAttributeValueId INT);
		DECLARE @PimDefaultFamily INT= dbo.Fn_GetDefaultPimProductFamilyId()
		DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @TBL_DefaultAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
		DECLARE @TBL_MediaAttributeId TABLE (PimAttributeId INT PRIMARY KEY, AttributeCode VARCHAR(600))
		DECLARE @TBL_TextAreaAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
		DECLARE @TBL_MediaAttributeValue TABLE (PimAttributeValueId INT ,LocaleId INT ,AttributeValue VARCHAr(300),MediaId INT)
		DECLARE @TBL_DefaultAttributeValue TABLE (PimAttributeValueId INT , LocaleId INT , AttributeValue INT)
		DECLARE @ZnodePimAttributeValue TABLE (PimAttributeValueId  INT, PimAttributeFamilyId INT,PimAttributeId INT);

		DECLARE @AssociatedProduct VARCHAR(4000);
		DECLARE @ConfigureAttributeId VARCHAR(4000);
		DECLARE @ConfigureFamilyId VARCHAR(4000);
		DECLARE @PimAttributeFamilyId INT;
		DECLARE @LocaleId INT 

		DECLARE @pimSkuAttributeId VARCHAR(50) = [dbo].[Fn_GetProductSKUAttributeId] ()
		DECLARE @pimProductNameAttributeId VARCHAR(50) =[dbo].Fn_GetProductNameAttributeId ()
		DECLARE @PimIsDownlodableAttributeId VARCHAR(50) = [dbo].[Fn_GetIsDownloadableAttributeId]()
		Declare @SKU nvarchar(300),@ProductName nvarchar(300)
		Select * into ##PimProductData from @PimProductDetail
		--DECLARE @PimAttributeFamily VARCHAR(50) =  [dbo].[Fn_GetAttributeFamilyId]()
		--Update ##PimProductData SET AttributeValue = 
		--(SELECT FamilyCode from ZnodePimAttributeFamily where PimAttributeFamilyId = @PimAttributeFamilyId)
		--where PimAttributeId = @PimAttributeFamily

		--DECLARE @PimAttributeIsPublish VARCHAR(50) =  [dbo].[Fn_GetAttributeIsPublish]()
			 
		--insert into ##PimProductData ([PimAttributeId],[PimAttributeFamilyId],[ProductAttributeCode],[ProductAttributeDefaultValueId],
		--[PimAttributeValueId],	[LocaleId],[PimProductId],[AttributeValue],[AssociatedProducts],[ConfigureAttributeIds],[ConfigureFamilyIds]) 
			 
		--SELECT TOP 1 @PimAttributeIsPublish,[PimAttributeFamilyId],'PublishStatus' ProductAttributeCode,NULL ProductAttributeDefaultValueId,
		--NULL PimAttributeValueId,	[LocaleId],[PimProductId],
		--CASE when isnull([PimProductId] ,0) > 1 then 'Draft' else 'Not Publish' END AttributeValue,
		--[AssociatedProducts],[ConfigureAttributeIds],[ConfigureFamilyIds]
		--from @PimProductDetail  

		INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
		SELECT PimAttributeId,AttributeCode FROM  [dbo].[Fn_GetDefaultAttributeId] ()
			 
		INSERT INTO @TBL_MediaAttributeId (PimAttributeId,AttributeCode)
		SELECT PimAttributeId,AttributeCode FROM [dbo].[Fn_GetProductMediaAttributeId]()

		INSERT INTO @TBL_TextAreaAttributeId (PimAttributeId ,AttributeCode)
		SELECT PimAttributeId, AttributeCode   FROM [dbo].[Fn_GetTextAreaAttributeId]()

			 
		SELECT TOP 1 @PimAttributeFamilyId = PimAttributeFamilyId
        FROM ##PimProductData;
             			 
			 
		SELECT TOP 1 @LocaleId = LocaleId
        FROM ##PimProductData;

        -- Retrive input productId from ##PimProductData table ( having multiple attribute values with common productId) 
		DECLARE @PublishStateIdForDraft INT =  [dbo].[Fn_GetPublishStateIdForDraftState]()

        SELECT TOP 1 @PimProductId = PimProductId
        FROM ##PimProductData;
			
        IF ISNULL(@PimProductId, 0) = 0
        BEGIN
            INSERT INTO ZnodePimProduct
            (PimAttributeFamilyId,
            CreatedBy,
            CreatedDate,
            ModifiedBy,
            ModifiedDate
            )
            SELECT @PimAttributeFamilyId,
                    @UserId,
                    @GetDate,
                    @UserId,
                    @GetDate;

            SET @PimProductId = SCOPE_IDENTITY();

			If EXISTS (select TOP 1 1 from ##PimProductData where PimAttributeId = @PimIsDownlodableAttributeId and AttributeValue = 'true'  )
			Begin		
				Select TOP 1 @SKU  =  AttributeValue from  ##PimProductData where PimAttributeId =  @pimSkuAttributeId
				Select TOP 1 @ProductName  = AttributeValue from  ##PimProductData where PimAttributeId =  @pimProductNameAttributeId
				insert into ZnodePimDownloadableProduct(SKU,ProductName,  CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				Select @SKU, @ProductName, @UserId , @GetDate, @UserId , @GetDate 
			End

        END;
        ELSE 
        BEGIN
            UPDATE ZNodePimProduct
            SET
                PublishStateId = @PublishStateIdForDraft, 
                ModifiedBy = @UserId,
                ModifiedDate = @GetDate
            WHERE PimProductId = @PimProductId;
            									
			INSERT INTO @TBL_PimProductId(PimAttributeValueId)
			SELECT ZPAV.PimAttributeValueId
            FROM ZnodePimAttributeValue ZPAV
			INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId AND ( @localeID = @DefaultLocaleId OR ZPA.IsLocalizable = 1 OR EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetProductMediaAttributeId]() FN WHERE FN.PimAttributeId = ZPAV.PimAttributeId)))
			--INNER JOIN ZnodePimFamilyGroupMapper ZPFGMI  ON (ZPFGMI.PimAttributeId = ZPAV.PimAttributeId AND ZPFGMI.PimAttributeFamilyId = @PimAttributeFamilyId)
			WHERE ZPAV.PimProductId = @PimProductId
			AND NOT EXISTS
			(
				SELECT TOP 1 1
				FROM ##PimProductData TBPDI
				WHERE TBPDI.PimAttributeId = ZPAV.PimAttributeId
					AND TBPDI.PimProductId = ZPAV.PimProductId
			)

			If EXISTS (select TOP 1 1 from ##PimProductData where PimAttributeId = @PimIsDownlodableAttributeId and AttributeValue = 'true'  )
			Begin
				Select TOP 1 @SKU  =  AttributeValue from  ##PimProductData where PimAttributeId =  @pimSkuAttributeId

				Select TOP 1 @ProductName  = AttributeValue from  ##PimProductData where PimAttributeId =  @pimProductNameAttributeId

				insert into ZnodePimDownloadableProduct(SKU,ProductName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				Select TOP 1 PD.AttributeValue, @ProductName,@UserId , @GetDate, @UserId , @GetDate from ##PimProductData P
				where  PD.PimAttributeId = @pimSkuAttributeId 
					AND not exists (select top 1 1 from  ZnodePimDownloadableProduct where  ZnodePimDownloadableProduct.SKU  =  PD.AttributeValue)

				IF NOT Exists (	select top 1 1 from  ZnodePimDownloadableProduct where  ZnodePimDownloadableProduct.SKU  = @SKU)
				insert into ZnodePimDownloadableProduct (SKU,ProductName,  CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				Select @SKU, @ProductName, @UserId , @GetDate, @UserId , @GetDate 
			End
		END;
		
		MERGE INTO ZnodePimAttributeValue TARGET
        USING ##PimProductData SOURCE
        ON(
		TARGET.PimProductId = @PimProductId
        AND TARGET.PimAttributeId = SOURCE.PimAttributeId)
            WHEN MATCHED
            THEN UPDATE SET
                            TARGET.PimAttributeFamilyId = CASE
                                                            WHEN Source.PimAttributeFamilyId = 0
                                                            THEN NULL
                                                            ELSE Source.PimAttributeFamilyId
                                                        END,
                            TARGET.CreatedBy = @UserId,
                            TARGET.CreatedDate = @GetDate,
                            TARGET.ModifiedBy = @UserId,
                            TARGET.ModifiedDate = @GetDate
            WHEN NOT MATCHED
            THEN INSERT(PimAttributeFamilyId,
                        PimProductId,
                        PimAttributeId,
                        PimAttributeDefaultValueId,                            
                        CreatedBy,
                        CreatedDate,
                        ModifiedBy,
                        ModifiedDate) VALUES
        (CASE
            WHEN Source.PimAttributeFamilyId = 0
            THEN @PimDefaultFamily
            ELSE Source.PimAttributeFamilyId
        END,
        @PimProductId,
        SOURCE.PimAttributeId,
        CASE
            WHEN SOURCE.ProductAttributeDefaultValueId = 0
            THEN NULL
            ELSE SOURCE.ProductAttributeDefaultValueId
        END,              
        @UserId,
        @GetDate,
        @UserId,
        @GetDate
        )
             
        OUTPUT INSERTED.PimAttributeValueId,
            INSERTED.PimAttributeFamilyId,
            INSERTED.PimAttributeId
            INTO @ZnodePimAttributeValue;
        		 
		INSERT INTO @TBL_MediaAttributeValue (PimAttributeValueId,LocaleId , AttributeValue,MediaId)
		SELECT a.PimAttributeValueId,
                b.LocaleId,
                    zm.Path AttributeValue
					,ZM.MediaId
		FROM @ZnodePimAttributeValue AS a
		INNER JOIN ##PimProductData AS b ON(a.PimAttributeId = b.PimAttributeId)
												--AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
		INNER JOIN @TBL_MediaAttributeId c ON ( c.PimAttributeId  = b.PimAttributeId )
		INNER JOIN ZnodeMedia ZM ON (EXISTS (SELECT TOP 1 1 FROM dbo.split(b.AttributeValue ,',') SP WHERE sp.Item = ZM.MediaId ))
		
		DELETE FROM ZnodePimProductAttributeMedia 
		WHERE EXISTS 
			(SELECT TOP 1 1 FROM @TBL_MediaAttributeValue TBLM WHERE ZnodePimProductAttributeMedia.PimAttributeValueId = TBLM.PimAttributeValueId 
			AND TBLM.MediaId <> ZnodePimProductAttributeMedia.MediaId  AND ZnodePimProductAttributeMedia.Localeid = @LocaleId)

		MERGE INTO ZnodePimProductAttributeMedia TARGET 
		USING @TBL_MediaAttributeValue SOURCE 
		ON (        TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
				AND TARGET.MediaPAth = SOURCE.AttributeValue
					AND TARGET.LocaleId = SOURCE.LocaleId)
		WHEN MATCHED THEN 
		UPDATE SET
					TARGET.MediaPath = SOURCE.AttributeValue,
					TARGET.MediaId   = SOURCE.MediaId,
                    TARGET.CreatedBy = @UserId,
                    TARGET.CreatedDate = @GetDate,
                    TARGET.ModifiedBy = @UserId,
                    TARGET.ModifiedDate = @GetDate
            WHEN NOT MATCHED
            THEN 
		INSERT(PimAttributeValueId,
							LocaleId,
							MediaPath,
							MediaId ,
							CreatedBy,
							CreatedDate,
							ModifiedBy,
							ModifiedDate) 
		VALUES
			(SOURCE.PimAttributeValueId,
			SOURCE.LocaleId,
			SOURCE.AttributeValue,
			SOURCE.MediaId,
			@UserId,
			@GetDate,
			@UserId,
			@GetDate
			);
		
		;With Cte_TextAreaAttributeValue AS 
			(
		SELECT a.PimAttributeValueId,
						b.LocaleId,
						AttributeValue
		FROM @ZnodePimAttributeValue AS a
		INNER JOIN ##PimProductData AS b ON(a.PimAttributeId = b.PimAttributeId)
												--AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
		INNER JOIN @TBL_TextAreaAttributeId c ON ( c.PimAttributeId  = b.PimAttributeId)
		)
		
		MERGE INTO ZnodePimProductAttributeTextAreaValue TARGET 
		USING Cte_TextAreaAttributeValue SOURCE 
		ON (TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
				AND TARGET.LocaleId = SOURCE.LocaleId)
		WHEN MATCHED THEN 
		UPDATE SET
					TARGET.AttributeValue = SOURCE.AttributeValue,
					TARGET.CreatedBy = @UserId,
					TARGET.CreatedDate = @GetDate,
					TARGET.ModifiedBy = @UserId,
					TARGET.ModifiedDate = @GetDate
            WHEN NOT MATCHED
            THEN 
		INSERT(PimAttributeValueId,
							LocaleId,
							AttributeValue,
							CreatedBy,
							CreatedDate,
							ModifiedBy,
							ModifiedDate) 
		VALUES
			(SOURCE.PimAttributeValueId,
			SOURCE.LocaleId,
			SOURCE.AttributeValue,
			@UserId,
			@GetDate,
			@UserId,
			@GetDate
			);


		INSERT INTO @TBL_DefaultAttributeValue (PimAttributeValueId,LocaleId,AttributeValue)  
		SELECT a.PimAttributeValueId,
						b.LocaleId,
						d.PimAttributeDefaultValueId  AttributeValue
		FROM @ZnodePimAttributeValue AS a
		INNER JOIN ##PimProductData AS b ON(a.PimAttributeId = b.PimAttributeId)
		INNER JOIN @TBL_DefaultAttributeId c ON ( c.PimAttributeId  = b.PimAttributeId )
		INNER JOIN ZnodePimAttributeDefaultValue d ON (EXISTS (SELECT TOP 1 1 FROM dbo.split(b.AttributeValue,',') SP WHERE d.PimAttributeId = b.PimAttributeId AND ltrim(rtrim(SP.Item ))= ltrim(rtrim(d.AttributeDefaultValueCode))))    
	    
		DELETE FROM ZnodePimProductAttributeDefaultValue 
		WHERE  EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeValue TBLAV WHERE TBLAV.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId 
												AND TBLAV.AttributeValue   <> ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId 
													AND ZnodePimProductAttributeDefaultValue.LocaleId = @LocaleId )

		MERGE INTO ZnodePimProductAttributeDefaultValue TARGET 
		USING @TBL_DefaultAttributeValue SOURCE 
		ON (TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
				AND TARGET.PimAttributeDefaultValueId =  SOURCE.AttributeValue
				AND TARGET.LocaleId = SOURCE.LocaleId)
		WHEN MATCHED THEN 
		UPDATE SET
					TARGET.PimAttributeDefaultValueId = SOURCE.AttributeValue,
					TARGET.CreatedBy = @UserId,
					TARGET.CreatedDate = @GetDate,
					TARGET.ModifiedBy = @UserId,
					TARGET.ModifiedDate = @GetDate
					WHEN NOT MATCHED
					THEN 
		INSERT(PimAttributeValueId,
							LocaleId,
							PimAttributeDefaultValueId,
							CreatedBy,
							CreatedDate,
							ModifiedBy,
							ModifiedDate) 
		VALUES
			(SOURCE.PimAttributeValueId,
			SOURCE.LocaleId,
			SOURCE.AttributeValue,
			@UserId,
			@GetDate,
			@UserId,
			@GetDate
			);
			 
		MERGE INTO ZnodePimAttributeValueLocale TARGET
        USING
        (
            SELECT a.PimAttributeValueId,
                b.LocaleId,
                AttributeValue
            FROM @ZnodePimAttributeValue AS a
                INNER JOIN ##PimProductData AS b ON(a.PimAttributeId = b.PimAttributeId)
                                                        --AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
            WHERE NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBLDA WHERE TBLDA.PimAttributeId = b.PimAttributeId  )
			AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_MediaAttributeId TBLMA WHERE TBLMA.PimAttributeId = b.PimAttributeId  )
			AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_TextAreaAttributeId TBLTA WHERE TBLTA.PimAttributeId = b.PimAttributeId  )
		) SOURCE
        ON(TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
        AND TARGET.LocaleId = SOURCE.LocaleId)
            WHEN MATCHED
            THEN UPDATE SET
                            TARGET.AttributeValue = SOURCE.AttributeValue,
                            TARGET.CreatedBy = @UserId,
                            TARGET.CreatedDate = @GetDate,
                            TARGET.ModifiedBy = @UserId,
                            TARGET.ModifiedDate = @GetDate
            WHEN NOT MATCHED
            THEN INSERT(PimAttributeValueId,
                        LocaleId,
                        AttributeValue,
                        CreatedBy,
                        CreatedDate,
                        ModifiedBy,
                        ModifiedDate) VALUES
        (SOURCE.PimAttributeValueId,
        SOURCE.LocaleId,
        SOURCE.AttributeValue,
        @UserId,
        @GetDate,
        @UserId,
        @GetDate
        );
        SET @AssociatedProduct =
        (
            SELECT MAX(AssociatedProducts)
            FROM ##PimProductData AS a
        );
        INSERT INTO ZnodePimProductTypeAssociation
        (PimParentProductId,
        PimProductId,
        DisplayOrder,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate
        )
        SELECT @PimProductId,
                Item,
                ID AS RowId,
                @UserId,
                @GetDate,
                @UserId,
                @GetDate
        FROM dbo.Split(@AssociatedProduct, ',') AS b
                INNER JOIN ZNodePimProduct AS q ON(q.PimProductId = b.Item)
				WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductTypeAssociation PPT
				WHERE PPT.PimParentProductId = @PimProductId AND PPT.PimProductId = b.Item)

        SET @ConfigureAttributeId =
        (
            SELECT MAX(ConfigureAttributeIds)
            FROM ##PimProductData AS a
        );
        SET @ConfigureFamilyId =
        (
            SELECT MAX(ConfigureFamilyIds)
            FROM ##PimProductData AS a
        );
	
		IF EXISTS (SELECT TOP  1 1 FROM @PimProductDetail P INNER JOIN ZnodePimAttribute PA ON P.PimAttributeId=PA.PimAttributeId
				INNER JOIN ZnodeAttributeType c ON PA.AttributeTypeId=c.AttributeTypeId 
				WHERE c.AttributeTypeName = 'link' AND ISNULL(P.AttributeValue,'')<>'')
		BEGIN
			DECLARE @PimAttributeId INT =(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'SKU');

			SELECT ZPAV.PimProductId, ZPAVL.AttributeValue as SKU
			INTO #ProductSKU
			FROM ##PimProductData tt
			INNER JOIN ZnodePimAttributeValue ZPAV ON ZPAV.PimProductId=tt.PimProductId AND ZPAV.PimAttributeId=tt.PimAttributeId
			INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
			WHERE ZPAV.PimAttributeId=@PimAttributeId;

			--To get the product SKU in temp table
			--SELECT ZPAV.PimProductId, ZPAVL.AttributeValue as SKU
			--INTO #ProductSKU
			--FROM ZnodePimAttributeValue ZPAV
			--INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId 
			--WHERE EXISTS(SELECT * FROM ZnodePimAttribute ZPA WHERE ZPAV.PimAttributeId = zpa.PimAttributeId AND ZPA.AttributeCode = 'SKU')

			--Inserting the link attribute product association
			INSERT INTO ZnodePimLinkProductDetail (PimParentProductId,PimProductId,PimAttributeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder)  
			SELECT a.PimProductId, f.PimProductId, b.PimAttributeId,@UserId,@GetDate,@UserId,@GetDate,1 displayOrder  
			FROM @PimProductDetail a 
			CROSS APPLY DBO.SPLIT(a.AttributeValue,',') s
			INNER JOIN ZnodePimAttribute b ON (b.PimAttributeId = a.PimAttributeId )  
			INNER JOIN ZnodeAttributeType c ON (c.AttributeTypeId = b.AttributeTypeId)  
			INNER JOIN #ProductSKU f ON (f.SKU = s.Item)  
			WHERE c.AttributeTypeName = 'link'  
				AND a.PimProductId != f.PimProductId  
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimLinkProductDetail ER WHERE ER.PimParentProductId = a.PimProductId AND ER.PimProductId = f.PimProductId AND ER.PimAttributeId = b.PimAttributeId)  
		END  

        INSERT INTO [ZnodePimConfigureProductAttribute]
        (PimProductId,
        PimFamilyId,
        PimAttributeId,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate
        )
        SELECT @PimProductId,
                @ConfigureFamilyId,
                q.PimAttributeId,
                @UserId,
                @GetDate,
                @UserId,
                @GetDate
        FROM dbo.Split(@ConfigureAttributeId, ',') AS b
                INNER JOIN ZnodePimAttribute AS q ON(q.PimAttributeId = b.Item)
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute RTR  WHERE  RTR.PimProductId = @PimProductId AND RTR.PimAttributeId = q.PimAttributeId);

        IF @IsNotReturnOutput = 0
            SELECT @PimProductId AS Id,
                CAST(1 AS BIT) AS Status;
        SET @status = 1;

		IF @CopyPimProductId > 0 
		BEGIN 
			INSERT INTO ZnodePimAttributeValueLocale  (PimAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT ZPAVI.PimAttributeValueId,ZPAVL.LocaleId,ZPAVL.AttributeValue,@UserId,@GetDate,@UserId,@GetDate
			FROM ZnodePimAttributeValueLocale ZPAVL 
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId )
			INNER JOIN ZnodePimAttributeValue ZPAVI ON (ZPAVI.PimAttributeId = ZPAV.PimAttributeId AND ZPAVI.PimProductId = @PimProductId )
			WHERE ZPAVL.LocaleId <> dbo.Fn_GetDefaultLocaleId()
			AND ZPAV.PimProductId = @CopyPimProductId

			INSERT INTO ZnodePimProductAttributeDefaultValue  (PimAttributeValueId,LocaleId,PimAttributeDefaultValueId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT ZPAVI.PimAttributeValueId,ZPAVL.LocaleId,ZPAVL.PimAttributeDefaultValueId,@UserId,@GetDate,@UserId,@GetDate
			FROM ZnodePimProductAttributeDefaultValue ZPAVL 
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId )
			INNER JOIN ZnodePimAttributeValue ZPAVI ON (ZPAVI.PimAttributeId = ZPAV.PimAttributeId AND ZPAVI.PimProductId = @PimProductId )
			WHERE ZPAVL.LocaleId <> dbo.Fn_GetDefaultLocaleId()
			AND ZPAV.PimProductId = @CopyPimProductId

			INSERT INTO ZnodePimProductAttributeTextAreaValue  (PimAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT ZPAVI.PimAttributeValueId,ZPAVL.LocaleId,ZPAVL.AttributeValue,@UserId,@GetDate,@UserId,@GetDate
			FROM ZnodePimProductAttributeTextAreaValue ZPAVL 
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId )
			INNER JOIN ZnodePimAttributeValue ZPAVI ON (ZPAVI.PimAttributeId = ZPAV.PimAttributeId AND ZPAVI.PimProductId = @PimProductId )
			WHERE ZPAVL.LocaleId <> dbo.Fn_GetDefaultLocaleId()
			AND ZPAV.PimProductId = @CopyPimProductId
			   			   
			INSERT INTO ZnodePimProductAttributeMedia  (PimAttributeValueId,LocaleId,MediaPath,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT ZPAVI.PimAttributeValueId,ZPAVL.LocaleId,ZPAVL.MediaPath,@UserId,@GetDate,@UserId,@GetDate
			FROM ZnodePimProductAttributeMedia ZPAVL 
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId )
			INNER JOIN ZnodePimAttributeValue ZPAVI ON (ZPAVI.PimAttributeId = ZPAV.PimAttributeId AND ZPAVI.PimProductId = @PimProductId )
			WHERE ZPAVL.LocaleId <> dbo.Fn_GetDefaultLocaleId()
			AND ZPAV.PimProductId = @CopyPimProductId
		END 

		IF @LocaleId = @DefaultLocaleId
		BEGIN
			DECLARE @sqlt NVARCHAr(max) = ''
			DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr int 

			DECLARE Cur_AttributeDataUpdate CURSOR FOR 

			SELECT b.AttributeCode , PimAttributeId 
			FROM INFORMATION_SCHEMA.COLUMNS a 
			INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )
			WHERE TABLE_NAME = 'ZnodePimProduct'
			AND IsCategory = 0 
			AND IsShowOnGrid = 1 
			AND EXISTS (SELECT TOP 1 1 FROM ##PimProductData n  WHERE n.ProductAttributeCode = b.AttributeCode  )
			OPEN Cur_AttributeDataUpdate 
			FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
			WHILE @@FETCH_STATUS = 0 
			BEGIN 

			SET @sqlt = 'UPDATE a  
			SET '+@AttributeCodeAtt+'= AttributeValue 
			FROM ZnodePimProduct a 
			INNER JOIN ##PimProductData m ON(m.PimProductId = a.pimProductId ) 
			WHERE m.ProductAttributeCode = '''+@AttributeCodeAtt+'''
			' 

			EXEC (@sqlt)

			FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
			END 
			CLOSE Cur_AttributeDataUpdate
			DEALLOCATE Cur_AttributeDataUpdate
		END 

		If Object_id ('Tempdb..##PimProductData')  is not null 
		DROP TABLE Tempdb..##PimProductData

        COMMIT TRAN A;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE()
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPartialInsertUpdatePimProduct @UserId = '+CAST(@UserId AS VARCHAR(50))+',@IsNotReturnOutput='+CAST(@IsNotReturnOutput AS VARCHAR(50))+',@CopyPimProductId='+CAST(@CopyPimProductId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		ROLLBACK TRAN A;
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportPartialInsertUpdatePimProduct',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
    END CATCH;
END;
