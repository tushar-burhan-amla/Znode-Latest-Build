CREATE TABLE [dbo].[ZnodeCaseRequestHistory] (
    [CaseRequestHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [CaseRequestId]        INT            NOT NULL,
    [EmailSubject]         VARCHAR (255)  NOT NULL,
    [EmailMessage]         NVARCHAR (MAX) NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCaseRequestHistory] PRIMARY KEY CLUSTERED ([CaseRequestHistoryId] ASC),
    CONSTRAINT [FK_ZnodeCaseRequestHistory_ZnodeCaseRequest] FOREIGN KEY ([CaseRequestId]) REFERENCES [dbo].[ZnodeCaseRequest] ([CaseRequestId])
);

