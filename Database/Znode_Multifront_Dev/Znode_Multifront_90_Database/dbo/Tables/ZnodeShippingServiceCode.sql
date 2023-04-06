CREATE TABLE [dbo].[ZnodeShippingServiceCode] (
    [ShippingServiceCodeId] INT            IDENTITY (1, 1) NOT NULL,
    [ShippingTypeId]        INT            NOT NULL,
    [Code]                  NVARCHAR (MAX) NOT NULL,
    [Description]           NVARCHAR (MAX) NOT NULL,
    [DisplayOrder]          INT            NULL,
    [IsActive]              BIT            NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeShippingServiceCode] PRIMARY KEY CLUSTERED ([ShippingServiceCodeId] ASC),
    CONSTRAINT [FK_ZnodeShippingServiceCode_ZnodeShippingTypes] FOREIGN KEY ([ShippingTypeId]) REFERENCES [dbo].[ZnodeShippingTypes] ([ShippingTypeId])
);



