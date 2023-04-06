
CREATE PROCEDURE [dbo].[Znode_UnassociateGroupEntity](
       @GlobalAttributeGroupId  VARCHAR(2000) ,
       @GlobalEntityId INT  )
AS 
    /*
    Summary:  Check and unassociate the group from family if attribute of group is present in pim values then dont
    		  unassociate the group from family
 
   */
     BEGIN
         BEGIN TRAN B;
         BEGIN TRY
             SET NOCOUNT ON; 
			 Declare @TableName nvarchar(200) ,@SQL nvarchar(max) 
             ---- Declare the table to store the comma seperated data into record format ------------
             DECLARE @TBL_Group TABLE (
                                      ID                  INT ,
                                      GlobalAttributeGroupId INT
									  );
			DECLARE @TBL_NotToDeleteGroup TABLE (GlobalAttributeGroupId INT
									  );

			Select @TableName=TableName
			from ZnodeGlobalEntity
			Where GlobalEntityId =@GlobalEntityId

			if @TableName is not null
			Begin
		    	Set @SQL =' Select GlobalAttributeGroupId 
						from [dbo].['+@TableName+'] a
						inner join ZnodeGlobalAttributeGroupMapper b on a.GlobalAttributeId=b.GlobalAttributeId
						inner join dbo.Split('''+@GlobalAttributeGroupId+''','','') c on c.item =b.GlobalAttributeGroupId
						group by GlobalAttributeGroupId '
				   Begin Try
					  insert into @TBL_NotToDeleteGroup
					   EXEC SP_EXECUTESQl  @SQL
				   End Try
					Begin Catch
					End  Catch;
			end 


             INSERT INTO @TBL_Group
                    SELECT ID , item 
                    FROM ZnodeGlobalAttributeGroup  ZGAG 
					INNER JOIN dbo.split ( @GlobalAttributeGroupId , ',' ) SP ON (SP.Item = ZGAG.GlobalAttributeGroupId) --- store the comma separeted category id into variable table 
					Where not exists (Select 1 
					from @TBL_NotToDeleteGroup d
					Where d.GlobalAttributeGroupId=sp.Item )
					
					 

                BEGIN
                     DELETE FROM ZnodeGlobalGroupEntityMapper
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_Group AS gwa
                                    WHERE gwa.GlobalAttributeGroupId = ZnodeGlobalGroupEntityMapper.GlobalAttributeGroupId									
                                  )
					 AND ZnodeGlobalGroupEntityMapper.GlobalEntityId =@GlobalEntityId;
                    

					  IF( SELECT COUNT(1) FROM @TBL_Group ) =
					 ( SELECT COUNT(1) FROM Split(@GlobalAttributeGroupId, ',') )   
						 BEGIN
								SELECT 1 AS ID,CAST(1 AS BIT) AS [Status];
						 END;
					 ELSE
						 BEGIN
							  SELECT 1 AS ID,CAST(0 AS BIT) AS [Status];
						 END;

                 END;
             COMMIT TRAN B;
         END TRY
         BEGIN CATCH
               DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UnassociateGroupEntity @GlobalAttributeGroupId = '+@GlobalAttributeGroupId+',@GlobalEntityId='+CAST(@GlobalEntityId AS VARCHAR(200));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN B;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_UnassociateGroupEntity',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;