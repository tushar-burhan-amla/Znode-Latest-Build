
--dt 28-01-2020 ZPD-8291 --> ZPD-8921
insert into ZnodeApplicationCache(ApplicationType,IsActive,StartDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Duration)
select 'CloudflareCache' ApplicationType,1 IsActive,GETDATE() StartDate,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate,0 Duration
where not exists(select * from ZnodeApplicationCache where ApplicationType = 'CloudflareCache' )

--dt 31-03-2020 ZPD-9507 and ZPD-9257
truncate table ZnodePublishedXml
truncate table ZnodePublishProductAttributeXML
truncate table ZnodePublishCatalogProductDetail
