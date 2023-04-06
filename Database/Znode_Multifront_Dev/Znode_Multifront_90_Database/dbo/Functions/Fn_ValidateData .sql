CREATE FUNCTION Fn_ValidateData 
( 
	@AttributeType VARCHAR(200)
	,@Data VARCHAR(Max)
	,@validationRule VARCHAR(300) = ''
	,@validationValue VARCHAR(400) = ''
)

RETURNS BIT

AS
BEGIN
	DECLARE @ATTRIBUTEVALIDATION AS BIT
	-- FOR NUMBER VALIDATION
	IF @AttributeType ='Number'
	BEGIN
		IF ISNUMERIC(@Data) =1
		BEGIN
			SET @ATTRIBUTEVALIDATION = CASE 
				WHEN @validationRule= 'AllowNegative' AND @validationValue = 'FALSE' AND CAST(@Data AS INT) >= 0 THEN 1
				WHEN @validationRule= 'AllowDecimals' AND @validationValue = 'TRUE' AND @Data NOT Like '%_._%' THEN 1
				WHEN @validationRule= 'MinNumber' AND CAST(IIF(@validationValue='',@Data,@validationValue) AS INT) <=CAST(@Data AS INT) THEN 1
				WHEN @validationRule= 'MaxNumber' AND CAST(IIF(@validationValue='',@Data,@validationValue) AS INT) >=CAST(@Data AS INT) THEN 1 ELSE 0 END				
		END
		ELSE 
		BEGIN 
			SET @ATTRIBUTEVALIDATION=0					
		END
	END

	--FOR TEXT VALIDATION
	ELSE IF @AttributeType ='Text'
	BEGIN
		IF @Data='' 
		BEGIN					
			SET @ATTRIBUTEVALIDATION=0				
		END
		ELSE 
		BEGIN
			SET @ATTRIBUTEVALIDATION = CASE
				WHEN @validationRule= 'RegularExpression' AND @Data LIKE '%'+@validationValue+'%' THEN 1 
				WHEN @validationRule= 'MaxCharacters' AND IIF(@validationValue='',LEN(@Data),@validationValue) >= LEN(@Data) THEN 1 ELSE 0 END			
		END
	END

	--FOR DATE VALIDATION
	ELSE IF @AttributeType ='Date'
	BEGIN
		IF ISDATE(@Data)=1
		BEGIN
			SET @ATTRIBUTEVALIDATION = CASE 
				WHEN @validationRule= 'MinDate' AND CAST(IIF(@validationValue='',@Data,@validationValue) AS DATE) >=CAST(@Data AS DATE) THEN 1
				WHEN @validationRule= 'MaxDate' AND CAST(IIF(@validationValue='',@Data,@validationValue) AS DATE) <=CAST(@Data AS DATE) THEN 1 ELSE 0 END 					
		END
		ELSE 
		BEGIN 
			SET @ATTRIBUTEVALIDATION=0				
		END
	END
	
	--FOR YES/NO VALIDATION
	ELSE IF @AttributeType ='Yes/No'
	BEGIN
		IF @Data IN ('1','0','YES','NO','TRUE','FALSE')
		BEGIN
			SET @ATTRIBUTEVALIDATION=1				
		END ELSE 
		BEGIN 
			SET @ATTRIBUTEVALIDATION=0				
		END
	END

	RETURN @ATTRIBUTEVALIDATION
END