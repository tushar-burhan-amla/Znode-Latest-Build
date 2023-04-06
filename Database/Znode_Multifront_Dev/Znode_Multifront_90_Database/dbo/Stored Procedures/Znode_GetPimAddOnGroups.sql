
CREATE PROCEDURE [dbo].[Znode_GetPimAddOnGroups]
(@WhereClause VARCHAR(max),
 @Rows        INT           = 100,
 @PageNo      INT           = 0,
 @Order_BY    VARCHAR(1000) = '',
 @RowsCount   INT OUT,
 @LocaleId    INT           = 1)
AS
 /* 
  Summary : - This procedure is used to get the addon group details from all locale
			  Result is fetched filtered by PimAddonGroupId in descending order 
Unit Testing:
   EXEC Znode_GetPimAddOnGroups @WhereClause='',@RowsCount=null,@Rows = 10,@PageNo=1,@Order_BY = Null

 */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @DefaultLocaleId INT =  dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_AddonGroupDetail TABLE(PimAddonGroupId INT,AddOnGroupName NVARCHAR(MAX),DisplayType NVARCHAR(400),RowId INT,CountNo INT )
			
             SET @SQL = ' 
				 ;With Cte_AddonsWithBothLocale AS 
				 (
				 SELECT ag.PimAddonGroupId, ag.DisplayType, agl.AddonGroupName,agl.LocaleId
				 FROM  dbo.ZnodePimAddonGroup AS ag 
				 INNER JOIN dbo.ZnodePimAddonGroupLocale AS agl ON ag.PimAddonGroupId = agl.PimAddonGroupId
				 WHERE agl.LocaleId IN ('+CAST(@DefaultLocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+')

				 )
				  , Cte_AddOnsWithFirstLocale 	 AS 
				 (
				 SELECT PimAddonGroupId, DisplayType, AddonGroupName
				 FROM Cte_AddonsWithBothLocale 
				 WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
				 ) 
				 , Cte_AddonsList AS 
				 (
				 SELECT PimAddonGroupId, DisplayType, AddonGroupName 
				 FROM Cte_AddOnsWithFirstLocale 
				 UNION ALL 
				 SELECT PimAddonGroupId, DisplayType, AddonGroupName
				 FROM Cte_AddonsWithBothLocale PQ
				 WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
				 AND NOT EXISTS (SELECT * FROM   Cte_AddOnsWithFirstLocale SD WHERE SD.PimAddonGroupId = PQ.PimAddonGroupId )
				 )
				 ,Cte_GetFilterAddOngroup AS 
				 (
				 SELECT PimAddonGroupId,AddOnGroupName,DisplayType  ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PimAddonGroupId DESC')+',Count(*)Over() CountNo
				 FROM '+' Cte_AddonsList '+'WHERE 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				 ) 

				 SELECT PimAddonGroupId,AddOnGroupName,DisplayType,RowId,CountNo 
				 FROM Cte_GetFilterAddOngroup '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

				 print @sql
				 INSERT INTO @TBL_AddonGroupDetail (PimAddonGroupId,AddOnGroupName,DisplayType,RowId,CountNo )
				 EXEC (@SQL)

				 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AddonGroupDetail ),0)

				 SELECT PimAddonGroupId,AddOnGroupName,DisplayType
				 FROM @TBL_AddonGroupDetail

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAddOnGroups @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAddOnGroups',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                  
         END CATCH;
     END;