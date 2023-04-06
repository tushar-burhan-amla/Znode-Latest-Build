CREATE PROCEDURE [dbo].[Znode_GetTaxClassRule]
(
	@CountryCode   NVARCHAR(20)= ''
	,@StateCode		 NVARCHAR(40)= ''
	,@PortalId           INT = 0 
	,@ProfileId          INT = 0 
	,@UserId              INT = 0  
	,@OrderId              INT = 0  
)
AS 
 /*
   Summary :- This procedure is used to get tax class detail with rule associated to particular tax
   Unit Testing
   EXEC Znode_GetTaxClassDetail
 */
 BEGIN 
  BEGIN TRY 
   SET NOCOUNT ON 
       DECLARE @TaxclassId VARCHAr(2000) = ''
	   DECLARE @TBL_TaxClass TABLE (TaxClassId INT )
	   EXEC [dbo].[Znode_GetCommonTaxClass] @PortalId,@ProfileId,@TaxclassId OUT,@UserId

	 INSERT INTO @TBL_TaxClass(TaxClassId)
	 SELECT Item
	 FROM  dbo.split(@TaxclassId,',') SP 
	   
	 if EXISTS(select * from ZnodeOmsTaxRule WHERE OmsOrderId = @OrderId)
	 
	 begin

		SELECT TaxRuleId,@PortalId AS PortalId,ZTC.TaxClassId,ClassName,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence
		,SalesTax,VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,ZipCode,CONVERT(bit,1) as IsDefault
		FROM ZnodeTaxClass ZTC 
		INNER JOIN ZnodeOmsTaxRule ZTR  ON (ZTR.TaxClassId = ZTC.TaxClassId)
		INNER JOIN ZnodeTaxRuleTypes ZTRT ON (ZTRT.TaxRuleTypeId = ZTR.TaxRuleTypeId AND ZTRT.IsActive = 1 )
		WHERE (ZTR.DestinationCountryCode = @CountryCode  or ZTR.DestinationCountryCode  is null)
		AND (ZTR.DestinationStateCode = @StateCode or ZTR.DestinationStateCode  is null)
		AND ZTR.OmsOrderId = @OrderId 
		AND ZTC.IsActive = 1 
		AND EXISTS (SELECT TOP 1 1 FROM ZnodeTaxClass TBTC WHERE TBTC.TaxClassId = ZTC.TaxClassId)
		ORDER BY Precedence

	end 

	else begin
	   	   
		SELECT TaxRuleId,ZPTC.PortalId,ZTC.TaxClassId,ClassName,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence
		,SalesTax,VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,ZipCode,IsDefault
		FROM ZnodeTaxClass ZTC 
		INNER JOIN ZnodePortalTaxClass ZPTC ON (ZPTC.TaxClassId = ZTC.TaxClassId)
		INNER JOIN ZnodeTaxRule ZTR  ON (ZTR.TaxClassId = ZTC.TaxClassId)
		INNER JOIN ZnodeTaxRuleTypes ZTRT ON (ZTRT.TaxRuleTypeId = ZTR.TaxRuleTypeId AND ZTRT.IsActive = 1 )
		WHERE (ZTR.DestinationCountryCode = @CountryCode  or ZTR.DestinationCountryCode  is null)
		AND (ZTR.DestinationStateCode = @StateCode or ZTR.DestinationStateCode  is null)
		AND ZPTC.PortalId = @PortalId
		AND ZTC.IsActive = 1 
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_TaxClass TBTC WHERE TBTC.TaxClassId = ZTC.TaxClassId)
		ORDER BY Precedence

	end
 END TRY 
 BEGIN CATCH 
DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTaxClassRule @CountryCode = '+@CountryCode+',@StateCode='+@StateCode+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTaxClassRule',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
 END CATCH 
END