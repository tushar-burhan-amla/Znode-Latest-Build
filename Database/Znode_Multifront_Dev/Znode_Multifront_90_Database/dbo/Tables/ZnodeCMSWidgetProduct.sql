CREATE TABLE [dbo].[ZnodeCMSWidgetProduct] (
    [CMSWidgetProductId] INT            IDENTITY (1, 1) NOT NULL,
    [PublishProductId]   INT            NULL,
    [CMSWidgetsId]       INT            NOT NULL,
    [WidgetsKey]         NVARCHAR (128) NOT NULL,
    [CMSMappingId]       INT            NOT NULL,
    [TypeOFMapping]      NVARCHAR (50)  NOT NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    [DisplayOrder]       INT            NULL,
    [SKU]                VARCHAR (600)  NULL,
    CONSTRAINT [PK_ZnodeCMSOfferPageProduct] PRIMARY KEY CLUSTERED ([CMSWidgetProductId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetProduct_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);







