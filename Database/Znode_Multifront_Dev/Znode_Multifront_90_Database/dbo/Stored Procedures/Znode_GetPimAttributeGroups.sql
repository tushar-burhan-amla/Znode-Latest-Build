CREATE PROCEDURE [dbo].[Znode_GetPimAttributeGroups]
( @WhereClause NVARCHAR(Max) = '',
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY VARCHAR(1000)    = '',
  @RowsCount   INT OUT)
AS
/*
Summary: This procedure is used get PimAttributeGroups from both the locale
		 The result is displayed order by PimAttributeGroupId in descending order
Unit Testing:
		
		 DECLARE @Ree INT 
         EXEC Znode_GetPimAttributeGroups @WhereClause= 'LocaleId = 1' , @RowsCount =  @Ree OUT, @PageNo = 1, @Rows = 10 
			
		 
*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_PimAttributeGroups TABLE (PimAttributeGroupId int,GroupCode VARCHAR(200), AttributeGroupName NVARCHAR(600),IsSystemDefined BIT,LocaleId INT,RowId INT,CountNo INT )
           
		  SET @SQL = '
					;WITH CTE_PimGroupList AS
					( SELECT ZPAG.PimAttributeGroupId,ZPAG.GroupCode ,ZPAGL.AttributeGroupName,ZPAG.IsSystemDefined,ZPAG.IsCategory,ZPAG.IsNonEditable,ZPAGL.LocaleId
					  FROM ZnodePimAttributeGroup ZPAG 
				      Inner JOIN ZnodePimAttributeGroupLocale ZPAGL on (ZPAG.PimAttributeGroupId = ZPAGL.PimAttributeGroupId ) 
					  WHERE  ZPAG.IsNonEditable = 0)

				    ,CTE_PimGroup AS
				     ( SELECT PimAttributeGroupId,GroupCode,AttributeGroupName,IsSystemDefined,LocaleId
					   ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PimAttributeGroupId DESC')+',Count(*)Over() CountNo
					   FROM CTE_PimGroupList
				       WHERE 1=1 
					   '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'				
					 )
					SELECT PimAttributeGroupId,GroupCode,AttributeGroupName,IsSystemDefined,LocaleId,RowId,CountNo
					FROM CTE_PimGroup
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

					INSERT INTO @TBL_PimAttributeGroups(PimAttributeGroupId,GroupCode,AttributeGroupName,IsSystemDefined,LocaleId,RowId,CountNo)
					EXEC(@SQL)

					SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_PimAttributeGroups),0)

					SELECT PimAttributeGroupId,GroupCode,AttributeGroupName,IsSystemDefined,LocaleId
					FROM  @TBL_PimAttributeGroups
	
         END TRY
         BEGIN CATCH
          DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributeGroups @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributeGroups',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                            
         END CATCH;
     END;