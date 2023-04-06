-- SELECT  * FROM ZNodeMediaCategory 
-- EXEC ZnodeDeleteMediaFolder 15
-- SELECT * FROM Znodemedia


CREATE PROCEDURE [dbo].[ZnodeDeleteMediaFolder](@MediaPathId INT)
AS 
    -----------------------------------------------------------------------------
    --Summary:  Remove media folder 
    --		   	Retrive list of MediaFolderId with their respective parent child id through recurrsive  query (With option) 
    --          
    --Unit Testing   
    --			SELECT * FROM zNodeUser
    --SELECT * FROM AspNetUserRoles b  INNER JOIN
    --                      AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
    --                      AspNetUsers d ON (b.userId = d .id)   WHERE d.Username ='admin12345' AND c.Name = 'Admin'
    --DECLARE @EDE INT  EXEC Znode_AdminUsers '','admin12345',@WhereClause='accountid = 34 and isaccountcustomer = 1',@Order_By='',@RowCount=@EDE OUT  SELECT @EDE
    ----------------------------------------------------------------------------- 
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
                    WHERE(MediaPathId = @MediaPathId
                          OR ParentMediaPathId = @MediaPathId)
                         AND ParentMediaPathId IS NOT NULL;
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
             FROM View_ZnodeDeleteMediaFolder AS a
                  INNER JOIN @V_MediaId AS B ON(a.MediaId = b.MediaId);
             COMMIT TRANSACTION;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE(),
                    ERROR_LINE(),
                    ERROR_PROCEDURE();
             ROLLBACK TRANSACTION;
         END CATCH;
     END;