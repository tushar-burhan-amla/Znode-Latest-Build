
CREATE PROCEDURE [dbo].[Znode_ApplicationSetting]
( @ItemName VARCHAR(200))
AS
/*
 Summary: To get application setting of an item
 EXEC Znode_ApplicationSetting @ItemName='ZnodeMediaAttribute'
*/
BEGIN
  SET NOCOUNT ON
  BEGIN TRAN ApplicationSetting;
  BEGIN TRY
  SELECT ApplicationSettingId, ItemName,Setting             
  FROM ZnodeApplicationSetting
  WHERE ItemName = @ItemName;

  select 1/0
  SELECT ListViewId,ApplicationSettingId,ViewName,XmlSetting,IsSelected,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate 
  FROM ZnodeListView;
  COMMIT TRAN ApplicationSetting;
  END TRY
  BEGIN CATCH
  DECLARE @STATUS BIT;
  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ApplicationSetting @ItemName = '+@ItemName+',@Status='+CAST(@Status AS VARCHAR(200));           
			 SET @Status = 0;
             SELECT 0 AS ID, CAST(0 AS BIT) AS Status;                  
 			 rollback tran 	ApplicationSetting;				
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName    = 'Znode_ApplicationSetting',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage     = @ErrorMessage,
                  @ErrorLine        = @ErrorLine,
                  @ErrorCall        = @ErrorCall;
  END CATCH
END;