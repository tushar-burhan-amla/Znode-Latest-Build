CREATE PROCEDURE [dbo].[Znode_ImportDynamicData]
(@TableName Varchar(200), @ColumnName Varchar(MAX),@NewGUID nvarchar(200))
AS
  -----------------------------------------------------------------------------
    --Summary:  Import dynamic data of product 
    --		   	
    --          
    --Unit Testing   

    --Drop table ##test  
   
    --CREATE TABLE ##test (
				--	                ProductType NVARCHAR(100) ,
				--	                ProductName NVARCHAR(100) ,
				--	                LongDescription NVARCHAR(100) ,
				--	   		    Price NVARCHAR(100) ,
				--	                GUID NVARCHAR(300)
    --                  );
    --SELECT *
    --FROM ##test;
    --INSERT INTO ##test  ( ProductType , ProductName , LongDescription ,Price, GUID)
    --VALUES		    ( 'SimpleProduct' , 'Apple' , 'Test Test Test ..' ,'400', NEWID()) ,
    --			    ( 'SimpleProduct' , 'Grabes' , 'Test Test Test ..' ,'500', NEWID()) , ( 'SimpleProduct' , 'Mango' , 'Test Test Test ..' ,'1000', NEWID()) ,
    --			    ( 'SimpleProduct' , 'SweetApple' , 'Test Test Test ..' ,'300', NEWID()) , ( 'SimpleProduct' , 'Banana' , 'Test Test Test ..' ,'50', NEWID());
    --Select * from ##test

    --UPDATE ##test SET price = 'wrong price'  WHERE  ProductName='Apple'


    --	EXEC [Znode_ImportDynamicData] @TableName = '##test' , @ColumnName =' ProductType , ProductName , LongDescription ,Price, GUID',@NewGUID =GUID
    ----------------------------------------------------------------------------- 
    
     BEGIN

		   IF NOT EXISTS (SELECT TOP 1 1 from INFORMATION_SCHEMA.TABLES  WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#ZnodeImportedAttributeName')
			 Create TABLE #ZnodeImportedAttributeName (ColumnName nVarchar(100), GUID nvarchar(120)  )
	
		
		   SELECT  @NewGUID = NewID() 
		   DECLARE @SQLQuery NVARCHAR(MAX);
		   INSERT INTO #ZnodeImportedAttributeName(ColumnName, GUID )  
		   	       SELECT item,@NewGUID FROM dbo.split ( @ColumnName , ',');
			  
			 BEGIN
			  DECLARE @AttributeTypeName NVARCHAR(10) , @AttributeCode NVARCHAR(100) , @AttributeId INT;
		   
			  --Declare error Log Table 
			  Declare @ErrorLog TABLE (ErrorDescription nvarchar(max) , ColumnName nvarchar(max) , Data nvarchar(max) ,  GUID nvarchar(120))
		   
			  --Read all attribute details with their datatype 
			  DECLARE @AttributeDetails Table (PimAttributeId int , AttributeTypeName nVarchar(100), AttributeCode nVarchar(100))	  
			  INSERT INTO @AttributeDetails (PimAttributeId , AttributeTypeName , AttributeCode )
					   SELECT zpa.PimAttributeId , zat.AttributeTypeName , zpa.AttributeCode
					   FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId
					   WHERE zpa.AttributeCode IN ( SELECT ColumnName  FROM #ZnodeImportedAttributeName WHERE GUID = @NewGUID  )
			  SELECT * FROM @AttributeDetails 
			  DECLARE Cr_Attribute CURSOR LOCAL FAST_FORWARD
			  FOR SELECT PimAttributeId , AttributeTypeName , AttributeCode  FROM @AttributeDetails  OPEN Cr_Attribute;
			  FETCH NEXT FROM Cr_Attribute INTO @AttributeId , @AttributeTypeName , @AttributeCode;
			  WHILE @@FETCH_STATUS = 0
				 BEGIN
			
					--Datatype Validation
					    IF @AttributeTypeName = 'Number'
					    BEGIN
						   SET @SQLQuery = 'SELECT ''Data should be numeric'' ErrorDescription, 
						   (Select ColumnName from #ZnodeImportedAttributeName Where ColumnName = ''' + @AttributeCode + ''') as [ColumnName], ' + @AttributeCode + ' AS  AttributeValue,
						   GUID  FROM ' + @TableName + '    where Isnumeric('+@AttributeCode+') = 0 ;';
						   INSERT INTO @ErrorLog  (ErrorDescription , ColumnName , Data ,  GUID )
						   EXEC sys.sp_sqlexec @SQLQuery;
					    END;
					    IF @AttributeTypeName = 'Date'
					    BEGIN
						    SET @SQLQuery = 'SELECT ''Invalid Date'' ErrorDescription, 
						   (Select ColumnName from #ZnodeImportedAttributeName Where ColumnName = ''' + @AttributeCode + ''') as [ColumnName], ' + @AttributeCode + ' AS  AttributeValue,
						   GUID  FROM ' + @TableName + ' where IsDate('+@AttributeCode+') = 0 ;';
						   INSERT INTO @ErrorLog  (ErrorDescription , ColumnName , Data ,  GUID )
						   EXEC sys.sp_sqlexec @SQLQuery;
					    END;
		  
					--Functional Validation include data integraty 
					--SET @sSQL = 'SELECT ' + Convert(varchar(100),@AttributeId ) + 'AttributeId,'+ @AttributeCode + ' AttributeValue FROM test;'
					--EXEC sys.sp_sqlexec @sSQL  
					FETCH NEXT FROM Cr_Attribute INTO @AttributeId , @AttributeTypeName , @AttributeCode;
				 END;
			  CLOSE Cr_Attribute;
			  DEALLOCATE Cr_Attribute;
			 
			  If Exists (SELECT TOP 1 1  FROM @ErrorLog )
				SELECT * FROM @ErrorLog 
			  ELSE 
			  BEGIN 
				SELECT 'Call Function Validation' 
			  END   
         END;
     END;