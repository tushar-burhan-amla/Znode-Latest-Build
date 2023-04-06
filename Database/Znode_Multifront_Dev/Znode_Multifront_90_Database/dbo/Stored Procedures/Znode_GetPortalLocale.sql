
CREATE PROCEDURE [dbo].[Znode_GetPortalLocale]
(   @WhereClause NVARCHAR(3000),
    @Rows        INT            = 100,
    @PageNo      INT            = 0,
    @Order_BY    VARCHAR(1000)  = '',
    @RowsCount   INT OUT)
AS 
/*
 Summary: This Procedure is used to get Portal information filter by @WhereClause provided
 Unit Testing:
    EXEC Znode_GetPortalLocale '' ,@RowsCount= 0 
*/
     BEGIN
         BEGIN TRY
            SET NOCOUNT ON;
            DECLARE @SQL NVARCHAR(MAX);
            SET @SQL = '
            DECLARE @TBL_PortalLocale TABLE (LocaleId INT,PortalLocaleId INT,PortalId INT ,Code VARCHAR(200),IsDefault	BIT ,Name NVARCHAR(2000),StoreName NVARCHAR(3000) ,IsActive	Bit) 
			
			INSERT INTO @TBL_PortalLocale
			SELECT ZL.LocaleId,ZPL.PortalLocaleId , ZP.PortalId ,ZL.Code ,ISNULL(ZPL.IsDefault,0),ZL.Name,ZP.StoreName,
			CASE WHEN ZPL.PortalLocaleId IS NULL THEN 0 ELSE 1 END IsActive	FROM ZnodeLocale ZL 
			Cross Apply ZnodePortal ZP 
			LEFT JOIN ZnodePortalLocale ZPL ON (ZPL.LocaleId = ZL.LocaleId AND ZPL.PortalId = ZP.PortalId) 
			WHERE ZL.IsActive = 1 
					    
			SELECT @COUNT=COUNT(1) FROM  @TBL_PortalLocale 
			WHERE '+CASE WHEN @WhereClause = '' THEN ' 1=1 ' ELSE @WhereClause END+'

			SELECT LocaleId,PortalLocaleId,PortalId,Code,IsDefault,Name,StoreName,IsActive FROM @TBL_PortalLocale 
			WHERE '+CASE WHEN @WhereClause = '' THEN ' 1=1 ' ELSE @WhereClause END+' order by '+CASE WHEN @Order_BY = '' THEN '1' ELSE @Order_BY END;
            EXEC SP_ExecuteSql
                  @SQL,
                  N'@COUNT INT OUT ',
                  @COUNT = @RowsCount OUT;
            SET @RowsCount = ISNULL(@RowsCount, 0);
			 
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalLocale @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPortalLocale',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;