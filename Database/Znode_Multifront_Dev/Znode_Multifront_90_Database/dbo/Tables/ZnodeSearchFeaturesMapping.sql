CREATE TABLE [dbo].[ZnodeSearchFeaturesMapping] (
    [SearchFeaturesMappingId] INT      IDENTITY (1, 1) NOT NULL,
    [SearchFeatureId]         INT      NOT NULL,
    [SearchQueryTypeId]       INT      NOT NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeSearchFeaturesMapping] PRIMARY KEY CLUSTERED ([SearchFeaturesMappingId] ASC),
    CONSTRAINT [FK_ZnodeSearchFeaturesMapping_ZnodeSearchQueryType] FOREIGN KEY ([SearchQueryTypeId]) REFERENCES [dbo].[ZnodeSearchQueryType] ([SearchQueryTypeId])
);



