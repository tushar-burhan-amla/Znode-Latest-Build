CREATE TABLE [dbo].[ZnodeNote] (
    [NoteId]        INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        INT            NOT NULL,
    [AccountId]     INT            NULL,
    [CaseRequestId] INT            NULL,
    [NoteTitle]     NVARCHAR (MAX) NOT NULL,
    [NoteBody]      NVARCHAR (MAX) NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [ModifiedBy]    INT            NOT NULL,
    [ModifiedDate]  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeNote] PRIMARY KEY CLUSTERED ([NoteId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeNote_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodeNote_ZnodeCaseRequest] FOREIGN KEY ([CaseRequestId]) REFERENCES [dbo].[ZnodeCaseRequest] ([CaseRequestId]),
    CONSTRAINT [FK_ZnodeNote_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);







