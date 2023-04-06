CREATE PROCEDURE [dbo].[Znode_GetMediaAttributeGroups]
( @WhereClause VARCHAR(Max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
 /*
  Summary: Get Group Information of Media Attribute where IsHidden = 0
		   Get the Attribute List using inner join ZnodeMediaAttributeGroup, ZnodeMediaAttributeGroupLocale table
		   Output is Stored in temp table #mediaGroupList
		   Result is Fetched filtered by MediaAttributeGroupId in descending order
  Unit Testing:
	EXEC Znode_GetMediaAttributeGroups '' ,  @RowsCount = 0

*/
     BEGIN
         BEGIN TRY
		  SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_AttributeGroup TABLE(MediaAttributeGroupId INT,GroupCode VARCHAR(200),AttributeGroupName NVARCHAR(max),IsHidden BIT ,IsSystemDefined BIT
								,LocaleId INT,RowId INT,CountNo INT)
             SET @SQL = ';With Cte_MediaGroup AS (
			              SELECT ZMAG.MediaAttributeGroupId,ZMAG.GroupCode ,ZMAGL.AttributeGroupName, ZMAG.IsHidden ,ZMAG.IsSystemDefined,ZMAGL.LocaleId								 
						  FROM ZnodeMediaAttributeGroup ZMAG 
						  INNER JOIN ZnodeMediaAttributeGroupLocale ZMAGL on (ZMAG.MediaAttributeGroupId = ZMAGL.MediaAttributeGroupId ) 
						  WHERE  ZMAG.IsHidden = 0 
								)
						  , Cte_MediaAttributeGroupFilter AS 
						  (
						  SELECT MediaAttributeGroupId,GroupCode ,AttributeGroupName,IsHidden ,IsSystemDefined
						  ,LocaleId ,'+dbo.Fn_GetPagingRowId(@Order_BY,'MediaAttributeGroupId DESC')+',Count(*)Over() CountNo 
						  FROM Cte_MediaGroup CTMG 
						  WHERE 1=1 
						  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						  )
						  SELECT MediaAttributeGroupId,GroupCode ,AttributeGroupName,IsHidden ,IsSystemDefined,LocaleId,RowId,CountNo								
						  FROM Cte_MediaAttributeGroupFilter 
						 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)		
							
			  INSERT INTO @TBL_AttributeGroup (MediaAttributeGroupId,GroupCode ,AttributeGroupName,IsHidden ,IsSystemDefined,LocaleId,RowId,CountNo)
								
			  EXEC (@SQL)
			  SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AttributeGroup),0)
			
			 SELECT MediaAttributeGroupId,GroupCode ,AttributeGroupName,IsHidden ,IsSystemDefined
								,LocaleId
			 FROM @TBL_AttributeGroup																																																																																																								 
             
         END TRY
         BEGIN CATCH
                DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaAttributeGroups @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaAttributeGroups',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                           
         END CATCH;
     END;