CREATE PROCEDURE [dbo].[Znode_DeleteAttributeGroup]
( @PimAttributegroupedId VARCHAR(2000),
  @Status                BIT OUT,
  @IsDebug bit = 0)
AS 
   /*
     Summary: Remove attribute group with details from child tables
     Before delete check is not system defined group or is not associated with any family or attribute 
     output dataset contain the status if passed @PimAttributegroupedId all ids are deleted then this will true other wise false 
     dbo.Split function use to make comma separeted data in table rows 
     Delete table sequence 
     1.[ZnodePimAttributeGroupMapper]
     2.[ZnodePimAttributeGroupLocale]
     3.[ZnodePimAttributeGroup]
     Unit Testing
	 begin tran   
     EXEC Znode_DeleteAttributeGroup 15,1
     rollback tran
  */
     BEGIN
         BEGIN TRY
             BEGIN TRAN DeleteAttributeGroup;
			 -- to hold the group ids
             DECLARE @TBL_DeletedGroupedId TABLE(PimAttributeGroupId INT); 
             INSERT INTO @TBL_DeletedGroupedId(PimAttributeGroupId)
                    SELECT PimAttributeGroupId
                    FROM ZnodePimAttributeGroup AS ZPAG
                         INNER JOIN dbo.Split(@PimAttributegroupedId, ',') AS SP ON(ZPAG.PimAttributeGroupId = SP.Item)
                    -- is system defined check
					WHERE IsSystemDefined <> 1   
                    --      AND NOT EXISTS
                    --(   -- check is not associated with any family or attribute
                    --    SELECT TOP 1 1
                    --    FROM ZnodePimFamilyGroupMapper AS ZPFGM
                    --    WHERE ZPFGM.PimAttributeGroupId = ZPAG.PimAttributeGroupId
                    --);  
            DELETE FROM ZnodePimFamilyGroupMapper
			WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.PimAttributeGroupId = ZnodePimFamilyGroupMapper.PimAttributeGroupId
             );

             DELETE FROM [dbo].[ZnodePimAttributeGroupMapper]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.PimAttributeGroupId = ZnodePimAttributeGroupMapper.PimAttributeGroupId
             );
             DELETE FROM [dbo].[ZnodePimAttributeGroupLocale]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.PimAttributeGroupId = ZnodePimAttributeGroupLocale.PimAttributeGroupId
             );
             DELETE FROM [dbo].[ZnodePimAttributeGroup]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.PimAttributeGroupId = ZnodePimAttributeGroup.PimAttributeGroupId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeletedGroupedId
             ) =
             (   -- check statement with counts if equal then data set return true else return false
                 SELECT COUNT(1)
                 FROM Split(@PimAttributegroupedId, ',')
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
             COMMIT TRAN DeleteAttributeGroup;
         END TRY
         BEGIN CATCH
		    -- SELECT ERROR_MESSAGE()
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteAttributeGroup @PimAttributegroupedId = '+@PimAttributegroupedId+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 1 AS ID,
                    CAST(0 AS BIT) AS [Status];
             SET @Status = 0;
             ROLLBACK TRAN DeleteAttributeGroup;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteAttributeGroup',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;