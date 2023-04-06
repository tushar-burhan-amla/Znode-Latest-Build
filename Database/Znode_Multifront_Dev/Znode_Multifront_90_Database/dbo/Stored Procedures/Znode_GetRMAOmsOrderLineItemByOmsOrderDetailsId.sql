     
CREATE PROCEDURE [dbo].[Znode_GetRMAOmsOrderLineItemByOmsOrderDetailsId]   
(        
	 @OmsOrderDetailsId		INT  ,        
	 @RMAId					INT=0 ,        
	 @IsReturnable			INT =0,        
	 @Flag					VARCHAR(10) = null        
)        
AS  
                   
	-- Summary :- RETURNS OrderLineItems based on the OrderId and RMA RequestId  
	-- UNIT Testing 
	-- SELECT * FROM ZnodeOrderLineItems     
	-- EXEC     Znode_GetRMAOmsOrderLineItemByOmsOrderDetailsId 1174,0,0,''

BEGIN       
			if(@Flag ='')        
			BEGIN        
		
			 SELECT        
				  ZOOLI.OmsOrderLineItemsId ,        
				  ZOOLI.OmsOrderDetailsId,        
				  ZOOLI.[ProductName],        
				  ZOOLI.[Description],        
				  ZOOLI.Quantity,        
				  ZOOLI.[Price],        
				  ZOOLI.[SKU],    
				  (ZOOLI.[Price] /ZOOLI.Quantity  ) UnitPrice   ,
				  ZOOLI.[DiscountAmount],        
				  ZOOLI.[ShippingCost],        
				  ZOOLI.PromoDescription,        
				  (ZOTOLD.[SalesTax]/ZOOLI.Quantity +  ZOTOLD.[VAT]/ZOOLI.Quantity + ZOTOLD.[GST]/ZOOLI.Quantity + ZOTOLD.[PST]/ZOOLI.Quantity +  ZOTOLD.[HST]/ZOOLI.Quantity) as  SalesTax,     
				  ISnull((
				  Select Sum(Quantity) 
				  from ZNodeRMARequestItem A 
				  INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId  and B.RMARequestId=@RMAId ),0)   
				 RMAMaxQuantity,        
				 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where  OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId and B.RMARequestId=@RMAId ),0) 
				 RMAQuantity,        
				 ISnull((Select top 1 ISReturnable from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where  OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId ),0) ISReturnable,        

				 ISnull((Select top 1 ISReceived from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where  OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId  ),0)
				  ISReceived,        
				  RmaReasonForReturnId,
				-- (select Name from [dbo].[ZNodeReasonForReturn] where ReasonforReturnId = ZRRI.ReasonforReturnId ) AS 
				'' ReasonforReturn ,
				(Select top 1 TaxCost from ZNodeRMARequest Where RMARequestID = @RMAId) TaxCost,        
				(Select top 1 SubTotal from ZNodeRMARequest Where RMARequestID = @RMAId) SubTotal,        
				(Select top 1 Total from ZNodeRMARequest Where RMARequestID = @RMAId) Total    
				FROM [dbo].[ZnodeOmsOrderLineItems] ZOOLI 
				LEFT JOIN dbo.ZnodeOmsTaxOrderLineDetails ZOTOLD ON (ZOTOLD.OmsOrderLineItemsId = ZOOLI.OmsOrderLineItemsId)      
				LEFT JOIN [dbo].[ZnodeRMARequestItem] ZRRI ON (ZRRI.OmsOrderLineItemsId= ZOOLI.OmsOrderLineItemsId )
				
				WHERE ZOOLI.[OmsOrderDetailsId] = @OmsOrderDetailsId        
				 AND (ZRRI.RMARequestId =@RMAId  OR @RMAId = 0 ) 
				 AND (@IsReturnable =0 OR ZRRI.ISReturnable=@IsReturnable)        
				 --AND ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId  and B.RMARequestId=@RMAId ),0)> 
				 --ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where  OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId and B.RMARequestId=@RMAId ),0)        
    
			 END        
			--ELSE if (@Flag ='view')      
			--BEGIN      
			--SELECT        
			--	 RItem.[OrderLineItemID] as[OrderLineItemID],        
			--	 OItem.[OrderID],        
			--	 OItem.[ProductNum],        
			--	 OItem.[Name],        
			--	 OItem.[Description],        
			--	 OItem.[Quantity] MaxQuantity,        
			--	 OItem.[Price],        
			--	 OItem.[SKU],        
			--	 OItem.[DiscountAmount],        
			--	 OItem.[ShippingCost],        
			--	 OItem.[PromoDescription],        
			--	 (OItem.[SalesTax]/OItem.Quantity +  OItem.[VAT]/OItem.Quantity + OItem.[GST]/OItem.Quantity + OItem.[PST]/OItem.Quantity +  OItem.[HST]/OItem.Quantity) as  SalesTax,   
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId>0 and B.RMARequestId=@RMAId ),0) 



			--RMAMaxQuantity,        
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and B.RMARequestId=@RMAId and ReasonForReturnId is null)

  
    
			--,1) RMAQuantity,        
			--	 ISnull((Select top 1 ISReturnable from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId >0 ),0) ISReturnable,        

			--	 ISnull((Select top 1 ISReceived from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId >0  ),0) ISReceived,        
			--	 RItem.ReasonforReturnId, 
			--	 (select Name from [dbo].[ZNodeReasonForReturn] where ReasonforReturnId = RItem.ReasonforReturnId ) AS ReasonforReturn ,       
			--	(Select top 1 TaxCost from ZNodeRMARequest Where RMARequestID = @RMAId) TaxCost,        
			--(Select top 1 SubTotal from ZNodeRMARequest Where RMARequestID = @RMAId) SubTotal,        
			--(Select top 1 Total from ZNodeRMARequest Where RMARequestID = @RMAId) Total        
        
             
			--	FROM        
			--	 [dbo].[ZNodeOrderLineItem] OItem      
			--	INNER JOIN [dbo].[ZNodeRMARequestItem] RItem ON OItem.OrderlineItemID = RItem.OrderlineItemID        
			--	INNER JOIN ZNodeRMARequest B ON B.RMARequestID = RItem.RMARequestID       
			--	WHERE        
			--	 [OrderID] = @OrderID        
           
			--	 AND RItem.RMARequestId =@RMAId and (RItem.ISReturnable is null or RItem.ISReturnable=@IsReturnable)      
			--END       
			--ELSE        
			--BEGIN        
			--if(@RMAID=0)        
			--Begin        
            
			--	SELECT        
			--	 OItem.[OrderLineItemID],        
			--	 OItem.[OrderID],        
			--	 OItem.[ProductNum],        
			--	 OItem.[Name],        
			--	 OItem.[Description],        
			--	 OItem.[Quantity] MaxQuantity,        
			--	 OItem.[Price],        
			--	 OItem.[SKU],        
			--	 OItem.[DiscountAmount],        
			--	 OItem.[ShippingCost],        
			--	 OItem.[PromoDescription],        
			--	 (OItem.[SalesTax]/OItem.Quantity +  OItem.[VAT]/OItem.Quantity + OItem.[GST]/OItem.Quantity + OItem.[PST]/OItem.Quantity +  OItem.[HST]/OItem.Quantity) as  SalesTax,     
			--	 ISNull((Select Top 1 ISReturnable from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and B.RMARequestId=@RMAId),0) ISReturnable,        

			--	 ISNull((Select Top 1 ISReceived from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and B.RMARequestId=@RMAId),0) ISReceived,        
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId>0 ),0) RMAMaxQuantity,        
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and B.RMARequestId=@RMAId and ReasonForReturnId>0),0) RMAQuantity,        
			--	 0 ReasonForReturnId ,
			--	 '' AS ReasonforReturn 
			--	FROM        
			--	 [dbo].[ZNodeOrderLineItem] OItem        
			--	WHERE        
			--	 [OrderID] = @OrderID        
			--	 AND OItem.[Price]>0        
			--	 AND OItem.[Quantity]>Isnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId>0),0)        



            
			-- END          
			-- ELSE        
			-- BEGIN        
			-- SELECT        
			--	 RItem.[OrderLineItemID] as[OrderLineItemID],        
			--	 OItem.[OrderID],        
			--	 OItem.[ProductNum],        
			--	 OItem.[Name],        
			--	 OItem.[Description],        
			--	 OItem.[Quantity] MaxQuantity,        
			--	 OItem.[Price],        
			--	 OItem.[SKU],        
			--	 OItem.[DiscountAmount],        
			--	 OItem.[ShippingCost],        
			--	 OItem.[PromoDescription],        
			--	(OItem.[SalesTax]/OItem.Quantity +  OItem.[VAT]/OItem.Quantity + OItem.[GST]/OItem.Quantity + OItem.[PST]/OItem.Quantity +  OItem.[HST]/OItem.Quantity) as  SalesTax,     
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId>0 and B.RMARequestId=@RMAId ),0) RMAMaxQuantity,        
			--	 ISnull((Select Sum(Quantity) from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and B.RMARequestId=@RMAId and ReasonForReturnId is null),0) RMAQuantity,        
			--	 ISnull((Select top 1 ISReturnable from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId >0 ),0) ISReturnable,        

			--	 ISnull((Select top 1 ISReceived from ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID Where B.RequestStatusID != 4 and  OrderlineItemId=OItem.[OrderLineItemID] and ReasonForReturnId >0  ),0) ISReceived,        
			--	 RItem.ReasonforReturnId,        
			--	 (select Name from [dbo].[ZNodeReasonForReturn] where [ZNodeReasonForReturn].ReasonForReturnID = RItem.ReasonforReturnId ) AS  ReasonforReturn ,
			--	(Select top 1 TaxCost from ZNodeRMARequest Where RMARequestID = @RMAId) TaxCost,        
			--(Select top 1 SubTotal from ZNodeRMARequest Where RMARequestID = @RMAId) SubTotal,        
			--(Select top 1 Total from ZNodeRMARequest Where RMARequestID = @RMAId) Total        
        
             
			--	FROM        
			--	 [dbo].[ZNodeOrderLineItem] OItem      
			--	INNER JOIN [dbo].[ZNodeRMARequestItem] RItem ON OItem.OrderlineItemID = RItem.OrderlineItemID        
			--	INNER JOIN ZNodeRMARequest B ON B.RMARequestID = RItem.RMARequestID       
			--	WHERE        
			--	 [OrderID] = @OrderID        
           
			--	 and B.RequestStatusID != 4      
			--	 AND RItem.RMARequestId =@RMAId and (RItem.ISReturnable is null or RItem.ISReturnable=@IsReturnable)        
			-- END        
        
END