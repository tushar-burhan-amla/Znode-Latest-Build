CREATE TABLE [dbo].[ZnodePortalFeature] (
    [PortalFeatureId]   INT           IDENTITY (1, 1) NOT NULL,
    [PortalFeatureName] VARCHAR (100) NOT NULL,
    [CreatedBy]         INT           NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedBy]        INT           NOT NULL,
    [ModifiedDate]      DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePortalFeature] PRIMARY KEY CLUSTERED ([PortalFeatureId] ASC),
    CONSTRAINT [IX_ZnodePortalFeature] UNIQUE NONCLUSTERED ([PortalFeatureName] ASC)
);

