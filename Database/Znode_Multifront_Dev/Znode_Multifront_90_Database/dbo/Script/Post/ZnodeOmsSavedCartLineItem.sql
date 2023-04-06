
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IDX_ZnodeOmsPersonalizeCartItem_OmsSavedCartLineItemId' AND object_id = OBJECT_ID('ZnodeOmsPersonalizeCartItem'))
    BEGIN
       CREATE NONCLUSTERED INDEX IDX_ZnodeOmsPersonalizeCartItem_OmsSavedCartLineItemId
			ON [dbo].ZnodeOmsPersonalizeCartItem (OmsSavedCartLineItemId)
			
    END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IDX_ZnodeOmsSavedCartLineItemDetails_OmsSavedCartLineItemId' AND object_id = OBJECT_ID('ZnodeOmsSavedCartLineItemDetails'))
    BEGIN
        CREATE NONCLUSTERED INDEX IDX_ZnodeOmsSavedCartLineItemDetails_OmsSavedCartLineItemId
			ON [dbo].ZnodeOmsSavedCartLineItemDetails (OmsSavedCartLineItemId)
    END

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsSavedCart_OmsCookieMappingId' AND object_id = OBJECT_ID('ZnodeOmsSavedCart'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_ZnodeOmsSavedCart_OmsCookieMappingId
			ON [dbo].[ZnodeOmsSavedCart] (OmsCookieMappingId)
    END


    IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsSavedCart_OmsCookieMappingId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_ZnodeOmsSavedCart_OmsCookieMappingId
	ON [dbo].[ZnodeOmsSavedCart] (OmsCookieMappingId)
END


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsCookieMapping_UserId_PortalId' AND object_id = OBJECT_ID('ZnodeOmsCookieMapping'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ZnodeOmsCookieMapping_UserId_PortalId
	ON [dbo].ZnodeOmsCookieMapping (UserId,PortalId)
END  

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsCookieMapping_UserId_PortalId')
BEGIN
    Create Index IX_ZnodeOmsCookieMapping_UserId_PortalId on ZnodeOmsCookieMapping(UserId,PortalId)
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsSavedCartLineItem_OmsSavedCartId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_ZnodeOmsSavedCartLineItem_OmsSavedCartId
	ON [dbo].[ZnodeOmsSavedCartLineItem] (OmsSavedCartId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_ParentOmsOrderLineItemsId_Sku' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	 CREATE INDEX IX_ZnodeOmsOrderLineItems_ParentOmsOrderLineItemsId_Sku
		ON dbo.ZnodeOmsOrderLineItems (ParentOmsOrderLineItemsId, Sku) INCLUDE (OrderLineItemRelationshipTypeId, OmsOrderDetailsId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsTaxRule_OmsOrderId_DestinationCountryCode_DestinationStateCode' AND object_id = OBJECT_ID('ZnodeOmsTaxRule'))
BEGIN
	CREATE INDEX IX_ZnodeOmsTaxRule_OmsOrderId_DestinationCountryCode_DestinationStateCode
		ON dbo.ZnodeOmsTaxRule (OmsOrderId,DestinationCountryCode, DestinationStateCode)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeAccountUserPermisasion_UserId' AND object_id = OBJECT_ID('ZnodeAccountUserPermission'))
BEGIN
	CREATE INDEX IX_ZnodeAccountUserPermisasion_UserId ON dbo.ZnodeAccountUserPermission (UserId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsTaxRule_OmsOrderId' AND object_id = OBJECT_ID('ZnodeOmsTaxRule'))
BEGIN
	CREATE INDEX IX_ZnodeOmsTaxRule_OmsOrderId ON dbo.ZnodeOmsTaxRule (OmsOrderId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeAccountUserPermission_UserId' AND object_id = OBJECT_ID('ZnodeAccountUserPermission'))
BEGIN
	CREATE INDEX IX_ZnodeAccountUserPermission_UserId ON dbo.ZnodeAccountUserPermission (UserId) INCLUDE (AccountPermissionAccessId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeState_StateName' AND object_id = OBJECT_ID('ZnodeState'))
BEGIN
	CREATE INDEX IX_ZnodeState_StateName ON dbo.ZnodeState (StateName)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_ParentOmsOrderLineItemsId' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderLineItems_ParentOmsOrderLineItemsId ON dbo.ZnodeOmsOrderLineItems
		(ParentOmsOrderLineItemsId) INCLUDE (OrderLineItemRelationshipTypeId, OmsOrderDetailsId, Sku)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDiscount_OmsOrderDetailsId' AND object_id = OBJECT_ID('ZnodeOmsOrderDiscount'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderDiscount_OmsOrderDetailsId ON dbo.ZnodeOmsOrderDiscount (OmsOrderDetailsId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_ParentOmsOrderLineItemsId' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_ParentOmsOrderLineItemsId 
		ON dbo.ZnodeOmsOrderLineItems (OmsOrderDetailsId,ParentOmsOrderLineItemsId) 
			INCLUDE (ProductName)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_IsActive 
		ON dbo.ZnodeOmsOrderLineItems (OmsOrderDetailsId, IsActive) 
			INCLUDE (OrderLineItemStateId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderLineItems_IsActive ON dbo.ZnodeOmsOrderLineItems (IsActive) INCLUDE (OmsOrderDetailsId, OrderLineItemStateId)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeState_StateCode' AND object_id = OBJECT_ID('ZnodeState'))
BEGIN
	CREATE INDEX IX_ZnodeState_StateCode ON dbo.ZnodeState (StateCode)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodePublishProductEntity_VersionId_ZnodeCatalogId_LocaleId_IsActive_SKULower' AND object_id = OBJECT_ID('ZnodePublishProductEntity'))
BEGIN
	CREATE INDEX IX_ZnodePublishProductEntity_VersionId_ZnodeCatalogId_LocaleId_IsActive_SKULower 
		ON dbo.ZnodePublishProductEntity (VersionId, ZnodeCatalogId, LocaleId, IsActive, SKULower)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodePublishProductEntity_VersionId_ZnodeCatalogId_LocaleId_SKULower' AND object_id = OBJECT_ID('ZnodePublishProductEntity'))
BEGIN
	CREATE INDEX IX_ZnodePublishProductEntity_VersionId_ZnodeCatalogId_LocaleId_SKULower ON dbo.ZnodePublishProductEntity (VersionId, ZnodeCatalogId, LocaleId, SKULower)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeUser_Email' AND object_id = OBJECT_ID('ZnodeUser'))
BEGIN
	CREATE INDEX IX_ZnodeUser_Email ON dbo.ZnodeUser (Email)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderLineItems'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderLineItems_OmsOrderDetailsId_IsActive ON dbo.ZnodeOmsOrderLineItems (OmsOrderDetailsId, IsActive)
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_OmsOrderId_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX [IX_ZnodeOmsOrderDetails_OmsOrderId_IsActive] ON [dbo].[ZnodeOmsOrderDetails] ([OmsOrderId], [IsActive])
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_OmsOrderId' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderDetails_OmsOrderId ON [dbo].[ZnodeOmsOrderDetails] ([OmsOrderId])
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_PortalId_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX [IX_ZnodeOmsOrderDetails_PortalId_IsActive] ON [dbo].[ZnodeOmsOrderDetails] ([PortalId], [IsActive]) INCLUDE ([OmsOrderId], [UserId], [OmsOrderStateId])
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_PortalId_IsActive_OrderDate' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX [IX_ZnodeOmsOrderDetails_PortalId_IsActive_OrderDate] ON [dbo].[ZnodeOmsOrderDetails] ([PortalId], [IsActive],[OrderDate]) INCLUDE ([OmsOrderId], [OmsOrderStateId])
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_IsActive_PortalId_OrderDate' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX [IX_ZnodeOmsOrderDetails_IsActive_PortalId_OrderDate] ON [dbo].[ZnodeOmsOrderDetails] ([IsActive],[PortalId], [OrderDate]) INCLUDE ([OmsOrderId], [OmsOrderStateId])
END

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX [IX_ZnodeOmsOrderDetails_IsActive] ON [dbo].[ZnodeOmsOrderDetails] ([IsActive]) INCLUDE ([PortalId], [UserId], [CurrencyCode], [Total])
END

-- ZPD-ZPD-24182 Dt-16-Jan-2022
ALTER INDEX [PK_ZnodeOmsSavedCart] ON [dbo].[ZnodeOmsSavedCart] SET ( ALLOW_PAGE_LOCKS = OFF )
GO
ALTER INDEX [PK_ZnodeOmsSavedCartLineItem] ON [dbo].[ZnodeOmsSavedCartLineItem] SET ( ALLOW_PAGE_LOCKS = OFF )
