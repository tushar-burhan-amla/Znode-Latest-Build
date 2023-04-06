CREATE TABLE [dbo].[ZnodeMediaConfiguration] (
    [MediaConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [MediaServerMasterId]  INT            NULL,
    [Server]               VARCHAR (100)  NULL,
    [AccessKey]            NVARCHAR (50)  NULL,
    [SecretKey]            NVARCHAR (100) NULL,
    [URL]                  NVARCHAR (200) NULL,
    [BucketName]           VARCHAR (100)  NULL,
    [Custom1]              VARCHAR (100)  NULL,
    [Custom2]              VARCHAR (100)  NULL,
    [Custom3]              VARCHAR (100)  NULL,
    [IsActive]             BIT            CONSTRAINT [DF__ZnodeMedi__IsAct__76969D2E] DEFAULT ((1)) NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [CDNUrl]               NVARCHAR (200) NULL,
    CONSTRAINT [PK_ZnodeMediaConfiguration] PRIMARY KEY CLUSTERED ([MediaConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeMediaConfigurationZnodeServerMaster] FOREIGN KEY ([MediaServerMasterId]) REFERENCES [dbo].[ZnodeMediaServerMaster] ([MediaServerMasterId])
);














GO
CREATE TRIGGER [dbo].[Trg_ZnodeMediaConfiguration] ON [dbo].[ZnodeMediaConfiguration]
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZnodeMediaConfiguration'
                       END