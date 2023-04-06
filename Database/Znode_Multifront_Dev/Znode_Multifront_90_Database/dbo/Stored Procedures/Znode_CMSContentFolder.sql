

CREATE Procedure  [dbo].[Znode_CMSContentFolder]
(
	@LocaleId   int = 1 
)
AS
-----------------------------------------------------------------------------
--Summary:To get list of media filder associated with users      

--Unit Testing   

--Exec Znode_CMSContentFolder 
----------------------------------------------------------------------------- 
  
BEGIN 
  BEGIN TRY 
	    SELECT  zmp.CMSContentPageGroupId,zmp.[Code],zmpl.[Name] ,zmp.ParentCMSContentPageGroupId
		FROM ZnodeCMSContentPageGroup zmp
		INNER JOIN ZnodeCMSContentPageGroupLocale  zmpl ON (zmp.CMSContentPageGroupId = zmpl.CMSContentPageGroupId)
		WHERE zmpl.LocaleId = @LocaleId
  END TRY 
BEGIN CATCH 
	   SELECT ERROR_LINE(),ERROR_NUMBER (),ERROR_MESSAGE()
END CATCH  
 
END