CREATE TABLE [dbo].[ZnodeRmaReturnType] (
    [RmaReturnTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [ReturnTypeName]  NVARCHAR (300) NULL,
    [ReturnTypeCode]  VARCHAR (100)  NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnType] PRIMARY KEY CLUSTERED ([RmaReturnTypeId] ASC),
    CONSTRAINT [UK_ZnodeRmaReturnType] UNIQUE NONCLUSTERED ([ReturnTypeCode] ASC)
);

