
CREATE PROCEDURE [dbo].[Znode_DeletePimAttribute](
       @PimAttributeId VARCHAR(300) = NULL ,
       @Status         INT OUT)
AS 
  /*
    Summary:  Remove PimAttribute with their respective details and from reference tables		   	             
    Unit Testing   
    			SELECT * FROM zNodeUser
                SELECT * FROM AspNetUserRoles b  INNER JOIN
                AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
                AspNetUsers d ON (b.userId = d .id)   WHERE d.Username ='admin12345' AND c.Name = 'Admin'
     DECLARE @Status INT 
	 EXEC Znode_DeletePimAttribute @PimAttributeId = 437 ,@Status=@Status OUT  
    */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeletdAttributeId TABLE (
                                              PimAttributeId INT
                                              );
             INSERT INTO @DeletdAttributeId
                    SELECT Item
                    FROM dbo.split ( @PimAttributeId , ','
                                   ) AS a INNER JOIN ZnodePimAttribute AS B ON ( a.item = b.PimAttributeId )
                    WHERE b.IsSystemDefined <> 1
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimAttributeGroupMapper AS c
                                       WHERE c.PimAttributeId = b.PimAttributeId
                                     )
                         
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimAttributeValue AS e
                                       WHERE e.PimAttributeId = b.PimAttributeId
                                     )
                         
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimFamilyGroupMapper AS g
                                       WHERE g.PimAttributeId = b.PimAttributeId
                                     )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimLinkProductDetail AS h
                                       WHERE h.PimAttributeId = b.PimAttributeId
                                     )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimProductImage AS i
                                       WHERE i.PimAttributeId = b.PimAttributeId
                                     )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimProductTypeAssociation AS k
                                       WHERE k.PimAttributeId = b.PimAttributeId
                                     );
           
             DELETE FROM ZnodePimAttributeGroupMapper
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeGroupMapper.PimAttributeId
                          );
             DELETE FROM ZnodePimAttributeLocale
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeLocale.PimAttributeId
                          );
             DELETE FROM ZnodePimAttributeValidation
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeValidation.PimAttributeId
                          );
             DELETE FROM ZnodePimAttributeDefaultValueLocale
             WHERE EXISTS ( SELECT 1
                            FROM ZnodePimAttributeDefaultValue
                            WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.PimAttributeId = ZnodePimAttributeDefaultValue.PimAttributeId
                                         )
                                  AND
                                  ZnodePimAttributeDefaultValueLocale.PimAttributeDefaultValueId = ZnodePimAttributeDefaultValue.PimAttributeDefaultValueId
                          );
             DELETE FROM ZnodePimAttributeDefaultValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeDefaultValue.PimAttributeId
                          );
             DELETE FROM ZnodePimAttributeValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeValue.PimAttributeId
                          );
             DELETE FROM ZnodePimFamilyGroupMapper
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimFamilyGroupMapper.PimAttributeId
                          );
             DELETE FROM ZnodePimLinkProductDetail
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimLinkProductDetail.PimAttributeId
                          );
             DELETE FROM ZnodePimProductImage
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimProductImage.PimAttributeId
                          );
         
             DELETE FROM ZnodePimProductTypeAssociation
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimProductTypeAssociation.PimAttributeId
                          );
             DELETE FROM ZnodePimFrontendProperties
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimFrontendProperties.PimAttributeId
                          );
             DELETE FROM ZnodePimAttribute
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttribute.PimAttributeId
                          );
             IF ( SELECT COUNT(1)
                  FROM @DeletdAttributeId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @PimAttributeId , ','
                                     ) AS a
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimAttribute @PimAttributeId = '+@PimAttributeId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimAttribute',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;