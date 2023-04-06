CREATE PROCEDURE [dbo].[Znode_GetGlobalAttributeFamilies]    
( @WhereClause NVARCHAR(Max) = '',    
  @Rows        INT           = 100,    
  @PageNo      INT           = 1,    
  @Order_BY VARCHAR(1000)    = '',    
  @RowsCount   INT OUT)    
AS    
/*    
Summary: This procedure is used get ZnodeGlobalAttributeFamily from both the locale    
   The result is displayed order by GlobalAttributeFamilyId in descending order    
Unit Testing:    
      
   DECLARE @Ree INT     
         EXEC Znode_GetGlobalAttributeFamilies @WhereClause= 'LocaleId = 1' , @RowsCount =  @Ree OUT, @PageNo = 1, @Rows = 10     
       
       
*/    
     BEGIN    
         SET NOCOUNT ON;    
         BEGIN TRY    
             DECLARE @SQL NVARCHAR(MAX);    
    DECLARE @TBL_GlobalAttributeFamily TABLE (GlobalAttributeFamilyId int,FamilyCode VARCHAR(200),AttributeFamilyName NVARCHAR(300),LocaleId INT,GlobalEntityId INT,EntityName NVARCHAR(300),RowId INT,CountNo INT )    
               
    SET @SQL = '    
     ;WITH CTE_GlobalAttributeFamilyList AS    
     ( SELECT ZGAF.GlobalAttributeFamilyId,ZGAF.FamilyCode,ZGAFL.LocaleId,ZGAFL.AttributeFamilyName,ZGAF.GlobalEntityId,ZGE.EntityName    
       FROM ZnodeGlobalAttributeFamily ZGAF     
          Inner JOIN ZnodeGlobalAttributeFamilyLocale ZGAFL on (ZGAF.GlobalAttributeFamilyId = ZGAFL.GlobalAttributeFamilyId )    
    Inner JOIN ZnodeGlobalEntity ZGE on (ZGAF.GlobalEntityId = ZGE.GlobalEntityId )    
      )    
    
        ,CTE_GlobalAttributeFamily AS    
         ( SELECT GlobalAttributeFamilyId,FamilyCode,LocaleId,AttributeFamilyName,GlobalEntityId,EntityName  
        ,'+dbo.Fn_GetPagingRowId(@Order_BY,'GlobalAttributeFamilyId DESC')+',Count(*)Over() CountNo    
        FROM CTE_GlobalAttributeFamilyList    
           WHERE 1=1     
        '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'        
      )    
     SELECT GlobalAttributeFamilyId,FamilyCode,LocaleId,AttributeFamilyName,GlobalEntityId,EntityName,RowId,CountNo    
     FROM CTE_GlobalAttributeFamily    
     '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)    
   
  
     INSERT INTO @TBL_GlobalAttributeFamily(GlobalAttributeFamilyId,FamilyCode,LocaleId,AttributeFamilyName,GlobalEntityId,EntityName,RowId,CountNo)    
     EXEC(@SQL)    
    
     SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GlobalAttributeFamily),0)    


    
     SELECT GlobalAttributeFamilyId,FamilyCode,LocaleId,AttributeFamilyName,GlobalEntityId,EntityName    
     FROM  @TBL_GlobalAttributeFamily    
     
         END TRY    
         BEGIN CATCH    
          DECLARE @Status BIT ;    
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGlobalAttributeFamilies @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
             EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'Znode_GetGlobalAttributeFamilies',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;                                
         END CATCH;    
     END;