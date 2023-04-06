CREATE TABLE [dbo].[ZnodeCasePriority] (
    [CasePriorityId]   INT           NOT NULL,
    [CasePriorityName] VARCHAR (100) NOT NULL,
    [ViewOrder]        INT           NOT NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NULL,
    CONSTRAINT [PK_CasePriority] PRIMARY KEY CLUSTERED ([CasePriorityId] ASC)
);



