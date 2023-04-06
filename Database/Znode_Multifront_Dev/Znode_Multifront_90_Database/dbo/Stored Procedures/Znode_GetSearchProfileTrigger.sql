
CREATE  PROCEDURE [dbo].[Znode_GetSearchProfileTrigger]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT

)
AS 
   /* 
   SUMMARY : Stored Procedure to Get list of searchProfileid 
   Unit Testing:

   -- EXEC Znode_GetSearchProfileTrigger N'keyword like ''%vvvvvv%'' ',@RowsCount = 0 , @Order_BY = 'UserProfile '
   
   searchProfileid = 2
   	*/

     BEGIN
         BEGIN TRY

		 SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 
		
			DECLARE @TBL_ProfileTrigger TABLE (SearchProfileTriggerId INT,Keyword nvarchar(2000),ProfileId INT,SearchProfileId INT,UserProfile nvarchar(400), RowId INT, CountNo INT)


		SET @SQL = '
						;With Cte_GetProfileTriggerList 
						AS (
						Select ZSPT.SearchProfileTriggerId, ZSPT.Keyword,ZSPT.ProfileId,ZSPT.SearchProfileId,ZP.ProfileName as UserProfile
						FROM  ZnodeSearchProfileTrigger ZSPT 
						--left JOIN ZnodeUserProfile ZUP ON (ZSPT.ProfileId = ZUP.UserProfileID)
						left JOIN ZnodeProfile ZP ON (ZSPT.ProfileId = ZP.ProfileId)
						
									
						)	
						
						
						,Cte_GetFilterProfileTrigger
						AS (
						SELECT SearchProfileTriggerId,Keyword,ProfileId,SearchProfileId,UserProfile,
						'+dbo.Fn_GetPagingRowId(@Order_BY,'SearchProfileTriggerId DESC')+',Count(*)Over() CountNo 
						FROM  Cte_GetProfileTriggerList CGPTL 
						WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						
						)
																								
						SELECT SearchProfileTriggerId,Keyword,ProfileId,SearchProfileId,UserProfile,RowId,CountNo
						FROM Cte_GetFilterProfileTrigger
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
						
						print @sql
						INSERT INTO @TBL_ProfileTrigger(SearchProfileTriggerId,Keyword,ProfileId,SearchProfileId,UserProfile,RowId,CountNo)
						EXEC(@SQL)

						SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_ProfileTrigger ),0)
			
						SELECT SearchProfileTriggerId,Keyword,ProfileId,SearchProfileId,UserProfile
						FROM @TBL_ProfileTrigger

				
		 END TRY
		 BEGIN CATCH
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSearchProfileTrigger @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSearchProfileTrigger',
				@ErrorInProcedure = 'Znode_GetSearchProfileTrigger',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END