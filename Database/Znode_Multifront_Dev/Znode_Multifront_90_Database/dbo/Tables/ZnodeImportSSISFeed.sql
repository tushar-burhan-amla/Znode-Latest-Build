CREATE TABLE [dbo].[ZnodeImportSSISFeed] (
    [ImportSSISFeedId] INT            IDENTITY (1, 1) NOT NULL,
    [ProcessDate]      DATETIME       NULL,
    [ProcessName]      NVARCHAR (200) NULL,
    [TableName]        NVARCHAR (200) NULL,
    [ProcessStatus]    NVARCHAR (50)  NULL,
    [CreatedBy]        INT            NULL,
    [CreatedDate]      DATETIME       NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedDate]     DATETIME       NULL,
    CONSTRAINT [PK_ZnodeImportSSISFeed] PRIMARY KEY CLUSTERED ([ImportSSISFeedId] ASC)
);

