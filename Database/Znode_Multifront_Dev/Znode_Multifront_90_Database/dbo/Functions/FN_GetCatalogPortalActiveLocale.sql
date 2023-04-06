CREATE FUNCTION DBO.FN_GetCatalogPortalActiveLocale
(
    @PublishCatalogId INT
)
RETURNS @Items TABLE
(
    LocaleId Int
)
AS
BEGIN
    INSERT INTO @Items(LocaleId)
    SELECT LocaleId FROM ZnodeLocale MT WHERE IsActive =1 
    AND EXISTS(SELECT * FROM ZnodePortalCatalog ZPC 
        INNER JOIN ZnodePortalLocale ZPL ON ZPC.PortalId = ZPL.PortalId
        WHERE ZPC.PublishCatalogId = @PublishCatalogId AND MT.LocaleId = ZPL.LocaleId )
        --AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

    --If catalog not associated with stire then getting default active locale for catalog
    IF NOT EXISTS(SELECT * FROM @Items)
        INSERT INTO @Items (LocaleId) 
        SELECT LocaleId FROM ZnodeLocale MT WHERE IsActive =1 and IsDefault = 1

    RETURN
END