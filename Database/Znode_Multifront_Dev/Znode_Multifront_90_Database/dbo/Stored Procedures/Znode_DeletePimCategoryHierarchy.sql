CREATE PROCEDURE [dbo].[Znode_DeletePimCategoryHierarchy]
(
    @PimCategoryHierarchyId INT ,
    @PimCatalogId   INT ,
    @Status         BIT           = 1 OUT
)
AS 
   /*
    Summary:  Delete the category Hierarchy  by using multiple category Ids  
    Unit Testing   
    Begin 
    	Begin Transaction 
    		Exec Znode_DeletePimCategoryHierarchy  @PimCategoryIds ='',@PimCatalogId=1  , @Status = 0   
     SELECT * FROM ZnodePimCategoryHierarchy 
     SELECT * FROM ZnodeCMSAreaMessageKey
    	Rollback Transaction 
    ENd  
  */
     BEGIN
         BEGIN TRAN B;
         BEGIN TRY
             SET NOCOUNT ON; 
             ---- Declare the table to store the comma seperated data into record format----------
             DECLARE @TBL_Category TABLE (
                                         ID            INT IDENTITY(1,1),
                                         PimCategoryHierarchyId INT
										
										 );
             DECLARE @TBL_CategoryHierarchy TABLE (
                                                  PimCategoryId INT, PimCategoryHierarchyId INT
                                                  );

             ---- Declare this table to find the actual deleted ids -----
             DECLARE @TBL_DeletedCategoryId TABLE (
                                                  id            INT IDENTITY(1 , 1) ,
                                                  CMSCategoryId INT
                                                  );
            
			
			 INSERT INTO @TBL_Category (PimCategoryHierarchyId)
                    SELECT  @PimCategoryHierarchyId
                     --- store the comma separeted category id into variable table 


             ;WITH CategoryDetails
                  AS (
                  -- Add the SELECT statement with parameter references here
                  SELECT zpch.PimCategoryId , ParentPimCategoryHierarchyId,zpch.PimCategoryHierarchyId
                  FROM ZnodePimCategoryHierarchy AS zpch 
				  INNER JOIN @TBL_Category AS tc ON ( zpch.PimCategoryHierarchyId = tc.PimCategoryHierarchyId )
                  WHERE PimCatalogId = @PimCatalogId
                  UNION ALL
                  SELECT a.PimCategoryId , a.ParentPimCategoryHierarchyId,a.PimCategoryHierarchyId
                  FROM ZnodePimCategoryHierarchy AS a 
				  INNER JOIN CategoryDetails AS b ON ( a.ParentPimCategoryHierarchyId = b.PimCategoryHierarchyId )
                  WHERE a.PimCatalogId = @PimCatalogId)
                 
				  INSERT INTO @TBL_CategoryHierarchy (PimCategoryId,PimCategoryHierarchyId)
                         SELECT PimCategoryId,PimCategoryHierarchyId
                         FROM CategoryDetails;
				
				 
			 
			 DELETE FROM ZnodePimCategoryHierarchy
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_CategoryHierarchy AS tch
                            WHERE tch.PimCategoryHierarchyId = ZnodePimCategoryHierarchy.PimCategoryHierarchyId );
            
			
		
			
			 
             SET @Status = 1;
             SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
             COMMIT TRAN B;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE() 
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimCategoryHierarchy @PimCategoryIds = '++',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN B;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimCategoryHierarchy',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;