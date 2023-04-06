CREATE TABLE [dbo].[ZnodeCMSSearchWidget] (
    [CMSSearchWidgetId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSWidgetsId]      INT            NOT NULL,
    [AttributeCode]     VARCHAR (300)  NULL,
    [SearchKeyword]     VARCHAR (300)  NOT NULL,
    [LocaleId]          INT            NOT NULL,
    [WidgetsKey]        NVARCHAR (128) NOT NULL,
    [CMSMappingId]      INT            NOT NULL,
    [TypeOFMapping]     NVARCHAR (50)  NOT NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSearchWidget] PRIMARY KEY CLUSTERED ([CMSSearchWidgetId] ASC)
);

