
CREATE PROCEDURE [dbo].[Znode_GetDynamicReportData]
( 
	@ReportType NVARCHAR(100)
)
AS 
/*
Summary : - this procedure is used to get the Report on default family attribute 
Unit Testing 
EXEC Znode_GetDynamicReportData 'pricing'
*/ 
BEGIN
BEGIN TRY
	IF @ReportType IN('Product')
	BEGIN
			                        
		Declare @Tlb_Product TABLE (AttributeCode  varchar(300), GroupDisplayOrder int, DisplayOrder int)
		Declare @Tlb_FilterrableColumn TABLE (PimAttributeId  int , AttributeCode varchar(300),DataType Nvarchar(300),GroupDisplayOrder int, DisplayOrder int )
				  
		Insert into @Tlb_Product (AttributeCode  ,GroupDisplayOrder ,DisplayOrder )
		SELECT DISTINCT AttributeCode ,ZPAG.DisplayOrder ,ZPA.DisplayOrder 
		FROM ZnodePimAttribute ZPA
		INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
		INNER JOIN ZnodePimAttributeGroup ZPAG ON ZPFM.PimAttributeGroupId = ZPAG.PimAttributeGroupId
		WHERE ZPA.IsCategory = 0 ;

		Select AttributeCode as ColumnList from @Tlb_Product  ORDER BY 
		CASE when GroupDisplayOrder IS null then 1 else 0 end , GroupDisplayOrder ,
		CASE when DisplayOrder IS null then 1 else 0 end , DisplayOrder		

		insert into @Tlb_FilterrableColumn (PimAttributeId , AttributeCode ,DataType ,GroupDisplayOrder,DisplayOrder )
		SELECT DISTINCT ZPA.PimAttributeId AS Id, ZPA.AttributeCode Name,                                             
		CASE
		WHEN ZAT.AttributeTypeName IN('Multi Select', 'Simple Select', 'Text', 'Text Area')
		THEN 'String'
		WHEN ZAT.AttributeTypeName IN('Number')
		THEN 'Int32'
		WHEN ZAT.AttributeTypeName IN('Yes/No')
		THEN 'Boolean'
		ELSE ZAT.AttributeTypeName
		END DataType,ZPAG.DisplayOrder,ZPA.DisplayOrder
		FROM ZnodePimAttribute ZPA
		INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
		INNER JOIN ZnodePimAttributeGroup ZPAG ON ZPFM.PimAttributeGroupId = ZPAG.PimAttributeGroupId
		WHERE ZPA.IsFilterable = 1  AND ZPA.IsCategory = 0 
		AND ZAT.AttributeTypeName NOT IN('Image') ;

		SELECT PimAttributeId Id, AttributeCode Name ,DataType FROM @Tlb_FilterrableColumn  ORDER BY 
		CASE when GroupDisplayOrder IS null then 1 else 0 end , GroupDisplayOrder ,
		CASE when DisplayOrder IS null then 1 else 0 end , DisplayOrder		 
	END

	ELSE IF @ReportType IN('Category')
	BEGIN 
		Declare @Tlb_Category TABLE (AttributeCode  varchar(300), GroupDisplayOrder int)
				 
		insert into @Tlb_Category (AttributeCode, GroupDisplayOrder)
		SELECT DISTINCT AttributeCode ,ZPA.DisplayOrder               
		FROM ZnodePimAttribute ZPA
		INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		-- INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
		WHERE ZPA.IsCategory = 1;

		SELECT AttributeCode as ColumnList from @Tlb_Category  Order by GroupDisplayOrder

		insert into @Tlb_FilterrableColumn (PimAttributeId , AttributeCode ,DataType ,GroupDisplayOrder )
		SELECT DISTINCT ZPA.PimAttributeId AS Id,ZPA.AttributeCode Name,                                            
		CASE
		WHEN ZAT.AttributeTypeName IN('Multi Select', 'Simple Select', 'Text', 'Text Area')
		THEN 'String'
		WHEN ZAT.AttributeTypeName IN('Number')
		THEN 'Int32'
		WHEN ZAT.AttributeTypeName IN('Yes/No')
		THEN 'Boolean'
		ELSE ZAT.AttributeTypeName
		END DataType,
		ZPA.DisplayOrder
		FROM ZnodePimAttribute ZPA
		INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		-- INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
		WHERE ZPA.IsFilterable = 1 AND ZPA.IsCategory = 1 
		AND ZAT.AttributeTypeName NOT IN('Image');
		SELECT PimAttributeId Id, AttributeCode Name ,DataType FROM @Tlb_FilterrableColumn  ORDER BY GroupDisplayOrder 

	END 

	ELSE IF @ReportType IN('Inventory')
	BEGIN 
		SELECT  Replace(COLUMN_NAME, 'WarehouseId','WarehouseCode') ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeInventory' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
				
		SELECT ORDINAL_POSITION AS Id,Replace(COLUMN_NAME, 'WarehouseId','WarehouseCode')  Name,                                              
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char') OR COLUMN_NAME =  'WarehouseId'
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END DataType				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeInventory' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
	END 

	ELSE IF @ReportType IN('Orders')
	BEGIN 
		SELECT  COLUMN_NAME ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeOmsOrderDetails' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
		SELECT  ORDINAL_POSITION AS Id, COLUMN_NAME Name,                                           
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char')
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END DataType                				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeOmsOrderDetails' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
	END 

	ELSE IF @ReportType IN('Pricing')
	BEGIN 
		SELECT  Replace(COLUMN_NAME ,'PriceListId','PriceListCode') ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodePrice' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate','UomId','UnitSize');
				 
		SELECT ORDINAL_POSITION AS Id, Replace(COLUMN_NAME ,'PriceListId','PriceListCode') Name,   
		CASE when COLUMN_NAME = 'PriceListId' then 'String'  ELSE                                           
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char')
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END END DataType                 				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodePrice' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate','UomId','UnitSize');
	END 

	ELSE IF @ReportType IN('User')
	BEGIN 
		SELECT  COLUMN_NAME ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeUser' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');

		SELECT ORDINAL_POSITION AS Id,COLUMN_NAME Name,                                             
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char')
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END DataType                				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeUser' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
	END 

	ELSE IF @ReportType IN('Account')
	BEGIN 
		SELECT  COLUMN_NAME ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeAccount' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');

		SELECT ORDINAL_POSITION AS Id,COLUMN_NAME Name,                                             
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char')
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END DataType				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeAccount' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
	END 

	ELSE IF @ReportType IN('Voucher')
	BEGIN 
		SELECT  COLUMN_NAME ColumnList
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeGiftCard' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');

		SELECT ORDINAL_POSITION AS Id,COLUMN_NAME Name,                                             
		CASE
		WHEN DATA_TYPE IN('varchar', 'Nvarchar', 'Text', 'Text Area','nchar','char')
		THEN 'String'
		WHEN DATA_TYPE IN('int','numeric')
		THEN 'Int32'
		WHEN DATA_TYPE IN('BIT')
		THEN 'Boolean'
		ELSE DATA_TYPE
		END DataType				
		FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME = 'ZnodeGiftCard' 
		AND COLUMN_NAME not IN ('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate');
	END 

END TRY
BEGIN CATCH
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetDynamicReportData @ReportType = '+@ReportType+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetDynamicReportData',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH
END;