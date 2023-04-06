
CREATE PROCEDURE [dbo].[Znode_MediaFolderUserShare]
    (  @LocaleId INT = 1,
       @UserId   INT=-1  
      )
AS 
/*
     Summary: To get list of media folder associated with users 
	Unit Testing:
	 EXEC Znode_MediaFolderUserShare 45 ,1
	*/
     BEGIN
         BEGIN TRY
             DECLARE @MediaFolderUserShare TABLE (
                                                 MediaPathId       INT ,
                                                 [PathName]        NVARCHAR(600) ,
                                                 ParentMediaPathId INT
                                                 );
             IF EXISTS (SELECT TOP 1 1 FROM ZnodeUser ZU INNER JOIN  AspNetUsers ASP ON (ASP.Id = ZU.AspNetUserId ) 
									INNER JOIN AspNetUserRoles ASPR  ON (ASPR.UserId = ASP.Id) 
									INNER JOIN AspNetRoles ASPRR  ON (ASPRR.Id = ASPR.RoleId) WHERE ASPRR.Name = 'Admin' AND ZU.UserId = @UserId  )
			BEGIN 
			SET @UserId = -1 

			END 

			 INSERT INTO @MediaFolderUserShare
                    SELECT zmp.MediaPathId , zmpl.[PathName] , zmp.ParentMediaPathId
                    FROM ZnodeMediaPath AS zmp INNER JOIN ZnodeMediaPathLocale AS zmpl ON ( zmpl.MediaPathId = zmp.MediaPathId )
                    WHERE (zmp.CreatedBy = @UserId OR @UserId = -1 )  AND  zmpl.LocaleId = @LocaleId
                    UNION ALL
                    SELECT zmp.MediaPathId , zmpl.[PathName] , zmp.ParentMediaPathId
                    FROM ZnodeMediaFolderUser AS zmfu INNER JOIN ZnodeMediaPath AS zmp ON ( zmfu.MediaPathId = zmp.MediaPathId )
                                                      INNER JOIN ZnodeMediaPathLocale AS zmpl ON ( zmpl.MediaPathId = zmp.MediaPathId )
                    WHERE (zmfu.UserId = @UserId OR @UserId = -1 )
                          AND
                          zmpl.LocaleId = @LocaleId;
             SELECT DISTINCT  *
             FROM @MediaFolderUserShare
			 UNION ALL 
			 SELECT 1,'Root',NULL
			 WHERE NOT EXISTS (SELECT TOP 1 1 FROM @MediaFolderUserShare)

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_MediaFolderUserShare @LocaleId = '+CAST(@LocaleId AS VARCHAR(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_MediaFolderUserShare',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;