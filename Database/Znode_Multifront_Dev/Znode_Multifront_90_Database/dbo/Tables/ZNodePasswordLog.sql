CREATE TABLE [dbo].[ZnodePasswordLog] (
    [PasswordLogId] INT            IDENTITY (1, 1) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    [Password]      NVARCHAR (MAX) NOT NULL,
    [CreatedBy]     INT            NULL,
    [CreatedDate]   DATETIME       NULL,
    [ModifiedBy]    INT            NULL,
    [ModifiedDate]  DATETIME       NULL,
    CONSTRAINT [PK_ZNodePasswordLog] PRIMARY KEY CLUSTERED ([PasswordLogId] ASC)
);



