
	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Landing Form Info Template','LandingFormInfoTemplate',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Landing Form Info Template')

	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider','LandingBannerContentListTemplate',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')

	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Simple Search With Image And Multiple Text Widgets','SimpleProductPromotionImageContent',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Simple Search With Image And Multiple Text Widgets')

	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Simple Search With Content And Banner Slider','SimpleSearchBannerContent',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Simple Search With Content And Banner Slider')

	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Simple Search With Video','SimpleSearchBannerContent',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Simple Search With Video')

	insert into ZnodeCMSTemplate(Name,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,MediaId)
	select 'Landing Page With Content And Form Sections','LandingFormInfoTemplate',2,getdate(),2,getdate(),null
	where not exists(select * from ZnodeCMSTemplate where Name = 'Landing Page With Content And Form Sections')
