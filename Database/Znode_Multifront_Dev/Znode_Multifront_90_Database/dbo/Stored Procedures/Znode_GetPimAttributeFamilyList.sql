CREATE PROCEDURE [dbo].[Znode_GetPimAttributeFamilyList]
( @IsCategory BIT = 0,
  @LocaleId   INT)
AS 
   /* 
    Summary:  List of Attribute Family name by Locale wise if attribute family not present then default family data.
	          Result is fetched order by PimAttributeFamilyId   		               
    Unit Testing   
    begin tran			
    EXEC Znode_GetPimAttributeFamilyList 0,0
	rollback tran
	*/
     BEGIN
         SET NOCOUNT ON;
		 BEGIN TRY
				DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();

				;WITH FindFamilyNameFirstLocale  AS 
				(
					    SELECT PimAttributeFamilyId,AttributeFamilyName FROM View_PimAttributeFamilyLocale AS a WHERE IsCategory = @IsCategory AND LocaleId = @LocaleId
				)
				,FindFamilyNameDefaultLocale AS
				(
						SELECT PimAttributeFamilyId, AttributeFamilyName  FROM FindFamilyNameFirstLocale
						UNION ALL
						SELECT PimAttributeFamilyId, AttributeFamilyName FROM View_PimAttributeFamilyLocale AS p WHERE IsCategory = @IsCategory AND LocaleId = @DefaultLocaleId
						AND NOT EXISTS ( SELECT TOP 1 1 FROM FindFamilyNameFirstLocale AS aws WHERE aws.PimAttributeFamilyId = p.PimAttributeFamilyId )
				)

				SELECT * FROM FindFamilyNameDefaultLocale ORDER BY PimAttributeFamilyId;
			  END TRY
			  BEGIN CATCH
					DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributeFamilyList @IsCategory = '+CAST(@IsCategory AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributeFamilyList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
			  END CATCH
     END;