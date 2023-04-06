CREATE TABLE [dbo].[ZnodeBlogComment] (
    [BlogCommentId] INT      IDENTITY (1, 1) NOT NULL,
    [BlogNewsId]    INT      NULL,
    [UserId]        INT      NULL,
    [IsApproved]    BIT      NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeBlogComment] PRIMARY KEY CLUSTERED ([BlogCommentId] ASC),
    CONSTRAINT [FK_ZnodeBlogComment_ZnodeBlogNews] FOREIGN KEY ([BlogNewsId]) REFERENCES [dbo].[ZnodeBlogNews] ([BlogNewsId]),
    CONSTRAINT [FK_ZnodeBlogComment_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

