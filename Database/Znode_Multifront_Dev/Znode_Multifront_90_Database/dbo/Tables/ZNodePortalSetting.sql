CREATE TABLE [dbo].[ZnodePortalSetting] (
    [PortalSettingId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]        INT            NULL,
    [FeatureName]     NVARCHAR (100) NULL,
    [FeatureValues]   NVARCHAR (MAX) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodePortalSetting] PRIMARY KEY CLUSTERED ([PortalSettingId] ASC),
    CONSTRAINT [FK_ZnodePortalSetting_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





