
--dt 27-03-2020 ZPD-7626 --> ZPD-9585
insert into ZnodePortalFeature (PortalFeatureName,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate)
select 'Enable_CMS_Page_Results',2,getdate(),2,getdate()
where not exists(select * from ZnodePortalFeature where PortalFeatureName = 'Enable_CMS_Page_Results')

INSERT INTO ZnodePortalFeature (PortalFeatureName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'Enable_Barcode_Scanner',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodePortalFeature WHERE PortalFeatureName='Enable_Barcode_Scanner')

insert into ZnodePortalFeature(PortalFeatureName,	CreatedBy,	CreatedDate	,ModifiedBy,	ModifiedDate)
select 'Enable_Save_For_Later',	2,	getdate(),	2	,getdate()
where not exists(select * from ZnodePortalFeature where PortalFeatureName = 'Enable_Save_For_Later')

insert into ZnodePortalFeature(PortalFeatureName,	CreatedBy,	CreatedDate	,ModifiedBy,	ModifiedDate)
select 'Enable_Product_Inheritance',	2,	getdate(),	2	,getdate()
where not exists(select * from ZnodePortalFeature where PortalFeatureName = 'Enable_Product_Inheritance')

--dt 23-Nov-2022 ZPD-21185
insert into ZnodePortalFeature(PortalFeatureName,	CreatedBy,	CreatedDate	,ModifiedBy,	ModifiedDate)
select 'Enable_Add_To_Cart_Option_For_Product_Sliders',	2,	getdate(),	2	,getdate()
where not exists(select * from ZnodePortalFeature where PortalFeatureName = 'Enable_Add_To_Cart_Option_For_Product_Sliders')