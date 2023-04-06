CREATE PROCEDURE [dbo].[Znode_GetDynamicReport]
( @WhereClause nvarchar(max),
  @Columns varchar(max),
  @ReportType varchar(600), 
  @LocaleId int,
  @CatalogId    int = 0,
  @PriceListId  int = 0,
  @WarehouseId  int = 0,
  @CategoreyId  nvarchar(max) = '',
  @Isdebug bit = 0 , 
  @IscallforTest BIT =0 
  )

AS 
/*
	 Summary:-  This Procedure is Used to get the dynamic report data of product
	 Unit Testing
	 begin tran
	 EXEC Znode_GetDynamicReport '','SKU','Product',1,@CatalogId =1 
	 
	 EXEC Znode_GetDynamicReport '','ProductType,ProductName,SKU,ProductCode,ShortDescription,LongDescription,Assortment,Brand,Vendor,Highlights,Tags,FeatureDescription,ProductSpecification,File,IsActive,UOM,MinimumQuantity,MaximumQuantity,NewProduct,OutOfStockOptions,IsFeatured,CallForPricing,VideoURL,Calories,SpecialFeatures,PriceRange,Family,Specials,Sharp,Mild,GrapeColor,Season,Shelled,Salted,Color,testAttribute,testAttributeMultiSelect,Size,FreeShipping,ShippingCostRules,Weight,Height,Width,Length,ShippingInformation,testNumber,testCode,xbv,ShipSeparately','Product',1,@CatalogId =1 

	 EXEC Znode_GetDynamicReport '','CategoryName,ShortDescription,LongDescription,CategoryTitle,DisplayOrderCategory,AdditionalDescription,CategoryBanner,CategoryImage,ImageAltText','Category',1

      EXEC Znode_GetDynamicReport '', 'OmsOrderDetailsId,OmsOrderId,PortalId,UserId,OrderDate,OmsOrderStateId,ShippingId,PaymentTypeId,BillingFirstName
      ,BillingLastName,BillingCompanyName,BillingStreet1,BillingStreet2,BillingCity,BillingStateCode,BillingPostalCode,BillingCountry,BillingPhoneNumber
      ,BillingEmailId,TaxCost,ShippingCost,Total,subTotal,DiscountAmount,CurrencyCode,OverDueAmount,ShippingNumber,TrackingNumber,CouponCode,PromoDescription
      ,ReferralUserId,PurchaseOrderNumber,OmsPaymentStateId,WebServiceDownloadDate,PaymentSettingId,PaymentTransactionToken,ShipDate,ReturnDate,AddressId
      ,PoDocument,IsActive,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate','Orders',1
	 
	  EXEC Znode_GetDynamicReport '','UserId,AccountId,AspNetUserId,FirstName,LastName,MiddleName,CustomerPaymentGUID,BudgetAmount,Email
	  ,PhoneNumber,EmailOptIn,ReferralStatus,ReferralCommission,ReferralCommissionTypeId,IsActive,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate','User',1
 
	 rollback tran
	 
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @SQL NVARCHAR(max),
				@PimProductId TransferId,
				@OutPimProductIds VARCHAR(max),
				@RowsCount INT  = 0 ,
				@SubTotalString nvarchar(100),
				@PimProductIds VARCHAR(max),
				@IsProductNotIn bit

		DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleId() 
		DECLARE @TransferPimProductId TransferId 
		DECLARE @TBL_CatalogDetails TABLE (PimCatalogId INT,CatalogName NVARCHAR(max) )	
		DECLARE @CategoryNameAttributeId INT = dbo.Fn_GetCategoryNameAttributeId()
		
		INSERT INTO @TBL_CatalogDetails (PimCatalogId,CatalogName)
		SELECT PimCatalogId , CatalogName
		FROM ZnodePimCatalog  ZPC
		WHERE (ZPC.PimCatalogId = Isnull(@CatalogId ,0) OR Isnull(@CatalogId,0)  =0 )
	
	    SELECT PimCategoryId , CategoryValue CategoryName
		INTO #TBL_CategoryDetails
		FROM ZnodePimCategoryAttributeValue ZPCAV 
		INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL ON (ZPCAVL.PimCategoryAttributeValueId = ZPCAV.PimCategoryAttributeValueId AND ZPCAVL.LocaleId = @DefaultLocaleId)
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPCAV.PimAttributeId AND IsCategory = 1 AND ZPA.PimAttributeId = @CategoryNameAttributeId)
		WHERE (EXISTS (SELECT TOP 1 1 FROM dbo.split(@CategoreyId,',') SP WHERE SP.Item= ZPCAV.PimCategoryId) OR @CategoreyId ='')

		DECLARE @TBL_DetailTable TABLE
		( 
			Id int, 
			CountId int, 
			RowId int 
		);

	   DECLARE @TBL_AttributeValue TABLE
        (	    PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  varchar(300),
              PimAttributeId INT,
		    DisplayOrder Bigint
        );
		DECLARE @TBL_AttributeCode TABLE(AttributeCode varchar(300),DisplayOrder Bigint )
		
		DECLARE @ProductData TABLE 
		(AttributeValue Nvarchar(Max), AttributeCode varchar(300), PimProductId  Bigint)

		IF @ReportType = 'Product'
		BEGIN
		      
			 IF Isnull(@CatalogId,0) <> 0 
			 BEGIN 
				Insert into @TransferPimProductId 
				    SELECT  Distinct ZPCC.PimProductId  
				    FROM ZnodePimCategoryProduct ZPCC
					inner join ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
				    WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_CatalogDetails TBCD WHERE TBCD.PimCatalogId = ZPCH.PimCatalogId ) OR Isnull(@CatalogId,0) =0 ) and PimProductId Is not null 
					AND (PimProductId =  @IscallforTest OR @IscallforTest = 0) 
			
				SET @IsProductNotIn  = 0 
			 END 
			 ELSE 
			 BEGIN 
			  Insert into @TransferPimProductId 
			  SELECT 1 
			  WHERE @IscallforTest = 1
			   
			  SET @PimProductIds = CASE WHEN @IscallforTest = 1 THEN 1 ELSE  '' END 
			  SET @IsProductNotIn  = 0 
			 END 
			 
			 EXEC Znode_GetProductIdForPaging
                  @whereClauseXML = @WhereClause,
                  @Rows = 10000000,
                  @PageNo = 1,
                  @Order_BY = '',
                  @RowsCount = @RowsCount OUT,
                  @LocaleId = @LocaleId,
                  @AttributeCode = '',
                  @PimProductId = @TransferPimProductId ,
                  @IsProductNotIn = @IsProductNotIn  ,
			   @OutProductId = @OutPimProductIds OUT;
			
	
			 INSERT INTO @TBL_DetailTable (Id ,RowId,CountId)             
			 SELECT item,Id,@RowsCount
			 FROM  dbo.split(@OutPimProductIds,',') SP 
			
			 --SET @SQL = SUBSTRING((SELECT ','+CAST(Id AS VARCHAR(50)) FROM  @TBL_DetailTable FOR XML PATH('') ),2,4000)
			 INSERT INTO @PimProductId ( Id )
			 SELECT Id FROM  @TBL_DetailTable
		
			 INSERT INTO @TBL_AttributeValue (PimProductId,AttributeValue,AttributeCode,PimAttributeId)
			 EXEC [dbo].[Znode_GetProductsAttributeValue] @PimProductId,@Columns,@LocaleId
		
			 UPDATE A SET A.AttributeValue 
					= SUBSTRING ((SELECT ','+CAST([FileName] AS VARCHAR(50)) FROM ZnodeMedia ZM WHERE EXISTS
					(SELECT TOP 1 1 FROM dbo.split(A.AttributeValue ,',') SP WHERE ltrim(rtrim(sp.Item)) = ZM.MediaId) FOR XML PATH ('') ) ,2,4000)   
					FROM @TBL_AttributeValue   A   where A.AttributeCode in ('ProductImage','GalleryImages','SwatchImage') 

			
			 INSERT INTO @TBL_AttributeCode (AttributeCode ,DisplayOrder )
			 select ltrim(rtrim(SPL.item)),SPL.Id
			 from DBO.Split(@Columns,',') SPL
			 where NOT EXISTS (SELECT TOP 1  1 FROM @TBL_AttributeValue TBAV WHERE TBAV.AttributeCode = SPL.item)
	           
			 SET @Columns = 'CatalogName,CategoryName,' + @Columns 
	           ;With CTE_ProductData AS 
			 (
				SELECT tblAv.AttributeValue, tblAv.AttributeCode, tblAv.PimProductId  
				FROM @TBL_AttributeValue  tblAv 
				UNION All 
				select NULL  AttributeValue, SPL.AttributeCode , AV.PimProductId   
				from @TBL_AttributeCode SPL
				CROSS APPLY @TBL_AttributeValue AV 
				UNION ALL 
				SELECT Distinct  TBCD.CatalogName,'CatalogName' AttributeCode ,  ZPCC.PimProductId 
				FROM ZnodePimCategoryProduct ZPCC
				inner join ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
				INNER JOIN @TBL_CatalogDetails TBCD ON TBCD.PimCatalogId = ZPCH.PimCatalogId 
				WHERE ZPCC.PimProductId IS NOT NULL 
				AND EXISTS (SELECT TOP 1 1 FROM @TBL_AttributeValue TBL WHERE TBL.PimProductId = ZPCC.PimProductId )
				GROUP BY  ZPCC.PimProductId ,TBCD.CatalogName
				UNION ALL 
				SELECT SUBSTRING(( SELECT DISTINCT ','+TBCD.CategoryName 
							FROM #TBL_CategoryDetails TBCD 
							INNER JOIN ZnodePimCategoryProduct ZPCCI ON (ZPCCI.PimCategoryId = TBCD.PimCategoryId )
							inner join ZnodePimCategoryHierarchy ZPCH ON ZPCCI.PimCategoryId = ZPCH.PimCategoryId
						WHERE  ZPCH.PimCatalogId = ZPCH.PimCatalogId AND ZPCC.PimProductId = ZPCCI.PimProductId  FOR XML PATH('')),2,8000)   ,'CategoryName' AttributeCode ,  ZPCC.PimProductId 
				FROM ZnodePimCategoryProduct ZPCC
				INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
				WHERE ZPCC.PimProductId IS NOT NULL  
				AND (PimCatalogId in (Select PimCatalogId FROM @TBL_CatalogDetails) OR Isnull(@CatalogId,0)  =0)
				AND EXISTS (SELECT TOP 1 1 FROM @TBL_AttributeValue TBL WHERE TBL.PimProductId = ZPCC.PimProductId )
				GROUP BY ZPCC.PimProductId ,ZPCH.PimCatalogId 
			 )
			 	Select  AttributeValue, AttributeCode, PimProductId  AS Id
				from CTE_ProductData INNER JOIN DBO.Split(@Columns,',') SPLORD  ON CTE_ProductData.AttributeCode = SPLORD.Item 
				order by PimProductId,SPLORD.id
		END;
		ELSE IF @ReportType = 'Category'
			 BEGIN

			  IF Isnull(@CatalogId,0) <> 0  
			 BEGIN 

			 SET @PimProductIds = SUBSTRING ((SELECT  ','+CAST(ZPCC.PimCategoryId AS VARCHAr(50)) 
												FROM ZnodePimCategoryProduct ZPCC
												INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
												WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_CatalogDetails TBCD WHERE TBCD.PimCatalogId = ZPCH.PimCatalogId ) OR Isnull(@CatalogId,0) =0 )
												FOR XML PATH('')
											),2,4000)
			
			 END 
			 ELSE 
			 BEGIN 
			  SET @PimProductIds = ''
			 END 

				INSERT INTO @TBL_DetailTable( Id, CountId, RowId )
				EXEC Znode_GetCategoryIdForPaging 
				@WhereClauseXML = @WhereClause, 
				@Rows = 10000000, 
				@PageNo = 1, 
				@Order_BY = '', 
				@RowsCount = 0, 
				@LocaleId = @LocaleId, 
				@AttributeCode = '', 
				@PimCategoryId = @PimProductIds,
				@IsAssociated = 1;
				
				Insert into @TBL_AttributeValue (AttributeValue,AttributeCode,PimProductId)
				SELECT CategoryValue AS AttributeValue, AttributeCode, PimCategoryId AS Id
				FROM View_PimCategoryAttributeValue AS VICAV
				WHERE EXISTS
				(
					SELECT TOP 1 1
					FROM @TBL_DetailTable AS TBP
					WHERE VICAV.PimCategoryId = TBP.Id
				) AND 
					  EXISTS
				(
					SELECT TOP 1 1
					FROM dbo.split( @Columns, ',' ) AS SP
					WHERE SP.Item = VICAV.AttributeCode
				) AND  LocaleId = @LocaleId;

						
				Insert into @TBL_AttributeValue (AttributeValue,AttributeCode,PimProductId)
				SELECT CategoryValue AS AttributeValue, AttributeCode, PimCategoryId AS Id
				FROM View_PimCategoryAttributeValue AS VICAV
				WHERE EXISTS
				(
					SELECT TOP 1 1
					FROM @TBL_DetailTable AS TBP
					WHERE VICAV.PimCategoryId = TBP.Id
				) AND 
					  EXISTS
				(
					SELECT TOP 1 1
					FROM dbo.split( @Columns, ',' ) AS SP
					WHERE SP.Item = VICAV.AttributeCode
				) AND  LocaleId = @DefaultLocaleId
				AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_AttributeValue RTRR WHERE VICAV.PimCategoryId = RTRR.PimProductId AND VICAV.AttributeCode = RTRR.AttributeCode)
				;


					
				UPDATE A SET A.AttributeValue 
				= SUBSTRING ((SELECT ','+CAST([FileName] AS VARCHAR(50)) FROM ZnodeMedia ZM WHERE EXISTS
				(SELECT TOP 1 1 FROM dbo.split(A.AttributeValue ,',') SP WHERE ltrim(rtrim(sp.Item)) = ZM.MediaId) FOR XML PATH ('') ) ,2,4000)   
				FROM @TBL_AttributeValue   A   where A.AttributeCode in ('CategoryImage') 

			
				;With CTE_CategoryData AS 
				(
				    select AttributeValue  ,AttributeCode,PimProductId  from @TBL_AttributeValue
				    UNION All 
				    select  NULL  AttributeValue, SPL.item , AV.PimProductId  from DBO.Split(@Columns,',') SPL
				    CROSS JOIN @TBL_AttributeValue AV
				    where SPL.item Not in (Select Distinct AttributeCode from @TBL_AttributeValue)
				    UNION ALL
				    SELECT DISTINCT TBCD.CatalogName,'CatalogName' AttributeCode ,  ZPCC.PimCategoryId 
				    FROM ZnodePimCategoryHierarchy ZPCC
				    INNER JOIN @TBL_CatalogDetails TBCD ON TBCD.PimCatalogId = ZPCC.PimCatalogId 
				)
				Select  AttributeValue, AttributeCode, PimProductId  AS Id  from CTE_CategoryData LEft Outer JOIN DBO.Split(@Columns,',') SPLORD  
				ON CTE_CategoryData.AttributeCode = SPLORD.Item 
				Order by CTE_CategoryData.PimProductId, SPLORD.Id



	    END
	    ELSE IF @ReportType = 'Inventory' 
	    BEGIN 
		SET @Columns = Replace (@Columns, 'Quantity','dbo.Fn_GetDefaultInventoryRoundOff(Quantity) Quantity')   
		SET @Columns = Replace (@Columns, 'ReOrderLevel','dbo.Fn_GetDefaultInventoryRoundOff(ReOrderLevel) ReOrderLevel')   
		SET @SQL = 
		'
					;With Cte_Inventory AS (
					SELECT '+@Columns+' FROM ZnodeInventory ZI Inner join ZnodeWarehouse ZW on ZI.WarehouseId  = ZW.WarehouseId  
					WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)  
					+' AND ( ZI.WarehouseId =  ' + CONVERT(Varchar(300), Isnull(@WarehouseId,0)) + ' OR ' + CONVERT(Varchar(300), Isnull( @WarehouseId,0) ) + '  = 0 ))

					SELECT * FROM Cte_Inventory '
		EXEC (@SQL)
		END 
		ELSE IF @ReportType = 'Orders' 
		BEGIN 
		    if (Charindex('SubTotal',@Columns) > 0 )
				SET @Columns = Replace (@Columns, 'SubTotal','[SBT]')  

		    SET @Columns = Replace (@Columns, 'TaxCost','dbo.Fn_GetDefaultPriceRoundOff(TaxCost) TaxCost')   
		    SET @Columns = Replace (@Columns, 'ShippingCost','dbo.Fn_GetDefaultPriceRoundOff(ShippingCost) ShippingCost')
		    SET @Columns = Replace (@Columns, 'Total','dbo.Fn_GetDefaultPriceRoundOff(Total) Total')   
		    SET @Columns = Replace (@Columns, 'DiscountAmount','dbo.Fn_GetDefaultPriceRoundOff(DiscountAmount) DiscountAmount')   
		    SET @Columns = Replace (@Columns, 'OverDueAmount','dbo.Fn_GetDefaultPriceRoundOff(OverDueAmount) OverDueAmount')   
		    SET @Columns = Replace (@Columns, '[SBT]','dbo.Fn_GetDefaultPriceRoundOff(SubTotal) SubTotal')   
		   	    
		    SET @SQL = '
					    ;With Cte_Orders AS (
					    SELECT '+@Columns+'
					    FROM ZnodeOmsOrderDetails
					    WHERE 1=1
					    '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
						) 
					    SELECT * 
					    FROM Cte_Orders '
		    EXEC (@SQL)
		END 
		ELSE IF @ReportType = 'User' 
		BEGIN 
		SET @Columns = Replace (@Columns, 'BudgetAmount','dbo.Fn_GetDefaultPriceRoundOff(BudgetAmount) BudgetAmount')   
		SET @SQL = '
					;With Cte_User AS (
					SELECT '+@Columns+'
					FROM ZnodeUser 
					WHERE 1=1
					'+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
					 ) 
					SELECT * 
					FROM Cte_User '
		EXEC (@SQL)
		END 
		ELSE IF @ReportType = 'Pricing' 
		BEGIN 

			 SET @Columns = Replace (@Columns, 'SalesPrice','dbo.Fn_GetDefaultPriceRoundOff(SalesPrice) SalesPrice')   
			 SET @Columns = Replace (@Columns, 'RetailPrice','dbo.Fn_GetDefaultPriceRoundOff(RetailPrice) RetailPrice')
			 SET @Columns = Replace (@Columns, 'ActivationDate','ZP.ActivationDate ActivationDate')
			 SET @Columns = Replace (@Columns, 'ExpirationDate','ZP.ExpirationDate ExpirationDate')
			 SET @Columns = Replace (@Columns, 'PriceListCode','ZPL.ListCode PriceListCode')   

		

		SET @SQL = '
					;With Cte_Pricing AS (
					SELECT '+@Columns+'
					FROM ZnodePrice ZP Inner join ZnodePriceList ZPL ON ZP.PriceListId = ZPL.PriceListId
					WHERE 1=1
					'+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' AND (ZP.PriceListId =  ' + CONVERT(Varchar(300), Isnull(@PriceListId ,0) ) + ' OR ' + CONVERT(Varchar(300), Isnull( @PriceListId,0) ) + '  = 0 ) 
					 ) 
					SELECT * 
					FROM Cte_Pricing '
		EXEC (@SQL)
		--print @SQL
		END 

	END TRY
	BEGIN CATCH
		  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
			 @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetDynamicReport @WhereClause = '+@WhereClause+',@Columns='+@Columns+',@ReportType='+@ReportType+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@CatalogId='+CAST(isnull(@CatalogId,0) AS VARCHAR(50)) +',@PriceListId='+@PriceListId+',@WarehouseId='+@WarehouseId+',@CategoreyId='+@CategoreyId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetDynamicReport',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;