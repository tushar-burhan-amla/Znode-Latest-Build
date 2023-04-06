CREATE TABLE [dbo].[ZnodeSupplier] (
    [SupplierId]                INT            IDENTITY (1, 1) NOT NULL,
    [SupplierTypeId]            INT            NULL,
    [ExternalSupplierNo]        NVARCHAR (100) NULL,
    [Name]                      NVARCHAR (100) NOT NULL,
    [Description]               NVARCHAR (MAX) NULL,
    [ContactFirstName]          NVARCHAR (MAX) NULL,
    [ContactLastName]           NVARCHAR (MAX) NULL,
    [ContactPhone]              NVARCHAR (MAX) NULL,
    [ContactEmail]              NVARCHAR (MAX) NULL,
    [NotificationEmailId]       NVARCHAR (MAX) NULL,
    [EmailNotificationTemplate] NVARCHAR (MAX) NULL,
    [EnableEmailNotification]   BIT            NOT NULL,
    [DisplayOrder]              INT            NOT NULL,
    [IsActive]                  BIT            NOT NULL,
    [Custom1]                   NVARCHAR (MAX) NULL,
    [Custom2]                   NVARCHAR (MAX) NULL,
    [Custom3]                   NVARCHAR (MAX) NULL,
    [Custom4]                   NVARCHAR (MAX) NULL,
    [Custom5]                   NVARCHAR (MAX) NULL,
    [PortalID]                  INT            NULL,
    [ExternalId]                VARCHAR (50)   NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeSupplier] PRIMARY KEY CLUSTERED ([SupplierId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeActivityLog_ZnodeSupplier] FOREIGN KEY ([PortalID]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeSupplier_ZnodeSupplierTypes] FOREIGN KEY ([SupplierTypeId]) REFERENCES [dbo].[ZnodeSupplierTypes] ([SupplierTypeID])
);





