CREATE TABLE [dbo].[ZnodeProceduresErrorLog] (
    [ProcedureErrorLogId] INT            IDENTITY (1, 1) NOT NULL,
    [ProcedureName]       VARCHAR (1000) NULL,
    [ErrorInProcedure]    VARCHAR (1000) NULL,
    [ErrorMessage]        NVARCHAR (MAX) NULL,
    [ErrorLine]           VARCHAR (100)  NULL,
    [ErrorCall]           NVARCHAR (MAX) NULL,
    [CreatedBy]           VARCHAR (500)  NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_Znode_ProcdureErrorLog] PRIMARY KEY CLUSTERED ([ProcedureErrorLogId] ASC) WITH (FILLFACTOR = 90)
);



