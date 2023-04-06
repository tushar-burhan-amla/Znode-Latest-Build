

CREATE view  [dbo].[View_GetCMSCustomerReviewInformation] AS 
SELECT CMSCustomerReviewId,a.PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,Status,ZPPD.ProductName,ZPPD.LocaleId
,CONVERT(DATE,a.CreatedDate)CreatedDate
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,a.CreatedBy,a.ModifiedBy,ZCSD.SEOUrl,ZCSD.PortalId
FROM ZNODECMSCUSTOMERREVIEW A 
INNER JOIN ZnodePublishProductDetail ZPPD ON (A.PUBLISHPRODUCTID = ZPPD.PUBLISHPRODUCTID)
LEFT OUTER JOIN ZnodeCMSSEODetail ZCSD on ZPPD.SKU = ZCSD.SEOCode
LEFT OUTER JOIN ZnodeCMSSEOType ZCST ON ZCSD.CMSSEOTypeId = ZCST.CMSSEOTypeId AND ZCST.NAME = 'Product'