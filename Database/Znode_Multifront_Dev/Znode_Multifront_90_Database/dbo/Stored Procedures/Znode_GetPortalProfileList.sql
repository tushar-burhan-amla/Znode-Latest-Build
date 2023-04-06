
CREATE PROCEDURE [dbo].[Znode_GetPortalProfileList]
(   @WhereClause VARCHAR(1000),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT)
AS 
/*
    Summary: This Procedure Is Used to find the profile associated to the portal
     EXEC Znode_GetPortalProfileList ' '  AND portalid = 1  ',@RowsCount = 1,@Order_BY=' ProfileName DESC'  
    */
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX), @RowsStart VARCHAR(50), @RowsEnd VARCHAR(50);
             SET @RowsStart = CASE
                                  WHEN @Rows >= 1000000
                                  THEN 0
                                  ELSE(@Rows * (@PageNo - 1)) + 1
                              END;
             SET @RowsEnd =   CASE
                                WHEN @Rows >= 1000000
                                THEN @Rows
                                ELSE @Rows * (@PageNo)
                              END;
             SET @SQL = ' 

		     DECLARE @TBL_PortalProfileList TABLE (PortalProfileId	INT, PortalId INT, ProfileId INT, ProfileName NVARCHAR(1000),IsDefaultAnonymousProfile BIT,IsDefaultRegistedProfile BIT,ParentProfileId	INT,ProfileNumber VARCHAR(100),ROWID INT,Counts INT )
		
			 ;With Cte_PortalProfileList AS 
			 (
			 SELECT Zpp.PortalProfileId ,zpp.PortalId ,Zpp.ProfileId ,ZP.ProfileName ,ZPP.IsDefaultAnonymousProfile,ZPP.IsDefaultRegistedProfile,ZP.ParentProfileId,ZPP.ProfileNumber
			 FROM ZnodePortalProfile ZPP INNER JOIN ZnodeProfile ZP ON (ZP.ProfileId = ZPP.ProfileId )) 

			 ,Cte_AfterFilter AS 
			 (
			 SELECT * , DENSE_RANK()OVER( ORDER BY '+CASE WHEN @Order_BY = '' THEN '' ELSE @Order_BY+',' END+' PortalProfileId DESC ) ROWID, COUNT(*)OVER() COUNTS 
			 FROM Cte_PortalProfileList CTPPL '+CASE WHEN @WhereClause = '' THEN '' ELSE ' WHERE '+@WhereClause END+') 
		
			 INSERT INTO  @TBL_PortalProfileList 
			 SELECT PortalProfileId,PortalId,ProfileId,ProfileName,IsDefaultAnonymousProfile,IsDefaultRegistedProfile,ParentProfileId,ProfileNumber,ROWID,Counts FROM Cte_AfterFilter 

			 SET @Count =  ISNULL((SELECT TOP 1 Counts FROM @TBL_PortalProfileList) , 0 )   

			 SELECT PortalProfileId , PortalId , ProfileId , ProfileName ,IsDefaultAnonymousProfile,IsDefaultRegistedProfile,ParentProfileId,ProfileNumber FROM  @TBL_PortalProfileList  WHERE ROWID  BETWEEN '+@RowsStart+' AND '+@RowsEnd+'';
             
             EXEC Sp_Executesql
                  @SQL,
                  N' @Count INT OUT ',
                  @Count = @RowsCount OUT;
             SET @RowsCount = CASE WHEN @RowsCount IS NULL THEN 0 ELSE @RowsCount END;
			
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalProfileList @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPortalProfileList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;