-- EXEC Znode_DeleteFormBuilder 256 ,1 

CREATE PROCEDURE [dbo].[Znode_DeleteFormBuilder](
       @FormBuilderId VARCHAR(300) ,
	  @Status BIT OUT)
AS 
    -----------------------------------------------------------------------------
    --Summary:  Remove GlobalAttribute still in used 
    --		   	
    --          
    --Unit Testing   

    ----------------------------------------------------------------------------- 


     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 --Declare @Status bit=0
             BEGIN TRAN A;
             DECLARE @DeleteFormBuilderId TABLE (
                                              FormBuilderId INT
                                              );
             INSERT INTO @DeleteFormBuilderId
                    SELECT Item
                    FROM dbo.split ( @FormBuilderId , ','
                                   ) AS a 
					INNER JOIN ZnodeFormBuilder AS B ON ( a.item = b.FormBuilderId )
					Where not exists(  Select 1 
					from ZnodeFormBuilderSubmit dd
					where dd.FormBuilderId =b.FormBuilderId)
                   
				   Delete From ZnodeFormBuilderAttributeMapper
				   where  FormBuilderId in (Select FormBuilderId from @DeleteFormBuilderId)

				   Delete From ZnodeFormBuilder
				   where  FormBuilderId in (Select FormBuilderId from @DeleteFormBuilderId)
            
             IF ( SELECT COUNT(1)
                  FROM @DeleteFormBuilderId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @FormBuilderId , ','
                                     ) AS a
                    )
                 BEGIN
                    SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                      SELECT 0 AS ID,
                            CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
             SELECT ERROR_MESSAGE() , ERROR_LINE() , ERROR_PROCEDURE();
             ROLLBACK TRAN A;
         END CATCH;
     END;