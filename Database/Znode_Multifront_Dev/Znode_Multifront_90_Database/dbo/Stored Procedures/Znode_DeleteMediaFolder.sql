
CREATE PROCEDURE [dbo].[Znode_DeleteMediaFolder]
(@MediaPathId INT)
AS 
   /* 
    Summary:  Remove media folder 
    		  Retreive list of MediaFolderId with their respective parent child id through recursive  query (With option) 
              
    Unit Testing   
    			SELECT * FROM zNodeUser
                SELECT * FROM AspNetUserRoles b  INNER JOIN
                AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
                AspNetUsers d ON (b.userId = d .id)   WHERE d.Username ='admin12345' AND c.Name = 'Admin'
   
	EXEC Znode_DeleteMediaFolder  65 
  */
     BEGIN
         BEGIN TRANSACTION;
         BEGIN TRY
             SET NOCOUNT ON;
			
             DECLARE @V_MediaPathId TABLE(MediaPathId INT); -- Find the relative Media path ids 

             DECLARE @V_MediaId TABLE(MediaId INT);-- find the how many media are deleted 
             -- Retrive all related mediapathid (Parent / child) inserted into table variable @V_MediaPathId   
             INSERT INTO @V_MediaPathId
                    SELECT MediaPathID
                    FROM [dbo].[FN_GetMediaPathHierarchy](@MediaPathId)
                    --WHERE(MediaPathId = @MediaPathId
                    --      OR ParentMediaPathId = @MediaPathId)
                    --     AND ParentMediaPathId IS NOT NULL;
             DELETE FROM ZnodeMediaAttributeValue
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeMediaCategory AS b
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @V_MediaPathId AS a
                     WHERE a.MediaPathId = b.MediaPathId
                 )
                       AND b.MediaCategoryId = ZnodeMediaAttributeValue.MediaCategoryId
             );

             DELETE FROM ZnodeMediaCategory
             OUTPUT DELETED.MediaId
                    INTO @V_MediaId
             WHERE EXISTS
             (
                 SELECT 1
                 FROM @V_MediaPathId AS a
                 WHERE a.MediaPathId = ZnodeMediaCategory.MediaPathId
             );
             DELETE FROM ZnodeMediaFolderUser
             WHERE MediaPathId = MediaPathID;

             DELETE FROM ZnodeMedia
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_MediaId AS a
                 WHERE a.MediaId = ZnodeMedia.MediaId
             );
             DELETE FROM ZnodeMediaPathLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_MediaPathId AS a
                 WHERE a.MediaPathId = ZnodeMediaPathLocale.MediaPathId
             );

             DELETE FROM ZnodeMediaPath
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @V_MediaPathId AS a
                 WHERE a.MediaPathId = ZnodeMediaPath.MediaPathId
             );
			    SELECT a.MediaId,
                    a.Path,
                    a.FileName
					,a.MediaConfigurationId
             FROM View_MediaPathDetails AS a
                  INNER JOIN @V_MediaId AS B ON(a.MediaId = b.MediaId);
			 
			 DELETE FROM ZnodeMedia 
			 WHERE EXISTS (SELECT TOP 1 1 FROM @V_MediaId TBM WHERE TBM.MediaId = ZnodeMedia.MediaId )	 

          
             COMMIT TRANSACTION;
         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE()

             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMediaFolder @MediaPathId = '+@MediaPathId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRANSACTION;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMediaFolder',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;