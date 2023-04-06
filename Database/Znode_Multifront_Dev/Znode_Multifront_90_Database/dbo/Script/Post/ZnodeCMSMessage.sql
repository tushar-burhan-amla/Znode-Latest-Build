--dt\14\11\2019 ZPD-7906

INSERT INTO ZnodeCMSMessageKey(MessageKey,MessageTag,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'RecommendedProductsTitle',NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSMessageKey WHERE MessageKey='RecommendedProductsTitle')

INSERT INTO ZnodeCMSMessage(LocaleId,Message,IsPublished,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishStateId)
SELECT 1,'<p>Recommended Products</p>',NULL,2,GETDATE(),2,GETDATE(),3
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSMessage WHERE Message='<p>Recommended Products</p>')

INSERT INTO ZnodeCMSPortalMessage(PortalId,CMSMessageKeyId,CMSMessageId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,(SELECT TOP 1 CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecommendedProductsTitle'),
(SELECT TOP 1 CMSMessageId  FROM ZnodeCMSMessage WHERE Message='<p>Recommended Products</p>'),2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSPortalMessage WHERE CMSMessageKeyId IN (SELECT CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecommendedProductsTitle'))
AND NOT EXISTS 
(SELECT * FROM ZnodeCMSPortalMessage WHERE CMSMessageId IN (SELECT CMSMessageId  FROM ZnodeCMSMessage WHERE Message='<p>Recommended Products</p>') and PortalId is null)

GO
--dt 05-02-2020 ZPD-9011
update c set Message = '<div class="row">  <div class="col-12 col-md-6 col-lg-12 p-3 promo-one"><img class="img-fluid w-100" src="http://api9x.znodellc.com/Data/Media/1f0b6a6e-9e09-445a-b314-75f1bbee5abb10percent-Dewalt.png" alt="" /></div>  <div class="col-12 col-md-6 col-lg-12 p-3"><img class="img-fluid w-100" src="http://api9x.znodellc.com/Data/Media/9c27a9ad-339f-43b9-847c-41a12d2a37f0freebattery-mketool.png" alt="" /></div>  <div class="col-12 col-md-6 col-lg-12 p-3"><img class="img-fluid w-100" src="http://api9x.znodellc.com/Data/Media/1f0b6a6e-9e09-445a-b314-75f1bbee5abb10percent-Dewalt.png" alt="" /></div>  </div>' 
from ZnodeCMSMessageKey a
inner join ZnodeCMSPortalMessage b on a.CMSMessageKeyId = b.CMSMessageKeyId
inner join ZnodeCMSMessage c on b.CMSMessageId = c.CMSMessageId
where a.MessageKey = 'PromoSpot'
go

--dt 07-02-2020 ZPD-8904
update c set message = replace(replace(message,'2019','2020'),'2018','2020')
from ZnodeCMSPortalMessage a
inner join znodecmsmessagekey b on a.CMSMessageKeyId = b.CMSMessageKeyId
inner join znodecmsmessage c on a.CMSMessageId = c.CMSMessageId
 where b.messagekey = 'FooterCopyrightText'

 --dt 09-09-2020 ZPD-12270
if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin 
INSERT INTO ZnodeCMSMessage(LocaleId,Message,IsPublished,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishStateId)
SELECT 1,'<p>Recently Viewed Products</p>',NULL,2,GETDATE(),2,GETDATE(),3
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSMessage WHERE Message='<p>Recently Viewed Products</p>')

insert into ZnodeCMSMessageKey(MessageKey,MessageTag,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'RecentlyViewProduct',null,2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodeCMSMessageKey where MessageKey = 'RecentlyViewProduct')

INSERT INTO ZnodeCMSPortalMessage(PortalId,CMSMessageKeyId,CMSMessageId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,(SELECT TOP 1 CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecentlyViewProduct'),
(SELECT TOP 1 CMSMessageId  FROM ZnodeCMSMessage WHERE Message='<p>Recently Viewed Products</p>'),2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSPortalMessage WHERE CMSMessageKeyId IN (SELECT CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecentlyViewProduct'))
AND NOT EXISTS 
(SELECT * FROM ZnodeCMSPortalMessage WHERE CMSMessageId IN (SELECT CMSMessageId  FROM ZnodeCMSMessage WHERE Message='<p>Recently Viewed Products</p>') and PortalId is null)

insert into ZnodeCMSPortalMessageKeyTag(PortalId,CMSMessageKeyId,TagXML,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,(SELECT TOP 1 CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecentlyViewProduct'),null,2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodeCMSPortalMessageKeyTag where PortalId is null 
and CMSMessageKeyId = (SELECT TOP 1 CMSMessageKeyId FROM ZnodeCMSMessageKey WHERE MessageKey='RecentlyViewProduct'))
end