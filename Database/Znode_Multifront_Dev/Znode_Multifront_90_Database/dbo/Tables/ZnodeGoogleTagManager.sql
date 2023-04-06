CREATE TABLE [dbo].[ZnodeGoogleTagManager] (
    [GoogleTagManagerId]           INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]                     INT           NOT NULL,
    [IsActive]                     BIT           NOT NULL,
    [ContainerId]                  NVARCHAR (50) NULL,
    [AnalyticsIdForAddToCart]      NVARCHAR (50) NULL,
    [AnalyticsIdForRemoveFromCart] NVARCHAR (50) NULL,
    [AnalyticsUId]                 NVARCHAR (50) NULL,
    [AnalyticsIsActive]            BIT           NULL,
    [CreatedBy]                    INT           NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   INT           NOT NULL,
    [ModifiedDate]                 DATETIME      NOT NULL,
	[EnableEnhancedEcommerce] BIT CONSTRAINT [DF_ZnodeGoogleTagManager_EnableEnhancedEcommece] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_ZnodeGoogleTagManager] PRIMARY KEY CLUSTERED ([GoogleTagManagerId] ASC),
    CONSTRAINT [FK_ZnodeGoogleTagManager_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





