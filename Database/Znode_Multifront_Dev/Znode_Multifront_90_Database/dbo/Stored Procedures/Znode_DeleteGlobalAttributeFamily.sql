CREATE PROCEDURE [dbo].[Znode_DeleteGlobalAttributeFamily]
(@GlobalAtrributeFamilyId  VARCHAR(2000),
@status  BIT OUT)


AS

BEGIN
			BEGIN TRY
			 BEGIN TRAN DeleteAttributeFamily
			 DECLARE @AttributeFamilyId TABLE(GlobalAtrributeFamilyId int)

			 INSERT @AttributeFamilyId (GlobalAtrributeFamilyId)
			 SELECT item FROM  dbo.Split(@GlobalAtrributeFamilyId, ',')

			 DECLARE @DeleteFamily TABLE(DeleteFamilyId int)
			 INSERT @DeleteFamily (DeleteFamilyId )
			 SELECT  GlobalAtrributeFamilyId from @AttributeFamilyId AFI
			 inner JOIN ZnodeGlobalAttributeFamily GEFM ON AFI.GlobalAtrributeFamilyId = GEFM.GlobalAttributeFamilyId
			 WHERE NOT EXISTS
			 (
				SELECT TOP 1 1 FROM ZnodeGlobalEntityFamilyMapper GFM where 
				GFM.GlobalAttributeFamilyId = GlobalAtrributeFamilyId
			 )

			DELETE FROM ZnodeGlobalFamilyGroupMapper
			WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeleteFamily AS DF
                 WHERE DF.DeleteFamilyId = ZnodeGlobalFamilyGroupMapper.GlobalAttributeFamilyId
             );

           
             DELETE FROM ZnodeGlobalAttributeFamilyLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeleteFamily AS DF
                 WHERE  DF.DeleteFamilyId = ZnodeGlobalAttributeFamilyLocale.GlobalAttributeFamilyId
             );

             DELETE FROM ZnodeGlobalAttributeFamily
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeleteFamily AS DF
                 WHERE  DF.DeleteFamilyId = ZnodeGlobalAttributeFamily.GlobalAttributeFamilyId
             );


			   IF
             (
                 SELECT COUNT(1)
                 FROM @DeleteFamily
             ) =
             (   -- check statement with counts if equal then data set return true else return false
                 SELECT COUNT(1)
                 FROM @AttributeFamilyId
             )   
                 BEGIN
                     SET @Status = 1;
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
                 END;
             ELSE
                 BEGIN
                     SET @Status = 0;
                     SELECT 1 AS ID,
                            CAST(0 AS BIT) AS [Status];
                 END;
             COMMIT TRAN DeleteAttributeFamily;
			END TRY

		BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			 @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteGlobalAttributeFamily 
			 @GlobalAtrributeFamilyId = '+@GlobalAtrributeFamilyId+',@Status='+CAST(@Status AS VARCHAR(50));  
			  
			  SET @Status =0  
			  SELECT 1 AS ID,@Status AS Status;  
              ROLLBACK TRAN DeleteAttributeFamily;
              EXEC Znode_InsertProcedureErrorLog
                   @ProcedureName = 'Znode_DeleteGlobalAttributeFamily',
                   @ErrorInProcedure = @Error_procedure,
                   @ErrorMessage = @ErrorMessage,
                   @ErrorLine = @ErrorLine,
                   @ErrorCall = @ErrorCall;
       
		END CATCH

END
