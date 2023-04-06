
CREATE PROCEDURE [dbo].[Znode_GetPimProductAttributeFamilyId]
(   @PimProductId    TransferId READONLY,--  VARCHAR(MAX) = '',
    @IsMultipleProduct BIT          = 0)
AS
/*
     Summary : - This procedure is used to find the attribute family of product except default attribute family 
     Unit Testing 
	 begin tran
     Exec Znode_GetPimProductAttributeFamilyId 7
	 rollback tran
*/
     BEGIN
	 BEGIN TRAN PimProductAttributeFamilyId
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultFamilyId INT= dbo.Fn_GetDefaultPimProductFamilyId();
             DECLARE @TBL_PimProductId TABLE(PimProductId INT);

             --INSERT INTO @TBL_PimProductId SELECT Item FROM dbo.Split(@PimProductId, ',') SP;
			 INSERT INTO @TBL_PimProductId 
			 SELECT Id FROM @PimProductId

             IF @IsMultipleProduct = 0
                 BEGIN
                     SELECT PimAttributeFamilyId AS ProductFamily FROM ZnodePimProduct AS ZPP 
					 WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimProductId TBP WHERE ZPP.PimProductId = TBP.PimProductId);
                 END;
             ELSE
                 BEGIN
                     SELECT PimAttributeFamilyId AS ProductFamily,ZPP.PimProductId FROM ZnodePimProduct AS ZPP
					 WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PimProductId TBP WHERE ZPP.PimProductId = TBP.PimProductId );
                 END;
				 
		 COMMIT TRAN PimProductAttributeFamilyId;
         END TRY
         BEGIN CATCH
          DECLARE @Status BIT ;
		  SET @Status = 0;
		  --DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimProductAttributeFamilyId @PimProductId = '+cast (@PimProductId AS VARCHAR(50))+',@IsMultipleProduct='+CAST(@IsMultipleProduct AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  ROLLBACK TRAN PimProductAttributeFamilyId;

          --EXEC Znode_InsertProcedureErrorLog
          --  @ProcedureName = 'Znode_GetPimProductAttributeFamilyId',
          --  @ErrorInProcedure = @Error_procedure,
          --  @ErrorMessage = @ErrorMessage,
          --  @ErrorLine = @ErrorLine,
          --  @ErrorCall = @ErrorCall;
         END CATCH;
     END;