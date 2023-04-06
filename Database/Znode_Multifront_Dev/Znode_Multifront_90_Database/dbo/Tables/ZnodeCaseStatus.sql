CREATE TABLE [dbo].[ZnodeCaseStatus] (
    [CaseStatusId]   INT           NOT NULL,
    [CaseStatusName] VARCHAR (100) NOT NULL,
    [ViewOrder]      INT           NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NULL,
    CONSTRAINT [PK_CaseStatus] PRIMARY KEY CLUSTERED ([CaseStatusId] ASC)
);



