CREATE TABLE [dbo].[ZnodeGlobalSetting] (
    [ZNodeGlobalSettingId] INT             IDENTITY (1, 1) NOT NULL,
    [FeatureName]          NVARCHAR (100)  NOT NULL,
    [FeatureValues]        NVARCHAR (MAX) NULL,
    [FeatureSubValues]     NVARCHAR (600)  NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalSetting] PRIMARY KEY CLUSTERED ([ZNodeGlobalSettingId] ASC)
);












GO
Create TRIGGER [dbo].[Trg_ZnodeGlobalSetting] ON dbo.ZnodeGlobalSetting
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZnodeGlobalSetting'
                       END