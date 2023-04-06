CREATE  PROCEDURE [dbo].[Znode_GetListOfPimAttributeValues]
( @AttributeCode  VARCHAR(100),
  @AttributeValue VARCHAR(1000) = '')
AS
  /*
    
    Summary: Get list of attribute values for PIm only 
    		   Input pass Attribute code and for find out specific string pass  AttributeValue will be use in query with like operator 
    		    		
    Unit Testing      
	begin tran
    Exec Znode_GetListOfPimAttributeValues @AttributeCode = 'SKU' , @AttributeValue = '234234234234'
	rollback tran
    Exec Znode_GetListOfPimAttributeValues @AttributeCode = 'ProductName' , @AttributeValue = 's'
   
*/

     BEGIN
		BEGIN TRY
		 Declare @PimAttributeId int 
		 
		 Select @PimAttributeId  = PimAttributeId  from ZnodePimAttribute where AttributeCode = @AttributeCode
         If @AttributeCode = 'SKU'
		 Begin
				SELECT TOP 100 ISNULL(NULL, 0) AS RowId, ZPAVL.AttributeValue , Case when ZPDP.SKU is not null then 1 else 0 END IsDownloadable              
				Into #AttributeValueList			
				FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL 
				ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId AND  ZPAV.PimAttributeId = @PimAttributeId
				LEFT OUTER JOIN ZnodePimDownloadableProduct ZPDP ON ZPDP.SKU = ZPAVL.AttributeValue
				WHERE ZPAVL.AttributeValue LIKE '%'+@AttributeValue+'%'

				Select  Distinct RowId ,AttributeValue ,IsDownloadable from #AttributeValueList
		 END
		 Else 
		 Begin
				SELECT TOP 100 ISNULL(NULL, 0) AS RowId, ZPAVL.AttributeValue              
				FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL 
				ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
				WHERE ZPAV.PimAttributeId = @PimAttributeId
				AND ZPAVL.AttributeValue LIKE '%'+@AttributeValue+'%'
				GROUP BY ZPAVL.AttributeValue;
		 End
		 END TRY
		 BEGIN CATCH
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetListOfPimAttributeValues @AttributeCode = '+@AttributeCode+',@AttributeValue='+@AttributeValue+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetListOfPimAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;  
		 END CATCH
     END;