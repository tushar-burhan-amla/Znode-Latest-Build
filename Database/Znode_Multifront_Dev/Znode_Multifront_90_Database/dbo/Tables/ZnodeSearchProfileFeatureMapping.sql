CREATE TABLE [dbo].[ZnodeSearchProfileFeatureMapping] (
    [SearchProfileFeatureMappingId] INT             IDENTITY (1, 1) NOT NULL,
    [SearchProfileId]               INT             NOT NULL,
    [SearchFeatureId]               INT             NOT NULL,
    [SearchFeatureValue]            NVARCHAR (2000) NULL,
    [CreatedBy]                     INT             NOT NULL,
    [CreatedDate]                   DATETIME        NOT NULL,
    [ModifiedBy]                    INT             NOT NULL,
    [ModifiedDate]                  DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeSearchProfileFeatureMapping] PRIMARY KEY CLUSTERED ([SearchProfileFeatureMappingId] ASC),
    CONSTRAINT [FK_ZnodeSearchProfileFeatureMapping_ZnodeSearchFeature] FOREIGN KEY ([SearchFeatureId]) REFERENCES [dbo].[ZnodeSearchFeature] ([SearchFeatureId]),
    CONSTRAINT [FK_ZnodeSearchProfileFeatureMapping_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);







