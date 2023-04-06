
CREATE PROCEDURE [dbo].[Znode_CheckUniqueGlobalAttributevalues]
(     
	  @AttributeCodeValues xml,
	  @EntityType nvarchar(100), 
	  @EntityValueId int= 0, 
	  @LocaleId int)
AS
 /*
	
Summary : Check existance of attribute code with values,Input parameter will be @AttributeCodeValues as a XML string 	
    	  Used Fn_GetDefaultLocaleId function to find locale default value 
    	
Unit testing 
1. For Global Product Attribute
begin tran 
Exec [Znode_CheckUniqueAttributevalues] @AttributeCodeValues = '' ,@IsCategory=  0 , @LocaleID =1 ,@ProductId = 1
rollback tran
2. For Global Category Attribute 
begin tran
Exec [Znode_CheckUniqueAttributevalues] @AttributeCodeValues = '' ,@IsCategory=  1 , @LocaleID =1,@CategoryId = 1
rollback tran    
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON; 
		 Declare @TableName nvarchar(200) ,@SQL nvarchar(max) ,@ColumnName Nvarchar(2000)
		--Table variable to store string formated attribute code with their values concatenate through _
		DECLARE @DefaultLocaleId int= dbo.Fn_GetDefaultLocaleId()
		Select @TableName=TableName
			from ZnodeGlobalEntity
			Where EntityName =@EntityType
			Set @ColumnName= replace(+replace(@TableName,'Znode',''),'GlobalAttributeValue','')
        DECLARE @TBL_Mappingvalues TABLE
		( 
		  AttributeValue nvarchar(max), GlobalAttributeId int,EntityValueId int 
		);
		DECLARE @TBL_XMLParser TABLE
		( 
		 AttributeCode varchar(600), AttributeValue nvarchar(max), GlobalAttributeId int, AttributeName nvarchar(max)
		);
		INSERT INTO @TBL_XMLParser
		SELECT Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ) AS AttributeCode, Tbl.Col.value( 'AttributeValues[1]', 'NVARCHAR(max)' ) AS AttributeValues, zpa.GlobalAttributeId, zpal.AttributeName
		FROM @AttributeCodeValues.nodes( '//ArrayOfGlobalAttributeCodeValueModel/GlobalAttributeCodeValueModel' ) AS Tbl(Col)
		INNER JOIN ZnodeGlobalAttribute AS ZPA ON(ZPA.AttributeCode = Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ))
		LEFT JOIN ZnodeGlobalAttributeLocale AS ZPAL ON( ZPAL.GlobalAttributeId = ZPA.GlobalAttributeId AND  ZPAL.LocaleId = @DefaultLocaleId);

		DECLARE @AttriburteId TABLE
		( 
		AttributeName nvarchar(max)
		); 
		-- if attribute name in table then it exists otherwise its unique
		BEGIN 

		     	if @TableName is not null
			Begin
		    	Set @SQL =' Select zpavl.AttributeValue, ZPAV.GlobalAttributeId ,ZPAV.'
				         +@ColumnName+'Id 
						from [dbo].['+@TableName+'] ZPAV
						INNER JOIN [dbo].['+@TableName+'Locale]  AS ZPAVL
						ON( ZPAVL.'+@ColumnName+'GlobalAttributeValueId = ZPAV.'+@ColumnName+'GlobalAttributeValueId AND ZPAVL.LocaleId IN('+convert( nvarchar(100),@LocaleId )+' , '+convert( nvarchar(100), @DefaultLocaleId)+' ))
						WHERE ZPAVL.AttributeValue <> '''' '
				   Begin Try
					  insert into @TBL_Mappingvalues
					   EXEC SP_EXECUTESQl  @SQL
				   End Try
					Begin Catch
					select @SQL
					End  Catch;
			end 

		    -- check the attribute value is exists 
			INSERT INTO @AttriburteId 
			SELECT AttributeName FROM @TBL_XMLParser AS TBXP 
			WHERE EXISTS
			(
			SELECT TOP 1 1 FROM @TBL_Mappingvalues a
			WHERE TBXP.GlobalAttributeId = a.GlobalAttributeId 
			AND ISNULL(RTRIM(LTRIM(a.AttributeValue)), '') = TBXP.AttributeValue 
			AND a.EntityValueId <> @EntityValueId
			); 
		END;
		
		SELECT AttributeName FROM @AttriburteId;
	END TRY
	BEGIN CATCH
		--DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_CheckUniqueAttributeValues @AttributeCodeValues = '+CAST(@AttributeCodeValues AS nvarchar(max))+' ,@IsCategory='+CAST(@IsCategory AS varchar(100))+' ,@CategoryId= '+CAST(@CategoryId AS varchar(50))+',@ProductId='+CAST(@ProductId AS varchar(50))+',@LocaleId='+CAST(@LocaleId AS varchar(50));
		--EXEC Znode_InsertProcedureErrorLog 
		--	 @ProcedureName = 'Znode_CheckUniqueAttributeValues',
		--	 @ErrorInProcedure = @Error_procedure,
		--	 @ErrorMessage = @ErrorMessage,
		--	 @ErrorLine = @ErrorLine, 
		--	 @ErrorCall = @ErrorCall;
	END CATCH;
END;