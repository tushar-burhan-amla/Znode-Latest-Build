CREATE PROCEDURE [dbo].[Znode_ImportInsertUpdatePimProduct]
(
    @PimProductDetail  PIMPRODUCTDETAIL READONLY,
    @UserId            INT       ,
    @status            BIT    OUT,
    @IsNotReturnOutput BIT    = 0,
	@CopyPimProductId  INT	  = 0 )
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
		Select * into #PimProductDetail from @PimProductDetail
			
		INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
		SELECT PimAttributeId,AttributeCode FROM  [dbo].[Fn_GetDefaultAttributeId] ()
			 
		INSERT INTO @TBL_MediaAttributeId (PimAttributeId,AttributeCode)
		SELECT PimAttributeId,AttributeCode FROM [dbo].[Fn_GetProductMediaAttributeId]()

		INSERT INTO @TBL_TextAreaAttributeId (PimAttributeId ,AttributeCode)
		SELECT PimAttributeId, AttributeCode   FROM [dbo].[Fn_GetTextAreaAttributeId]()

			 
		SELECT TOP 1 @PimAttributeFamilyId = PimAttributeFamilyId
        FROM #PimProductDetail;

		SELECT TOP 1 @LocaleId = LocaleId
        FROM #PimProductDetail;

        -- Retrive input productId from #PimProductDetail table ( having multiple attribute values with common productId) 

        SELECT TOP 1 @PimProductId = PimProductId
        FROM #PimProductDetail;
			
		DECLARE @PublishStateIdForDraft INT =  [dbo].[Fn_GetPublishStateIdForDraftState]()
		DECLARE @PublishStateIdForNotPublished INT = [dbo].[Fn_GetPublishStateIdForForNotPublishedState]()

        IF ISNULL(@PimProductId, 0) = 0
            BEGIN
                INSERT INTO ZnodePimProduct
                (PimAttributeFamilyId,
                CreatedBy,
                CreatedDate,
                ModifiedBy,
                ModifiedDate ,PublishStateId
                )
                    SELECT @PimAttributeFamilyId,
                            @UserId,
                            @GetDate,
                            @UserId,
                            @GetDate,@PublishStateIdForNotPublished ;
                SET @PimProductId = SCOPE_IDENTITY();

				UPDATE #PimProductDetail SET PimProductId = @PimProductId

				If EXISTS (select TOP 1 1 from #PimProductDetail where PimAttributeId = @PimIsDownlodableAttributeId and AttributeValue = 'true'  )
				Begin
						
				Select TOP 1 @SKU  =  AttributeValue from  #PimProductDetail where PimAttributeId =  @pimSkuAttributeId
				Select TOP 1 @ProductName  = AttributeValue from  #PimProductDetail where PimAttributeId =  @pimProductNameAttributeId
				insert into ZnodePimDownloadableProduct(SKU,ProductName,  CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				Select @SKU, @ProductName, @UserId , @GetDate, @UserId , @GetDate 
				End
            END;
        ELSE 
            BEGIN
                UPDATE ZNodePimProduct
                SET
                    PimAttributeFamilyId = @PimAttributeFamilyId,
					PublishStateId = @PublishStateIdForDraft,
                    ModifiedBy = @UserId,
                    ModifiedDate = @GetDate
                WHERE PimProductId = @PimProductId;

			Update ZnodePimProduct SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate  where PimProductId in
			(select PimProductId  From ZnodePimProductTypeAssociation where PimParentProductId=@PimProductId )
            									
				INSERT INTO @TBL_PimProductId(PimAttributeValueId)
				SELECT ZPAV.PimAttributeValueId
                FROM ZnodePimAttributeValue ZPAV
				INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId AND ( @localeID = @DefaultLocaleId OR ZPA.IsLocalizable = 1 OR EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetProductMediaAttributeId]() FN WHERE FN.PimAttributeId = ZPAV.PimAttributeId)))
				INNER JOIN ZnodePimFamilyGroupMapper ZPFGMI  ON (ZPFGMI.PimAttributeId = ZPAV.PimAttributeId AND ZPFGMI.PimAttributeFamilyId = @PimAttributeFamilyId)
				WHERE ZPAV.PimProductId = @PimProductId
				AND NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM #PimProductDetail TBPDI
                        WHERE TBPDI.PimAttributeId = ZPAV.PimAttributeId
                                AND TBPDI.PimProductId = ZPAV.PimProductId
						)

                DELETE FROM ZnodePimAttributeValueLocale
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PimProductId TBPD
                    WHERE TBPD.PimAttributeValueId = ZnodePimAttributeValueLocale.PimAttributeValueId 
								
                ) AND LocaleId = @LocaleId;
				DELETE  ZnodePimProductAttributeDefaultValue 
				WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PimProductId TBPD
                    WHERE TBPD.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId 
								
                ) AND LocaleId = @LocaleId;

				DELETE FROM ZnodePimProductAttributeMedia 
				WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PimProductId TBPD
                    WHERE TBPD.PimAttributeValueId = ZnodePimProductAttributeMedia.PimAttributeValueId 
								
                ) 
				AND LocaleId = @LocaleId;

				DELETE FROM ZnodePimProductAttributeTextAreaValue
				WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PimProductId TBPD
                    WHERE TBPD.PimAttributeValueId = ZnodePimProductAttributeTextAreaValue.PimAttributeValueId 
								
                ) AND LocaleId = @LocaleId ;

                DELETE FROM ZnodePimAttributeValue
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PimProductId TBPD
                    WHERE TBPD.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId
                )
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale ZPVD WHERE ZPVD.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId )
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeTextAreaValue ZPVD WHERE ZPVD.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId )
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPVD WHERE ZPVD.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId )
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPVD WHERE ZPVD.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId );

			If EXISTS (select TOP 1 1 from #PimProductDetail where PimAttributeId = @PimIsDownlodableAttributeId and AttributeValue = 'true'  )
				Begin
				Select TOP 1 @SKU  =  AttributeValue from  #PimProductDetail where PimAttributeId =  @pimSkuAttributeId
				Select TOP 1 @ProductName  = AttributeValue from  #PimProductDetail where PimAttributeId =  @pimProductNameAttributeId

				insert into ZnodePimDownloadableProduct(SKU,ProductName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				Select TOP 1 PD.AttributeValue, @ProductName,@UserId , @GetDate, @UserId , @GetDate from  #PimProductDetail PD where  PD.PimAttributeId = @pimSkuAttributeId 
				AND not exists (select top 1 1 from  ZnodePimDownloadableProduct where  ZnodePimDownloadableProduct.SKU  =  PD.AttributeValue)
				IF NOT Exists (	select top 1 1 from  ZnodePimDownloadableProduct where  ZnodePimDownloadableProduct.SKU  = @SKU)
					insert into ZnodePimDownloadableProduct(SKU,ProductName,  CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
					Select @SKU, @ProductName, @UserId , @GetDate, @UserId , @GetDate 

				End
            END;

		MERGE INTO ZnodePimAttributeValue TARGET
        USING #PimProductDetail SOURCE
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
                        --,AttributeValue
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
        		
		DECLARE @MediaData Table (MediaId INT , PimProductId INT , PimAttributeId INT ,PimAttributeFamilyId INT,LocaleId INT  )
	    
		INSERT INTO @MediaData (MediaId,PimProductId,PimAttributeId ,PimAttributeFamilyId, LocaleId)
		SELECT sp.item, a.PimProductId, a.PimAttributeId ,PimAttributeFamilyId,a.LocaleId
		FROM #PimProductDetail a
		CROSS APPLY dbo.split(a.AttributeValue,',' ) SP
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_MediaAttributeId c WHERE c.PimAttributeId = a.PimAttributeId ) 
				
				 
		INSERT INTO @TBL_MediaAttributeValue (PimAttributeValueId,LocaleId , AttributeValue,MediaId)
		SELECT a.PimAttributeValueId,
						b.LocaleId,
							zm.Path AttributeValue
							,ZM.MediaId
		FROM @ZnodePimAttributeValue AS a
		INNER JOIN  @MediaData AS b ON(a.PimAttributeId = b.PimAttributeId
												AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
				INNER JOIN ZnodeMedia ZM ON ( b.MediaId = ZM.MediaId )
		
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
		INNER JOIN #PimProductDetail AS b ON(a.PimAttributeId = b.PimAttributeId
												AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
		INNER JOIN @TBL_TextAreaAttributeId c ON ( c.PimAttributeId  = b.PimAttributeId )
		
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
			INNER JOIN #PimProductDetail AS b ON(a.PimAttributeId = b.PimAttributeId
												AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
		INNER JOIN @TBL_DefaultAttributeId c ON ( c.PimAttributeId  = b.PimAttributeId )
		INNER JOIN ZnodePimAttributeDefaultValue d ON (EXISTS (SELECT TOP 1 1 FROM dbo.split(b.AttributeValue,',') SP WHERE d.PimAttributeId = b.PimAttributeId AND SP.Item = d.AttributeDefaultValueCode))
	
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
                INNER JOIN #PimProductDetail AS b ON(a.PimAttributeId = b.PimAttributeId
                                                        AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0))
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
            FROM #PimProductDetail AS a
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
            FROM #PimProductDetail AS a
        );
        SET @ConfigureFamilyId =
        (
            SELECT MAX(ConfigureFamilyIds)
            FROM #PimProductDetail AS a
        );
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
		AND EXISTS (SELECT TOP 1 1 FROM #PimProductDetail n  WHERE n.ProductAttributeCode = b.AttributeCode  )
		OPEN Cur_AttributeDataUpdate 
		FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
		WHILE @@FETCH_STATUS = 0 
		BEGIN 

		SET @sqlt = 'UPDATE a  
		SET '+@AttributeCodeAtt+'= AttributeValue 
		FROM ZnodePimProduct a 
		INNER JOIN #PimProductDetail m ON(m.PimProductId = a.pimProductId ) 
		WHERE m.ProductAttributeCode = '''+@AttributeCodeAtt+'''
		' 

		EXEC (@sqlt)

		FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
		END 
		CLOSE Cur_AttributeDataUpdate
		DEALLOCATE Cur_AttributeDataUpdate 

END 

        COMMIT TRAN A;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE()
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInsertUpdatePimProduct @UserId = '+CAST(@UserId AS VARCHAR(50))+',@IsNotReturnOutput='+CAST(@IsNotReturnOutput AS VARCHAR(50))+',@CopyPimProductId='+CAST(@CopyPimProductId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	ROLLBACK TRAN A;
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportInsertUpdatePimProduct',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;