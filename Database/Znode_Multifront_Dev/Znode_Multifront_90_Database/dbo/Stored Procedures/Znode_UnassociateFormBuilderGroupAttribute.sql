-- EXEC Znode_UnassociateFormBuilderGroupAttribute 329,0


CREATE PROCEDURE [dbo].[Znode_UnassociateFormBuilderGroupAttribute](
       @GlobalAttributeId      VARCHAR(1000) =null,
       @GlobalAttributeGroupId INT           = 0,
	   @FormBuilderId int )
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
                                           )
              Declare @Status bit =0
           
             BEGIN

			 if Isnull(@GlobalAttributeGroupId,0) =0
			 BEGIN

				INSERT INTO @TBL_GlobalAttributeId
				SELECT Item
				FROM dbo.split ( @GlobalAttributeId , ',') AS sp
				where not exists (Select 1 from ZnodeFormBuilderSubmit dd
				inner join ZnodeFormBuilderGlobalAttributeValue ss on ss.FormBuilderSubmitId=dd.FormBuilderSubmitId
				Where dd.FormBuilderId=@FormBuilderId
				and ss.GlobalAttributeId=sp.Item)
				  
				 Delete From ZnodeFormBuilderAttributeMapper    
				 Where  FormBuilderId=@FormBuilderId
				 and exists (select 1 from @TBL_GlobalAttributeId ga
				 where ga.GlobalAttributeId=ZnodeFormBuilderAttributeMapper.GlobalAttributeId)

				  IF ( SELECT COUNT(1)
                  FROM @TBL_GlobalAttributeId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @GlobalAttributeId , ','
                                     ) AS a
                    )
                 BEGIN
				 Set @Status =1
				 ENd 
			END

			 if Isnull(@GlobalAttributeGroupId,0) <>0
			 BEGIN

				if not exists (
				SELECT 1
				FROM ZnodeGlobalAttributeGroupMapper AS sp
				where  SP.GlobalAttributeGroupId=@GlobalAttributeGroupId AND
				 exists (Select 1 from ZnodeFormBuilderSubmit dd
				inner join ZnodeFormBuilderGlobalAttributeValue ss on ss.FormBuilderSubmitId=dd.FormBuilderSubmitId
				Where dd.FormBuilderId=@FormBuilderId
				and ss.GlobalAttributeId=sp.GlobalAttributeId))
				Begin
				  
				 Delete From ZnodeFormBuilderAttributeMapper    
				 Where  FormBuilderId=@FormBuilderId
				 and GlobalAttributeGroupId=@GlobalAttributeGroupId

				 Set @Status=1
				END
				 
			END
                SELECT 1 AS ID , CAST(@Status AS BIT) AS Status
			 End
			 COMMIT TRAN;
         END TRY
         BEGIN CATCH
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UnassociateFormBuilderGroupAttribute @GlobalAttributeId = '+@GlobalAttributeId+',@GlobalAttributeGroupId='+CAST(@GlobalAttributeGroupId AS VARCHAR(50))+',@FormBuilderId='+CAST(@FormBuilderId AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_UnassociateFormBuilderGroupAttribute',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;