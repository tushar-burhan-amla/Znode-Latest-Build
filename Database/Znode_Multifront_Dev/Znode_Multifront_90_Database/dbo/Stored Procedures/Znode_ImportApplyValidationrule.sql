
CREATE PROCEDURE [dbo].[Znode_ImportApplyValidationrule]
       @AttributeTypeName nvarchar(100),@AttributeCode nvarchar(300),@AttributeValue nvarchar(Max),@TableName  VARCHAR(200),  @ColumnName VARCHAR(MAX) ,
       @NewGUID    NVARCHAR(200) 
AS
BEGIN 
		  --Declare error Log Table 
		
	           DECLARE @SQLQuery NVARCHAR(MAX);
			    IF NOT EXISTS ( SELECT TOP 1 1
                         FROM INFORMATION_SCHEMA.TABLES
                         WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#ZnodeAttributeName'
                       )
			 BEGIN
				CREATE TABLE #ZnodeAttributeName (
				ColumnName NVARCHAR(100) ,
				NewGUID    NVARCHAR(120)
				)
			 END 
				INSERT INTO #ZnodeAttributeName ( ColumnName , NewGUID
				    )
				SELECT item , @NewGUID
				FROM dbo.split ( @ColumnName , ','
				);

	   DECLARE @AttributeValidation TABLE (ControlName varchar(300), ValidationName varchar(300), SubValidationName varchar(300), ValidationValue Varchar(300) , RegExp Varchar(300))
	   DECLARE 
		  @ControlName varchar(300), 
		  @ValidationName varchar(300), 
		  @SubValidationName varchar(300), 
		  @ValidationValue Varchar(300) , 
		  @RegExp Varchar(300)

	    
	   INSERT INTO @AttributeValidation (ControlName , ValidationName , SubValidationName , ValidationValue , RegExp )
	   SELECT DISTINCT 
		  b.ControlName, 
		  b.Name AS ValidationName, c.ValidationName AS SubValidationName, a.Name AS ValidationValue, c.RegExp
	   FROM				     
		  dbo.ZnodePimAttribute AS d INNER JOIN
		  dbo.ZnodePimAttributeLocale AS h ON d.PimAttributeId = h.PimAttributeId INNER JOIN
		  dbo.ZnodePimAttributeGroupMapper AS f ON d.PimAttributeId = f.PimAttributeId LEFT OUTER JOIN
		  dbo.ZnodePimFamilyGroupMapper AS g ON f.PimAttributeGroupId = g.PimAttributeGroupId LEFT OUTER JOIN
		  dbo.ZnodeAttributeType AS j ON d.AttributeTypeId = j.AttributeTypeId LEFT OUTER JOIN
		  dbo.ZnodePimAttributeValidation AS a ON d.PimAttributeId = a.PimAttributeId LEFT OUTER JOIN
		  dbo.ZnodeAttributeInputValidation AS b ON a.InputValidationId = b.InputValidationId LEFT OUTER JOIN
		  dbo.ZnodeAttributeInputValidationRule AS c ON a.InputValidationRuleId = c.InputValidationRuleId
	   WHERE j.AttributeTypeName = @AttributeTypeName AND d.AttributeCode =@AttributeCode
	
	   DECLARE Cr_AttributeValidation CURSOR LOCAL FAST_FORWARD
             FOR SELECT ControlName , ValidationName , SubValidationName , ValidationValue , RegExp 
                 FROM @AttributeValidation;
             OPEN Cr_AttributeValidation;
             FETCH NEXT FROM Cr_AttributeValidation INTO @ControlName , @ValidationName , @SubValidationName , @ValidationValue , @RegExp 
             WHILE @@FETCH_STATUS = 0
                 BEGIN
				 -- In Case of Date datatype check max date and min date if present in validation rule
				 IF @ControlName = 'Date' AND @ValidationName IN ('MaxDate','MinDate')
				 BEGIN
					  IF Isnull(@ValidationValue , '') <> ''   
					  Begin 
						 SET @SQLQuery = 'SELECT ' + CASE WHEN @ValidationName = 'MaxDate' THEN  'Date should be greater than ' + @ValidationValue ELSE 
						 'Date should be less than ' + @ValidationValue END + 
							 ' ErrorDescription, (Select ltrim(rtrim(ColumnName)) from #ZnodeAttributeName Where ltrim(rtrim(ColumnName)) = '''
							+@AttributeCode+''') as [ColumnName], ' 
							+ @AttributeCode + ' AS  AttributeValue,GUID  FROM ' + @TableName + ' where ' + @AttributeCode +
							 CASE WHEN @ValidationName = 'MaxDate' THEN  '<=' + @ValidationValue ELSE '>=' + @ValidationValue  END ;

						  INSERT INTO ZnodeImportLog ( ErrorDescription , ColumnName , Data , GUID )
						  EXEC sys.sp_sqlexec @SQLQuery;
					   END 
				 END
				 
				 IF @ControlName = 'Number' AND @ValidationName IN ('MaxNumber','MinNumber')
				 BEGIN
				
					  IF Isnull(@ValidationValue , '') <> '' 
					  Begin 
						   SET @SQLQuery = 'SELECT ''Invalid Date'' ErrorDescription, 
							  (Select ltrim(rtrim(ColumnName)) from #ZnodeAttributeName Where ltrim(rtrim(ColumnName)) = '''+@AttributeCode+''') as [ColumnName], ' 
							  + @AttributeCode + ' AS  AttributeValue,
							  GUID  FROM ' + @TableName + ' where ' + @AttributeCode + CASE WHEN @ValidationName = 'MaxNumber' THEN  '<=' + @ValidationValue 
							  ELSE '>=' + @ValidationValue  END ;
						   INSERT INTO ZnodeImportLog ( ErrorDescription , ColumnName , Data , GUID )
						   EXEC sys.sp_sqlexec @SQLQuery;
						
					   END 
				 END
				 IF @ControlName = 'Yes/No' AND @ValidationName IN ('AllowNegative') AND  @ValidationValue = 'false' 
				 BEGIN
					  IF Isnull(@ValidationValue , '') <> '' 
					  Begin 
						 SET @SQLQuery = 'SELECT ''Not allow negative value'' ErrorDescription, 
							  (Select ltrim(rtrim(ColumnName)) from #ZnodeAttributeName Where ltrim(rtrim(ColumnName)) = '''+@AttributeCode+''') as [ColumnName], ' 
							  + @AttributeCode + ' AS  AttributeValue,
							  GUID  FROM ' + @TableName + ' where ' + @AttributeCode + ' <= 0 ';
					
						      INSERT INTO ZnodeImportLog ( ErrorDescription , ColumnName , Data , GUID )
						      EXEC sys.sp_sqlexec @SQLQuery;
						
					   END 
				 END
			


			  FETCH NEXT FROM Cr_AttributeValidation INTO @ControlName , @ValidationName , @SubValidationName , @ValidationValue , @RegExp 
			  END
		   CLOSE Cr_AttributeValidation;
             DEALLOCATE Cr_AttributeValidation;
		 --  SELECT ErrorDescription , ColumnName , Data , GUID from ZnodeImportLog	
		   
END