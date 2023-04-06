CREATE TABLE [dbo].[ZnodePortalFeatureMapper] (
    [PortalFeatureMapperId]    INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]                 INT      NOT NULL,
    [PortalFeatureId]          INT      NOT NULL,
    [PortalFeatureMapperValue] BIT      NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalFeatureMapper] PRIMARY KEY CLUSTERED ([PortalFeatureMapperId] ASC),
    CONSTRAINT [FK_ZnodePortalFeatureMapper_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalFeatureMapper_ZnodePortalFeature] FOREIGN KEY ([PortalFeatureId]) REFERENCES [dbo].[ZnodePortalFeature] ([PortalFeatureId]),
    CONSTRAINT [IX_ZnodePortalFeatureMapper] UNIQUE NONCLUSTERED ([PortalId] ASC, [PortalFeatureId] ASC, [PortalFeatureMapperValue] ASC)
);





