CREATE TABLE [dbo].[ZnodePortal] (
    [PortalId]                     INT             IDENTITY (1, 1) NOT NULL,
    [CompanyName]                  NVARCHAR (MAX)  NOT NULL,
    [StoreName]                    NVARCHAR (MAX)  NOT NULL,
    [LogoPath]                     NVARCHAR (MAX)  NULL,
    [UseSSL]                       BIT             NOT NULL,
    [AdminEmail]                   NVARCHAR (MAX)  NULL,
    [SalesEmail]                   NVARCHAR (MAX)  NULL,
    [CustomerServiceEmail]         NVARCHAR (MAX)  NULL,
    [SalesPhoneNumber]             NVARCHAR (MAX)  NULL,
    [CustomerServicePhoneNumber]   NVARCHAR (MAX)  NULL,
    [ImageNotAvailablePath]        NVARCHAR (MAX)  NOT NULL,
    [ShowSwatchInCategory]         BIT             NOT NULL,
    [ShowAlternateImageInCategory] BIT             NOT NULL,
    [ExternalID]                   VARCHAR (50)    NULL,
    [MobileLogoPath]               NVARCHAR (MAX)  NULL,
    [DefaultOrderStateID]          INT             NULL,
    [DefaultReviewStatus]          NVARCHAR (1)    NULL,
    [SplashCategoryID]             INT             NULL,
    [SplashImageFile]              NVARCHAR (MAX)  NULL,
    [MobileTheme]                  NVARCHAR (MAX)  NULL,
    [CopyContentBasedOnPortalId]   INT             NULL,
    [CreatedBy]                    INT             NOT NULL,
    [CreatedDate]                  DATETIME        NOT NULL,
    [ModifiedBy]                   INT             NOT NULL,
    [ModifiedDate]                 DATETIME        NOT NULL,
    [InStockMsg]                   NVARCHAR (MAX)  NULL,
    [OutOfStockMsg]                NVARCHAR (MAX)  NULL,
    [BackOrderMsg]                 NVARCHAR (MAX)  NULL,
    [OrderAmount]                  NUMERIC (28, 6) NULL,
    [Email]                        VARCHAR (500)   NULL,
    [StoreCode]                    NVARCHAR (200)  NULL,
    [UserVerificationType]         VARCHAR (50)    CONSTRAINT [DF_ZnodePortal_UserVerificationType] DEFAULT ('NoVerificationCode') NOT NULL,
    CONSTRAINT [PK_ZnodePortal] PRIMARY KEY CLUSTERED ([PortalId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePortal_ZnodeOrderState] FOREIGN KEY ([DefaultOrderStateID]) REFERENCES [dbo].[ZnodeOmsOrderState] ([OmsOrderStateId]),
    CONSTRAINT [UC_ZnodePortal] UNIQUE NONCLUSTERED ([StoreCode] ASC)
);


























GO

CREATE TRIGGER [dbo].[ZNodePortal_AspNet_SqlCacheNotification_Trigger] ON [dbo].[ZNodePortal]
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZNodePortal'
                       END

