


CREATE PROCEDURE [dbo].[Znode_DeleteGlobalAttributeGroup]
( @GlobalAttributeGroupId VARCHAR(2000),
  @Status                BIT OUT,
  @IsDebug bit = 0)
AS 
   /*
     Summary: Remove attribute group with details from child tables
     Before delete check is not system defined group or is not associated with any family or attribute 
     output dataset contain the status if passed @GlobalAttributegroupedId all ids are deleted then this will true other wise false 
     dbo.Split function use to make comma separeted data in table rows 
     Delete table sequence 
     1.[ZnodeGlobalAttributeGroupMapper]
     2.[ZnodeGlobalAttributeGroupLocale]
     3.[ZnodeGlobalAttributeGroup]
     Unit Testing
	 begin tran   
     EXEC ZNode_DeleteGlobalAttributeGroup 15,1
     rollback tran
  */
     BEGIN
         BEGIN TRY
             BEGIN TRAN DeleteAttributeGroup;
			 -- to hold the group ids
             DECLARE @TBL_DeletedGroupedId TABLE(GlobalAttributeGroupId INT); 
             INSERT INTO @TBL_DeletedGroupedId(GlobalAttributeGroupId)
                    SELECT Item
                    FROM  dbo.Split(@GlobalAttributeGroupId, ',') a
					inner join ZnodeGlobalAttributeGroup b on a.item = b.GlobalAttributeGroupId
					Where NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeGlobalFamilyGroupMapper AS c
                                       WHERE c.GlobalAttributeGroupId = item
                                     )
					and b.IsSystemDefined <> 1
			                   
            DELETE FROM ZnodeGlobalFamilyGroupMapper
			WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.GlobalAttributeGroupId = ZnodeGlobalFamilyGroupMapper.GlobalAttributeGroupId
             );

             DELETE FROM [dbo].[ZnodeGlobalAttributeGroupMapper]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.GlobalAttributeGroupId = ZnodeGlobalAttributeGroupMapper.GlobalAttributeGroupId
             );
             DELETE FROM [dbo].[ZnodeGlobalAttributeGroupLocale]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.GlobalAttributeGroupId = ZnodeGlobalAttributeGroupLocale.GlobalAttributeGroupId
             );
             DELETE FROM [dbo].[ZnodeGlobalAttributeGroup]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedGroupedId AS a
                 WHERE a.GlobalAttributeGroupId = ZnodeGlobalAttributeGroup.GlobalAttributeGroupId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeletedGroupedId
             ) =
             (   -- check statement with counts if equal then data set return true else return false
                 SELECT COUNT(1)
                 FROM Split(@GlobalAttributeGroupId, ',')
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
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC ZNode_DeleteGlobalAttributeGroup @GlobalAttributegroupedId = '+@GlobalAttributeGroupId+',@Status='+CAST(@Status AS VARCHAR(50));
            	SET @Status = 0  
				SELECT 1 AS ID,@Status AS Status;  
				ROLLBACK TRAN DeleteAttributeGroup;
				EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'ZNode_DeleteGlobalAttributeGroup',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
