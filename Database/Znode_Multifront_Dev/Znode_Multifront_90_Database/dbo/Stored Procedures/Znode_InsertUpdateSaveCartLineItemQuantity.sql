CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemQuantity](
	  @CartLineItemXML xml, @UserId int,@Status bit OUT )
AS 
   /* 
    Summary: THis Procedure is USed to save and edit the saved cart line item      
    Unit Testing 
	begin tran  
    Exec Znode_InsertUpdateSaveCartLineItem @CartLineItemXML= '<ArrayOfSavedCartLineItemModel>
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
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @AddOnQuantity numeric(28, 6)= 0;
		DECLARE @IsAddProduct   BIT = 0 
		DECLARE @OmsSavedCartLineItemId INT = 0
		DECLARE @TBL_SavecartLineitems TABLE
		( 
			RowId int , OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
			CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute XML, 
			AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max),
			Custom1	nvarchar(max),Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4
			nvarchar(max),Custom5 nvarchar(max),GroupId NVARCHAR(max) ,ProductName Nvarchar(1000) , Description NVARCHAR(max),AddOnQuantity NVARCHAR(max), CustomUnitPrice numeric(28, 6)
		);

		DECLARE @OrderLineItemRelationshipTypeIdAddon int =
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'AddOns'
		);
		DECLARE @OrderLineItemRelationshipTypeIdSimple int =
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Simple'
		);
		DECLARE @OrderLineItemRelationshipTypeIdGroup int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Group'
		);
		DECLARE @OrderLineItemRelationshipTypeIdConfigurable int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Configurable'
		);
		 DECLARE @OrderLineItemRelationshipTypeIdBundle int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Bundles'
		);
		INSERT INTO @TBL_SavecartLineitems( RowId,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails,
		Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,Description,AddOnQuantity,CustomUnitPrice )
			   SELECT DENSE_RANK()Over(Order BY Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' )) RowId ,Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartLineItemId, Tbl.Col.value( 'ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsSavedCartLineItemId, Tbl.Col.value( 'OmsSavedCartId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity
			   , Tbl.Col.value( 'OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)' ) AS OrderLineItemRelationshipTypeID, Tbl.Col.value( 'CustomText[1]', 'NVARCHAR(2000)' ) AS CustomText, Tbl.Col.value( 'CartAddOnDetails[1]', 'NVARCHAR(2000)' ) AS CartAddOnDetails, Tbl.Col.value( 'Sequence[1]', 'NVARCHAR(2000)' ) AS Sequence, Tbl.Col.value( 'AddonProducts[1]', 'NVARCHAR(2000)' ) AS AddOnValueIds, ISNULL(Tbl.Col.value( 'BundleProducts[1]', 'NVARCHAR(2000)' ),'') AS BundleProductIds, ISNULL(Tbl.Col.value( 'ConfigurableProducts[1]', 'NVARCHAR(2000)' ),'') AS ConfigurableProductIds, ISNULL(Tbl.Col.value( 'GroupProducts[1]', 'NVARCHAR(Max)' ),'') AS GroupProductIds, 
			          Tbl.Col.query('(PersonaliseValuesDetail/node())') AS PersonaliseValuesDetail, Tbl.Col.value( 'AutoAddon[1]', 'NVARCHAR(Max)' ) AS AutoAddon, Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(Max)' ) AS OmsOrderId,
					  Tbl.Col.value( 'ItemDetails[1]', 'NVARCHAR(Max)' ) AS ItemDetails,
					  Tbl.Col.value( 'Custom1[1]', 'NVARCHAR(Max)' ) AS Custom1,
					  Tbl.Col.value( 'Custom2[1]', 'NVARCHAR(Max)' ) AS Custom2,
					  Tbl.Col.value( 'Custom3[1]', 'NVARCHAR(Max)' ) AS Custom3,
					  Tbl.Col.value( 'Custom4[1]', 'NVARCHAR(Max)' ) AS Custom4,
					  Tbl.Col.value( 'Custom5[1]', 'NVARCHAR(Max)' ) AS Custom5,
					  Tbl.Col.value( 'GroupId[1]', 'NVARCHAR(Max)' ) AS GroupId,
					  Tbl.Col.value( 'ProductName[1]', 'NVARCHAR(Max)' ) AS ProductName,
					  Tbl.Col.value( 'Description[1]', 'NVARCHAR(Max)' ) AS Description, 
					  Tbl.Col.value( 'AddOnQuantity[1]', 'NVARCHAR(2000)' ) AS AddOnQuantity,
					  Tbl.Col.value( 'CustomUnitPrice[1]', 'NVARCHAR(2000)' ) AS CustomUnitPrice
			   FROM @CartLineItemXML.nodes( '//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel' ) AS Tbl(Col);
			  

			  IF OBJECT_ID('tempdb..#TBL_SavecartLineitems') is not null
				drop table #TBL_SavecartLineitems

			 IF OBJECT_ID('tempdb..#OldValueForAddon') is not null
				drop table #OldValueForAddon

			  SELECT * INTO #TBL_SavecartLineitems FROM @TBL_SavecartLineitems
			

			UPDATE ZnodeOmsSavedCart
			SET ModifiedDate = @GetDate
			WHERE OmsSavedCartId = (SELECT TOP 1  OmsSavedCartId FROM @TBL_SavecartLineitems)
				

			  UPDATE  @TBL_SavecartLineitems
			  SET 	Description = ISNUll(Description,'') 

			IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' )
			 BEGIN 				
				 IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
				 BEGIN 
				    SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0 )

					UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0)
					, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					WHERE ( OmsSavedCartLineItemId = @OmsSavedCartLineItemId  
					OR ParentOmsSavedCartLineItemId =  @OmsSavedCartLineItemId   ) 
					 
					--UPDATE ZnodeOmsSavedCartLineItem 
					--SET Quantity = (SELECT TOP 1 AddOnQuantity FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0)
					--WHERE ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId  
					--AND OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					FROM ZnodeOmsSavedCartLineItem ZOSCLI
					INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
					WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					AND SCLI.BundleProductIds <> ''

					DELETE	FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0
				 END 
				  DECLARE @TBL_bundleProduct TT_SavecartLineitems 
				  INSERT INTO @TBL_bundleProduct 
				  SELECT *  
				  FROM @TBL_SavecartLineitems 
				  WHERE ISNULL(BundleProductIds,'') <> '' 
				
				  EXEC Znode_InsertUpdateSaveCartLineItemBundle @TBL_bundleProduct,@userId,@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdAddon
				 
				  DELETE FROM  @TBL_SavecartLineitems WHERE ISNULL(BundleProductIds,'') <> '' 
				  SET @OmsSavedCartLineItemId = 0 
				END 
			IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' )
			    BEGIN 				
				 IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
				 BEGIN 

				   SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0 )
				 
				   	UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId = @OmsSavedCartLineItemId )
					, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId
					
					--UPDATE ZnodeOmsSavedCartLineItem 
					--SET Quantity = (SELECT TOP 1 AddOnQuantity FROM @TBL_SavecartLineitems WHERE  ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0)
					--WHERE ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId  
					--AND OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = AddOnQuantity, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					FROM ZnodeOmsSavedCartLineItem ZOSCLI
					INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
					WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					AND SCLI.ConfigurableProductIds <> ''

					DELETE	FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0
				 END 
				  DECLARE @TBL_Configurable TT_SavecartLineitems 
				  INSERT INTO @TBL_Configurable 
				  SELECT *  
				  FROM @TBL_SavecartLineitems 
				  WHERE ISNULL(ConfigurableProductIds,'') <> '' 

				  
				  EXEC Znode_InsertUpdateSaveCartLineItemConfigurable @TBL_Configurable,@userId,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdAddon
				  
				  DELETE FROM @TBL_SavecartLineitems 
				  WHERE ISNULL(ConfigurableProductIds,'') <> ''
				  SET @OmsSavedCartLineItemId = 0  
				END 
				IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' )
			    BEGIN 				
				 IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
				 BEGIN 
				   SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0 )
				   	UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId = @OmsSavedCartLineItemId ), CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId
					
					--UPDATE ZnodeOmsSavedCartLineItem 
					--SET Quantity = (SELECT TOP 1 AddOnQuantity FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND  OmsSavedCartLineItemId <> 0)
					--WHERE ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId  
					--AND OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
					FROM ZnodeOmsSavedCartLineItem ZOSCLI
					INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
					WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					AND SCLI.GroupProductIds <> ''

					DELETE	FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0
				 END 
				  DECLARE @TBL_Group TT_SavecartLineitems 
				  INSERT INTO @TBL_Group 
				  SELECT *  
				  FROM @TBL_SavecartLineitems 
				  WHERE ISNULL(GroupProductIds,'') <> '' 

				
				  EXEC Znode_InsertUpdateSaveCartLineItemGroup @TBL_Group,@userId,@OrderLineItemRelationshipTypeIdGroup,@OrderLineItemRelationshipTypeIdAddon
				  
				  DELETE FROM @TBL_SavecartLineitems 
				  WHERE ISNULL(GroupProductIds,'') <> ''
				  SET @OmsSavedCartLineItemId = 0  
				END 
				 
                IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId <> 0  ) 
				 BEGIN 
				 
				   SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId <> 0 )
				   	UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId = @OmsSavedCartLineItemId )
					, ModifiedDate = @GetDate
					WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId
				
				 --   UPDATE ZnodeOmsSavedCartLineItem 
					--SET Quantity = (SELECT TOP 1 AddOnQuantity FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId <> 0)
					--WHERE ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId  
					--AND OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					UPDATE ZnodeOmsSavedCartLineItem 
					SET Quantity = AddOnQuantity, ModifiedDate = @GetDate
					FROM ZnodeOmsSavedCartLineItem ZOSCLI
					INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
					WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					
					DELETE	FROM @TBL_SavecartLineitems WHERE OmsSavedCartLineItemId <> 0
				 END 
			 
			

			  DECLARE @OmsInsertedData TABLE (OmsSavedCartLineItemId INT )
			  DECLARE @TBL_Personalise TABLE (OmsSavedCartLineItemId INT ,PersonalizeCode NVARCHAr(max),PersonalizeValue NVARCHAr(max),DesignId NVARCHAr(max), ThumbnailURL NVARCHAr(max))
			  INSERT INTO @TBL_Personalise
			  SELECT DISTINCT NULL 
							,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			  		  ,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
					  ,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
					  ,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			  FROM (SELECT TOP 1 PersonalisedAttribute Valuex FROM  @TBL_SavecartLineitems TRTR  ) a 
			  CROSS APPLY	a.Valuex.nodes( '//PersonaliseValueModel' ) AS Tbl(Col) 
			  
			   ----To update saved cart item personalise value from given line item
			  DECLARE @TBL_Personalise1 TABLE (OmsSavedCartLineItemId INT ,PersonalizeCode NVARCHAr(max),PersonalizeValue NVARCHAr(max),DesignId NVARCHAr(max), ThumbnailURL NVARCHAr(max))
			  INSERT INTO @TBL_Personalise1
			  SELECT DISTINCT a.OmsSavedCartLineItemId 
					  ,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			  		  ,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
					  ,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
					  ,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			  FROM (SELECT TOP 1 OmsSavedCartLineItemId,PersonalisedAttribute Valuex FROM  #TBL_SavecartLineitems TRTR ) a 
			  CROSS APPLY	a.Valuex.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  
		    
			
			  CREATE TABLE #tempoi (GenId INT IDENTITY(1,1),RowId	int	,OmsSavedCartLineItemId	int	 ,ParentOmsSavedCartLineItemId	int,OmsSavedCartId	int
									,SKU	nvarchar(max) ,Quantity	numeric(28,6)	,OrderLineItemRelationshipTypeID	int	,CustomText	nvarchar(max)
									,CartAddOnDetails	nvarchar(max),Sequence	int	,AutoAddon	varchar(max)	,OmsOrderId	int	,ItemDetails	nvarchar(max)
									,Custom1	nvarchar(max)  ,Custom2	nvarchar(max),Custom3	nvarchar(max),Custom4	nvarchar(max),Custom5	nvarchar(max)
									,GroupId	nvarchar(max) ,ProductName	nvarchar(max),Description	nvarchar(max),Id	int,ParentSKU NVARCHAR(max))
				   
			   INSERT INTO #tempoi
			   SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
					,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU 
			   FROM @TBL_SavecartLineitems a 
			   GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
					,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
			  
			   INSERT INTO #tempoi
			   SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
					,Quantity, @OrderLineItemRelationshipTypeIdSimple, CustomText, CartAddOnDetails, Sequence
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU 
			   FROM @TBL_SavecartLineitems  a 
			   WHERE ISNULL(BundleProductIds,'') =  '' 
			   AND  ISNULL(GroupProductIds,'') = ''	AND ISNULL(	ConfigurableProductIds,'') = ''
			   	   GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
					,Quantity,  CustomText, CartAddOnDetails, Sequence
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
			  
     		   INSERT INTO #tempoi
			   SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
					,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
					,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
					WHEN  GroupProductIds <> '' THEN GroupProductIds 
					WHEN BundleProductIds <> '' THEN BundleProductIds 
					 ELSE SKU END     ParentSKU 
			   FROM @TBL_SavecartLineitems  a 
			   WHERE AddOnValueIds <> ''
			   	   GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
					,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
					,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU
           --hack
		
		 CREATE TABLE #OldValue (OmsSavedCartId INT ,OmsSavedCartLineItemId INT,ParentOmsSavedCartLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
		 
		INSERT INTO #OldValue  
		SELECT  a.OmsSavedCartId,a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	  	FROM ZnodeOmsSavedCartLineItem a   
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId AND ISNULL(a.SKU,'') = ISNULL(TY.SKU,'')   )   
        AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple   

			

		INSERT INTO #OldValue 
		SELECT DISTINCT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsSavedCartLineItem b 
		INNER JOIN #OldValue c ON (c.ParentOmsSavedCartLineItemId  = b.OmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
		AND  b.OrderLineItemRelationshipTypeID IS NULL 
		 
		DELETE a FROM #OldValue a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldValue b WHERE b.ParentOmsSavedCartLineItemId IS NULL AND b.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)
		AND a.ParentOmsSavedCartLineItemId IS NOT NULL 
		
		------Merge Addon for same product
		SELECT * INTO #OldValueForAddon from #OldValue
		
		INSERT INTO #OldValue 
		SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsSavedCartLineItem b 
		INNER JOIN #OldValue c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		
		
		------Merge Addon for same product
		IF EXISTS(SELECT * FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'') <> '' )
		BEGIN

			INSERT INTO #OldValueForAddon 
			SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
			FROM ZnodeOmsSavedCartLineItem b 
			INNER JOIN #OldValueForAddon c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
			WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
			AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

			SELECT distinct SKU, STUFF(
										( SELECT  ', ' + SKU FROM    
											( SELECT DISTINCT SKU FROM     #OldValueForAddon b 
											  where a.OmsSavedCartLineItemId=b.ParentOmsSavedCartLineItemId and OrderLineItemRelationshipTypeID = 1 ) x 
											  FOR XML PATH('')
										), 1, 2, ''
									 ) AddOns
			INTO #AddOnsExists
			from #OldValueForAddon a where a.ParentOmsSavedCartLineItemId is not null and OrderLineItemRelationshipTypeID<>1

			SELECT distinct a.SKU, STUFF(
										 ( SELECT  ', ' + x.AddOnValueIds FROM    
											( SELECT DISTINCT b.AddOnValueIds FROM @TBL_SavecartLineitems b
											  where a.SKU=b.SKU ) x
											  FOR XML PATH('')
										 ), 1, 2, ''
									   ) AddOns
			INTO #AddOnAdded
			from @TBL_SavecartLineitems a

			if not exists(select * from #AddOnsExists a inner join #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
			begin
				delete from #OldValue
			end

		END

		IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_SavecartLineitems ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldValue a WHERE	
		ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN 
		
		DELETE FROM #OldValue 
		END 
		ELSE 
		BEGIN 
	    
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
		 BEGIN 
		 
		 DECLARE @parenTofAddon  TABLE( ParentOmsSavedCartLineItemId INT  )  
		 INSERT INTO  @parenTofAddon 
		 SELECT  ParentOmsSavedCartLineItemId FROM #OldValue WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  

		 DELETE FROM #OldValue WHERE OmsSavedCartLineItemId NOT IN (SELECT ParentOmsSavedCartLineItemId FROM  @parenTofAddon)   
					AND ParentOmsSavedCartLineItemId IS NOT NULL  
					AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

		 DELETE FROM #OldValue WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldValue m)
		 AND ParentOmsSavedCartLineItemId IS  NULL  
		 
		 END 
		 ELSE IF (SELECT COUNT (OmsSavedCartLineItemId) FROM #OldValue WHERE ParentOmsSavedCartLineItemId IS NULL ) > 1 
		 BEGIN 

		 -- SELECT 3
		    DECLARE @TBL_deleteParentOmsSavedCartLineItemId TABLE (OmsSavedCartLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsSavedCartLineItemId 
			SELECT ParentOmsSavedCartLineItemId
			FROM ZnodeOmsSavedCartLineItem a 
			WHERE ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM #OldValue WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple  )
			AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE FROM #OldValue WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
			OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
		    
			 DELETE FROM #OldValue WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldValue m)
		 AND ParentOmsSavedCartLineItemId IS  NULL  

		 END
		 ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldValue ty WHERE ty.OmsSavedCartId = wt.OmsSavedCartId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple AND wt.ParentOmsSavedCartLineItemId= ty.OmsSavedCartLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
		      AND EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  = '' )
		 BEGIN 

		   DELETE FROM #OldValue
		 END 
		END 
			
	

		DECLARE @TBL_Personaloldvalues TABLE (OmsSavedCartLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
		INSERT INTO @TBL_Personaloldvalues
		SELECT OmsSavedCartLineItemId , PersonalizeCode, PersonalizeValue
		FROM ZnodeOmsPersonalizeCartItem  a 
		WHERE EXISTS (SELECT TOP 1 1 FROM #OldValue TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
		

		IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		   AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
		BEGIN 
		 DELETE FROM #OldValue
		END 
		ELSE 
		BEGIN 
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldValue WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		 BEGIN 
		   
		   DELETE FROM #OldValue WHERE OmsSavedCartLineItemId IN (
		   SELECT OmsSavedCartLineItemId FROM #OldValue WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
		   AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ) ) 
		   OR OmsSavedCartLineItemId IN ( SELECT ParentOmsSavedCartLineItemId FROM #OldValue WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
		   AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ))
		   
		
		   
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldValue WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		 BEGIN 
		   
		   

		   DELETE n FROM #OldValue n WHERE OmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem WHERE n.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId  )
		   OR ParentOmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem   )
		   
		  
		   
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		        AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldValue YU WHERE YU.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId )) 
		       AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldValue WHERE ParentOmsSavedCartLineItemId IS nULL ) = 1
		 BEGIN 
		     DELETE FROM #OldValue WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		 END 


		  
		END 

		----delete old value from table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU not having personalise value for new cart item
		;with cte as
		(
			select distinct b.*
			FROM @TBL_SavecartLineitems a 
			Inner Join #OldValue b on ( a.SKU = b.sku)
			where isnull(cast(a.PersonalisedAttribute as varchar(max)),'') = ''
		)
		,cte2 as
		(
			select c.ParentOmsSavedCartLineItemId
			from #OldValue a
			inner join ZnodeOmsSavedCartLineItem c on a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId
			inner join ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
		)
		delete a from #OldValue a
		inner join cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		inner join cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		----delete old value from table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU having personalise value for new cart item
		;with cte as
		(
			select distinct b.*, 
			Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
			FROM @TBL_SavecartLineitems a 
			Inner Join #OldValue b on ( a.SKU = b.sku)
			CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  
			where isnull(cast(a.PersonalisedAttribute as varchar(max)),'') <> ''
		)
		,cte2 as
		(
			select a.ParentOmsSavedCartLineItemId, b.PersonalizeCode, b.PersonalizeValue
			from #OldValue a
			inner join ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
			where not exists(select * from cte c where b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and b.PersonalizeCode = c.PersonalizeCode 
			                 and b.PersonalizeValue = c.PersonalizeValue )
		)
		delete a from #OldValue a
		inner join cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		inner join cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		;with cte as
		(
			SELECT b.OmsSavedCartLineItemId ,b.ParentOmsSavedCartLineItemId , a.SKU as SKU
					,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			  		,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
					,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
					,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			FROM @TBL_SavecartLineitems a 
			Inner Join #OldValue b on a.SKU = b.SKU
			CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  
			Inner join ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		delete b1
		from #OldValue b1 
		where not exists(select * from cte c where (b1.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or b1.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId))
	    and exists(select * from cte)

		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is same for same line item then Quantity will added
		;with cte as
		(
			SELECT b.OmsSavedCartLineItemId ,a.ParentOmsSavedCartLineItemId , a.SKU
					,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			  		,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
					,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
					,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			FROM @TBL_SavecartLineitems a 
			Inner Join #OldValue b on a.SKU = b.SKU
			CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  
			Inner join ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		delete b1
		from cte a1		  
		Inner Join #OldValue b1 on a1.sku = b1.SKU
		where not exists(select * from ZnodeOmsPersonalizeCartItem c where a1.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and a1.PersonalizeValue = c.PersonalizeValue)

		IF EXISTS (SELECT TOP 1 1 FROM #OldValue )
		BEGIN 

		UPDATE a
		SET a.Quantity = a.Quantity+ty.Quantity,
		a.Custom1 = ty.Custom1,
		a.Custom2 = ty.Custom2,
		a.Custom3 = ty.Custom3,
		a.Custom4 = ty.Custom4,
		a.Custom5 = ty.Custom5, 
		a.ModifiedDate = @GetDate
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #OldValue b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN #tempoi ty ON (ty.SKU = b.SKU)


		END 
		ELSE 
		BEGIN 
		
		
			   
    SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU  
     INTO #yuuete   
     FROM  #tempoi  
     GROUP BY ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails ,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU   
     ORDER BY RowId, Id   
       	
			     
    DELETE FROM #yuuete WHERE Quantity <=0  
  
     ;WITH VTTY AS   
    (  
    SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU   
       FROM #yuuete m  
    INNER JOIN  #yuuete TY1 ON TY1.SKU = m.ParentSKU   
    WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon   
    )   
	
    UPDATE m1   
    SET m1.RowId = TYU.RowId  
    FROM #yuuete m1   
    INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
        
    ;WITH VTRET AS   
    (  
    SELECT RowId,id,Min(NewRowId) NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID  
    FROM #yuuete   
    GROUP BY RowId,id ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID
	Having  SKU = ParentSKU  AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdSimple
    )   
    
    DELETE FROM #yuuete WHERE NewRowId IN (SELECT NewRowId FROM VTRET)  
	
	

	
     
       INSERT INTO  ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description)  
       OUTPUT INSERTED.OmsSavedCartLineItemId  INTO @OmsInsertedData 
	   SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description   
       FROM  #yuuete  TH  

 
	 --;with Cte_newData AS   
  --  (  
    SELECT  MAX(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
	, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
	INTO #Cte_newData
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
    WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
	--	AND NOT EXISTS (SELECT TOP 1 1 FROM #OldValue TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		AND CASE WHEN EXISTS (SELECT TOP 1 1 FROM #yuuete TU WHERE TU.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple)  THEN ISNULL(a.OrderLineItemRelationshipTypeID,0) ELSE 0 END = 0 
     GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID
				
    --)   
	
  
    UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #Cte_newData  r  
    WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
    WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
    AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
    AND a.ParentOmsSavedCartLineItemId IS nULL   
  

    
    --;with Cte_newAddon AS   
    --(  
    SELECT a.OmsSavedCartLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(Order BY c.OmsSavedCartLineItemId )RowIdNo
    INTO #Cte_newAddon
	FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ( b.Id = 1  ))  
	INNER JOIN ZnodeOmsSavedCartLineItem c on b.sku = c.sku and b.OmsSavedCartId=c.OmsSavedCartId and b.Id = 1 
    WHERE ( ISNULL(a.ParentOmsSavedCartLineItemId,0) <> 0   )
    AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  and c.ParentOmsSavedCartLineItemId is null
  --  )   
  


  --  SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId  ) RowIdNo
	 --FROM ZnodeOmsSavedCartLineItem a
	 --WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
  --   AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
  --   AND a.ParentOmsSavedCartLineItemId IS NULL  
	 --AND EXISTS (SELECT TOP 1 1  FROM  #yuuete ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
	 --AND EXISTS (SELECT TOP 1 1 FROM #Cte_newAddon TI WHERE TI.SKU = a.SKU)



   ;with table_update AS 
   (
     SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId  ) RowIdNo
	 FROM ZnodeOmsSavedCartLineItem a
	 WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
     AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
     AND a.ParentOmsSavedCartLineItemId IS NULL  
	 AND EXISTS (SELECT TOP 1 1  FROM  #yuuete ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
	 AND EXISTS (SELECT TOP 1 1 FROM #Cte_newAddon TI WHERE TI.SKU = a.SKU)
   )

    UPDATE a SET  
   --SELECT  a.OmsSavedCartLineItemId,
	a.ParentOmsSavedCartLineItemId =(SELECT TOP 1 max(OmsSavedCartLineItemId) 
	FROM #Cte_newAddon  r  
    WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
    FROM table_update a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
    WHERE (SELECT TOP 1 max(OmsSavedCartLineItemId) 
	  FROM #Cte_newAddon  r  
    WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU   GROUP BY r.ParentSKU, r.SKU  )    IS NOT NULL 
	 

	
	  
    ;with Cte_Th AS   
    (             
      SELECT RowId    
     FROM #yuuete a   
     GROUP BY RowId   
     HAVING COUNT(NewRowId) <= 1   
      )   
    UPDATE a SET a.Quantity =  NULL , a.ModifiedDate = @GetDate  
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0)   
    WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
     AND a.OrderLineItemRelationshipTypeId IS NULL   
  
    UPDATE  ZnodeOmsSavedCartLineItem   
    SET GROUPID = NULL   
    WHERE  EXISTS (SELECT TOP 1 1  FROM #yuuete RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
    AND OrderLineItemRelationshipTypeId IS NOT NULL     
       ;With Cte_UpdateSequence AS   
     (  
       SELECT OmsSavedCartLineItemId ,Row_Number()Over(Order By OmsSavedCartLineItemId) RowId , Sequence   
       FROM ZnodeOmsSavedCartLineItem   
       WHERE EXISTS (SELECT TOP 1 1 FROM #yuuete TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
     )   
    UPDATE Cte_UpdateSequence  
    SET  Sequence = RowId  
	
	----To update saved cart item personalise value from given line item	
	if exists(select * from @TBL_Personalise1 where isnull(PersonalizeValue,'') <> '' and isnull(OmsSavedCartLineItemId,0) <> 0)
	Begin
		DELETE FROM ZnodeOmsPersonalizeCartItem 
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise1 yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )

		MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
		USING @TBL_Personalise1 SOURCE
			   ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
		WHEN NOT MATCHED THEN 
				INSERT  ( OmsSavedCartLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
				VALUES (  SOURCE.OmsSavedCartLineItemId,  @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
	end		
	
	UPDATE @TBL_Personalise
	SET OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
	FROM @OmsInsertedData a 
	INNER JOIN ZnodeOmsSavedCartLineItem b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
	WHERE b.ParentOmsSavedCartLineItemId IS NOT NULL 
	
	DELETE FROM ZnodeOmsPersonalizeCartItem 
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )
						
    MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
	USING @TBL_Personalise SOURCE
		   ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
	WHEN NOT MATCHED THEN 
		    INSERT  ( OmsSavedCartLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
							,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
			VALUES (  SOURCE.OmsSavedCartLineItemId,  @userId, @getdate, @userId, @getdate
							,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
  
		
		 
END 

	
	SET @Status = 1;
	COMMIT TRAN InsertUpdateSaveCartLineItem;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveCartLineItem @CartLineItemXML = '+CAST(@CartLineItemXML
 AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50))+',@Status='+CAST(@Status AS varchar(10));

		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN InsertUpdateSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveCartLineItem', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;