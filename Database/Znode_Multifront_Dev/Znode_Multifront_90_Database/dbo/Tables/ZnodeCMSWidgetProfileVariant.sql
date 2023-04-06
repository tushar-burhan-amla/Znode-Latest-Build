CREATE TABLE ZnodeCMSWidgetProfileVariant (
    CMSWidgetProfileVariantId int NOT NULL  IDENTITY(1,1),
    CMSContentWidgetId int  NOT NULL,
	ProfileId int,
	PortalId int ,
	[CreatedBy] INT            NULL,
    [CreatedDate] DATETIME       NULL,
    [ModifiedBy] INT            NULL,
    [ModifiedDate] DATETIME       NULL,
	CONSTRAINT [PK_ZnodeCMSWidgetProfileLocale] PRIMARY KEY CLUSTERED ([CMSWidgetProfileVariantId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSWidgetProfileLocale_ZnodeCMSContentWidget] FOREIGN KEY ([CMSContentWidgetId]) REFERENCES [dbo].[ZnodeCMSContentWidget] ([CMSContentWidgetId]),
    CONSTRAINT [FK_ZnodeCMSWidgetProfileLocale_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
);