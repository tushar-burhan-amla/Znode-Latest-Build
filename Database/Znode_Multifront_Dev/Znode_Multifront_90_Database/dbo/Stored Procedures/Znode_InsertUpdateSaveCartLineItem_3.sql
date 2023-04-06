CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItem_3]
(   @CartLineItemXML XML,
	@UserId          INT,
	@Status          BIT OUT)
AS 
   /* 
    Summary: THis Procedure is USed to save and edit the saved cart line item      
    Unit Testing 
	begin tran  
    Exec Znode_InsertUpdateSaveCartLineItem_3 @CartLineItemXML= '<ArrayOfSavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>1259</OmsSavedCartId>
    <SKU>BlueGreenYellow</SKU>
    <Quantity>1.000000</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>1</Sequence>
    <AddonProducts>YELLOW</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts>GREEN</ConfigurableProducts>
  </SavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>1259</OmsSavedCartId>
    <SKU>ap1534</SKU>
    <Quantity>1.0</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>2</Sequence>
    <AddonProducts >PINK</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts />
    <PersonaliseValuesList>Address~Hello</PersonaliseValuesList>
  </SavedCartLineItemModel>
</ArrayOfSavedCartLineItemModel>' , @UserId=1 ,@Status=0
	rollback tran
*/
     BEGIN
         BEGIN TRAN InsertUpdateSaveCartLineItem;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			 DECLARE @AddOnQuantity NUMERIC( 28,6) = 0 
			 DECLARE @SaveCartLineItemIdForGroup INT  = 0 
             DECLARE @TBL_SavecartLineitems TABLE
             (RowId                           INT IDENTITY(1, 1),
              OmsSavedCartLineItemId          INT,
              ParentOmsSavedCartLineItemId    INT,
              OmsSavedCartId                  INT,
              SKU                             NVARCHAR(600),
              Quantity                        NUMERIC(28, 6),
              OrderLineItemRelationshipTypeID INT,
              CustomText                      NVARCHAR(MAX),
              CartAddOnDetails                NVARCHAR(MAX),
              Sequence                        INT,
              AddOnValueIds                   VARCHAR(MAX),
              BundleProductIds                VARCHAR(MAX),
              ConfigurableProductIds          VARCHAR(MAX),
              GroupProductIds                 VARCHAR(MAX),
              PersonalisedAttribute           VARCHAR(MAX)
             );
		
	  DECLARE @TBL_OrderLineItemRelationshipTypeId TABLE (OrderLineItemRelationshipTypeId INT, NAME VARCHAR(200))
	  INSERT INTO @TBL_OrderLineItemRelationshipTypeId
	  SELECT  OrderLineItemRelationshipTypeId, Name
                 FROM ZnodeOmsOrderLineItemRelationshipType
                 WHERE [Name] IN ('AddOns','Bundles','Configurable','Group') 

			
-------------------------------------------------------------------
      /*       DECLARE @OrderLineItemRelationshipTypeIdAddon INT=
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

			 */
             INSERT INTO @TBL_SavecartLineitems 
             (OmsSavedCartLineItemId,
              ParentOmsSavedCartLineItemId,
              OmsSavedCartId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails,
              Sequence,
              AddOnValueIds,
              BundleProductIds,
              ConfigurableProductIds,
              GroupProductIds,
              PersonalisedAttribute
             )
                    SELECT Tbl.Col.value('OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)') AS OmsSavedCartLineItemId,
                           Tbl.Col.value('ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)') AS ParentOmsSavedCartLineItemId,
                           Tbl.Col.value('OmsSavedCartId[1]', 'NVARCHAR(2000)') AS OmsSavedCartId,
                           Tbl.Col.value('SKU[1]', 'NVARCHAR(2000)') AS SKU,
                           Tbl.Col.value('Quantity[1]', 'NVARCHAR(2000)') AS Quantity,
                           Tbl.Col.value('OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)') AS OrderLineItemRelationshipTypeID,
                           Tbl.Col.value('CustomText[1]', 'NVARCHAR(2000)') AS CustomText,
                           Tbl.Col.value('CartAddOnDetails[1]', 'NVARCHAR(2000)') AS CartAddOnDetails,
                           Tbl.Col.value('Sequence[1]', 'NVARCHAR(2000)') AS Sequence,
                           Tbl.Col.value('AddonProducts[1]', 'NVARCHAR(2000)') AS AddOnValueIds,
                           Tbl.Col.value('BundleProducts[1]', 'NVARCHAR(2000)') AS BundleProductIds,
                           Tbl.Col.value('ConfigurableProducts[1]', 'NVARCHAR(2000)') AS ConfigurableProductIds,
                           Tbl.Col.value('GroupProducts[1]', 'NVARCHAR(Max)') AS GroupProductIds,
                           Tbl.Col.value('PersonaliseValuesList[1]', 'NVARCHAR(Max)') AS GroupProductIds
                    FROM @CartLineItemXML.nodes('//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel') AS Tbl(Col);
		   
		     DECLARE @OmsSavedCartId INT
					,@OmsSavedCartLineItemId INT;

-----------------------------------------------------------------------------------------------
				DECLARE @AddonProductSKU TABLE (AddOnValue NVARCHAR(100))
				INSERT INTO @AddonProductSKU
				SELECT  AddOnValueIds
				FROM @TBL_SavecartLineitems WHERE AddOnValueIds NOT LIKE ''


				DECLARE @BundleProductSKU TABLE (BundleProduct NVARCHAR(100))
             
			     INSERT INTO @BundleProductSKU
                 SELECT BundleProductIds
                 FROM @TBL_SavecartLineitems WHERE BundleProductIds NOT LIKE ''
             
			 DECLARE @ConfigurableProductSKU TABLE (ConfigurableProduct NVARCHAR(100))
             
			     INSERT INTO @ConfigurableProductSKU
                 SELECT ConfigurableProductIds
                 FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds NOT LIKE ''

				DECLARE @GroupProductSKU TABLE (GroupProduct NVARCHAR(100))
             
			     INSERT INTO @GroupProductSKU
                 SELECT GroupProductIds
                 FROM @TBL_SavecartLineitems WHERE GroupProductIds NOT LIKE ''


				 SELECT * FROM @BundleProductSKU				
				 SELECT * FROM @ConfigurableProductSKU
				  SELECT * FROM  @AddonProductSKU


----------------------------------------------------------------------------------------------------

			 DECLARE @TBL_bundleAddonRows TABLE
             (RowId                           INT,
              SequenceId                      INT IDENTITY(1, 1),
              ParentOmsSavedCartLineItemId    INT,
              SKU                             NVARCHAR(1000),
              Quantity                        NUMERIC(28, 6),
              OrderLineItemRelationshipTypeID INT,
              CustomText                      NVARCHAR(MAX),
              CartAddOnDetails                NVARCHAR(MAX)
             );
            
             SET @OmsSavedCartId =
             (
                 SELECT TOP 1 OmsSavedCartId
                 FROM @TBL_SavecartLineitems
             );
             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeOmsSavedCartLineItem AS qa
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_SavecartLineitems AS ssds
                     WHERE ssds.sku = qa.SKU
                 )
             )
                 BEGIN
                     DELETE FROM ZnodeOmsPersonalizeCartItem
                     WHERE EXISTS
                     (
                         SELECT TOP 1 1
                         FROM ZnodeOmsSavedCartLineItem
                         WHERE OmsSavedCartId = @OmsSavedCartId
                               AND OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId
                     );
                     DELETE FROM ZnodeOmsSavedCartLineItem
                     WHERE OmsSavedCartId = @OmsSavedCartId;
                 END; 
		
     /*    INSERT INTO @TBL_bundleAddonRows
             (RowId,
              ParentOmsSavedCartLineItemId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails
             )
                    SELECT RowID,
                           NULL,
                           q.BundleProduct AS SKU,
                           a.Quantity,
                          ( SELECT OrderLineItemRelationshipTypeId FROM @TBL_OrderLineItemRelationshipTypeId WHERE NAME = 'Bundles'),
                           CustomText,
                           CartAddOnDetails
                    FROM @TBL_SavecartLineitems AS a  
					INNER JOIN @BundleProductSKU q ON (a.BundleProductIds = q.BundleProduct)                  
                    WHERE a.BundleProductIds IS NOT NULL ; 
          
		 INSERT INTO @TBL_bundleAddonRows
             (RowId,
              ParentOmsSavedCartLineItemId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails
             )
                    SELECT RowID,
                           NULL,
                           q.ConfigurableProduct AS SKU,
                           a.Quantity,
                            ( SELECT OrderLineItemRelationshipTypeId FROM @TBL_OrderLineItemRelationshipTypeId WHERE NAME = 'Configurable'),
                           CustomText,
                           CartAddOnDetails
                    FROM @TBL_SavecartLineitems AS a
                     INNER JOIN @ConfigurableProductSKU q ON (a.ConfigurableProductIds = q.ConfigurableProduct)    
                    WHERE a.ConfigurableProductIds IS NOT NULL;
           
			 INSERT INTO @TBL_bundleAddonRows
             (RowId,
              ParentOmsSavedCartLineItemId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails
             )
                    SELECT RowID,
                           NULL,
                           SUBSTRING(q.GroupProduct, 1, CHARINDEX('~', q.GroupProduct)-1) AS SKU,
                           SUBSTRING(q.GroupProduct, CHARINDEX('~', q.GroupProduct)+1, 4000) AS Quantity,
                           ( SELECT OrderLineItemRelationshipTypeId FROM @TBL_OrderLineItemRelationshipTypeId WHERE NAME = 'Group'),
                           CustomText,
                           CartAddOnDetails
                    FROM @TBL_SavecartLineitems AS a
                      INNER JOIN @GroupProductSKU q ON (a.GroupProductIds = q.GroupProduct)    
                    WHERE a.GroupProductIds IS NOT NULL ; 


            IF  EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE  GroupProductIds IS NOT NULL OR ConfigurableProductIds IS NOT NULL   )
			BEGIN  
			 SET @AddOnQuantity = (SELECT MAX (Quantity) FROM @TBL_bundleAddonRows )
			END 
			*/

		  INSERT INTO @TBL_bundleAddonRows
             (RowId,
              ParentOmsSavedCartLineItemId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails
             )
                    SELECT a.RowID,
                           NULL,
                          q.AddOnValue AS SKU,
                           CASE WHEN @AddOnQuantity = 0 THEN a.Quantity ELSE @AddOnQuantity END   ,
                         ( SELECT OrderLineItemRelationshipTypeId FROM @TBL_OrderLineItemRelationshipTypeId WHERE NAME = 'AddOns'),
                           CustomText,
                           CartAddOnDetails
                    FROM @TBL_SavecartLineitems AS a
                        INNER JOIN @AddonProductSKU q ON (a.AddOnValueIds = q.AddOnValue)
                    WHERE a.AddOnValueIds IS NOT NULL; 

					select * from @TBL_bundleAddonRows


             DECLARE @Tbl_SAvecartIds TABLE
             (OmsSavedCartLineItemId INT,
			 SKU					 NVARCHAR(max)  ,
              RowId                  INT
             );
             MERGE INTO ZnodeOmsSavedCartLineItem TARGET
             USING @TBL_SavecartLineitems SOURCE
             ON 1 = 0
                 WHEN NOT MATCHED
                 THEN INSERT(ParentOmsSavedCartLineItemId,
                             OmsSavedCartId,
                             SKU,
                             Quantity,
                             OrderLineItemRelationshipTypeID,
                             CustomText,
                             CartAddOnDetails,
                             Sequence,
                             CreatedBy,
                             CreatedDate,
                             ModifiedBy,
                             ModifiedDate) VALUES
             (NULL,
              @OmsSavedCartId,
              Source.SKU,
              Source.Quantity,
              CASE
                  WHEN Source.OrderLineItemRelationshipTypeID = 0
                  THEN NULL
                  ELSE OrderLineItemRelationshipTypeID
              END,
              Source.CustomText,
              Source.CartAddOnDetails,
              Source.Sequence,
              @UserId,
              @GetDate,
              @UserId,
              @GetDate
             )
             OUTPUT Inserted.OmsSavedCartLineItemId,Source.SKU,
                    SOURCE.RowID
                    INTO @Tbl_SAvecartIds; 
          
             INSERT INTO ZnodeOmsSavedCartLineItem
             (ParentOmsSavedCartLineItemId,
              OmsSavedCartId,
              SKU,
              Quantity,
              OrderLineItemRelationshipTypeID,
              CustomText,
              CartAddOnDetails,
              Sequence,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
                    SELECT b.OmsSavedCartLineItemId,
                           @OmsSavedCartId,
                           a.SKU,
                           Quantity,
                           CASE
                               WHEN OrderLineItemRelationshipTypeID = 0
                               THEN NULL
                               ELSE OrderLineItemRelationshipTypeID
                           END,
                           CustomText,
                           CartAddOnDetails,
                           SequenceId,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate
                    FROM @TBL_bundleAddonRows AS a
                         INNER JOIN @Tbl_SAvecartIds AS b ON(a.RowId = b.RowId)
                    WHERE a.SKU IS NOT NULL
                          AND a.SKU <> '';

		    IF  EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE  GroupProductIds IS NOT NULL OR ConfigurableProductIds IS NOT NULL  )
			BEGIN   
			 SET @SaveCartLineItemIdForGroup = (SELECT TOP 1 ZOSCL.OmsSavedCartLineItemId 
													FROM ZnodeOmsSavedCartLineItem ZOSCL 
													INNER JOIN @TBL_bundleAddonRows TBBR ON (ZOSCL.SKU = TBBR.SKU )
													INNER JOIN @TBL_SavecartLineitems TBSCRT ON (TBSCRT.OmsSavedCartId = ZOSCL.OmsSavedCartId)
													WHERE  TBSCRT.GroupProductIds IS NOT NULL OR TBSCRT.ConfigurableProductIds IS NOT NULL    )
			END 


             INSERT INTO ZnodeOmsPersonalizeCartItem
             (OmsSavedCartLineItemId,
              PersonalizeCode,
              PersonalizeValue,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
                    SELECT CASE WHEN ISNULL(b.OmsSavedCartLineItemId,0) = 0 THEN @SaveCartLineItemIdForGroup ELSE b.OmsSavedCartLineItemId END  ,
                           SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) AS Keyi,
                           SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000) AS Value,
                           0,
                           @GetDate,
                           0,
                           @GetDate
                    FROM @TBL_SavecartLineitems AS a
                         LEFT JOIN @Tbl_SAvecartIds b ON(b.RowId = a.RowId AND b.SKU = a.SKU)
                         CROSS APPLY dbo.Split(a.PersonalisedAttribute, ',') AS q
                    WHERE a.PersonalisedAttribute IS NOT NULL;  

         
             SET @Status = 1;
             COMMIT TRAN InsertUpdateSaveCartLineItem;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateSaveCartLineItem_3 @CartLineItemXML = '+CAST(@CartLineItemXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN InsertUpdateSaveCartLineItem;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdateSaveCartLineItem_3',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;