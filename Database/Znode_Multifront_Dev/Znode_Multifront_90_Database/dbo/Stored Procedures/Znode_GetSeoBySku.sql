CREATE PROCEDURE [dbo].[Znode_GetSeoBySku]
(
        @Skus varchar(max),
        @PortalId int,
        @Status BIT = 0 OUT
)
AS
BEGIN
BEGIN TRY
        SET NOCOUNT ON;
        IF OBJECT_ID('tempdb..#Skus') IS NOT NULL

        DROP TABLE #Skus
        SELECT Item INTO #Skus from dbo.Split(@Skus,',')

        SELECT SEOUrl as SeoUrl,SEOCode as SKU 
        FROM ZnodeCMSSeoDetail D
        WHERE PortalId=@PortalId 
        AND EXISTS(select * from #Skus S WHERE D.SEOCode = S.Item)

        SET @Status=1;
END TRY
BEGIN CATCH
set @Status=0;
SELECT ERROR_MESSAGE()
END CATCH
END