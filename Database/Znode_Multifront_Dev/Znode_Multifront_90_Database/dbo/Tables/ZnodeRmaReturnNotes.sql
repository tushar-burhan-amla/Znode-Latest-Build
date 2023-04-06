CREATE TABLE [dbo].[ZnodeRmaReturnNotes] (
    [RmaReturnNotesId]   INT            IDENTITY (1, 1) NOT NULL,
    [RmaReturnDetailsId] INT            NOT NULL,
    [Notes]              NVARCHAR (MAX) NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnNotes] PRIMARY KEY CLUSTERED ([RmaReturnNotesId] ASC),
    CONSTRAINT [FK_ZnodeRmaReturnNotes_ZnodeRmaReturnDetails] FOREIGN KEY ([RmaReturnDetailsId]) REFERENCES [dbo].[ZnodeRmaReturnDetails] ([RmaReturnDetailsId])
);

