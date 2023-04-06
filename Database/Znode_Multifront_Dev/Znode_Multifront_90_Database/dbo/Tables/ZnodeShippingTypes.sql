CREATE TABLE [dbo].[ZnodeShippingTypes] (
    [ShippingTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [ClassName]      NVARCHAR (50)  NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [IsActive]       BIT            NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    CONSTRAINT [PK_SC_ShippingType] PRIMARY KEY CLUSTERED ([ShippingTypeId] ASC) WITH (FILLFACTOR = 90)
);



