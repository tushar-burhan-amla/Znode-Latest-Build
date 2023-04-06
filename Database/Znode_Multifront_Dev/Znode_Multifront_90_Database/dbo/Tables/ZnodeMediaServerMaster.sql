CREATE TABLE [dbo].[ZnodeMediaServerMaster] (
    [MediaServerMasterId] INT           IDENTITY (1, 1) NOT NULL,
    [ServerName]          VARCHAR (200) NULL,
    [PartialViewName]     VARCHAR (200) NULL,
    [IsOtherServer]       BIT           NULL,
    [ThumbnailFolderName] VARCHAR (200) NULL,
    [ClassName]           VARCHAR (200) NULL,
    [CreatedBy]           INT           NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL,
    [ModifiedBy]          INT           NOT NULL,
    [ModifiedDate]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeServerMaster] PRIMARY KEY CLUSTERED ([MediaServerMasterId] ASC)
);



