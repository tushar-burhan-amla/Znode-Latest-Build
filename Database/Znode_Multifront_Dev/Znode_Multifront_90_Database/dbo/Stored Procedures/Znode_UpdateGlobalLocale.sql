CREATE PROCEDURE [dbo].[Znode_UpdateGlobalLocale]
(
@Status  BIT =0  OUT

)
AS
   /*
     Summary : To Insert / Update Locale 
     Update Logic: 

	 exec [Znode_UpdateGlobalLocale] 
*/
     BEGIN
         
         BEGIN TRY
		 SET NOCOUNT ON
			DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		    DECLARE @TBL_PortalDefaultLocale TABLE (PortalId INT , LocaleId INT,UserId INT )

			INSERT INTO @TBL_PortalDefaultLocale (PortalId,LocaleId,UserId)
			SELECT PortalId , LocaleId ,CreatedBy
			FROM ZnodePortalLocale 
			WHERE IsDefault = 1 

			DECLARE @DefaultLocale INT = dbo.FN_GetDefaultLocaleId()
			
		    DELETE ZnodePortalLocale WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeLocale ZL WHERE ZL.LocaleId = ZnodePortalLocale.LocaleId AND ZL.IsActive = 0 )
		    
			-- 
			UPDATE ZnodePortalLocale 
			SET IsDefault = 1 
			WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeLocale ZL WHERE ZL.LocaleId = ZnodePortalLocale.LocaleId AND ZL.IsDefault = 1  ) 
			AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalLocale XL WHERE XL.PortalId = ZnodePortalLocale.PortalId AND XL.IsDefault = 1  )
			
			INSERT INTO ZnodePortalLocale (PortalId,LocaleId,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT PortalId,@DefaultLocale,1,UserId,@GetDate,UserId,@GetDate
			FROM ZnodeLocale ZL 
			CROSS APPLY @TBL_PortalDefaultLocale TBPDL
			WHERE ZL.IsActive = 1 AND ZL.IsDefault = 1 
			AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalLocale ZPL WHERE ZPL.PortalId = TBPDL.PortalId AND ZPL.LocaleId = TBPDL.LocaleId  AND ZPL.IsDefault = 1  )

			--SELECT PortalId,@DefaultLocale,1,UserId,@GetDate,UserId,@GetDate
			--FROM @TBL_PortalDefaultLocale TBPDL 
			--WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalLocale ZPL WHERE ZPL.PortalId = TBPDL.PortalId AND ZPL.LocaleId = TBPDL.LocaleId )
		
         SET @Status =1 
		 SELECT 1 ID , CAST(1 AS BIT ) Status

		 END TRY

		 BEGIN CATCH
		  SET @Status =1 
		 SELECT 1 ID , CAST(0 AS BIT ) Status
		 SELECT ERROR_MESSAGE()
		 END CATCH

		END