
CREATE PROCEDURE [dbo].[Znode_ImportCheckUniqueAttributevalues]
(
		   @IsCategory          BIT = 0 ,
		   @ProductId           INT = 0 ,
		   @CategoryId          INT = 0 ,
		   @LocaleId            INT,
		   @SourceColumnName    Varchar(300) = '',
		   @AttributeValue	    NVarchar(MAX)= '',
		   @PimAttributeId      int = 0  ,
		   @TableName           Varchar(200)
)
AS
    --------------------------------------------------------------------------------------
    --  Summary : Check existance of attribute code with values
    --		   Input parameter will be @AttributeCodeValues as a XML string 	
    --		   here use Fn_GetDefaultValue function to find the locale default value 
    
    --	Unit testing 
    
    --	1. For Pim Product Attribute 
    --	Exec [Znode_ImportCheckUniqueAttributevalues] @IsCategory=0, @LocaleID =1,	@ProductId = 0,@AttributeCode = 'SKU',	@AttributeValue= 'Comp1_1510',@PimAttributeId=  248
    --	2. For Pim Category Attribute 
    --	Exec [Znode_ImportCheckUniqueAttributevalues] @IsCategory=0, @LocaleID =1,	@ProductId = 0,@AttributeCode = 'SKU',	@AttributeValue= 'Comp1_1510',@PimAttributeId=  248
    --------------------------------------------------------------------------------------

     BEGIN
       BEGIN TRY
             SET NOCOUNT ON; 
             --Table varible to store string formated attribute code with their values concatinate through _
             DECLARE @DefaultLocaleId  INT = dbo.Fn_GetDefaultValue('Locale')
		   DECLARE @SQLQuery NVarchar(max)
	
             IF @IsCategory = 0 
                 BEGIN
			-- Select * from @TBL_XMLParser
                     SET @SQLQuery  = ' Select  tlb.'+ @SourceColumnName + '
								FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON 
								(ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId AND ZPAVL.LocaleId IN (' + Convert( Varchar(100),@LocaleId) + ',' + Convert( Varchar(100),@DefaultLocaleId) + ')) 
								INNER JOIN ' + @TableName + ' tlb ON ZPAVL.AttributeValue = tlb.'+ @SourceColumnName + ' 
								WHERE ZPAV.PimAttributeId = ' + Convert( Varchar(100),@PimAttributeId ) + '
								AND
								ZPAV.PimProductId <> ' + Convert( Varchar(100),@ProductId) + '
								AND
								ZPAVL.AttributeValue <> ''''
                                      '  
 				EXEC sys.sp_sqlexec @SQLQuery;

                 END;
			  ELSE 
			  BEGIN
			           -- Need to test following query 
				      SET @SQLQuery  = ' Select  tlb.'+ @SourceColumnName + '
								FROM ZnodePimCategoryAttributeValue AS ZPAV INNER JOIN ZnodePimCategoryAttributeValueLocale AS ZPAVL ON 
								(ZPAVL.PimCategoryAttributeValueId = ZPAV.PimCategoryAttributeValueId AND ZPAVL.LocaleId IN (' + Convert( Varchar(100),@LocaleId) + ',' + Convert( Varchar(100),@DefaultLocaleId) + ')) 
								INNER JOIN ' + @TableName + ' tlb ON ZPAVL.CategoryValue = tlb.'+ @SourceColumnName + ' 
								WHERE ZPAV.PimAttributeId = ' + Convert( Varchar(100),@PimAttributeId ) + '
								AND
								ZPAV.PimProductId <> ' + Convert( Varchar(100),@ProductId) + '
								AND
								ZPAVL.AttributeValue <> ''''
                                      '  
 				EXEC sys.sp_sqlexec @SQLQuery;
			  END 

       --      ELSE
       --          BEGIN
				   -- SELECT 0 AS 'WIP'; 
				   ----          INSERT INTO @AttriburteId
				   ----                  SELECT PimAttributeId, AttributeValue
				   ----                 FROM @TBL_Parser AS asa
				   ----                 WHERE EXISTS ( SELECT TOP 1 1
				   ----                                FROM ZnodePimCategoryAttributeValue AS ZCPAV INNER JOIN ZnodePimCategoryAttributeValueLocale AS ZCPAVL ON
							--				--( ZCPAV.PimCategoryAttributeValueId = ZCPAVL.PimCategoryAttributeValueId AND ZCPAVL.LocaleId IN (@LocaleId,@DefaultLocaleId) )
				   ----                                WHERE ZCPAV.PimAttributeId = asa.PimAttributeId
				   ----                                      AND
				   ----                                      ZCPAVL.CategoryValue= Asa.AttributeValue
				   ----                                      AND
				   ----                                      ZCPAV.PimCategoryId <> @CategoryId
				   ----                                      AND
				   ----                                      ZCPAVl.CategoryValue <> ''
							--			   ); -- check the attribute value is exists 
       --          END;
         END TRY
         BEGIN CATCH
          DECLARE @Error_procedure VARCHAR(1000) = ERROR_PROCEDURE ()
					,@ErrorMessage NVARCHAR(max) = ERROR_MESSAGE()
					,@ErrorLine    VARCHAR(100)  = ERROR_LINE()
					,@ErrorCall    nvarchaR(MAX) = 'EXEC Znode_ImportCheckUniqueAttributevalues @IsCategory='+CAST(@IsCategory AS VARCHAR(100))
													+' ,@CategoryId= '+CAST(@CategoryId AS VARCHAR(50))+',@ProductId='+CAST(@ProductId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))
          EXEC Znode_InsertProcedureErrorLog @ProcedureName='Znode_ImportCheckUniqueAttributevalues',@ErrorInProcedure=@Error_procedure,@ErrorMessage=@ErrorMessage,@ErrorLine=@ErrorLine,@ErrorCall=@ErrorCall
         END CATCH;
END