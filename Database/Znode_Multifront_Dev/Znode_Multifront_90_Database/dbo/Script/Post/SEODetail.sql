--dt 30-07-2020 ZPD-11579 --> ZPD-11731
update ZnodeCMSContentPageslocale set PageTitle  = 'Landing Page' 
where CMSContentPagesId = ( select top 1 CMSContentPagesId from ZnodeCMSContentPages where PageNAme = 'Landing Page' )

update  ZnodeCMSSEODetail set MetaInformation = 'Landing-Page' , SEOUrl = 'Landing-Page' where SEOCode = 'Landing Page' 

update ZnodeCMSSEODetailLocale set SEOTitle = 'Landing Page' ,SEOKeywords = 'Landing Page' 
 where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Landing Page')

UPDATE ZnodeCMSSEODetail SET SEOUrl=LTRIM(TRIM(SEOUrl));

--ZPD-19716 Dt.12-July-2022
IF OBJECT_ID('tempdb..#ZnodeCMSSeoDetail') IS NOT NULL
    DROP TABLE #ZnodeCMSSeoDetail

SELECT A.CMSSEODetailId
INTO #ZnodeCMSSeoDetail
FROM 
(
    SELECT CMSSEODetailId,ROW_NUMBER() OVER (PARTITION BY SEOCode ORDER BY CreatedDate DESC) As Rn 
	FROM ZnodeCMSSeoDetail
	WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Brand')
) A WHERE A.Rn>1
IF EXISTS (SELECT TOP 1 1 FROM #ZnodeCMSSeoDetail)
BEGIN
	DELETE FROM ZnodeCMSSeoDetailLocale 
	WHERE CMSSEODetailId IN (SELECT CMSSEODetailId FROM #ZnodeCMSSeoDetail)

	DELETE 
	FROM ZnodeCMSSeoDetail 
	WHERE CMSSEODetailId IN (SELECT CMSSEODetailId FROM #ZnodeCMSSeoDetail)

	UPDATE ZnodeCMSSeoDetail
	SET PortalId=NULL
	WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Brand')
END