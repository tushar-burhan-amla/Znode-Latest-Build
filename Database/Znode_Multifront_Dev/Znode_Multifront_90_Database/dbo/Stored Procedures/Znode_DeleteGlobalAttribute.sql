


CREATE PROCEDURE [dbo].[Znode_DeleteGlobalAttribute](
       @GlobalAttributeId VARCHAR(300) = NULL ,
       @Status         INT OUT,
	   @GlobalAttributeIds TransferId READONLY, 
	   @IsForceFullyDelete BIT =0   )
AS 
    -----------------------------------------------------------------------------
    --Summary:  Remove GlobalAttribute still in used 
    --		   	
    --          
    --Unit Testing   
	--Begin Transaction 
		--DECLARE @Status INT  EXEC Znode_DeleteGlobalAttribute @GlobalAttributeId = '59,60,61,62' ,@Status=@Status OUT  SELECT @Status
		--select * from ZnodeGlobalAttributeValue where GlobalAttributeId in (59,60,61,62)
		--select * from ZnodeGlobalAttribute where AttributeCode in ( 'SpecValue','TempSettings','UPCcode', 'ratest') 	
	--Rollback Transaction 
    ----------------------------------------------------------------------------- 


     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
			 DECLARE @FinalCount INT =0 
             DECLARE @DeletdAttributeId TABLE (
                                              GlobalAttributeId INT
                                              );
            INSERT INTO @DeletdAttributeId
                    SELECT Item
                    FROM dbo.split ( @GlobalAttributeId , ','
                                   ) AS a 
					INNER JOIN ZnodeGlobalAttribute AS B ON ( CAST(a.item AS INT )  = b.GlobalAttributeId )
					Where 
					not exists(  Select 1 
					from ZnodeGlobalAttributeGroupMapper dd
					where dd.GlobalAttributeId =b.GlobalAttributeId)
					and ISNULL(b.IsSystemDefined,0) <> 1
             		AND @GlobalAttributeId <> ''
			 INSERT INTO  @DeletdAttributeId 
				 SELECT id 
                    FROM @GlobalAttributeIds AS a 
					INNER JOIN ZnodeGlobalAttribute AS B ON ( a.Id = b.GlobalAttributeId )
					AND ISNULL(b.IsSystemDefined,0) <> 1
			       
             DELETE FROM ZnodeGlobalAttributeLocale
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeLocale.GlobalAttributeId
                          );
             DELETE FROM ZnodeGlobalAttributeValidation
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeValidation.GlobalAttributeId
                          );

			 DELETE FROM ZnodeGlobalAttributeValueLocale 
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd Inner join ZnodeGlobalAttributeValue AS zpav ON sd.GlobalAttributeId=zpav.GlobalAttributeId
                            WHERE zpav.GlobalAttributeValueId = ZnodeGlobalAttributeValueLocale.GlobalAttributeValueId);

             DELETE FROM ZnodeGlobalAttributeValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeValue.GlobalAttributeId
                          );
			 DELETE FROM ZnodeGlobalAttributeDefaultValueLocale
             WHERE EXISTS ( SELECT 1
                            FROM ZnodeGlobalAttributeDefaultValue
                            WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeDefaultValue.GlobalAttributeId
                                         )
                                  AND
                                  ZnodeGlobalAttributeDefaultValueLocale.GlobalAttributeDefaultValueId = ZnodeGlobalAttributeDefaultValue.GlobalAttributeDefaultValueId
                          );
           
		    DELETE ZA FROM ZnodePortalGlobalAttributeValueLocale ZA  INNER JOIN ZnodePortalGlobalAttributeValue dg ON (dg.PortalGlobalAttributeValueId = ZA.PortalGlobalAttributeValueId) 
			 WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = dg.GlobalAttributeId
                                         )
			 DELETE FROM ZnodePortalGlobalAttributeValue 
			 WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = ZnodePortalGlobalAttributeValue.GlobalAttributeId
                                         )
           	 DELETE ZA FROM  ZnodeFormBuilderGlobalAttributeValueLocale	ZA INNER JOIN ZnodeFormBuilderGlobalAttributeValue dg ON (dg.FormBuilderGlobalAttributeValueId = ZA.FormBuilderGlobalAttributeValueId) 
			  WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = dg.GlobalAttributeId
                                         )
			 DELETE FROM ZnodeFormBuilderGlobalAttributeValue
			  WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = ZnodeFormBuilderGlobalAttributeValue.GlobalAttributeId
                                         )
			 DELETE FROM ZnodeFormBuilderAttributeMapper
			   WHERE EXISTS ( SELECT 1
                                           FROM @DeletdAttributeId AS sd
                                           WHERE sd.GlobalAttributeId = ZnodeFormBuilderAttributeMapper.GlobalAttributeId
                                         )


			 DELETE FROM ZnodeGlobalAttributeDefaultValue
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeDefaultValue.GlobalAttributeId
                          );
			 DELETE ZA FROM ZnodeUserGlobalAttributeValueLocale ZA INNER JOIN ZnodeUserGlobalAttributeValue dg ON (dg.UserGlobalAttributeValueId = ZA.UserGlobalAttributeValueId) 
			    WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = dg.GlobalAttributeId
                          );
			 DELETE FROM ZnodeUserGlobalAttributeValue 
			  WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeUserGlobalAttributeValue.GlobalAttributeId
                          );	
						  
		     DELETE FROM ZnodeAccountGlobalAttributeValueLocale WHERE AccountGlobalAttributeValueId IN (SELECT AccountGlobalAttributeValueId FROM ZnodeAccountGlobalAttributeValue WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeAccountGlobalAttributeValue.GlobalAttributeId
                          ) )		  


			 DELETE FROM ZnodeAccountGlobalAttributeValue  WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeAccountGlobalAttributeValue.GlobalAttributeId
                          ) 		  	
						  						  						  
									  		  
			DELETE FROM ZnodeGlobalAttributeGroupMapper  
				 WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttributeGroupMapper.GlobalAttributeId
                          );	
						  
			DELETE FROM ZnodeWidgetGlobalAttributeValueLocale
			WHERE EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValue
						 WHERE EXISTS ( SELECT 1FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeWidgetGlobalAttributeValue.GlobalAttributeId
                          ) AND ZnodeWidgetGlobalAttributeValueLocale.WidgetGlobalAttributeValueId = ZnodeWidgetGlobalAttributeValue.WidgetGlobalAttributeValueId);
			
			DELETE FROM ZnodeWidgetGlobalAttributeValue
			WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeWidgetGlobalAttributeValue.GlobalAttributeId
                          );

             DELETE FROM ZnodeGlobalAttribute
             WHERE EXISTS ( SELECT 1
                            FROM @DeletdAttributeId AS sd
                            WHERE sd.GlobalAttributeId = ZnodeGlobalAttribute.GlobalAttributeId
                          );
              SET @FinalCount = 	( SELECT COUNT(1) FROM dbo.split ( @GlobalAttributeId , ',')   AS a WHERE @GlobalAttributeId <> '')
			 SET @FinalCount = 	CASE WHEN @FinalCount = 0 OR @FinalCount IS nULL  THEN  ( SELECT COUNT(1) FROM @GlobalAttributeIds AS a ) ELSE   @FinalCount END 
			
			

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