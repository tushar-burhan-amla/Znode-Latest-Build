CREATE TABLE [dbo].[ZnodeCMSSliderBannerLocale] (
    [CMSSliderBannerLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSSliderBannerId]       INT            NOT NULL,
    [LocaleId]                INT            NOT NULL,
    [Description]             NVARCHAR (MAX) NULL,
    [ImageAlternateText]      NVARCHAR (500) NULL,
    [MediaId]                 INT            NULL,
    [Title]                   NVARCHAR (500) NULL,
    [ButtonLabelName]         NVARCHAR (600) NULL,
    [ButtonLink]              NVARCHAR (MAX) NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSliderBannerLocale] PRIMARY KEY CLUSTERED ([CMSSliderBannerLocaleId] ASC),
    CONSTRAINT [FK_ZnodeCMSSliderBannerLocale_ZnodeCMSSliderBanner] FOREIGN KEY ([CMSSliderBannerId]) REFERENCES [dbo].[ZnodeCMSSliderBanner] ([CMSSliderBannerId])
);





