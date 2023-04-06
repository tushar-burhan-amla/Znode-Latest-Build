CREATE TABLE [dbo].[ZnodeQueueManage] (
    [QueueManageId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [ReferenceId]   INT           NOT NULL,
    [Entity]        VARCHAR (200) NULL,
    [Status]        TINYINT       NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeQueueManage] PRIMARY KEY CLUSTERED ([QueueManageId] ASC)
);

