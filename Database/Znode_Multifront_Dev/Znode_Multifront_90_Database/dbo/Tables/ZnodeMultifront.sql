CREATE TABLE [dbo].[ZnodeMultifront] (
    [MultifrontId] INT             IDENTITY (1, 1) NOT NULL,
    [VersionName]  NVARCHAR (1000) NULL,
    [Descriptions] NVARCHAR (MAX)  NULL,
    [MajorVersion] INT             NULL,
    [MinorVersion] INT             NULL,
    [LowerVersion] INT             NULL,
    [BuildVersion] INT             NULL,
    [PatchIndex]   INT             NULL,
    [CreatedBy]    INT             NOT NULL,
    [CreatedDate]  DATETIME        NOT NULL,
    [ModifiedBy]   INT             NOT NULL,
    [ModifiedDate] DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeMultifront] PRIMARY KEY CLUSTERED ([MultifrontId] ASC)
);

