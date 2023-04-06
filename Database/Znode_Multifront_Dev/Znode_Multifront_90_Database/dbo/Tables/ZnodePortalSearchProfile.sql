CREATE TABLE [dbo].[ZnodePortalSearchProfile] (
    [PortalSearchProfileId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]              INT      NOT NULL,
    [PublishCatalogId]      INT      NOT NULL,
    [SearchProfileId]       INT      NOT NULL,
    [IsDefault]             BIT      CONSTRAINT [DF_ZnodePortalSearchProfile_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]             INT      NOT NULL,
    [CreatedDate]           DATETIME NOT NULL,
    [ModifiedBy]            INT      NOT NULL,
    [ModifiedDate]          DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalSearchProfile] PRIMARY KEY CLUSTERED ([PortalSearchProfileId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePortalSearchProfile_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalSearchProfile_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);





