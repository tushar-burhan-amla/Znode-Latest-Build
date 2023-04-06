
CREATE PROCEDURE [dbo].[Znode_GetAssociatedCMSThemeToPortal]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT            = 0)
AS

/*
 Summary : Associating CMSTheme to Portal
		   Result is fetched from view View_GetAssociatedCMSThemeToPortal order by PortalId in descending order
 Unit Testing 
 EXEC Znode_GetAssociatedCMSThemeToPortal 'CMSThemeId = 17 and IsAssociated = 0' , @RowsCount = 0

*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AssociatedCMSThemeToPortal TABLE(PortalId INT,StoreName NVARCHAR(MAX),CMSThemeId INT, CMSPortalThemeId INT,CreatedDate DATE,ModifiedDate DATE,IsAssociated INT,RowId INT,CountNo INT )
        
             SET @WhereClause = REPLACE(@WhereClause, 'IsAssociatedToPortal = 0', ' NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSPortalTheme a WHERE ( a.PortalId = View_GetAssociatedCMSThemeToPortal.PortalId ))');
            
			 SET @SQL = '
						; WITH CTE_AssociatedCMSThemeToPortal AS
						(
						 SELECT  PortalId,StoreName,CMSThemeId,CMSPortalThemeId,CreatedDate,ModifiedDate,IsAssociated
										,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo
						 FROM View_GetAssociatedCMSThemeToPortal
						 WHERE 1=1
										'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'

						)
			
			            SELECT PortalId,StoreName,CMSThemeId,CMSPortalThemeId,CreatedDate,ModifiedDate,IsAssociated,RowId,CountNo
						FROM CTE_AssociatedCMSThemeToPortal
										'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			
			
			 INSERT INTO @TBL_AssociatedCMSThemeToPortal(PortalId,StoreName,CMSThemeId,CMSPortalThemeId,CreatedDate,ModifiedDate,IsAssociated,RowId,CountNo)
			 EXEC(@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AssociatedCMSThemeToPortal),0)
			
			 SELECT  PortalId,StoreName,CMSThemeId,CMSPortalThemeId,CreatedDate,ModifiedDate,IsAssociated
			 FROM @TBL_AssociatedCMSThemeToPortal									
			
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAssociatedCMSThemeToPortal @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAssociatedCMSThemeToPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;