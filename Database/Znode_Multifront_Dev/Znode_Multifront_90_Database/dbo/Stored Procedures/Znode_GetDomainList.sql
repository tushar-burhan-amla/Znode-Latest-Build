CREATE PROCEDURE [dbo].[Znode_GetDomainList]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the Portal 
	 Unit Testig 
	 EXEC  Znode_GetDomainList '',100,1,'',0
	 select * from ZnodeDomain
*/
   BEGIN 
		BEGIN TRY 
			SET NOCOUNT ON 

			
			 DECLARE @SQL  NVARCHAR(MAX) 
			 DECLARE @TBL_Domainid TABLE (DomainId INT, PortalId INT,ApiKey NVARCHAR(MAX),DomainName NVARCHAR(MAX),IsActive BIT,ApplicationType NVARCHAR(MAX),IsDefault
			 BIT,StoreName NVARCHAR(MAX),PortalGlobalAttributeValueId INT,AttributeValue NVARCHAR(MAX),GlobalAttributeId INT,AttributeCode NVARCHAR(MAX),Countid INT,RowID INT)
	 
			SET @SQL = '
			;With Cte_Domain AS 
			(

			SELECT [Extent1].[DomainId] AS [DomainId], [Extent1].[PortalId] , [Extent1].[DomainName] AS [DomainName], 
			[Extent1].[IsActive] , [Extent1].[ApiKey] AS [ApiKey], [Extent1].[ApplicationType] AS [ApplicationType], 
			[Extent1].[IsDefault] AS [IsDefault], [Extent2].[StoreName] AS [StoreName], [Extent3].[PortalGlobalAttributeValueId] , 
			[Extent3].[AttributeValue] AS [AttributeValue], [Extent4].[GlobalAttributeId] , [Extent4].[AttributeCode] AS [AttributeCode]
			FROM [dbo].[ZnodeDomain] AS [Extent1]
			INNER JOIN [dbo].[ZnodePortal] AS [Extent2] ON [Extent1].[PortalId] = [Extent2].[PortalId]
			INNER JOIN [dbo].[ZnodePortalGlobalAttributeValue] AS [Extent3] ON [Extent1].[PortalId] = [Extent3].[PortalId]
			INNER JOIN [dbo].[ZnodeGlobalAttribute] AS [Extent4] ON [Extent3].[GlobalAttributeId] = [Extent4].[GlobalAttributeId]	
			INNER JOIN [dbo].[ZnodePortalGlobalAttributeValueLocale] AS [Extent5] ON [Extent3].[PortalGlobalAttributeValueId] = [Extent5].[PortalGlobalAttributeValueId]
			WHERE N''true'' = [Extent5].[AttributeValue]
			and ([Extent1].[IsActive] = 1) AND ([Extent1].[ApplicationType] IN (''WebStore'',''WebstorePreview'')) AND ( [Extent4].[AttributeCode] in (''IsCloudflareEnabled'' ,''CloudflareZoneId'')) 
		

			 )
			 ,Cte_DomainStatus AS 
			 (
			SELECT DomainId , PortalId ,ApiKey,DomainName ,IsActive ,ApplicationType ,IsDefault,StoreName,PortalGlobalAttributeValueId,AttributeValue ,GlobalAttributeId ,
			AttributeCode, '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PortalId  DESC')+',Count(*) over() CountId  FROM Cte_Domain 
			WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' 
			)	 
			SELECT DomainId , PortalId ,ApiKey,DomainName ,IsActive ,ApplicationType ,IsDefault,StoreName,PortalGlobalAttributeValueId,AttributeValue ,GlobalAttributeId ,
			AttributeCode,CountId,RowID
			FROM Cte_DomainStatus
			'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)
	
	         PRINT @SQL
			 INSERT INTO @TBL_Domainid (DomainId , PortalId ,ApiKey,DomainName ,IsActive ,ApplicationType ,IsDefault,StoreName,PortalGlobalAttributeValueId,AttributeValue ,GlobalAttributeId ,
			 AttributeCode,CountId,RowID)
			 EXEC (@SQL)

			 SET @RowsCount = ISNULL((SELECT top 1 CountId FROM @TBL_Domainid),0)

		 
			 SELECT DomainId ,PortalId ,ApiKey,DomainName ,IsActive ,ApplicationType ,IsDefault,StoreName,PortalGlobalAttributeValueId,AttributeValue ,GlobalAttributeId ,
		     AttributeCode
			 FROM @TBL_Domainid
	 
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetDomainList @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS
			VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status; 
			select ERROR_MESSAGE()

			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetDomainList',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
