CREATE TABLE [dbo].[ZnodeShippingRuleType] (
    [ShippingRuleTypeId] INT            NOT NULL,
    [ClassName]          NVARCHAR (50)  NULL,
    [Name]               NVARCHAR (MAX) NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [IsActive]           BIT            CONSTRAINT [DF_ZNodeShippingRuleType_ActiveInd] DEFAULT ((1)) NOT NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_SC_ShippingRuleType] PRIMARY KEY CLUSTERED ([ShippingRuleTypeId] ASC)
);





