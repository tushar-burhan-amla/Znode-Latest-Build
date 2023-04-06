CREATE TABLE [dbo].[ZnodeShippingSKU] (
    [ShippingSKUId]  INT           IDENTITY (1, 1) NOT NULL,
    [ShippingRuleId] INT           NULL,
    [SKU]            VARCHAR (300) NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeShippingSKU] PRIMARY KEY CLUSTERED ([ShippingSKUId] ASC),
    CONSTRAINT [FK_ZnodeShippingSKU_ZnodeShippingRule] FOREIGN KEY ([ShippingRuleId]) REFERENCES [dbo].[ZnodeShippingRule] ([ShippingRuleId])
);





