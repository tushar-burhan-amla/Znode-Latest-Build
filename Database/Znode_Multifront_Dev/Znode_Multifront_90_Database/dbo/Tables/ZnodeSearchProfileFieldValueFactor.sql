CREATE TABLE [dbo].[ZnodeSearchProfileFieldValueFactor] (
    [SearchProfileFieldValueId] INT           IDENTITY (1, 1) NOT NULL,
    [SearchProfileId]           INT           NOT NULL,
    [FieldName]                 VARCHAR (200) NOT NULL,
    [FieldValueFactor]          INT           NOT NULL,
    [CreatedBy]                 INT           NOT NULL,
    [CreatedDate]               DATETIME      NOT NULL,
    [ModifiedBy]                INT           NOT NULL,
    [ModifiedDate]              DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeSearchProfileFieldValueFactor] PRIMARY KEY CLUSTERED ([SearchProfileFieldValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeSearchProfileFieldValueFactor_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);

