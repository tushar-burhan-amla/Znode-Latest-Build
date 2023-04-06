CREATE TABLE [dbo].[ZnodePublishWidgetSliderBannerEntity] (
    [PublishWidgetSliderBannerEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                         INT            NOT NULL,
    [PublishStartTime]                  DATETIME       NULL,
    [WidgetSliderBannerId]              INT            NOT NULL,
    [MappingId]                         INT            NOT NULL,
    [PortalId]                          INT            NOT NULL,
    [LocaleId]                          INT            NOT NULL,
    [Type]                              VARCHAR (100)  NULL,
    [Navigation]                        VARCHAR (100)  NULL,
    [AutoPlay]                          BIT            NOT NULL,
    [AutoplayTimeOut]                   INT            NULL,
    [AutoplayHoverPause]                BIT            NOT NULL,
    [TransactionStyle]                  VARCHAR (100)  NULL,
    [WidgetsKey]                        VARCHAR (300)  NULL,
    [TypeOFMapping]                     VARCHAR (100)  NULL,
    [SliderId]                          INT            NOT NULL,
    [SliderBanners]                     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePublishWidgetSliderBannerEntity] PRIMARY KEY CLUSTERED ([PublishWidgetSliderBannerEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWidgetSliderBannerEntityVersionId]
    ON [dbo].[ZnodePublishWidgetSliderBannerEntity]([VersionId] ASC);

