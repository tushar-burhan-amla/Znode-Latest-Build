CREATE TABLE [dbo].[ZnodePortalRecommendationSetting] (
    [PortalRecommendationSettingId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]                      INT      NOT NULL,
    [IsHomeRecommendation]          BIT      CONSTRAINT [DF_ZnodePortalRecommendationSetting_IsHomeRecommendation] DEFAULT ((0)) NOT NULL,
    [IsPDPRecommendation]           BIT      CONSTRAINT [DF_ZnodePortalRecommendationSetting_IsPDPRecommendation] DEFAULT ((0)) NOT NULL,
    [IsCartRecommendation]          BIT      CONSTRAINT [DF_ZnodePortalRecommendationSetting_IsCartRecommendation] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                     INT      NOT NULL,
    [CreatedDate]                   DATETIME NOT NULL,
    [ModifiedBy]                    INT      NOT NULL,
    [ModifiedDate]                  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalRecommendationSetting] PRIMARY KEY CLUSTERED ([PortalRecommendationSettingId] ASC),
    CONSTRAINT [FK_ZnodePortalRecommendationSetting_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

