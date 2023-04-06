
CREATE PROCEDURE [dbo].[Znode_GetAttributeFamilyByName]
( @AttributeValidationValue VARCHAR(600))

AS
/*
Summary: Used to get AttributeFamily where AttributeName is passed
Unit Testing:
   begin tran
   EXEC Znode_GetAttributeFamilyByName 'YES'
   rollback tran
   SELECT * FROM ZnodeMediaAttributeFamily
   SELECT * FROM ZnodeInputValidationRule  WHERE InputValidationRuleId=53
   SELECT * FROM ZnodeMediaAttributevalidation 

*/
     BEGIN
		SET NOCOUNT ON
         BEGIN TRY
             DECLARE @V_TABLEADD TABLE
             (MediaFamilyLocaleId    INT,
              LocaleId               INT,
              MediaAttributeFamilyId INT,
              AttributeFamilyName    VARCHAR(300),
              Label                  VARCHAR(300),
              Description            VARCHAR(300),
              Name                   VARCHAR(300),
              CreatedBy              INT,
              CreatedDate            DATETIME,
              ModifiedBy             INT,
              ModifiedDate           DATETIME,
              IsDefaultFamily        BIT
             );
             INSERT INTO @V_TABLEADD
                    SELECT DISTINCT zfl.MediaFamilyLocaleId,zfl.LocaleId,zfl.MediaAttributeFamilyId,zfl.AttributeFamilyName,zfl.Label,zfl.Description                           ,
                          
                    (
                        SELECT Name
                        FROM ZnodeMediaAttributeValidation
                        WHERE MediaAttributeId = Zma.MediaAttributeId
                              AND InputValidationId IN
                        (
                            SELECT InputValidationId
                            FROM ZnodeAttributeInputValidation
                            WHERE Name = 'MaxFileSize'
                        )
                    ) AS Name,zfl.CreatedBy,zfl.CreatedDate,zfl.ModifiedBy,zfl.ModifiedDate,zaf.IsDefaultFamily
                    FROM dbo.ZnodeMediaFamilyLocale AS Zfl
                         INNER JOIN ZnodeMediaAttributefamily AS Zaf ON(zaf.MediaAttributeFamilyId = zfl.MediaAttributeFamilyId)
                         INNER JOIN ZnodeMediaFamilyGroupMapper AS Zmfgm ON zmfgm.MediaAttributeFamilyId = zaf.MediaAttributeFamilyId
                         INNER JOIN ZnodeMediaAttribute AS Zma ON Zma.MediaAttributeId = Zmfgm.MediaAttributeId
                                                                  AND Zma.IsSystemDefined = 1
                         LEFT OUTER JOIN ZnodeMediaAttributeValidation AS Zmav ON Zmfgm.MediaAttributeId = Zmav.MediaAttributeId
                         LEFT OUTER JOIN ZnodeAttributeInputValidationRule AS Zivr ON(Zmav.InputValidationRuleId = zivr.InputValidationRuleId)
                                                                                     AND Zmav.InputValidationId = Zivr.InputValidationId
                    WHERE zaf.IsSystemDefined = 1
                          AND Zfl.LocaleId = 1
                          AND REPLACE(zivr.ValidationName, '.', '') = REPLACE(@AttributeValidationValue, '.', ''); 

				SELECT *  FROM @V_TABLEADD; 
            
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAttributeFamilyByName @AttributeValidationValue = '+@AttributeValidationValue+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAttributeFamilyByName',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;  