insert into ZnodeGlobalMediaDisplaySetting(
MediaId,MaxDisplayItems,MaxSmallThumbnailWidth,MaxSmallWidth,MaxMediumWidth,MaxThumbnailWidth
,MaxLargeWidth,MaxCrossSellWidth,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,0,	38,	250,	400,	150	,800,	150,	2,	getdate(),	2,	getdate()
where not exists(select * from ZnodeGlobalMediaDisplaySetting where MaxSmallThumbnailWidth = 38
and MaxSmallWidth = 250 and MaxMediumWidth=400 and MediaId is null and MaxThumbnailWidth=150 and MaxLargeWidth=800 and MaxCrossSellWidth = 150)
