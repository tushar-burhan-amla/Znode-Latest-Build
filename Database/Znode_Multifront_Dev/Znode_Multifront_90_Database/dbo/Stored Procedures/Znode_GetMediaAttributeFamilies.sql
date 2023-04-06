
CREATE PROCEDURE [dbo].[Znode_GetMediaAttributeFamilies]
( @WhereClause NVARCHAR(Max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
/*
   Summary: Get Families of Media Attribute based on MediaAttributeFamilyId
			Output is Stored in temp table #mediafamilyList
			Get the Attribute List using inner join on ZnodeMediaAttributeFamily,ZnodeMediaFamilyLocale
    Unit Testing
	EXEC Znode_GetMediaAttributeFamilies '', @RowsCount = 0

*/
  BEGIN
    BEGIN TRY
	  SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_MediaAttributeFamily TABLE (MediaAttributeFamilyId INT,FamilyCode VARCHAR(200),IsSystemDefined BIT,IsDefaultFamily BIT, AttributeFamilyName NVARCHAR(max) 
									,LocaleId INT,RowId INT,CountNo INT )

             SET @SQL = '  ;With Cte_AttributeFamily AS 
							( 
			                 SELECT  ZMAF.MediaAttributeFamilyId,ZMAF.FamilyCode ,ZMAF.IsSystemDefined ,ZMAF.IsDefaultFamily,ZMFL.AttributeFamilyName,ZMFL.LocaleId 
							 FROM ZnodeMediaAttributeFamily ZMAF 
							 INNER JOIN ZnodeMediaFamilyLocale ZMFL ON (ZMAF.MediaAttributeFamilyId = ZMFL.MediaAttributeFamilyId) 
							)
							,Cte_AttributeFamilyFilter AS
							 (
							  SELECT MediaAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName 
									,LocaleId ,'+dbo.Fn_GetPagingRowId(@Order_BY,'MediaAttributeFamilyId')+',Count(*)Over() CountNo 
							  FROM Cte_AttributeFamily
							  WHERE 1=1 
							  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
							 )	
							 SELECT MediaAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName,LocaleId ,RowId,CountNo 									
							 FROM Cte_AttributeFamilyFilter
							 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)																																																																																																											   
            INSERT INTO @TBL_MediaAttributeFamily (MediaAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName,LocaleId ,RowId,CountNo) 									
			EXEC (@SQL)
			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_MediaAttributeFamily ),0)
			SELECT MediaAttributeFamilyId,FamilyCode ,IsSystemDefined ,IsDefaultFamily,AttributeFamilyName,LocaleId  									
		    FROM @TBL_MediaAttributeFamily
			
         END TRY
         BEGIN CATCH
                DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaAttributeFamilies @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaAttributeFamilies',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                     
         END CATCH;
     END;