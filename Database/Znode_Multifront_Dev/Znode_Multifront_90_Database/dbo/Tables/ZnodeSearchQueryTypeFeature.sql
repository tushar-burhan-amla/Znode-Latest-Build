CREATE TABLE [dbo].[ZnodeSearchQueryTypeFeature] (
    [SearchQueryTypeFeatureId] INT      IDENTITY (1, 1) NOT NULL,
    [SearchFeatureId]          INT      NOT NULL,
    [SearchQueryTypeId]        INT      NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeSearchQueryTypeFeature] PRIMARY KEY CLUSTERED ([SearchQueryTypeFeatureId] ASC),
    CONSTRAINT [FK_ZnodeSearchQueryTypeFeature_ZnodeSearchFeature] FOREIGN KEY ([SearchFeatureId]) REFERENCES [dbo].[ZnodeSearchFeature] ([SearchFeatureId]),
    CONSTRAINT [FK_ZnodeSearchQueryTypeFeature_ZnodeSearchQueryType] FOREIGN KEY ([SearchQueryTypeId]) REFERENCES [dbo].[ZnodeSearchQueryType] ([SearchQueryTypeId])
);

