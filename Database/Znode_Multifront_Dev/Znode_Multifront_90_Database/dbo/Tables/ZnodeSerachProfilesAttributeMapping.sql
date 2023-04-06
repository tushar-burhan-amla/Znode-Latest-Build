CREATE TABLE [dbo].[ZnodeSerachProfilesAttributeMapping] (
    [SerachProfilesAttributeMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [SearchProfileId]                  INT            NOT NULL,
    [AttributeCode]                    NVARCHAR (300) NOT NULL,
    [IsFacets]                         BIT            NOT NULL,
    [IsUseInSearch]                    BIT            NOT NULL,
    [BoostValue]                       INT            NULL,
    [CreatedBy]                        INT            NOT NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ModifiedBy]                       INT            NOT NULL,
    [ModifiedDate]                     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeSerachProfilesAttributeMapping] PRIMARY KEY CLUSTERED ([SerachProfilesAttributeMappingId] ASC),
    CONSTRAINT [FK_ZnodeSerachProfilesAttributeMapping_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);



