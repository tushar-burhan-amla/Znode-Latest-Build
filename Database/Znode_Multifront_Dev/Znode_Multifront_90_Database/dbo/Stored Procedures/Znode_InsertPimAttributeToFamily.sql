CREATE PROCEDURE [dbo].[Znode_InsertPimAttributeToFamily]
(   @PimAttributeId      INT,
    @PimAttributeGroupId INT,
    @UserId              INT)
AS
/*
Summary: This Procedure is used to insert Attribute to family 
Unit Testing:
EXEC [Znode_InsertPimAttributeToFamily] @PimAttributeId =11,@PimAttributeGroupId=7, @UserId= 1
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @FamilyIds TABLE
             (PimFamilyProductId INT,
              MaxDisplayOrder    INT
             );
             INSERT INTO @FamilyIds
                    SELECT PimAttributeFamilyId,
                           COUNT(GroupDisplayOrder)
                    FROM ZnodePimFamilyGroupMapper AS a
                    WHERE PimAttributeGroupId = @PimAttributeGroupId
                          AND PimAttributeId <> @PimAttributeId
                    GROUP BY PimAttributeFamilyId;

             INSERT INTO ZnodePimFamilyGroupMapper(PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined,CreatedBy,
             CreatedDate,ModifiedBy,ModifiedDate)

             SELECT PimFamilyProductId,@PimAttributeGroupId,@PimAttributeId,MaxDisplayOrder,0,@UserId,@GetDate,@UserId,@GetDate FROM @FamilyIds;
             SELECT 1 AS ID,
                    CAST(1 AS BIT) AS STATUS;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertPimAttributeToFamily @PimAttributeId = '+CAST(@PimAttributeId AS VARCHAR(max))+',@PimAttributeGroupId='+CAST(@PimAttributeGroupId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertPimAttributeToFamily',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;