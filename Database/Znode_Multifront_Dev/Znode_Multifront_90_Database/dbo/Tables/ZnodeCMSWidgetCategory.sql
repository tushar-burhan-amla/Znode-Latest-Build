CREATE TABLE [dbo].[ZnodeCMSWidgetCategory] (
    [CMSWidgetCategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [PublishCategoryId]   INT            NULL,
    [CMSWidgetsId]        INT            NOT NULL,
    [WidgetsKey]          NVARCHAR (128) NOT NULL,
    [CMSMappingId]        INT            NOT NULL,
    [TypeOFMapping]       NVARCHAR (50)  NOT NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [DisplayOrder]        INT            NULL,
    [CategoryCode]        VARCHAR (600)  NULL,
    CONSTRAINT [PK_ZnodeCMSOfferPageCategory] PRIMARY KEY CLUSTERED ([CMSWidgetCategoryId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetCategory_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);







