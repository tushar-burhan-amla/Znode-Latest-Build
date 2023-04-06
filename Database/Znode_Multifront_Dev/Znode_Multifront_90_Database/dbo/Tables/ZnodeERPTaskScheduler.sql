CREATE TABLE [dbo].[ZnodeERPTaskScheduler] (
    [ERPTaskSchedulerId] INT            IDENTITY (1, 1) NOT NULL,
    [ERPConfiguratorId]  INT            NULL,
    [SchedulerName]      NVARCHAR (100) NULL,
    [SchedulerType]      VARCHAR (20)   NULL,
    [TouchPointName]     NVARCHAR (100) NOT NULL,
    [SchedulerFrequency] NVARCHAR (50)  NULL,
    [StartDate]          DATETIME       NULL,
    [IsEnabled]          BIT            CONSTRAINT [DF_ZnodeERPTaskScheduler_IsEnabled] DEFAULT ((1)) NOT NULL,
    [SchedulerCallFor]   VARCHAR (200)  NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    [CronExpression]     VARCHAR (100)  NULL,
    [HangfireJobId]      VARCHAR (100)  NULL,
    CONSTRAINT [PK_ZnodeERPTaskScheduler] PRIMARY KEY CLUSTERED ([ERPTaskSchedulerId] ASC),
    CONSTRAINT [FK_ZnodeERPTaskScheduler_ZnodeERPConfigurator] FOREIGN KEY ([ERPConfiguratorId]) REFERENCES [dbo].[ZnodeERPConfigurator] ([ERPConfiguratorId])
);



























