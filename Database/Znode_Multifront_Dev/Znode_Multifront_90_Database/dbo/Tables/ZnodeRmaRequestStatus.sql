CREATE TABLE [dbo].[ZnodeRmaRequestStatus] (
    [RmaRequestStatusId]   INT            IDENTITY (1, 1) NOT NULL,
    [RequestCode]          VARCHAR (600)  NULL,
    [Name]                 NVARCHAR (500) NULL,
    [CustomerNotification] NVARCHAR (MAX) NULL,
    [AdminNotification]    NVARCHAR (MAX) NULL,
    [IsActive]             BIT            CONSTRAINT [DF_ZnodeRmaRequestStatus_IsActive] DEFAULT ((0)) NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaRequestStatus] PRIMARY KEY CLUSTERED ([RmaRequestStatusId] ASC) WITH (FILLFACTOR = 90)
);



