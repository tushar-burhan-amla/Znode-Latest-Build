CREATE PROCEDURE [dbo].[Znode_InsertUpdatePimProduct]
(   @ProductXml NVARCHAR(max),
    @UserId     INT,
    @status     BIT OUT,
	@CopyPimProductId INT =0 )
AS
/*
     Summary : To Insert / Update single Product with multiple attribute values 
     Update Logic: 
*/
     BEGIN

         BEGIN TRY

		     DECLARE @ConvertedXML XML = REPLACE(REPLACE(REPLACE(@ProductXml,' & ', '&amp;'),'"', '&quot;'),'''', '&apos;')
             DECLARE @PimProductId INT= 0;
             DECLARE @PimProductDetail_xml PIMPRODUCTDETAIL;
             INSERT INTO @pimProductDetail_xml
                    SELECT Tbl.Col.value('ProductAttributeId[1]', 'int') AS ProductAttributeId,
                           Tbl.Col.value('ProductAttributeFamilyId[1]', 'int') AS ProductAttributeFamilyId,
                           Tbl.Col.value('ProductAttributeCode[1]', 'NVARCHAR(300)') AS ProductAttributeCode,
                           Tbl.Col.value('ProductAttributeDefaultValueId[1]', 'int') AS ProductAttributeDefaultValueId,
                           Tbl.Col.value('ProductAttributeValueId[1]', 'int') AS ProductAttributeValueId,
                           Tbl.Col.value('LocaleId[1]', 'INT') AS LocaleId,
                           Tbl.Col.value('ProductId[1]', 'INT') AS ProductId,
                           Tbl.Col.value('ProductAttributeValue[1]', 'NVARCHAR(Max)') AS ProductAttributeValue,
                           Tbl.Col.value('AssociatedProducts[1]', 'NVARCHAR(2000)') AS AssociatedProducts,
                           Tbl.Col.value('ConfigureAttributeIds[1]', 'NVARCHAR(2000)') AS ConfigureAttributeIds,
                           Tbl.Col.value('ConfigureFamilyIds[1]', 'NVARCHAR(2000)') AS ConfigureFamilyIds
                    FROM @ConvertedXML.nodes('//ArrayOfProductAttributeModel/ProductAttributeModel') AS Tbl(Col);
					--Validating SKU is present or not  
					 SET @PimProductId=(SELECT top 1 ZPAV.PimProductId FROM ZnodePimAttributeValue ZPAV     
										INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId    
										WHERE EXISTS(SELECT * FROM ZnodePimAttribute ZPA WHERE ZPA.PimAttributeId = ZPAV.PimAttributeId AND ZPA.AttributeCode = 'SKU')    
										AND EXISTS(SELECT * FROM @pimProductDetail_xml Z WHERE ProductAttributeCode = 'SKU' AND ZPAVL.AttributeValue = Z.AttributeValue AND ISNULL(Z.PimProductId,0) = 0)    )
     
					IF (@PimProductId  <> 0) 
					BEGIN  
						SET @status = 0  
						SELECT @PimProductId AS ID,CAST(0 AS BIT) AS Status;     
						RETURN;  
					END  
             -- Retrieve input productId from @PimProductDetail table ( having multiple attribute values with common productId) 
         BEGIN TRAN A;
             SET @PimProductId =
             (
                 SELECT TOP 1 PimProductId
                 FROM @PimProductDetail_xml
             );
             EXEC [dbo].[Znode_ImportInsertUpdatePimProduct]
                  @PimProductDetail_xml,
                  @UserId,
                  @status OUT,0
				  ,@CopyPimProductId ; 

			DECLARE @PublishStateIdForDraftState INT = [dbo].[Fn_GetPublishStateIdForDraftState]()
			UPDATE ZCSD set PublishStateId = @PublishStateIdForDraftState
			FROM ZnodeCMSSeoDetail ZCSD
			WHERE EXISTS(select 1 from @PimProductDetail_xml x where x.ProductAttributeCode = 'SKU' AND ZCSD.SEOCode = LTRIM(RTRIM(x.AttributeValue)))
			
             SET @status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdatePimProduct @ProductXml = '+CAST(@ProductXml AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@CopyPimProductId='+CAST(@CopyPimProductId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdatePimProduct',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;