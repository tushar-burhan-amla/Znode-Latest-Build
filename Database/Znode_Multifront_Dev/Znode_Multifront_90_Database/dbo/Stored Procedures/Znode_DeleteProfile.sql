CREATE PROCEDURE [dbo].[Znode_DeleteProfile]
( @ProfileId VARCHAR(2000),
  @Status    INT OUT
 , @IsForceFullyDelete BIT = 0 )
AS 
   /*
     Summary : Remove profile only when this is not mapped with other entity 
     Check existence of profileid in other tables 
    1. ZnodeShipping
    2. ZnodeUserProfile
    3. ZnodePriceListProfile
    4. ZnodeProfilePromotion  ( Remove table ) 
    5. ZnodePortalProfile
    Unit Testing : 
	BEGIN TRAN
    Declare @Status  int 
    Exec [Znode_DeleteProfile]  @ProfileId =  ,@Status = @Status  oUt
    ROLLBACK TRAN
   */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeleteProfileId TABLE(ProfileId INT);
             INSERT INTO @DeleteProfileId
                    SELECT Item
                    FROM dbo.split(@ProfileId, ',') AS a
					WHERE @ProfileId <> '' 
					;
             DECLARE @V_tabledeleted TABLE(ProfileId INT);
             INSERT INTO @V_tabledeleted
                    SELECT a.ProfileId
                    FROM @DeleteProfileId AS a
                    WHERE (( NOT EXISTS
                           
                    (
                        SELECT TOP 1 1
                        FROM ZnodeUserProfile AS d
                        WHERE d.ProfileId = a.ProfileId
                    )
                         
                          AND NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePortalProfile AS f
                        WHERE f.ProfileId = a.ProfileId
                    )) OR @IsForceFullyDelete =1 ) ;
           
			DELETE FROM ZnodeProfileShipping
			WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodeProfileShipping.ProfileId)

			DELETE FROM ZnodeProfilePaymentSetting
			WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodeProfilePaymentSetting.ProfileId )
						
			 DELETE FROM ZnodeUserProfile  WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodeUserProfile.ProfileId)

			DELETE FROM ZnodePriceListProfile WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePortalProfile 	WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodePortalProfile.ProfileId) AND ZnodePortalProfile.PortalProfileId = ZnodePriceListProfile.PortalProfileId)

			DELETE FROM ZnodePortalProfile 	WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodePortalProfile.ProfileId)

			DELETE FROM ZnodeAccountProfile WHERE EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted DAI
			WHERE DAI.ProfileId = ZnodeAccountProfile.ProfileId) 
			 
			  DELETE FROM znodecmscontentpagesprofile   WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_tabledeleted AS DAI
                 WHERE DAI.ProfileId = znodecmscontentpagesprofile.ProfileId
             );

			 DELETE FROM ZnodeSearchProfileTrigger   WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_tabledeleted AS DAI
                 WHERE DAI.ProfileId = ZnodeSearchProfileTrigger.ProfileId
             );

			 DELETE FROM ZnodeCMSWidgetProfileVariant
			 WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_tabledeleted AS DAI
                 WHERE DAI.ProfileId = ZnodeCMSWidgetProfileVariant.ProfileId
             );

			   DELETE FROM ZnodeProfile
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_tabledeleted AS DAI
                 WHERE DAI.ProfileId = ZnodeProfile.ProfileId
             );


             IF
             (
                 SELECT COUNT(1)
                 FROM @V_tabledeleted
             ) =
             (
                 SELECT COUNT(1)
                 FROM @DeleteProfileId
             )
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
		   SELECT ERROR_MESSAGE()
          DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteProfile @ProfileId = '+@ProfileId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteProfile',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;