CREATE TABLE [dbo].[ZnodeERPConfigurator] (
    [ERPConfiguratorId] INT            IDENTITY (1, 1) NOT NULL,
    [ClassName]         NVARCHAR (80)  NOT NULL,
    [ERPName]           NVARCHAR (100) NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [Email]             NVARCHAR (MAX) NULL,
    [IsActive]          BIT            CONSTRAINT [DF_ZnodeERPConfigurator_IsActive] DEFAULT ((0)) NOT NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    [JsonSetting]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZNodeERPConfigurator] PRIMARY KEY CLUSTERED ([ERPConfiguratorId] ASC)
);





