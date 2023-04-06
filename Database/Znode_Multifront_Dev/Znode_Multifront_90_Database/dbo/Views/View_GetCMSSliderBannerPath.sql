

 CREATE View [dbo].[View_GetCMSSliderBannerPath]
	   AS
		SELECT  c.CMSSliderId,c.Name SliderName,a.CMSSliderBannerId,dbo.FN_GetMediaThumbnailMediaPath(b.Path) MediaPath,Title,ImageAlternateText,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,Description,ActivationDate
		,ExpirationDate

		FROM ZnodeCMSSlider c
		INNER join [dbo].[ZnodeCMSSliderBanner] a   ON (a.CMSSliderId = c.CMSSliderId)
		INNER JOIN ZnodeCMSSliderBannerLocale e ON (e.CMSSliderBannerId = a.CMSSliderBannerId AND  e.LocaleId = (SELECT FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'locale' ))
		left join ZnodeMedia b ON (b.mediaId = a.MediaId)