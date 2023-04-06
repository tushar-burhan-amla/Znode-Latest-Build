CREATE TABLE [dbo].[ZnodeEmailTemplateMapper] (
    [EmailTemplateMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [EmailTemplateId]       INT      NOT NULL,
    [PortalId]              INT      NULL,
    [EmailTemplateAreasId]  INT      NOT NULL,
    [IsActive]              BIT      CONSTRAINT [DF_ZnodeEmailTemplateMapper_IsActive] DEFAULT ((0)) NOT NULL,
    [CreatedBy]             INT      NOT NULL,
    [CreatedDate]           DATETIME NOT NULL,
    [ModifiedBy]            INT      NOT NULL,
    [ModifiedDate]          DATETIME NOT NULL,
    [IsEnableBcc]           BIT      DEFAULT ((0)) NOT NULL,
	[IsSmsNotificationActive] BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodeEmailTemplateMapper] PRIMARY KEY CLUSTERED ([EmailTemplateMapperId] ASC),
    CONSTRAINT [FK_ZnodeEmailTemplateMapper_ZnodeEmailTemplate] FOREIGN KEY ([EmailTemplateId]) REFERENCES [dbo].[ZnodeEmailTemplate] ([EmailTemplateId]),
    CONSTRAINT [FK_ZnodeEmailTemplateMapper_ZnodeEmailTemplateAreas] FOREIGN KEY ([EmailTemplateAreasId]) REFERENCES [dbo].[ZnodeEmailTemplateAreas] ([EmailTemplateAreasId])
);







