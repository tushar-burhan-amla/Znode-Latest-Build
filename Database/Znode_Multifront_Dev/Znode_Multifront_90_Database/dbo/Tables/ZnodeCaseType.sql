CREATE TABLE [dbo].[ZnodeCaseType] (
    [CaseTypeId]   INT           NOT NULL,
    [CaseTypeName] VARCHAR (100) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NULL,
    CONSTRAINT [PK_CaseType] PRIMARY KEY CLUSTERED ([CaseTypeId] ASC)
);



