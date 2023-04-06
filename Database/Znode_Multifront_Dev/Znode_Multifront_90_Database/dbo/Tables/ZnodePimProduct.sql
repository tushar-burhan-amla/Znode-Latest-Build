CREATE TABLE [dbo].[ZnodePimProduct] (
    [PimProductId]         INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeFamilyId] INT            NULL,
    [ExternalId]           NVARCHAR (50)  NULL,
    [IsProductPublish]     TINYINT        NULL,
    [CreatedBy]            INT            NULL,
    [CreatedDate]          DATETIME       NULL,
    [ModifiedBy]           INT            NULL,
    [ModifiedDate]         DATETIME       NULL,
    [PublishStateId]       TINYINT        NULL,
    [ProductType]          NVARCHAR (MAX) NULL,
    [ProductName]          NVARCHAR (MAX) NULL,
    [SKU]                  NVARCHAR (MAX) NULL,
    [ProductCode]          NVARCHAR (MAX) NULL,
    [Assortment]           NVARCHAR (MAX) NULL,
    [Brand]                NVARCHAR (MAX) NULL,
    [Vendor]               NVARCHAR (MAX) NULL,
    [Highlights]           NVARCHAR (MAX) NULL,
    [ProductImage]         NVARCHAR (MAX) NULL,
    [IsActive]             NVARCHAR (MAX) NULL,
    [Weight]               NVARCHAR (MAX) NULL,
    [IsDownloadable]       NVARCHAR (MAX) NULL,
    [Color]                NVARCHAR (MAX) NULL,
    [FootCondition]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePimProduct] PRIMARY KEY CLUSTERED ([PimProductId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimProduct_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodePimProduct_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);













