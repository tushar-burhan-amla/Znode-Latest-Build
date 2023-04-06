CREATE Procedure [dbo].[Znode_GetMediaAttributes]  
  (  
	 @WhereClause NVarchar(Max)  = ''   
	,@Rows INT					 = 100
	,@PageNo INT				 = 1
	,@Order_BY VARCHAR(1000)	 = ''  
	,@RowsCount int out  
	)  
AS  
 /*
 Summary: This procedure is used to get the media attribute list 
		  Output is Stored in temp table #mediaAttributes
	      Get the Attribute List using inner join on ZnodeMediaAttribute,ZnodeMediaAttributeLocale,ZnodeAttributeType table
		  Result is Fetched filtered by MediaAttributeId in descending order.
 Unit Testing 
		
		 EXEC [dbo].[Znode_GetMediaAttributes] 'localeId = 1', @RowsCount = 0

*/		
BEGIN    
 BEGIN TRY   
   SET NOCOUNT ON 
   DECLARE @SQL NVARCHAR(MAX)  
     
   DECLARE @TBL_MediaAttribute TABLE(MediaAttributeId INT, AttributeCode VARCHAR(300), AttributeName NVARCHAR(600) , AttributeTypeName VARCHAR(300), IsRequired BIT ,IsLocalizable BIT
								,RowId INT,CountNo INT)           
   SET @SQL = '
				;With Cte_MediaAttributeLocale AS 
				(
				SELECT ZMA.MediaAttributeId, ZMA.AttributeCode, ZMAL.AttributeName, ZAT.AttributeTypeName, ZMA.IsRequired ,ZMA.IsLocalizable,ZMAL.LocaleId
				FROM ZnodeMediaAttribute ZMA 
				INNER JOIN ZnodeMediaAttributeLocale ZMAL on (ZMA.MediaAttributeId =ZMAL.MediaAttributeId) 
				INNER JOIN ZnodeAttributeType ZAT on (ZMA.AttributeTypeId=ZAT.AttributeTypeId) 
				)
				,Cte_MediaAttribute AS
				(
				 SELECT  MediaAttributeId, AttributeCode, AttributeName, AttributeTypeName, IsRequired ,IsLocalizable,LocaleId
							,'+dbo.Fn_GetPagingRowId(@Order_BY,'MediaAttributeId DESC')+',Count(*)Over() CountNo
				 FROM Cte_MediaAttributeLocale
				 WHERE 1=1 
				 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				)

				SELECT MediaAttributeId, AttributeCode, AttributeName, AttributeTypeName, IsRequired ,IsLocalizable,RowId,CountNo
				FROM Cte_MediaAttribute
			    '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

   INSERT INTO @TBL_MediaAttribute (MediaAttributeId, AttributeCode, AttributeName, AttributeTypeName, IsRequired ,IsLocalizable,RowId,CountNo)
   EXEC(@SQL)

   SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_MediaAttribute),0)
   
   SELECT MediaAttributeId, AttributeCode, AttributeName, AttributeTypeName, IsRequired ,IsLocalizable
   FROM @TBL_MediaAttribute 
   
   END TRY     
   BEGIN CATCH        
	   DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaAttributes @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaAttributes',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;          
   END CATCH     
END