CREATE TABLE [dbo].[ZnodeRmaConfiguration] (
    [RmaConfigurationId]  INT            IDENTITY (1, 1) NOT NULL,
    [MaxDays]             INT            NULL,
    [DisplayName]         NVARCHAR (200) NULL,
    [Email]               NVARCHAR (200) NULL,
    [Address]             NVARCHAR (MAX) NULL,
    [ShippingDirections]  NVARCHAR (MAX) NULL,
    [IsEmailNotification] BIT            CONSTRAINT [DF_ZnodeRmaConfiguration_IsEmailNotification] DEFAULT ((0)) NOT NULL,
    [GcExpirationPeriod]  INT            NULL,
    [GcNotification]      NVARCHAR (MAX) NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaConfiguration] PRIMARY KEY CLUSTERED ([RmaConfigurationId] ASC)
);

