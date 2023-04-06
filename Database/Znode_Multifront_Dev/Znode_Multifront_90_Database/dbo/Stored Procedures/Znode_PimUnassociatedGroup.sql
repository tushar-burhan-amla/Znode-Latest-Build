
CREATE PROCEDURE [dbo].[Znode_PimUnassociatedGroup](
       @PimAttributeGroupId  VARCHAR(2000) ,
       @PimAttributeFamilyId INT ,
       @Status               BIT           = 1 OUT ,
       @IsCategory           BIT           = 0)
AS 
    /*
    Summary:  Check and unassociate the group from family if attribute of group is present in pim values then dont
    		  unassociate the group from family
    Unit Testing   
    Begin 
    	Begin Transaction 
    		Exec [Znode_PimUnassociatedGroup]  @PimAttributeFamilyId=77,@PimAttributeGroupId=168, @Status = 0   
     SELECT * FROM ZnodePimFamilyGroupMapper WHERE  PimAttributeGroupId=168 
     SELECT * FROM ZnodeCMSAreaMessageKey
     SELECT * FROM ZnodePimAttributeGroup
     SELECT * FROM ZnodePimAttributevalue WHERE PimAttributeId IN (540,480)
    	Rollback Transaction 
    ENd  
   */
     BEGIN
         BEGIN TRAN B;
         BEGIN TRY
             SET NOCOUNT ON; 
             ---- Declare the table to store the comma seperated data into record format ------------
             DECLARE @TBL_Group TABLE (
                                      ID                  INT ,
                                      PimAttributeGroupId INT,
                                      IsSystemDefained BIT 
									  );
             DECLARE @TBL_GroupWithAttribute TABLE (
                                                   PimAttributeGroupId INT
                                                   );
			 
             ---- Declare this table to find the actual deleted ids -----
             DECLARE @TBL_DeletedCategoryId TABLE (
                                                  id            INT IDENTITY(1 , 1) ,
                                                  CMSCategoryId INT
                                                  );
             INSERT INTO @TBL_Group
                    SELECT ID , item,IsSystemDefined 
                    FROM ZnodePimAttributeGroup  ZPAG 
					INNER JOIN dbo.split ( @PimAttributeGroupId , ',' ) SP ON (SP.Item = ZPAG.PimAttributeGroupId); --- store the comma separeted category id into variable table 


             IF @IsCategory = 0
                 BEGIN
                     INSERT INTO @TBL_GroupWithAttribute
                            SELECT DISTINCT
                                   zfgm.PimAttributeGroupId
                           FROM ZnodePimFamilyGroupMapper AS zfgm 
						  INNER JOIN ZnodePimAttributeFamily AS zaf ON ( zaf.PimAttributeFamilyId = zfgm.PimAttributeFamilyId
                                                                                                                  AND
                                                                                                                  zfgm.IsSystemDefined <> 1 )
                           INNER JOIN @TBL_Group AS tg ON ( tg.PimAttributeGroupId = zfgm.PimAttributeGroupId
                                                                                                    AND
                                                                                                    zfgm.PimAttributeFamilyId = @PimAttributeFamilyId )
					       WHERE tg.IsSystemDefained = 0  ;
                    
                     ---- delete the record  present in variable table 

                     DELETE FROM ZnodePimFamilyGroupMapper
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_GroupWithAttribute AS gwa
                                    WHERE gwa.PimAttributeGroupId = ZnodePimFamilyGroupMapper.PimAttributeGroupId
                                          AND
                                          ZnodePimFamilyGroupMapper.PimAttributeFamilyId = @PimAttributeFamilyId
                                  );
                     SET @Status = 1;
                     SELECT 1 AS ID ,
                                 CASE
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_Group
                                          ) = ( SELECT COUNT(1)
                                                FROM @TBL_GroupWithAttribute
                                              )
                                     THEN CAST(1 AS BIT)
                                     ELSE CAST(0 AS BIT)
                                 END AS Status; -- check the condition data deleted or not 
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @TBL_GroupWithAttribute
                            SELECT DISTINCT
                                   zfgm.PimAttributeGroupId
                            FROM ZnodePimFamilyGroupMapper AS zfgm INNER JOIN ZnodePimAttributeFamily AS zaf ON ( zaf.PimAttributeFamilyId = zfgm.PimAttributeFamilyId
                                                                                                                  AND
                                                                                                                  zfgm.IsSystemDefined <> 1 )
                                                                   INNER JOIN @TBL_Group AS tg ON ( tg.PimAttributeGroupId = zfgm.PimAttributeGroupId
                                                                                                    AND
                                                                                                    zfgm.PimAttributeFamilyId = @PimAttributeFamilyId )
							WHERE tg.IsSystemDefained = 0 ;
                    
                     ------ deleted the record  present in variable table 
                     DELETE FROM ZnodePimFamilyGroupMapper
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_GroupWithAttribute AS gwa
                                    WHERE gwa.PimAttributeGroupId = ZnodePimFamilyGroupMapper.PimAttributeGroupId
                                          AND
                                          ZnodePimFamilyGroupMapper.PimAttributeFamilyId = @PimAttributeFamilyId
                                  );
                     SET @Status = 1;
                     SELECT 1 AS ID ,
                                 CASE
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_Group
                                          ) = ( SELECT COUNT(1)
                                                FROM @TBL_GroupWithAttribute
                                              )
                                     THEN CAST(1 AS BIT)
                                     ELSE CAST(0 AS BIT)
                                 END AS Status; -- check the condition data deleted or not 
                 END;
             COMMIT TRAN B;
         END TRY
         BEGIN CATCH
               DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PimUnassociatedGroup @PimAttributeGroupId = '+@PimAttributeGroupId+',@PimAttributeFamilyId='+CAST(@PimAttributeFamilyId AS VARCHAR(200))+',@IsCategory='+CAST(@IsCategory AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN B;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_PimUnassociatedGroup',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;