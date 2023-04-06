
CREATE PROCEDURE [dbo].[Znode_InsertUpdatePimCategory]
(
       @CategoryXML XML ,
       @Status      BIT OUT ,
       @UserId      INT = 0
)
AS
/*
Summary: This Procedure is used to insert/update pimcategory details
Unit Testing
Create  procedure [dbo].[Znode_GetXmlToTable]
(
@XMLDOC varchar(8000)
,@qry nvarchar(max)
)
AS
begin
DECLARE @hdoc INT
DECLARE @xml VARCHAR(MAX)
SELECT @xml = @XMLDOC
DECLARE  @param NVARCHAR(50)
SELECT @param = N'@hdoc INT'
EXEC sp_xml_preparedocument @hdoc OUTPUT, @xml
EXEC sp_executesql @qry, @param, @hdoc
EXEC sp_xml_removedocument @hdoc
end
 SELECT * FROM ZNodePimcategory
 SELECT * FROM ZnodePimCategoryAttributevalues
*/
     BEGIN
         BEGIN TRAN InsertUpdatePimCategory;
         BEGIN TRY
			 SET NOCOUNT ON
		     DECLARE @InsertCategory PimCategoryDetail 
        
            
			 INSERT INTO @InsertCategory
			 SELECT Tbl.Col.value ( 'PimCategoryId[1]' , 'int'
			 ) , Tbl.Col.value ( 'PimAttributeId[1]' , 'int'
			 ) , Tbl.Col.value ( 'PimAttributeValueId[1]' , 'NVARCHAR(MAX)'
			 ) , Tbl.Col.value ( 'PimAttributeDefaultValueId[1]' , 'int'
			 ) , Tbl.Col.value ( 'PimAttributeFamilyId[1]' , 'int'
			 ) , Tbl.Col.value ( 'LocaleId[1]' , 'INT'
			 ) , Tbl.Col.value ( 'AttributeCode[1]' , 'VARCHAR(MAX)'
			 ) , Tbl.Col.value ( 'AttributeValue[1]' , 'NVARCHAR(MAX)'
			 )
			 FROM @CategoryXML.nodes ( '//ArrayOfPIMCategoryValuesListModel/PIMCategoryValuesListModel'
			 ) AS Tbl(Col);
		    
		     EXEC Znode_ImportInsertUpdatePimCategory @InsertCategory,1,@UserId
            
			 SET @Status = 1
             COMMIT TRAN InsertUpdatePimCategory;
         END TRY
         BEGIN CATCH
           
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdatePimCategory @CategoryXML = '+CAST(@CategoryXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN InsertUpdatePimCategory;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdatePimCategory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;