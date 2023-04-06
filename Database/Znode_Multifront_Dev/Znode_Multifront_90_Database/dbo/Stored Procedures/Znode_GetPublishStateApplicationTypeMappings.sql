

CREATE PROCEDURE [dbo].[Znode_GetPublishStateApplicationTypeMappings]
( @WhereClause VARCHAR(1000),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(100)  = '',
  @RowsCount   INT OUT)
AS
  /*  
    Summary : this procedure is used to Get the Publish State to Application Type
    Unit Testing 	
     EXEC Znode_GetPublishStateApplicationTypeMappings  '',@Order_BY = '' ,@RowsCount= 50
   
	*/
     BEGIN
       BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @PublishStateMapping TABLE ( PublishStateMappingId INT ,PublishStateId TINYINT ,PublishState NVARCHAR(max), PublishStateCode NVARCHAR(max), [Description] NVARCHAR(MAX), ApplicationType NVARCHAR(MAX), IsDefault BIT, IsEnabled BIT, DisplayOrder INT,IsActive BIT,RowId INT,CountNo INT  )
             			 
             SET @SQL = '

				;With PublishStateMapping AS 
				(	
				SELECT PSATM.PublishStateMappingId, PSATM.PublishStateId, PS.DisplayName as PublishState, PS.PublishStateCode, PSATM.Description, PSATM.ApplicationType, PS.IsDefaultContentState as IsDefault, PSATM.IsEnabled, PSATM.DisplayOrder, PSATM.IsActive
				FROM ZnodePublishStateApplicationTypeMapping as PSATM
				INNER JOIN ZnodePublishState as PS ON (PSATM.PublishStateId = PS.PublishStateId)
				) 
				, FilterAboveDataHere AS
				(
				 SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'DisplayOrder ASC')+',Count(*)Over() CountNo
				 FROM  PublishStateMapping 
				 WHERE IsActive=1
			     '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
     			)

				SELECT PublishStateMappingId,PublishStateId, PublishState,PublishStateCode, Description,ApplicationType,IsDefault,IsEnabled,DisplayOrder,IsActive,RowId,CountNo
				FROM FilterAboveDataHere
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
							
				INSERT INTO @PublishStateMapping 
				EXEC(@SQL)

				SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @PublishStateMapping),0)
				
				SELECT PublishStateMappingId,PublishStateId,PublishState,PublishStateCode,[Description],ApplicationType,IsDefault,IsEnabled,RowId,CountNo
				FROM @PublishStateMapping 
								                        
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishStateApplicationTypeMappings @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPublishStateApplicationTypeMappings',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;