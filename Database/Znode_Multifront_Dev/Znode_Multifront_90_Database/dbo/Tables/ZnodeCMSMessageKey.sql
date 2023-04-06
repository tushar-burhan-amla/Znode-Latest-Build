CREATE TABLE [dbo].[ZnodeCMSMessageKey] (
    [CMSMessageKeyId] INT             IDENTITY (1, 1) NOT NULL,
    [MessageKey]      NVARCHAR (50)   NOT NULL,
    [MessageTag]      NVARCHAR (1000) NULL,
    [CreatedBy]       INT             NOT NULL,
    [CreatedDate]     DATETIME        NOT NULL,
    [ModifiedBy]      INT             NOT NULL,
    [ModifiedDate]    DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeCMSMessageKey] PRIMARY KEY CLUSTERED ([CMSMessageKeyId] ASC)
);





