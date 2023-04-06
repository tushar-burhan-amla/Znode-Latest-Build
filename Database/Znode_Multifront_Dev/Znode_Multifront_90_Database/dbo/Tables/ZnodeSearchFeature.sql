CREATE TABLE [dbo].[ZnodeSearchFeature] (
    [SearchFeatureId]       INT             IDENTITY (1, 1) NOT NULL,
    [ParentSearchFeatureId] INT             NULL,
    [FeatureCode]           NVARCHAR (100)  NULL,
    [FeatureName]           NVARCHAR (200)  NULL,
    [IsAdvanceFeature]      BIT             CONSTRAINT [DF_ZnodeSearchProfileFeature_IsAdvanceFeature] DEFAULT ((0)) NOT NULL,
    [ControlType]           NVARCHAR (100)  NULL,
    [HelpDescription]       NVARCHAR (1000) NULL,
    [CreatedBy]             INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [ModifiedBy]            INT             NOT NULL,
    [ModifiedDate]          DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeSearchFeture] PRIMARY KEY CLUSTERED ([SearchFeatureId] ASC),
    CONSTRAINT [FK_ZnodeSearchFeature_ZnodeSearchFeature] FOREIGN KEY ([ParentSearchFeatureId]) REFERENCES [dbo].[ZnodeSearchFeature] ([SearchFeatureId])
);

