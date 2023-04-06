CREATE PROCEDURE [dbo].[Znode_GetPromotionByPublishProductId]
(@PublishProductIds VARCHAR(1000)
 ,@UserId  INT = -1 
 ,@PortalId  INT = 0 
   
)
AS 
   /* 
    Summary: This Procedure is used to get list of promotions of the Products 
    Unit Testing   
    EXEC Znode_GetPromotionByPublishProductId 45
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			  SET @UserId = CASE WHEN @UserId = 0 THEN -1 ELSE @UserId END 
             DECLARE @TBL_PromotionIds TABLE(PublishProductId INT);
			 DECLARE @ProfileId VARCHAR(2000) = ''
             INSERT INTO @TBL_PromotionIds SELECT Item FROM dbo.split(@PublishProductIds, ',');

			 EXEC [dbo].[Znode_GetUserPortalAndProfile]  @userid,@PortalId OUT, @ProfileId OUT 

             SELECT ZP.PromotionId,zpt.Name AS PromotionType,ZP.EndDate AS ExpirationDate,ZP.StartDate AS ActivationDate, ZPP.PublishProductId,ZP.PromotionMessage  
			 FROM ZnodePromotion AS ZP
             INNER JOIN ZnodePromotionProduct ZPP ON(ZPP.PromotionId = ZP.PromotionId)
             INNER JOIN ZnodePromotionType AS zpt ON(zpt.PromotionTypeId = ZP.PromotionTypeId)
             WHERE EXISTS( SELECT TOP 1 1 FROM @TBL_PromotionIds AS TPI WHERE TPI.PublishProductId = ZPP.PublishProductId )
			 AND ( ZP.PortalId = @portalid OR ZP.PortalId IS NULL )
			 AND EXISTS ( SELECT TOP 1 1 FROM dbo.split(@ProfileId,',') SP WHERE Sp.Item =  Zp.ProfileId OR Zp.ProfileId IS NULL )
         END TRY
         BEGIN CATCH
             DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPromotionByPublishProductId @PublishProductIds = '+CAST(@PublishProductIds AS nvarchar(500));
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPromotionByPublishProductId',
                  @ErrorInProcedure = @ERROR_PROCEDURE,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;