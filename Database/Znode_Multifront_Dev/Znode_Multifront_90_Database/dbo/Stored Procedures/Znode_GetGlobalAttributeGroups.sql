CREATE PROCEDURE [dbo].[Znode_GetGlobalAttributeGroups]  
( @WhereClause NVARCHAR(Max) = '',  
  @Rows        INT           = 100,  
  @PageNo      INT           = 1,  
  @Order_BY VARCHAR(1000)    = '',  
  @RowsCount   INT OUT)  
AS  
/*  
Summary: This procedure is used get GlobalAttributeGroups from both the locale  
   The result is displayed order by GlobalAttributeGroupId in descending order  
Unit Testing:  
    
   DECLARE @Ree INT   
         EXEC Znode_GetGlobalAttributeGroups @WhereClause= 'LocaleId = 1' , @RowsCount =  @Ree OUT, @PageNo = 1, @Rows = 10   
     
     
*/  
     BEGIN  
         SET NOCOUNT ON;  
         BEGIN TRY  
             DECLARE @SQL NVARCHAR(MAX);  
    DECLARE @TBL_GlobalAttributeGroups TABLE (GlobalAttributeGroupId int,GroupCode VARCHAR(200), AttributeGroupName NVARCHAR(600),LocaleId INT,GlobalEntityId INT,EntityName NVARCHAR(300),RowId INT,CountNo INT )  
             
    SET @SQL = '  
     ;WITH CTE_GlobalGroupList AS  
     ( SELECT ZPAG.GlobalAttributeGroupId,ZPAG.GroupCode ,ZPAGL.AttributeGroupName,ZPAGL.LocaleId,ZPAG.GlobalEntityId,ZGE.EntityName  
       FROM ZnodeGlobalAttributeGroup ZPAG   
          Inner JOIN ZnodeGlobalAttributeGroupLocale ZPAGL on (ZPAG.GlobalAttributeGroupId = ZPAGL.GlobalAttributeGroupId )  
		  Inner JOIN ZnodeGlobalEntity ZGE on (ZPAG.GlobalEntityId = ZGE.GlobalEntityId )  
      )  
  
        ,CTE_GlobalGroup AS  
         ( SELECT GlobalAttributeGroupId,GroupCode,AttributeGroupName,LocaleId,GlobalEntityId,EntityName
        ,'+dbo.Fn_GetPagingRowId(@Order_BY,'GlobalAttributeGroupId DESC')+',Count(*)Over() CountNo  
        FROM CTE_GlobalGroupList  
           WHERE 1=1   
        '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'      
      )  
     SELECT GlobalAttributeGroupId,GroupCode,AttributeGroupName,LocaleId,GlobalEntityId,EntityName,RowId,CountNo  
     FROM CTE_GlobalGroup  
     '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
	
	 --PRINT @SQL
     INSERT INTO @TBL_GlobalAttributeGroups(GlobalAttributeGroupId,GroupCode,AttributeGroupName,LocaleId,GlobalEntityId,EntityName,RowId,CountNo)  
     EXEC(@SQL)  
  
     SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GlobalAttributeGroups),0)  
  
     SELECT GlobalAttributeGroupId,GroupCode,AttributeGroupName,LocaleId,GlobalEntityId,EntityName  
     FROM  @TBL_GlobalAttributeGroups  
   
         END TRY  
         BEGIN CATCH  
          DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGlobalAttributeGroups @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'Znode_GetGlobalAttributeGroups',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;                              
         END CATCH;  
     END;