CREATE TABLE [dbo].[ZnodeShippingRule] (
    [ShippingRuleId]       INT             IDENTITY (1, 1) NOT NULL,
    [ShippingId]           INT             NOT NULL,
    [ShippingRuleTypeCode] NVARCHAR (250)  NOT NULL,
    [ClassName]            NVARCHAR (50)   NULL,
    [LowerLimit]           NUMERIC (28, 6) NULL,
    [UpperLimit]           NUMERIC (28, 6) NULL,
    [BaseCost]             NUMERIC (28, 6) NOT NULL,
    [PerItemCost]          NUMERIC (28, 6) NOT NULL,
    [Custom1]              NVARCHAR (MAX)  NULL,
    [Custom2]              NVARCHAR (MAX)  NULL,
    [Custom3]              NVARCHAR (MAX)  NULL,
    [ExternalId]           VARCHAR (50)    NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    CONSTRAINT [PK_SC_ShippingRule] PRIMARY KEY CLUSTERED ([ShippingRuleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_SC_ShippingRule_SC_Shipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);







