
CREATE PROCEDURE [dbo].[Znode_CheckUniqueFormBuilderAttributevalues]
(
	  @AttributeCodeValues xml,
	  @FormBuilderId int= 0,
	  @PortalId      int=0
	  )
AS
 /*
	
Summary : Check existance of attribute code with values,Input parameter will be @AttributeCodeValues as a XML string 	
    	  Used Fn_GetDefaultLocaleId function to find locale default value 
  */
BEGIN
	BEGIN TRY
		SET NOCOUNT ON; 
		--Table variable to store string formated attribute code with their values concatenate through _
		DECLARE @DefaultLocaleId int= dbo.Fn_GetDefaultLocaleId()
		DECLARE @TBL_XMLParser TABLE
		( 
		 AttributeCode varchar(600), AttributeValue nvarchar(max), GlobalAttributeId int, AttributeName nvarchar(max)
		);
		INSERT INTO @TBL_XMLParser
		SELECT Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ) AttributeCode, 
		Tbl.Col.value( 'AttributeValues[1]', 'NVARCHAR(max)' ) AttributeValues,
		 zpa.GlobalAttributeId, 
		 zpal.AttributeName
		FROM @AttributeCodeValues.nodes( '//ArrayOfGlobalAttributeCodeValueModel/GlobalAttributeCodeValueModel' ) AS Tbl(Col)
		INNER JOIN ZnodeGlobalAttribute AS ZPA ON(ZPA.AttributeCode = Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(2000)' ))
		LEFT JOIN ZnodeGlobalAttributeLocale AS ZPAL ON( ZPAL.GlobalAttributeId = ZPA.GlobalAttributeId AND  ZPAL.LocaleId = @DefaultLocaleId);

		DECLARE @AttriburteId TABLE
		( 
		AttributeName nvarchar(max)
		); 
		    -- check the attribute value is exists 
			INSERT INTO @AttriburteId 
			SELECT AttributeName FROM @TBL_XMLParser 
			AS TBXP WHERE EXISTS
			(
			SELECT TOP 1 1 
			FROM ZnodeFormBuilderSubmit ss
			inner join ZnodeFormBuilderGlobalAttributeValue AS ZPAV on ss.FormBuilderSubmitId=ZPAV.FormBuilderSubmitId
			INNER JOIN ZnodeFormBuilderGlobalAttributeValueLocale AS ZPAVL
			ON  ( ZPAVL.FormBuilderGlobalAttributeValueId = ZPAV.FormBuilderGlobalAttributeValueId)
			WHERE TBXP.GlobalAttributeId = ZPAV.GlobalAttributeId 
			AND ISNULL(RTRIM(LTRIM(zpavl.AttributeValue)), '') = TBXP.AttributeValue 
			AND ss.PortalId = @PortalId
			and ss.FormBuilderId=@FormBuilderId
			AND ZPAVL.AttributeValue <> ''
			)

		SELECT AttributeName FROM @AttriburteId;
	END TRY
	BEGIN CATCH
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_CheckUniqueFormBuilderAttributevalues @AttributeCodeValues = '+CAST(@AttributeCodeValues AS nvarchar(max))--+',@LocaleId='+CAST(@LocaleId AS varchar(50));
		EXEC Znode_InsertProcedureErrorLog 
			 @ProcedureName = 'Znode_CheckUniqueFormBuilderAttributevalues',
			 @ErrorInProcedure = @Error_procedure,
			 @ErrorMessage = @ErrorMessage,
			 @ErrorLine = @ErrorLine, 
			 @ErrorCall = @ErrorCall;
	END CATCH;
END;