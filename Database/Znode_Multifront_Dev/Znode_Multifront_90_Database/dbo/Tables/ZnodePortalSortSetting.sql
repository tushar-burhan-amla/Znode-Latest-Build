CREATE TABLE [dbo].[ZnodePortalSortSetting] (
    [PortalSortSettingId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]            INT           NOT NULL,
    [SortSettingId]       INT           NOT NULL,
    [SortDisplayName]     VARCHAR (300) NOT NULL,
    [DisplayOrder]        INT           NOT NULL,
    [CreatedBy]           INT           NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL,
    [ModifiedBy]          INT           NOT NULL,
    [ModifiedDate]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePortalSortSetting] PRIMARY KEY CLUSTERED ([PortalSortSettingId] ASC),
    CONSTRAINT [FK_ZnodePortalSortSetting_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalSortSetting_ZnodeSortSetting] FOREIGN KEY ([SortSettingId]) REFERENCES [dbo].[ZnodeSortSetting] ([SortSettingId])
);

