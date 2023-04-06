CREATE PROCEDURE [dbo].[Znode_GetPimAttributeFamilies]
( @WhereClause VARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT = 1  )
AS
 /*
   Summary:- This procedure is used to get the family list from all locale
			 Result is fetched order by PimAttributeFamilyId in descending order
   Unit Testing:
   EXEC Znode_GetPimAttributeFamilies 'familycode = ''default'' and iscategory = ''False'' and LocaleId = 1' ,@RowsCount= 0 
  
 */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @DefaultLocaleId INT = dbo.Fn_getDefaultLocaleId() 
			 DECLARE @TBL_FamilyList TABLE(PimAttributeFamilyId INT,FamilyCode VARCHAR(200) ,IsSystemDefined BIT,IsDefaultFamily BIT,AttributeFamilyName NVARCHAR(max)
										,LocaleId INT,RowId INT,CountNo INT)
			 
             SET @SQL = '
			           ;With Cte_GetFamilyLocale AS 
					   ( 
					   SELECT  ZPAF.PimAttributeFamilyId,ZPAF.FamilyCode ,ZPAF.IsSystemDefined ,ZPAF.IsDefaultFamily,ZPFL.AttributeFamilyName,ZPFL.LocaleId ,ZPAF.IsCategory 									 
					   FROM ZnodePIMAttributeFamily ZPAF 
					   INNER JOIN ZnodePimFamilyLocale ZPFL ON (ZPAF.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId) 
					   WHERE ZPFL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+','+CAST(@DefaultLocaleId AS VARCHAR(50))+')
					   )
					   , Cte_filterForLocale AS 
					   (
					     SELECT PimAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName,LocaleId,IsCategory 
						 FROM Cte_GetFamilyLocale CTFL 
						 WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
     				   )
					   , Cte_FilterForDefaultLocale AS 
					   (
					    SELECT PimAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName ,LocaleId,IsCategory
						FROM Cte_filterForLocale CFFL
						UNION ALL 
						SELECT PimAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName ,LocaleId,IsCategory
						FROM Cte_GetFamilyLocale CGFL  
					    WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
						AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_filterForLocale CFFL WHERE CGFL.PimAttributeFamilyId = CFFL.PimAttributeFamilyId ) 
					   )
					   ,
					   Cte_filterFamily AS 
					   (
					     SELECT PimAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName 
									,LocaleId,'+dbo.Fn_GetPagingRowId(@Order_By,'PimAttributeFamilyId DESC')+',Count(*)Over() CountNo
						 FROM Cte_FilterForDefaultLocale
						 WHERE  1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
					   ) 	
						SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,AttributeFamilyName,LocaleId,RowId,CountNo
						FROM Cte_filterFamily
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@rows)
			 
			   INSERT INTO @TBL_FamilyList (PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,AttributeFamilyName,LocaleId,RowId,CountNo)
			   EXEC (@SQL)
			   
			   SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_FamilyList),0)	
				
               SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,AttributeFamilyName,LocaleId,RowId,CountNo
			   FROM @TBL_FamilyList
			   
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributeFamilies @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributeFamilies',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                    
         END CATCH;
     END;