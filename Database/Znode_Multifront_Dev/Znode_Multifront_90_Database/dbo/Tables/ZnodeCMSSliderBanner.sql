CREATE TABLE [dbo].[ZnodeCMSSliderBanner] (
    [CMSSliderBannerId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSSliderId]       INT            NOT NULL,
    [TextAlignment]     NVARCHAR (100) NOT NULL,
    [BannerSequence]    INT            NULL,
    [ActivationDate]    DATETIME       NULL,
    [ExpirationDate]    DATETIME       NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSliderBanner] PRIMARY KEY CLUSTERED ([CMSSliderBannerId] ASC),
    CONSTRAINT [FK_ZnodeCMSSliderBanner_ZnodeCMSSlider] FOREIGN KEY ([CMSSliderId]) REFERENCES [dbo].[ZnodeCMSSlider] ([CMSSliderId])
);

















