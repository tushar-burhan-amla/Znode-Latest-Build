CREATE FUNCTION Fn_ValidateDataErrorCode 
( 
	@AttributeType VARCHAR(200)
	,@validationRule VARCHAR(200)
)

RETURNS INT

AS
BEGIN
	DECLARE @ATTRIBUTEVALIDATION INT

	--FOR NUMBER VALIDATION
	IF @AttributeType ='Number'
	BEGIN
		SET @ATTRIBUTEVALIDATION = CASE 
				                        WHEN @validationRule= 'AllowNegative' THEN 4  
										WHEN @validationRule= 'AllowDecimals' THEN 26  
										WHEN @validationRule= 'MinNumber' THEN 3   
										WHEN @validationRule= 'MaxNumber' THEN 3 ELSE 2 END
	END
	ELSE IF @AttributeType ='Date' 
	BEGIN 
		SET @ATTRIBUTEVALIDATION = CASE 
						            WHEN @validationRule= 'MinDate' THEN 6 
						            WHEN @validationRule= 'MaxDate' THEN 7 ELSE 4 END

	END
	ELSE IF @AttributeType ='Text' 
	BEGIN
		SET @ATTRIBUTEVALIDATION = CASE 
                                    WHEN @validationRule= 'RegularExpression' THEN 79
                                    WHEN @validationRule= 'MaxCharacters' THEN 78
                                    WHEN @validationRule= 'UniqueValue' THEN 30 ELSE 50 END
	END 
				
	RETURN @ATTRIBUTEVALIDATION
END