CREATE TABLE [dbo].[ZnodeCMSWidgetBrand] (
    [CMSWidgetBrandId] INT           IDENTITY (1, 1) NOT NULL,
    [BrandId]          INT           NOT NULL,
    [CMSWidgetsId]     INT           NOT NULL,
    [WidgetsKey]       NVARCHAR (50) NOT NULL,
    [CMSMappingId]     INT           NOT NULL,
    [TypeOFMapping]    NVARCHAR (50) NOT NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    [DisplayOrder]     INT           NULL,
    CONSTRAINT [PK_ZnodeCMSWidgetBrand] PRIMARY KEY CLUSTERED ([CMSWidgetBrandId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetBrand_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId]),
    CONSTRAINT [FK_ZnodeCMSWidgetBrand_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);



