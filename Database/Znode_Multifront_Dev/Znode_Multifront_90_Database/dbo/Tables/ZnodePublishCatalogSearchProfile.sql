CREATE TABLE [dbo].[ZnodePublishCatalogSearchProfile] (
    [PublishCatalogSearchProfileId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]              INT      NOT NULL,
    [SearchProfileId]               INT      NOT NULL,
    [IsDefault]                     BIT      CONSTRAINT [DF_ZnodePublishCatalogSearchProfile_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                     INT      NOT NULL,
    [CreatedDate]                   DATETIME NOT NULL,
    [ModifiedBy]                    INT      NOT NULL,
    [ModifiedDate]                  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePublishCatalogSearchProfile] PRIMARY KEY CLUSTERED ([PublishCatalogSearchProfileId] ASC),
    CONSTRAINT [FK_ZnodePublishCatalogSearchProfile_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);



