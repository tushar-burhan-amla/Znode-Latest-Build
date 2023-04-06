
CREATE PROCEDURE [dbo].[Znode_CheckUniqueAttributevalues]
(
	  @AttributeCodeValues xml,
	  @IsCategory bit= 0,
	  @ProductId int= 0, 
	  @CategoryId int= 0, 
	  @LocaleId int)
AS
 /*
	
Summary : Check existance of attribute code with values,Input parameter will be @AttributeCodeValues as a XML string 	
    	  Used Fn_GetDefaultLocaleId function to find locale default value 
    	
Unit testing 
1. For Pim Product Attribute
begin tran 
Exec [Znode_CheckUniqueAttributevalues] @AttributeCodeValues = '' ,@IsCategory=  0 , @LocaleID =1 ,@ProductId = 1
rollback tran
2. For Pim Category Attribute 
begin tran
Exec [Znode_CheckUniqueAttributevalues] @AttributeCodeValues = '' ,@IsCategory=  1 , @LocaleID =1,@CategoryId = 1
rollback tran    
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON; 
		--Table variable to store string formated attribute code with their values concatenate through _
		DECLARE @DefaultLocaleId int= dbo.Fn_GetDefaultLocaleId()
		DECLARE @TBL_XMLParser TABLE
		( 
		 AttributeCode varchar(600), AttributeValue nvarchar(max), PimAttributeId int, AttributeName nvarchar(max)
		);
		INSERT INTO @TBL_XMLParser
		SELECT Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ) AS AttributeCode, Tbl.Col.value( 'AttributeValues[1]', 'NVARCHAR(max)' ) AS AttributeValues, zpa.PimAttributeId, zpal.AttributeName
		FROM @AttributeCodeValues.nodes( '//ArrayOfPIMAttributeCodeValueModel/PIMAttributeCodeValueModel' ) AS Tbl(Col)
		INNER JOIN ZnodePimAttribute AS ZPA ON(ZPA.AttributeCode = Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ))
		LEFT JOIN ZnodePimAttributeLocale AS ZPAL ON( ZPAL.PimAttributeId = ZPA.PimAttributeId AND  ZPAL.LocaleId = @DefaultLocaleId);

		DECLARE @AttriburteId TABLE
		( 
		AttributeName nvarchar(max)
		); 
		-- if attribute name in table then it exists otherwise its unique
		IF @IsCategory = 0
		BEGIN
		    -- check the attribute value is exists 
			INSERT INTO @AttriburteId SELECT AttributeName FROM @TBL_XMLParser AS TBXP WHERE EXISTS
			(
			SELECT TOP 1 1 FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL
			ON( ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId AND ZPAVL.LocaleId IN( @LocaleId, @DefaultLocaleId ))
			WHERE TBXP.PimAttributeId = ZPAV.PimAttributeId AND ISNULL(RTRIM(LTRIM(zpavl.AttributeValue)), '') = TBXP.AttributeValue AND ZPAV.PimProductId <> @ProductId AND ZPAVL.AttributeValue <> ''
			); 
		END;
		ELSE
		BEGIN
			INSERT INTO @AttriburteId
			-- check the attribute value is exists 
			SELECT AttributeName FROM @TBL_XMLParser AS asa WHERE EXISTS
			(
			SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValue AS ZCPAV
			INNER JOIN ZnodePimCategoryAttributeValueLocale AS ZCPAVL
			ON( ZCPAV.PimCategoryAttributeValueId = ZCPAVL.PimCategoryAttributeValueId AND ZCPAVL.LocaleId IN( @LocaleId, @DefaultLocaleId ))
			WHERE ZCPAV.PimAttributeId = asa.PimAttributeId AND ISNULL(RTRIM(LTRIM(ZCPAVL.CategoryValue)), '') = Asa.AttributeValue AND ZCPAV.PimCategoryId <> @CategoryId AND ZCPAVl.CategoryValue <> ''
			); 
		END;
		SELECT AttributeName FROM @AttriburteId;
	END TRY
	BEGIN CATCH
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_CheckUniqueAttributeValues @AttributeCodeValues = '+CAST(@AttributeCodeValues AS nvarchar(max))+' ,@IsCategory='+CAST(@IsCategory AS varchar(100))+' ,@CategoryId= '+CAST(@CategoryId AS varchar(50))+',@ProductId='+CAST(@ProductId AS varchar(50))+',@LocaleId='+CAST(@LocaleId AS varchar(50));
		EXEC Znode_InsertProcedureErrorLog 
			 @ProcedureName = 'Znode_CheckUniqueAttributeValues',
			 @ErrorInProcedure = @Error_procedure,
			 @ErrorMessage = @ErrorMessage,
			 @ErrorLine = @ErrorLine, 
			 @ErrorCall = @ErrorCall;
	END CATCH;
END;