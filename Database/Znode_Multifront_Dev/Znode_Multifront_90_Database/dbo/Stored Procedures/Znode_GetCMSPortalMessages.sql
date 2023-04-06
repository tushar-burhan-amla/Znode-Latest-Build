CREATE PROCEDURE [dbo].[Znode_GetCMSPortalMessages]
( @WhereClause NVARCHAR(MAX) = '',
  @LocaleId    INT           = 1)
AS
   /*
     Summary :- This Procedure is used to get the CMS portal messages 
     Unit Testing 
     EXEC Znode_GetCMSPortalMessages @WhereClause = 'PortalId = 7 AND LocaleId = 1  '

     */
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX), @DefaultLocaleId VARCHAR(20)= dbo.Fn_GetDefaultLocaleId();
             SET @SQL = ' 
	        

			;With Cte_PortalMessagesBothLocale AS 
			(
			SELECT a.CMSPortalMessageId ,a.CMSMessageId,a.CMSMessageKeyId,c.Message,b.MessageKey,a.PortalId ,e.StoreName ,c.LocaleId
			FROM ZnodeCMSPortalMessage a
			INNER JOIN ZnodeCMSMessageKey b ON (b.CMSMessageKeyId = a.CMSMessageKeyId )
			INNER JOIN ZnodeCMSMessage c ON (c.CMSMessageId = a.CMSMessageId )
			INNER JOIN ZnodePortal e ON (e.PortalId = a.PortalId)
			WHERE c.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(20))+','+@DefaultLocaleId+')
			)
			,Cte_FilterClause AS 
			(
			SELECT CMSPortalMessageId,CMSMessageId,CMSMessageKeyId,Message,MessageKey,PortalId,StoreName ,LocaleId
			FROM Cte_PortalMessagesBothLocale 
			WHERE '+CASE
                           WHEN @WhereClause = ''
                           THEN '1=1'
                           ELSE @WhereClause
                       END+'
			)
			, Cte_filterPortalMessageFirstLocale AS 
			(
			 SELECT  CMSPortalMessageId,CMSMessageId,CMSMessageKeyId,Message,MessageKey,PortalId,StoreName 
			 FROM Cte_FilterClause 
			 WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(20))+'
			)
			,Cte_filterPortalMessageForDefaultLocale AS 
			(
			 SELECT CMSPortalMessageId,CMSMessageId,CMSMessageKeyId,Message,MessageKey,PortalId,StoreName 
			 FROM Cte_filterPortalMessageFirstLocale 
			 UNION ALL 
			 SELECT CMSPortalMessageId,CMSMessageId,CMSMessageKeyId,Message,MessageKey,PortalId,StoreName 
			 FROM Cte_FilterClause CTFCL
			 WHERE  LocaleId = '+@DefaultLocaleId+'
			 AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_filterPortalMessageFirstLocale CTFPMFL WHERE CTFCL.PortalId = CTFPMFL.PortalId AND CTFCL.CMSMessageKeyId = CTFPMFL.CMSMessageKeyId )
			
			)
			
			SELECT CMSPortalMessageId,CMSMessageId,CMSMessageKeyId,Message,MessageKey,PortalId,StoreName FROM Cte_filterPortalMessageForDefaultLocale 
			';
             EXEC (@SQL);
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSPortalMessages @WhereClause = '+@WhereClause+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSPortalMessages',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;      
         END CATCH;
     END;