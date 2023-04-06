CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetVendorList]
(
--@LocaleId INT = 0
@WhereClause NVARCHAR(max) = ''
)

AS 
/*
    Summary: This procedure is used to find the vendor list
	Unit Testing: 
     EXEC ZnodeReport_GetVendorList 
	*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);

			 SET @SQL = '

              DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
			 			

			  ;WITH CTE_GetVendorBothLocale AS
			  (
				SELECT PimVendorId,VendorCode,ZPV.PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
				,CompanyName,ZPV.DisplayOrder,IsActive, VIPDVL.LocaleId,VIPDVL.PimAttributeDefaultValueId,VIPDVL.AttributeDefaultValue AS VendorName
				FROM ZnodePimVendor ZPV INNER JOIN ZnodePimAttributeDefaultValue ZVIPDV ON (ZPV.VendorCode =ZVIPDV.AttributeDefaultValueCode )
				INNER JOIN ZnodePimAttributeDefaultValueLocale VIPDVL ON (VIPDVL.PimAttributeDefaultValueId = ZVIPDV.PimAttributeDefaultValueId) 
				WHERE VIPDVL.LocaleId IN(CAST(@DefaultLocaleId AS VARCHAR(50)) ) '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				--,CAST(@LocaleId AS VARCHAR(50))) 			 
			  )

			  , CTE_GetVendorLocale AS
			  (
			    SELECT PimVendorId,VendorCode,PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
				,CompanyName,DisplayOrder,IsActive,LocaleId,PimAttributeDefaultValueId,VendorName
				FROM CTE_GetVendorBothLocale CVL
				WHERE 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				--WHERE LocaleId = CAST(@LocaleId AS VARCHAR(50))			  		  
			  )
			  , CTE_GetVendorDefault AS
			  (
			   SELECT PimVendorId,VendorCode,PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
			   ,CompanyName,DisplayOrder,IsActive,LocaleId,PimAttributeDefaultValueId,VendorName FROM CTE_GetVendorLocale
			   UNION ALL
			   SELECT PimVendorId,VendorCode,PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
			   ,CompanyName,DisplayOrder,IsActive,LocaleId,PimAttributeDefaultValueId,VendorName FROM CTE_GetVendorBothLocale CVBL
			   WHERE LocaleId = CAST(@DefaultLocaleId AS VARCHAR(50))
			   AND NOT EXISTS  (SELECT 1 FROM CTE_GetVendorLocale CVL WHERE CVL.PimAttributeDefaultValueId = CVBL.PimAttributeDefaultValueId)			  			  
			  )
			  ,CTE_GetVendorDetailList AS
			  ( SELECT  PimVendorId,VendorCode,PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
				,CompanyName,DisplayOrder,IsActive,LocaleId,PimAttributeDefaultValueId,VendorName 
			   FROM CTE_GetVendorDefault  )
			  
				SELECT PimVendorId,VendorCode,PimAttributeId,AddressId,ExternalVendorNo,Email,NotificationEmailID,EmailNotificationTemplate
				,CompanyName,DisplayOrder,IsActive,VendorName 
				FROM CTE_GetVendorDetailList 
				'
				PRINT @SQL
				EXEC(@SQL)

       END TRY
	   BEGIN CATCH
	       DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetVendorDetailList @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetVendorDetailList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	   END CATCH
	END