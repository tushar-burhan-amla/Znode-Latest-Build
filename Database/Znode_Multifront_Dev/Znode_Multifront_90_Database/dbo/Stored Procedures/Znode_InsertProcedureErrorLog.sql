/*
 Summary : - 
      This procedure is used to catch the error log of procedures 
	  'client_net_address' is Client Machine IPAddress on which error has occured
	  All ErrorLog Details is stored in ZnodeProceduresErrorLog table
*/

CREATE PROCEDURE [dbo].[Znode_InsertProcedureErrorLog]
( @ProcedureName    VARCHAR(1000),
  @ErrorInProcedure VARCHAR(1000),
  @ErrorMessage     NVARCHAR(MAX),
  @ErrorLine        VARCHAR(100),
  @ErrorCall        NVARCHAR(MAX))
AS 
   
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @ClientIPMachine NVARCHAR(100)= CONVERT(NVARCHAR(100), CONNECTIONPROPERTY('client_net_address'));

			 IF EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting ZGS WHERE FeatureName = 'IsDataBaseLoggingEnabled' AND FeatureValues = 'TRUE')
			 BEGIN
             INSERT INTO ZnodeProceduresErrorLog (ProcedureName,ErrorInProcedure,ErrorMessage,ErrorLine,ErrorCall,CreatedBy,CreatedDate)
                    SELECT @ProcedureName,@ErrorInProcedure,@ErrorMessage,@ErrorLine,@ErrorCall,@ClientIPMachine,@GetDate;
			 END
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE(),ERROR_PROCEDURE();
                    
         END CATCH;
     END;