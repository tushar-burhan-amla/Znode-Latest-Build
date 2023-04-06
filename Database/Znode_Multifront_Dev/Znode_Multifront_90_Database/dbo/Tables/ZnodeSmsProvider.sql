CREATE TABLE [dbo].[ZnodeSmsProvider] (
    [SmsProviderId] INT             IDENTITY (1, 1) NOT NULL,
    [ProviderCode]              NVARCHAR (300)   NULL,
    [ProviderName]              NVARCHAR (300)   NULL,
	[ClassName]              NVARCHAR (300)   NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
	[Description]          NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeSmsProvider] PRIMARY KEY CLUSTERED ([SmsProviderId] ASC) WITH (FILLFACTOR = 90));