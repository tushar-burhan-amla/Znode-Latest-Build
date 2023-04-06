-- EXEC [Znode_UnassignGlobalAttributeFromGroup] 329,0


CREATE PROCEDURE [dbo].[Znode_UnassignGlobalAttributeFromGroup](
       @GlobalAttributeId      VARCHAR(1000) ,
       @GlobalAttributeGroupId INT           = 0 )
AS
/*
Summary: This Procedure is used to Unassign Product Attribute from group
Unit Testing:
EXEC Znode_UnassignGlobalAttributeFromGroup

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
		 SET NOCOUNT ON
             DECLARE @TBL_GlobalAttributeId TABLE (
                                               GlobalAttributeId INT
                                               );
             DECLARE @TBL_DeletedIds TABLE (
                                           GlobalAttributeId INT
                                           );
           
             BEGIN
				INSERT INTO @TBL_GlobalAttributeId
				SELECT Item
				FROM dbo.split ( @GlobalAttributeId , ',') AS sp
				where dbo.[Fn_CheckGlobalAttributeTransactionUsed]('GlobalAttribute',sp.Item)=0
				  
				 Delete From ZnodeGlobalAttributeGroupMapper    
				 Where  GlobalAttributeGroupId=@GlobalAttributeGroupId
				 and exists (select 1 from @TBL_GlobalAttributeId ga
				 where ga.GlobalAttributeId=ZnodeGlobalAttributeGroupMapper.GlobalAttributeId)

			 IF ( SELECT COUNT(1)
                  FROM @TBL_GlobalAttributeId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @GlobalAttributeId , ','
                                     ) AS a
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             
			 End
			 COMMIT TRAN;
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UnassignGlobalAttributeFromGroup @GlobalAttributeId = '+@GlobalAttributeId+',@GlobalAttributeGroupId='+CAST(@GlobalAttributeGroupId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_UnassignGlobalAttributeFromGroup',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;