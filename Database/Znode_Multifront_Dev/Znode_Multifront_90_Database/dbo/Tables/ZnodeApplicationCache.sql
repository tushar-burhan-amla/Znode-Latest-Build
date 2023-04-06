CREATE TABLE [dbo].[ZnodeApplicationCache] (
    [ApplicationCacheId] INT           IDENTITY (1, 1) NOT NULL,
    [ApplicationType]    NVARCHAR (50) NOT NULL,
    [IsActive]           BIT           NOT NULL,
    [StartDate]          DATETIME      NULL,
    [CreatedBy]          INT           NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [ModifiedBy]         INT           NOT NULL,
    [ModifiedDate]       DATETIME      NOT NULL,
    [Duration]           INT           NULL,
    CONSTRAINT [PK_ZnodeApplicationCache] PRIMARY KEY CLUSTERED ([ApplicationCacheId] ASC)
);



