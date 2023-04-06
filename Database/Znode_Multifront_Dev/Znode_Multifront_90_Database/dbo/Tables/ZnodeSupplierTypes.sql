CREATE TABLE [dbo].[ZnodeSupplierTypes] (
    [SupplierTypeID] INT            IDENTITY (1, 1) NOT NULL,
    [ClassName]      NVARCHAR (50)  NULL,
    [Name]           NVARCHAR (MAX) NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [IsActive]       BIT            NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeSupplierType] PRIMARY KEY CLUSTERED ([SupplierTypeID] ASC)
);

