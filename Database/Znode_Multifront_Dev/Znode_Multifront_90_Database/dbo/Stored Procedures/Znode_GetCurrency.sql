CREATE PROCEDURE [dbo].[Znode_GetCurrency]            
(            
 @WhereClause NVARCHAR(MAX),            
    @Rows        INT           = 100,            
    @PageNo      INT           = 1,            
    @Order_BY    VARCHAR(100)  = '',            
    @RowsCount   INT OUT            
            
)            
AS             
/*            
  Summary :- This Procedure is used to get the Currency Details.            
  Unit Testig        
  DECLARE @R  int = 0     
  EXEC Znode_GetCurrency '',50,1,'',@R OUT    SELECT    @R 
           
*/            
   BEGIN             
  BEGIN TRY             
   SET NOCOUNT ON             
            
    DECLARE @SQL  NVARCHAR(MAX)             
            
   DECLARE @TBL_Currency TABLE (CurrencyId INT, CurrencyCode NVARCHAR(100),CurrencyName NVARCHAR(200), IsActive BIT, IsDefault BIT,Symbol NVARCHAR(100), CultureId INT,CultureCode NVARCHAR(100),CultureName NVARCHAR (200),RowId INT,CountId INT)          
          
            
    SET @SQL = '            
    ;With Cte_CurrencyDetails AS             
    (            
    SELECT distinct ZCC.CurrencyId ,ZCC.CurrencyCode ,ZCC.CurrencyName ,ZC.IsActive,ZC.IsDefault,ZC.Symbol,ZC.CultureId,ZC.CultureCode,ZC.CultureName          
 FROM  ZnodeCulture ZC LEFT JOIN ZnodeCurrency ZCC ON ZC.CurrencyId = ZCC.CurrencyId                
    )           
    ,Cte_Currency             
    AS (            
    SELECT CurrencyId,CurrencyCode,CurrencyName, IsActive,IsDefault,Symbol,CultureId,CultureCode,CultureName,          
    '+[dbo].[Fn_GetPagingRowId](@Order_BY,'CurrencyId, CultureId DESC')+',Count(*)Over() CountId FROM Cte_CurrencyDetails            
    WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )            
              
    SELECT CurrencyId,CurrencyCode,CurrencyName, IsActive,IsDefault,Symbol,CultureId,CultureCode,CultureName,RowId,CountId         
    FROM Cte_Currency             
    '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)            
             
            
 PRINT @SQL            
    INSERT INTO @TBL_Currency             
               
    EXEC (@SQL)            
            
    SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_Currency),0)            
            
              
   SELECT CurrencyId,CurrencyCode,CurrencyName, IsActive,IsDefault,Symbol,CultureId,CultureCode,CultureName          
   FROM @TBL_Currency            
              
                
            
   END TRY             
   BEGIN CATCH             
   DECLARE @Status BIT ;            
   SET @Status = 0;            
   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCurrency @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='
  
    
      
        
+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')            
                         
   SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                
               
   EXEC Znode_InsertProcedureErrorLog            
     @ProcedureName = 'Znode_GetCurrency',            
     @ErrorInProcedure = 'Znode_GetCurrency',            
     @ErrorMessage = @ErrorMessage,            
     @ErrorLine = @ErrorLine,            
     @ErrorCall = @ErrorCall;            
   END CATCH             
   END