
CREATE PROCEDURE [dbo].[Znode_DeleteMediaAttribute]
( @MediaAttributeId VARCHAR(2000) = NULL,
  @Status           INT OUT)
AS
/*
 Summary: Remove Media Attribute with their reference data 
		  This Procedure is used to check the media is associated to the product or not

 Unit Testing:
 begin tran
 DECLARE @DD INT = NULL  
 EXEC Znode_DeleteMediaAttribute '82',@DD OUT 
 rollback tran

*/
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @MediaAttribute TABLE(Item INT);
             INSERT INTO @MediaAttribute
                    SELECT Item
                    FROM dbo.split(@MediaAttributeId, ',') AS a
                         INNER JOIN ZnodeMediaAttribute AS b ON(a.Item = b.MediaAttributeId)
                    WHERE 
					--NOT EXISTS
     --               (
     --                   SELECT TOP 1 1
     --                   FROM ZnodeMediaAttributeGroupMapper AS f
     --                   WHERE f.MediaAttributeId = b.MediaAttributeId
     --               )
                          --AND 
						  --NOT EXISTS
        --            (
        --                SELECT TOP 1 1
        --                FROM ZnodeMediaAttributeValue AS g
        --                WHERE g.MediaAttributeId = b.MediaAttributeId
        --            )
                    --      AND NOT EXISTS
                    --(
                    --    SELECT TOP 1 1
                    --    FROM ZnodeMediaFamilyGroupMapper AS h
                    --    WHERE h.MediaAttributeId = b.MediaAttributeId
                    --)
                         -- AND
						   b.IsSystemDefined <> 1;
             DELETE FROM ZnodeMediaAttributeLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttributeLocale.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaAttributeValue
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttributeValue.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaFamilyGroupMapper
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaFamilyGroupMapper.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaAttributeGroupMapper
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttributeGroupMapper.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaAttributeDefaultValueLOcale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeMediaAttributeDefaultValue AS aaa
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @MediaAttribute AS a
                     WHERE a.item = aaa.MediaAttributeId
                           AND a.Item NOT IN
                     (
                         SELECT MediaAttributeId
                         FROM ZnodeMediaAttribute
                         WHERE IsSystemDefined = 1
                     )
                 )
                       AND aaa.MediaAttributeDefaultValueId = ZnodeMediaAttributeDefaultValueLOcale.MediaAttributeDefaultValueId
             );
             DELETE FROM ZnodeMediaAttributeDefaultValue
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttributeDefaultValue.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaAttributeValidation
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttributeValidation.MediaAttributeId
                       AND a.Item NOT IN
                 (
                     SELECT MediaAttributeId
                     FROM ZnodeMediaAttribute
                     WHERE IsSystemDefined = 1
                 )
             );
             DELETE FROM ZnodeMediaAttribute
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @MediaAttribute AS a
                 WHERE a.item = ZnodeMediaAttribute.MediaAttributeId
             )
                   AND ZnodeMediaAttribute.IsSystemDefined <> 1;
             IF
             (
                 SELECT COUNT(1)
                 FROM @MediaAttribute
             ) =
             (
                 SELECT COUNT(1)
                 FROM dbo.split(@MediaAttributeId, ',')
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
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMediaAttribute @MediaAttributeId='+@MediaAttributeId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMediaAttribute',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;