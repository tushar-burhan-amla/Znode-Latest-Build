CREATE TABLE [dbo].[ZnodePortalPageSetting] (
    [PortalPageSettingId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]            INT           NOT NULL,
    [PageSettingId]       INT           NOT NULL,
    [PageDisplayName]     VARCHAR (300) NOT NULL,
    [DisplayOrder]        INT           NOT NULL,
    [CreatedBy]           INT           NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL,
    [ModifiedBy]          INT           NOT NULL,
    [ModifiedDate]        DATETIME      NOT NULL,
    [IsDefault]           BIT           CONSTRAINT [DF_ZnodePortalPageSetting_IsDefault] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_ZnodePortalPageSetting] PRIMARY KEY CLUSTERED ([PortalPageSettingId] ASC),
    CONSTRAINT [FK_ZnodePortalPageSetting_ZnodePageSetting] FOREIGN KEY ([PageSettingId]) REFERENCES [dbo].[ZnodePageSetting] ([PageSettingId]),
    CONSTRAINT [FK_ZnodePortalPageSetting_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



