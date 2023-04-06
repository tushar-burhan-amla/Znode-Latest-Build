CREATE PROCEDURE [dbo].[Znode_GetCustomFieldDetail]
( @WhereClause VARCHAR(1000),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(100)  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT           = 1)
AS 
/*
    
    Summary : this procedure is used to Get the customised field 
    Unit Testing 	
     EXEC [Znode_GetCustomFieldDetail]  ' PimProductId = 84 ',@Order_BY = '' ,@RowsCount= 0  ,@Rows= 10, @PageNo = 2
     SELECT * FROM ZnodePublishProduct WHERE PimProductid  = 87
	 exec Znode_GetCustomFieldDetail 
   
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_ListOfCustomeFiled TABLE (PimCustomFieldId INT , PimProductId INT , CustomCode VARCHAR(300),CustomKey NVARCHAR(600) ,CustomValue NVARCHAR(600),RowId INT,CountNo INT)	

             DECLARE @DefaultLocaleId VARCHAR(100)=
             ( SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')

             SET @SQL = '
					
					;With ListOfCustomeFiled AS 
					(
					SELECT ZPCF.PimCustomFieldId ,PimProductId , CustomCode ,LocaleId ,CustomKey ,CustomKeyValue AS CustomValue 
					FROM ZnodePimCustomField ZPCF
					INNER JOIN ZnodePimCustomFieldLocale ZPCFL ON (ZPCFL.PimCustomFieldId =ZPCF.PimCustomFieldId) 
					WHERE ZPCFL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(100))+','+@DefaultLocaleId+')	
					)
					, ListOfCustomeFiledForFirstLoacel AS 
					(SELECT PimCustomFieldId,PimProductId , CustomCode  ,CustomKey ,CustomValue 
					 FROM ListOfCustomeFiled LCFL WHERE LCFL.localeId = '+CAST(@LocaleId AS VARCHAR(100))+'	)

					, ListOfCustomeFiledFoeLocale AS 
					( SELECT PimCustomFieldId,PimProductId , CustomCode  ,CustomKey ,CustomValue
					  FROM ListOfCustomeFiledForFirstLoacel LOCFL 
					  UNION ALL 
					  SELECT PimCustomFieldId,PimProductId , CustomCode ,CustomKey ,CustomValue 
					  FROM ListOfCustomeFiled LCF WHERE LCF.LocaleId = '+@DefaultLocaleId+'					  
					  AND NOT EXISTS (SELECT TOP 1 1 FROM ListOfCustomeFiledForFirstLoacel LOCFFL WHERE LOCFFL.CustomCode = LCF.CustomCode AND LOCFFL.PimProductId = LCF.PimProductId )
					)
					,
					 ListFilterAsPerRequired AS 
					 (SELECT PimCustomFieldId,PimProductId , CustomCode  ,CustomKey ,CustomValue,'+dbo.Fn_GetPagingRowId(@Order_BY,' PimProductId DESC,CustomCode')+',Count(*)Over() CountNo
					 FROM ListOfCustomeFiledFoeLocale
					 WHERE 1=1
			         '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
					 )

					 SELECT PimCustomFieldId,PimProductId , CustomCode  ,CustomKey ,CustomValue,RowId,CountNo 
					 FROM ListFilterAsPerRequired
					 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			
			print @sql
			INSERT INTO @TBL_ListOfCustomeFiled (PimCustomFieldId,PimProductId , CustomCode  ,CustomKey ,CustomValue,RowId,CountNo )
			EXEC (@SQL)
									
			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ListOfCustomeFiled),0)
								
			SELECT PimCustomFieldId CustomFieldId,PimProductId ProductId , CustomCode  ,CustomKey , CustomValue FROM  @TBL_ListOfCustomeFiled
			
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCustomFieldDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCustomFieldDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;