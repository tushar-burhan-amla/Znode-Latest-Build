CREATE TABLE [dbo].[ZnodeSearchProfileProductMapping] (
    [SearchProfileProductMappingid] INT            IDENTITY (1, 1) NOT NULL,
    [SearchProfileId]               INT            NOT NULL,
    [SKU]                           NVARCHAR (300) NOT NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeSearchProfileProductMapping] PRIMARY KEY CLUSTERED ([SearchProfileProductMappingid] ASC),
    CONSTRAINT [FK_ZnodeSearchProfileProductMapping_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);

