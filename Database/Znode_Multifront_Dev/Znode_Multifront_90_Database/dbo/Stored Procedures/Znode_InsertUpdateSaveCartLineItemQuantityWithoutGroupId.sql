


CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemQuantityWithoutGroupId]   
(  
 @CartLineItemXML xml, @UserId int, @Status bit OUT  
)  
AS   
BEGIN   
 SET NOCOUNT ON  
 BEGIN TRY   
 DECLARE @GetDate datetime= dbo.Fn_GetDate();  
  DECLARE @AddOnQuantity numeric(28, 6)= 0;  
  DECLARE @IsAddProduct   BIT = 0 

  DECLARE @TBL_SavecartLineitems TABLE  
  (   
   RowId int , OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max),   
   CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute XML,   
   AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max),  
   Custom1 nvarchar(max),Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4  
   nvarchar(max),Custom5 nvarchar(max),GroupId NVARCHAR(max) ,ProductName Nvarchar(1000) , Description NVARCHAR(max)  
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
  DECLARE @OrderLineItemRelationshipTypeIdBundle int=  
  (  
   SELECT TOP 1 OrderLineItemRelationshipTypeId  
   FROM ZnodeOmsOrderLineItemRelationshipType  
   WHERE [Name] = 'Bundles'  
  );  
  DECLARE @OrderLineItemRelationshipTypeIdConfigurable int=  
  (  
   SELECT TOP 1 OrderLineItemRelationshipTypeId  
   FROM ZnodeOmsOrderLineItemRelationshipType  
   WHERE [Name] = 'Configurable'  
  );  
  DECLARE @OrderLineItemRelationshipTypeIdGroup int=  
  (  
   SELECT TOP 1 OrderLineItemRelationshipTypeId  
   FROM ZnodeOmsOrderLineItemRelationshipType  
   WHERE [Name] = 'Group'  
  );  
  
   INSERT INTO @TBL_SavecartLineitems( RowId,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails,  
  Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,Description )  
      SELECT Row_NUMBER()Over(Order BY Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' )) RowId ,Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartLineItemId, Tbl.Col.value( 'ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsSavedCartLineItemId, Tbl.Col.value( 'OmsSavedCartId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity  
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
       Tbl.Col.value( 'Description[1]', 'NVARCHAR(Max)' ) AS Description  
      FROM @CartLineItemXML.nodes( '//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel' ) AS Tbl(Col);  
  
          CREATE TABLE #tempoi (GenId INT IDENTITY(1,1),RowId int ,OmsSavedCartLineItemId int  ,ParentOmsSavedCartLineItemId int,OmsSavedCartId int  
         ,SKU nvarchar(max) ,Quantity numeric(28,6) ,OrderLineItemRelationshipTypeID int ,CustomText nvarchar(max)  
         ,CartAddOnDetails nvarchar(max),Sequence int ,AutoAddon varchar(max) ,OmsOrderId int ,ItemDetails nvarchar(max)  
         ,Custom1 nvarchar(max)  ,Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4 nvarchar(max),Custom5 nvarchar(max)  
         ,GroupId nvarchar(max) ,ProductName nvarchar(max),Description nvarchar(max),Id int,ParentSKU NVARCHAR(max),GroupProducts NVARCHAr(max),ConfigurableProducts NVARCHAr(max))  
         
      INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU,GroupProducts,ConfigurableProducts)  
      SELECT  Min(RowId)RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description ,0 Id,NULL ParentSKU,GroupProductIds ,ConfigurableProductIds  
      FROM @TBL_SavecartLineitems a   
      GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName ,GroupProductIds ,ConfigurableProductIds  
       
      INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU)  
      SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, @OrderLineItemRelationshipTypeIdSimple, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description ,1 Id,SKU ParentSKU   
      FROM @TBL_SavecartLineitems  a   
      WHERE ISNULL(BundleProductIds,'') =  ''   
      AND  ISNULL(GroupProductIds,'') = '' AND ISNULL( ConfigurableProductIds,'') = ''  
          GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity,  CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName  
       
      INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU)  
      SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, ConfigurableProductIds  
     ,Quantity, @OrderLineItemRelationshipTypeIdConfigurable, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description ,1 Id,SKU ParentSKU   
      FROM @TBL_SavecartLineitems  a   
      WHERE ConfigurableProductIds <> ''  
          GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, ConfigurableProductIds  
     ,Quantity,  CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName ,SKU   
      INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU)  
      SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, GroupProductIds  
     ,Quantity, @OrderLineItemRelationshipTypeIdGroup, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description ,1 Id ,SKU ParentSKU   
      FROM @TBL_SavecartLineitems  a   
      WHERE GroupProductIds <> ''  
          GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, GroupProductIds  
     ,Quantity,  CustomText, CartAddOnDetails, Sequence  
        ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName ,SKU   
     INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU)   
      SELECT   Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, BundleProductIds  
     ,Quantity, @OrderLineItemRelationshipTypeIdBundle, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description ,1 Id,SKU ParentSKU    
      FROM @TBL_SavecartLineitems  a   
      WHERE BundleProductIds <> ''  
          GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, BundleProductIds  
     ,Quantity,  CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU  
      INSERT INTO #tempoi (RowId,OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU  
     ,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId,ProductName,Description,Id,ParentSKU)  
      SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds  
     ,Quantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description ,1 Id   
     ,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds  
     WHEN  GroupProductIds <> '' THEN GroupProductIds   
     WHEN BundleProductIds <> '' THEN BundleProductIds   
      ELSE SKU END     ParentSKU   
      FROM @TBL_SavecartLineitems  a   
      WHERE AddOnValueIds <> ''  
      GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds  
     ,Quantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds, BundleProductIds  
     ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU  
         
         
    SELECT a.OmsSavedCartLineItemId, a.SKU , b.SKU ParentSKU ,a.OmsSavedCartId ,b.GroupId,a.OrderLineItemRelationshipTypeID ,a.ParentOmsSavedCartLineItemId , CAST('' AS NVARCHAr(2000)) AddOnSKU 
    INTO #OldValue   
    FROM ZnodeOmsSavedCartLineItem a   
    LEFT JOIN ZnodeOmsSavedCartLineItem b ON (a.OmsSavedCartId = b.OmsSavedCartId   
          AND a.ParentOmsSavedCartLineItemId = b.OmsSavedCartLineItemId )  
    WHERE EXISTS (SELECT TOP 1 1 FROM #tempoi  TY WHERE TY.OmsSavedCartId= a.OmsSavedCartId AND ((B.SKU = TY.ParentSKU AND a.SKU = TY.SKU) OR B.SKU = TY.ParentSKU)   )   
   
    ALTER TABLE #tempoi ADD AddOnSKU NVARCHAr(2000)

    UPDATE a 
	SET AddOnSKU  = (SELECT TOP 1 TYI.SKU FROM #tempoi TYI WHERE  TYI.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon   ) 
	FROM #tempoi a 
	WHERE a.OrderLineItemRelationshipTypeID  IS nOT NULL 

    UPDATE a 
	SET AddOnSKU  = (SELECT TYI.SKU FROM ZnodeOmsSavedCartLineItem TYI 
					WHERE TYI.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND TYI.ParentOmsSavedCartLineItemId = a.OmsSavedCartLineItemId
					AND TYI.OmsSavedCartId = a.OmsSavedCartId ) 
	FROM #OldValue a 
	WHERE a.ParentOmsSavedCartLineItemId  IS NOT  NULL 
	AND a.OrderLineItemRelationshipTypeID  <> @OrderLineItemRelationshipTypeIdAddon 

     UPDATE a 
	SET AddOnSKU  = a.SKU
	FROM #OldValue a 
	WHERE a.OrderLineItemRelationshipTypeID  = @OrderLineItemRelationshipTypeIdAddon 

	--SELECT AddOnSKU,SKU,ParentSKU FROM #OldValue
	--SELECT AddOnSKU,SKU,ParentSKU FROM  #tempoi

	
			IF NOT EXISTS (SELECT TOP 1 1 FROM #OldValue  T WHERE ISNULL(dbo.fn_trim(t.AddOnSKU),'') IN  (SELECT ISNULL(dbo.fn_trim(m.AddOnSKU),'') FROM #tempoi m WHERE m.SKU = t.SKU  )
				AND dbo.fn_trim(t.SKU) IN ( SELECT m.SKU FROM #tempoi m )  AND dbo.fn_trim(t.ParentSKU) IN ( SELECT dbo.fn_trim(m.ParentSKU) FROM #tempoi m )      ) 
				
			BEGIN 

			 SET @IsAddProduct = 1 

		    END 
			  
    UPDATE ZSL   
    SET ZSL.Quantity = TH.Quantity  
    FROM  ZnodeOmsSavedCartLineItem ZSL   
    INNER JOIN #tempoi TH ON (ZSL.OmsSavedCartLineItemId = TH.OmsSavedCartLineItemId)   
    WHERE  @IsAddProduct = 0  

     UPDATE ZSLM  SET   
     ZSLM.Quantity = TH.Quantity  
    FROM  ZnodeOmsSavedCartLineItem ZSL   
    INNER JOIN #tempoi TH ON (ZSL.OmsSavedCartLineItemId = TH.OmsSavedCartLineItemId)   
    INNER JOIN ZnodeOmsSavedCartLineItem ZSLM ON (TH.SKU = ZSLM.SKU AND ZSLM.OrderLineItemRelationshipTypeId IN ( @OrderLineItemRelationshipTypeIdAddon , @OrderLineItemRelationshipTypeIdBundle) )  
    WHERE TH.OrderLineItemRelationshipTypeID IN ( @OrderLineItemRelationshipTypeIdAddon , @OrderLineItemRelationshipTypeIdBundle)  
    AND TH.Id =1   
    AND   @IsAddProduct = 0 
  
      
    IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(BundleProductIds,'') =  ''   
      AND  ISNULL(GroupProductIds,'') = '' AND ISNULL(ConfigurableProductIds,'') = ''  )   
     BEGIN   
        
     UPDATE ZSL   
     SET ZSL.Quantity = ZSL.Quantity+TH.Quantity  
     FROM  ZnodeOmsSavedCartLineItem ZSL   
     INNER JOIN #OldValue TYU ON (TYU.OmsSavedCartLineItemId = ZSL.OmsSavedCartLineItemId AND TYU.ParentOmsSavedCartLineItemId= ZSL.ParentOmsSavedCartLineItemId  )  
     INNER JOIN #tempoi TH ON (TH.OmsSavedCartId = TYU.OmsSavedCartId AND TH.SKU = TYU.SKU AND TH.ParentSKU = TYU.ParentSKU AND ISNULL(TYU.AddOnSKU,'' ) = ISNULL(TH.AddOnSKU ,'')   )  
     WHERE (ISNULL(TH.OmsSavedCartLineItemId,0) = 0 OR TH.OmsSavedCartLineItemId = '' )   
     AND TH.Id = 1     
	 AND   @IsAddProduct = 0 
  
     END   
     ELSE   
      BEGIN   
      UPDATE ZSL   
      SET ZSL.Quantity = ZSL.Quantity+TH.Quantity  
      FROM  ZnodeOmsSavedCartLineItem ZSL   
      INNER JOIN #tempoi TH ON (ZSL.OmsSavedCartId = TH.OmsSavedCartId AND ZSL.SKU = TH.SKU )  
      WHERE EXISTS(SELECT TOP 1 1 FROM #OldValue TY WHERE TY.OmsSavedCartLineItemId = ZSL.OmsSavedCartLineItemId AND ISNULL(TY.AddOnSKU,'-1') = ISNULL(TH.AddOnSKU,'-1'))   
      AND (ISNULL(TH.OmsSavedCartLineItemId,0) = 0 OR TH.OmsSavedCartLineItemId = '' )   
      AND   @IsAddProduct = 0 
      END   
         
	  DELETE TH FROM #tempoi TH WHERE (EXISTS (SELECT TOP 1 1 FROM #OldValue ZSL  WHERE ZSL.OmsSavedCartId = TH.OmsSavedCartId AND ZSL.SKU = TH.SKU    
         AND  ISNULL(TH.ParentSKU,'') = ISNULL(ZSL.ParentSKU,'') AND ISNULL(ZSL.AddOnSKU,'') = ISNULL(TH.AddOnSKU,'') )   
      OR EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem RT   
        WHERE RT.OmsSavedCartId = TH.OmsSavedCartId  AND  RT.SKU = TH.SKU  AND RT.ParentOmsSavedCartLineItemId IS NULL )  )
      AND    @IsAddProduct = 0 
        
		
			
       
  
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
    WHERE m.OrderLineItemRelationshipTypeID IN ( @OrderLineItemRelationshipTypeIdAddon , @OrderLineItemRelationshipTypeIdBundle)   
    )   
    UPDATE m1   
    SET m1.RowId = TYU.RowId  
    FROM #yuuete m1   
    INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
        
    
    ;WITH VTRET AS   
    (  
    SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU  
    FROM #yuuete   
    GROUP BY RowId,id ,SKU ,ParentSKU  
    )   
  
    DELETE FROM #yuuete WHERE NewRowId NOT IN (SELECT NewRowId FROM VTRET)   
     
       INSERT INTO  ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description)  
       SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,RowId,ProductName ,Description   
       FROM  #yuuete  TH  
  
     ;with Cte_newData AS   
    (  
    SELECT MAX(a.OmsSavedCartLineItemId) OmsSavedCartLineItemId , b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND CAST(b.RowId AS VARCHAr(200)) = a.GroupId  )  
    WHERE a.ParentOmsSavedCartLineItemId IS NULL  
		AND NOT EXISTS (SELECT TOP 1 1 FROM #OldValue TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
				AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
				GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID
    )   
  
       UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  Cte_newData  r  
    WHERE  r.RowId = b.RowId  )   
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
    WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
    AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
    AND a.ParentOmsSavedCartLineItemId IS nULL   
  
    ;with Cte_newAddon AS   
    (  
    SELECT a.OmsSavedCartLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU  AND b.id =1)  
    WHERE a.ParentOmsSavedCartLineItemId IS NOT NULL   
    AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
    )   
  
    UPDATE a SET  
      a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 max(OmsSavedCartLineItemId) FROM Cte_newAddon  r  
    WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND  b.id =1  )   
    WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
    AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
    AND a.ParentOmsSavedCartLineItemId IS NULL   
    --AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem bb WHERE bb.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId AND b.ParentSKU = bb.SKU )  
      
  
    ;with Cte_Th AS   
    (             
      SELECT RowId    
     FROM #yuuete a   
     GROUP BY RowId   
     HAVING COUNT(NewRowId) <= 1   
      )   
    UPDATE a SET a.Quantity =  NULL   
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN #yuuete b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0)   
    WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
     AND a.OrderLineItemRelationshipTypeId IS NULL   
  
    UPDATE Ab SET ab.Quantity = a.Quantity   
    FROM ZnodeOmsSavedCartLineItem a  
    INNER JOIN ZnodeOmsSavedCartLineItem ab ON (ab.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)  
    INNER JOIN @TBL_SavecartLineitems b ON (a.OmsSavedCartId = b.OmsSavedCartId  )   
    WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  
  
    UPDATE  ZnodeOmsSavedCartLineItem   
    SET GROUPID = NULL   
    WHERE  EXISTS (SELECT TOP 1 1  FROM #yuuete RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
        
       ;With Cte_UpdateSequence AS   
     (  
       SELECT OmsSavedCartLineItemId ,Row_Number()Over(Order By OmsSavedCartLineItemId) RowId , Sequence   
       FROM ZnodeOmsSavedCartLineItem   
       WHERE EXISTS (SELECT TOP 1 1 FROM #yuuete TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
     )   
    UPDATE Cte_UpdateSequence  
    SET  Sequence = RowId  
      
  END TRY   
  BEGIN CATCH   
  SELECT ERROR_MESSAGE()  
  END CATCH   
END