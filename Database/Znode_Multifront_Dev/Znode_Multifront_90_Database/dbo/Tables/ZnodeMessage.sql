CREATE TABLE [dbo].[ZnodeMessage] (
    [ZnodeMessageId] INT           IDENTITY (1, 1) NOT NULL,
    [MessageCode]    INT           NULL,
    [MessageType]    VARCHAR (10)  NOT NULL,
    [MessageName]    VARCHAR (500) NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMessage] PRIMARY KEY CLUSTERED ([ZnodeMessageId] ASC) WITH (FILLFACTOR = 90)
);



