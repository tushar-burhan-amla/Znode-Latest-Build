Create PROCEDURE [dbo].[Znode_DeletePimAttributeWithReference](
       @PimAttributeId VARCHAR(max) = '' ,
       @Status         INT OUT,
	   @PimAttributeIds TransferId READONLY 
	   
	   
	   )
AS 
    -----------------------------------------------------------------------------
    --Summary:  Remove PimAttribute still in used 
    --		   	
    --          
    --Unit Testing   
	--Begin Transaction 
		--DECLARE @Status INT  EXEC Znode_DeletePimAttributeWithReference @PimAttributeId = '59,60,61,62' ,@Status=@Status OUT  SELECT @Status
		--select * from ZnodePimAttributeValue where PimAttributeId in (59,60,61,62)
		--select * from ZnodePimattribute where AttributeCode in ( 'SpecValue','TempSettings','UPCcode', 'ratest') 	
	--Rollback Transaction 
    ----------------------------------------------------------------------------- 


     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
			  DECLARE @FinalCount INT =0 
             DECLARE @DeletdAttributeId TABLE (
                                              PimAttributeId INT 
                                              );
             INSERT INTO @DeletdAttributeId
                    SELECT Item
                    FROM dbo.split ( @PimAttributeId , ','
                                   ) AS a INNER JOIN ZnodePimAttribute AS B ON ( a.item = b.PimAttributeId )
                    WHERE b.IsSystemDefined <> 1
					AND @PimAttributeId <> ''
                          
			 INSERT INTO @DeletdAttributeId 
			 SELECT Id  
			 FROM @PimAttributeIds a 
			 INNER JOIN ZnodePimAttribute AS B ON ( a.id = b.PimAttributeId )   
			 WHERE b.IsSystemDefined <> 1            

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

			DELETE FROM ZnodePimCategoryAttributeValueLocale 
			WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd 
							Inner join ZnodePimCategoryAttributeValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValueLocale.PimCategoryAttributeValueId);

			 DELETE FROM ZnodePimAttributeValueLocale 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodePimAttributeValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimAttributeValueId = ZnodePimAttributeValueLocale.PimAttributeValueId);
			
            DELETE FROM ZnodePimProductAttributeMedia 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodePimAttributeValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimAttributeValueId = ZnodePimProductAttributeMedia.PimAttributeValueId);
			DELETE FROM ZnodePimProductAttributeTextAreaValue 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodePimAttributeValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimAttributeValueId = ZnodePimProductAttributeTextAreaValue.PimAttributeValueId);
			 DELETE FROM ZnodePimCategoryAttributeValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimCategoryAttributeValue.PimAttributeId
                          );
			 DELETE FROM ZnodePimProductAttributeDefaultValue 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodePimAttributeValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId);

             --Disable the constraint  
			 ALTER TABLE ZnodePimAttributeValue NOCHECK CONSTRAINT ALL

			 DELETE FROM ZnodePimAttributeValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeValue.PimAttributeId
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
           
			 DELETE FROM ZnodePimProductAttributeDefaultValue 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodePimAttributeDefaultValue AS zpav ON sd.PimAttributeId=zpav.PimAttributeId
                            WHERE zpav.PimAttributeDefaultValueId = ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId);

				

			 DELETE FROM ZnodePimAttributeDefaultValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.PimAttributeId = ZnodePimAttributeDefaultValue.PimAttributeId
                          );
			--Enable the constraint 		  
              ALTER TABLE ZnodePimAttributeValue CHECK CONSTRAINT ALL

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
               SET @FinalCount = 	( SELECT COUNT(1) FROM dbo.split ( @PimAttributeId , ',') AS a WHERE @PimAttributeId <> '')
			 SET @FinalCount = 	CASE WHEN @FinalCount = 0 THEN  ( SELECT COUNT(1) FROM @PimAttributeIds AS a ) ELSE   @FinalCount END 
			
		     IF ( SELECT COUNT(1)
                  FROM @DeletdAttributeId
                ) = @FinalCount
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
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
             SELECT ERROR_MESSAGE() , ERROR_LINE() , ERROR_PROCEDURE();
             SET @Status = 0;
             ROLLBACK TRAN A;
         END CATCH;
     END;