CREATE PROCEDURE [dbo].[Znode_InsertCartLineItemsForLater]
(
	@TemplateLineItemXML XML,
    @UserId              INT,
    @Status              BIT = 0 OUT
)
AS 
/* 
	Summary :- This Procedure is Used to create the Quote Template 
    Unit Testing 
    EXEC 
    SELECT * FROM ZnodeOmsTemplate
    SELECT * FROM ZnodeOmsTemplateLineItem
*/
    BEGIN
        BEGIN TRAN InsertUpdateSaveCartLineItem;
        BEGIN TRY
            SET NOCOUNT ON;
			DECLARE @OldQty INT, @SKU NVARCHAR(600), @GetDate DATETIME = dbo.Fn_GetDate();

            DECLARE @TBL_SavecartLineitems TABLE
            (
				RowId							INT IDENTITY(1, 1),
				OmsTemplateLineItemId           INT,
				ParentOmsTemplateLineItemId     INT,
				OmsTemplateId                   INT,
				SKU                             NVARCHAR(600),
				Quantity                        NUMERIC(28, 6),
				OrderLineItemRelationshipTypeID INT,
				CustomText                      NVARCHAR(MAX),
				CartAddOnDetails                NVARCHAR(MAX),
				[Sequence]                      INT,
				AddOnValueIds                   VARCHAR(MAX),
				BundleProductIds                VARCHAR(MAX),
				ConfigurableProductIds          VARCHAR(MAX),
				GroupProductIds                 VARCHAR(MAX)
            );

            DECLARE @OrderLineItemRelationshipTypeIdAddon INT=
            (
                SELECT TOP 1 OrderLineItemRelationshipTypeId
                FROM ZnodeOmsOrderLineItemRelationshipType
                WHERE [Name] = 'AddOns'
            );

            DECLARE @OrderLineItemRelationshipTypeIdBundle INT=
            (
                SELECT TOP 1 OrderLineItemRelationshipTypeId
                FROM ZnodeOmsOrderLineItemRelationshipType
                WHERE [Name] = 'Bundles'
            );

            DECLARE @OrderLineItemRelationshipTypeIdConfigurable INT=
            (
                SELECT TOP 1 OrderLineItemRelationshipTypeId
                FROM ZnodeOmsOrderLineItemRelationshipType
                WHERE [Name] = 'Configurable'
            );

            DECLARE @OrderLineItemRelationshipTypeIdGroup INT=
            (
                SELECT TOP 1 OrderLineItemRelationshipTypeId
                FROM ZnodeOmsOrderLineItemRelationshipType
                WHERE [Name] = 'Group'
            );

            INSERT INTO @TBL_SavecartLineitems
            (
				OmsTemplateLineItemId,
				ParentOmsTemplateLineItemId,
				OmsTemplateId,
				SKU,
				Quantity,
				OrderLineItemRelationshipTypeID,
				CustomText,
				CartAddOnDetails,
				[Sequence],
				AddOnValueIds,
				BundleProductIds,
				ConfigurableProductIds,
				GroupProductIds
            )
            SELECT Tbl.Col.value('OmsTemplateLineItemId[1]', 'NVARCHAR(2000)') AS OmsSavedCartLineItemId,
				Tbl.Col.value('ParentOmsTemplateLineItemId[1]', 'NVARCHAR(2000)') AS ParentOmsSavedCartLineItemId,
				Tbl.Col.value('OmsTemplateId[1]', 'NVARCHAR(2000)') AS OmsSavedCartId,
				Tbl.Col.value('SKU[1]', 'NVARCHAR(2000)') AS SKU,
				Tbl.Col.value('Quantity[1]', 'NVARCHAR(2000)') AS Quantity,
				Tbl.Col.value('OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)') AS OrderLineItemRelationshipTypeID,
				Tbl.Col.value('CustomText[1]', 'NVARCHAR(2000)') AS CustomText,
				Tbl.Col.value('CartAddOnDetails[1]', 'NVARCHAR(2000)') AS CartAddOnDetails,
				Tbl.Col.value('Sequence[1]', 'NVARCHAR(2000)') AS Sequence,
				Tbl.Col.value('AddonProducts[1]', 'NVARCHAR(2000)') AS AddOnValueIds,
				Tbl.Col.value('BundleProducts[1]', 'NVARCHAR(2000)') AS BundleProductIds,
				Tbl.Col.value('ConfigurableProducts[1]', 'NVARCHAR(2000)') AS ConfigurableProductIds,
				Tbl.Col.value('GroupProducts[1]', 'NVARCHAR(Max)') AS GroupProductIds
            FROM @TemplateLineItemXML.nodes('//ArrayOfAccountTemplateLineItemModel/AccountTemplateLineItemModel') AS Tbl(Col);
             
            DECLARE @OmsTemplateId INT, @OmsTemplateLineItemId INT;

            DECLARE @TBL_bundleAddonRows TABLE
            (
				RowId                           INT,
				SequenceId                      INT IDENTITY(1, 1),
				ParentOmsTemplateLineItemId     INT,
				SKU                             NVARCHAR(1000),
				Quantity                        NUMERIC(28, 6),
				OrderLineItemRelationshipTypeID INT,
				CustomText                      NVARCHAR(MAX),
				CartAddOnDetails                NVARCHAR(MAX)
            );

            DECLARE @AddonProductSKU NVARCHAR(MAX)=
            (
                SELECT TOP 1 AddOnValueIds
                FROM @TBL_SavecartLineitems
            ), @BundleProductSKU NVARCHAR(MAX)=
            (
                SELECT TOP 1 BundleProductIds
                FROM @TBL_SavecartLineitems
            );
            SET @OmsTemplateId =
            (
                SELECT TOP 1 OmsTemplateId
                FROM @TBL_SavecartLineitems
            );

            SET @SKU =
            (
                SELECT TOP 1 SKU
                FROM @TBL_SavecartLineitems
            );
    --        IF EXISTS
    --        (
    --            SELECT TOP 1 1
    --            FROM ZnodeOmsSavedCartLineItem AS qa
    --            WHERE EXISTS
    --            (
    --                SELECT TOP 1 1
    --                FROM @TBL_SavecartLineitems AS ssds
    --                WHERE ssds.sku = qa.SKU
    --            )
    --        )
    --        BEGIN
				--SELECT @OldQty = Quantity FROM ZnodeOmsTemplateLineItem
    --            WHERE OmsTemplateId = @OmsTemplateId AND SKU =@SKU;

    --            DELETE A 
				--FROM ZnodeOmsTemplateLineItem A
    --            WHERE OmsTemplateId = @OmsTemplateId AND SKU =@SKU
				--	AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem 
				--					WHERE OmsTemplateId=A.OmsTemplateId AND ParentOmsTemplateLineItemId IS NOT NULL);
    --        END; 
			
            INSERT INTO @TBL_bundleAddonRows
            (
				RowId,
				ParentOmsTemplateLineItemId,
				SKU,
				Quantity,
				OrderLineItemRelationshipTypeID,
				CustomText,
				CartAddOnDetails
            )
            SELECT a.RowID,
                    NULL,
                    q.Item AS SKU,
                    a.Quantity,
                    @OrderLineItemRelationshipTypeIdAddon,
                    CustomText,
                    CartAddOnDetails
            FROM @TBL_SavecartLineitems AS a
            CROSS APPLY dbo.Split(a.AddOnValueIds, ',') AS q
            WHERE a.AddOnValueIds IS NOT NULL  AND a.AddOnValueIds <> '';  

            INSERT INTO @TBL_bundleAddonRows
            (
				RowId,
				ParentOmsTemplateLineItemId,
				SKU,
				Quantity,
				OrderLineItemRelationshipTypeID,
				CustomText,
				CartAddOnDetails
            )
			SELECT RowID,
				NULL,
				q.Item AS SKU,
				a.Quantity,
				@OrderLineItemRelationshipTypeIdBundle,
				CustomText,
				CartAddOnDetails
			FROM @TBL_SavecartLineitems AS a
			CROSS APPLY dbo.Split(a.BundleProductIds, ',') AS q
			WHERE a.BundleProductIds IS NOT NULL AND a.BundleProductIds <>'';
            
			INSERT INTO @TBL_bundleAddonRows
            (
				RowId,
				ParentOmsTemplateLineItemId,
				SKU,
				Quantity,
				OrderLineItemRelationshipTypeID,
				CustomText,
				CartAddOnDetails
            )
            SELECT RowID,
                NULL,
                q.Item AS SKU,
                a.Quantity,
                @OrderLineItemRelationshipTypeIdConfigurable,
                CustomText,
                CartAddOnDetails
            FROM @TBL_SavecartLineitems AS a
            CROSS APPLY dbo.Split(a.ConfigurableProductIds, ',') AS q
            WHERE a.ConfigurableProductIds IS NOT NULL AND a.ConfigurableProductIds <>'';
            
			INSERT INTO @TBL_bundleAddonRows
            (
				RowId,
				ParentOmsTemplateLineItemId,
				SKU,
				Quantity,
				OrderLineItemRelationshipTypeID,
				CustomText,
				CartAddOnDetails
            )
            SELECT RowID,
                NULL,
                SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) AS SKU,
                SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000) AS Quantity,
                @OrderLineItemRelationshipTypeIdGroup,
                CustomText,
                CartAddOnDetails
            FROM @TBL_SavecartLineitems AS a
            CROSS APPLY dbo.Split(a.GroupProductIds, ',') AS q
            WHERE a.GroupProductIds IS NOT NULL AND GroupProductIds <>''
			AND isnull(q.Item,'') <> ''; 

            DECLARE @Tbl_SAvecartIds TABLE
            (
				OmsTemplateLineItemId INT,
				RowId                 INT
            );
			 
			---Parent Product Update
			UPDATE TARGET
			SET TARGET.Quantity = SOURCE.Quantity,
				TARGET.ModifiedDate = @GetDate,
				TARGET.CreatedBy = @UserId 
			FROM ZnodeOmsTemplateLineItem TARGET
			INNER JOIN @TBL_SavecartLineitems SOURCE ON (TARGET.OmsTemplateId = SOURCE.OmsTemplateId
			AND TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId)
			AND TARGET.SKU = SOURCE.SKU
			WHERE ISNULL(SOURCE.OmsTemplateLineItemId,0) <> 0

			---Child Product Update
			UPDATE TARGET
			SET TARGET.Quantity = SCLI.Quantity,
				TARGET.ModifiedDate = @GetDate,
				TARGET.CreatedBy = @UserId 
			FROM ZnodeOmsTemplateLineItem TARGET
			INNER JOIN @TBL_SavecartLineitems SOURCE ON (TARGET.OmsTemplateId = SOURCE.OmsTemplateId
			    AND TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId)
			INNER JOIN @TBL_bundleAddonRows SCLI ON Source.RowId = SCLI.RowId and TARGET.SKU = SCLI.SKU	
			WHERE ISNULL(SOURCE.OmsTemplateLineItemId,0) <> 0


			---Merging the Parent Product quantity
			UPDATE TARGET
			SET TARGET.Quantity = TARGET.Quantity+SOURCE.Quantity,
				TARGET.ModifiedDate = @GetDate,
				TARGET.CreatedBy = @UserId 
			FROM ZnodeOmsTemplateLineItem TARGET
			INNER JOIN @TBL_SavecartLineitems SOURCE ON (TARGET.OmsTemplateId = SOURCE.OmsTemplateId
			AND TARGET.SKU = SOURCE.SKU)
			WHERE ISNULL(SOURCE.OmsTemplateLineItemId,0) = 0 AND TARGET.ParentOmsTemplateLineItemId IS NULL

			---Merging the Child Product quantity
			UPDATE TARGET
			SET TARGET.Quantity = TARGET.Quantity+SCLI.Quantity,
				TARGET.ModifiedDate = @GetDate,
				TARGET.CreatedBy = @UserId 
			FROM ZnodeOmsTemplateLineItem TARGET
			INNER JOIN @TBL_SavecartLineitems SOURCE ON (TARGET.OmsTemplateId = SOURCE.OmsTemplateId)
			INNER JOIN @TBL_bundleAddonRows SCLI ON Source.RowId = SCLI.RowId and TARGET.SKU = SCLI.SKU	
			WHERE ISNULL(SOURCE.OmsTemplateLineItemId,0) = 0 AND TARGET.ParentOmsTemplateLineItemId IS NOT NULL

			DECLARE @OmsTemplateLineItem TABLE (OmsTemplateLineItemId INT,SKU VARCHAR(600))
				
			INSERT INTO ZnodeOmsTemplateLineItem(ParentOmsTemplateLineItemId,OmsTemplateId,SKU,Quantity
				,OrderLineItemRelationshipTypeID,CustomText,CartAddOnDetails,[Sequence],CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)    
			OUTPUT inserted.OmsTemplateLineItemId,inserted.SKU INTO @OmsTemplateLineItem
			SELECT NULL,@OmsTemplateId,Source.SKU,Source.Quantity,-- + ISNULL(@OldQty, 0),
				CASE
				WHEN Source.OrderLineItemRelationshipTypeID = 0
				THEN NULL
				ELSE Source.OrderLineItemRelationshipTypeID
				END,
				Source.CustomText,
				Source.CartAddOnDetails,
				1 --Source.Sequence
				,@UserId,@GetDate,@UserId,@GetDate
			FROM @TBL_SavecartLineitems Source
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem TARGET 
			WHERE TARGET.OmsTemplateId = Source.OmsTemplateId 
			AND TARGET.SKU = SOURCE.SKU AND TARGET.ParentOmsTemplateLineItemId IS NULL)

			UPDATE @TBL_bundleAddonRows SET ParentOmsTemplateLineItemId=(SELECT TOP 1 OmsTemplateLineItemId FROM @OmsTemplateLineItem);

			INSERT INTO ZnodeOmsTemplateLineItem(ParentOmsTemplateLineItemId,OmsTemplateId,SKU,Quantity
				,OrderLineItemRelationshipTypeID, CustomText,CartAddOnDetails,Sequence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
			SELECT Source.ParentOmsTemplateLineItemId,@OmsTemplateId,Source.SKU,Source.Quantity,
				CASE
				WHEN Source.OrderLineItemRelationshipTypeID = 0
				THEN NULL
				ELSE Source.OrderLineItemRelationshipTypeID
				END,
				Source.CustomText,
				Source.CartAddOnDetails,
				Source.SequenceId+1
				,@UserId,@GetDate,@UserId,@GetDate
			FROM @TBL_bundleAddonRows Source
			WHERE Source.ParentOmsTemplateLineItemId IS NOT NULL
			--inner join @TBL_SavecartLineitems SCLI ON Source.RowId = SCLI.RowId
			--inner join ZnodeOmsTemplateLineItem ZOTLI ON ZOTLI.OmsTemplateId = @OmsTemplateId AND ZOTLI.ParentOmsTemplateLineItemId IS NULL AND ZOTLI.OrderLineItemRelationshipTypeID is null
			--AND SCLI.SKU = ZOTLI.SKU
			--WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem TARGET 
			--WHERE TARGET.OmsTemplateId = SCLI.OmsTemplateId 
			--AND TARGET.OmsTemplateLineItemId = SCLI.OmsTemplateLineItemId)
  			
            SET @Status = 1;
            COMMIT TRAN InsertUpdateSaveCartLineItem;
        END TRY
    BEGIN CATCH
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertCartLineItemsForLater @TemplateLineItemXML = '+CAST(@TemplateLineItemXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;
			
		ROLLBACK TRAN InsertUpdateSaveCartLineItem;

        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_InsertCartLineItemsForLater',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
    END CATCH;
END;