CREATE TABLE [dbo].[ZnodePromotionProfileMapper] (
    [PromotionProfileMapperId]   INT      IDENTITY (1,1) NOT NULL,
	[PromotionId]                INT      NOT NULL,
	[ProfileId]                  INT      NULL,
	[CreatedBy]                  INT      NOT NULL,
	[CreatedDate]                DATETIME NOT NULL,
	[ModifiedBy]                 INT      NOT NULL,
	[ModifiedDate]               DATETIME NOT NULL,  
CONSTRAINT [PK_ZnodePromotionProfileMapper] PRIMARY KEY CLUSTERED ([PromotionProfileMapperId] ASC) WITH (FILLFACTOR = 90),
CONSTRAINT [FK_ZnodePromotionProfileMapper_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId]),
CONSTRAINT [FK_ZnodePromotionProfileMapper_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId])
);