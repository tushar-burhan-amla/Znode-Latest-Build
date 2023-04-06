CREATE PROCEDURE [dbo].[Znode_ImportInventory]
(   @InventoryXML XML,
    @Status       BIT OUT,
    @UserId       INT)
AS
  /*  
     Summary :  Import Inventory data 
    		   Input data in XML format Validate data with all scenario 
     Unit Testing : 
    BEGIN TRANSACTION;
    update ZnodeGlobalSetting set FeatureValues = '5' WHERE FeatureName = 'InventoryRoundOff' 
        DECLARE @status INT;
        EXEC [Znode_ImportInventory] @InventoryXML = '<ArrayOfImportInventoryModel>
     <ImportInventoryModel>
       <SKU>S1002</SKU>
       <Quantity>999998.33</Quantity>
       <ReOrderLevel>10</ReOrderLevel>
       <RowNumber>1</RowNumber>
       <ListCode>TestInventory</ListCode>
       <ListName>TestInventory</ListName>
     </ImportInventoryModel>
    </ArrayOfImportInventoryModel>' , @status = @status OUT , @UserId = 2;
        SELECT @status;
        ROLLBACK TRANSACTION;

	*/

     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             DECLARE @RoundOffValue INT, @MessageDisplay NVARCHAR(100), @MessageDisplayForFloat NVARCHAR(100); 
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             -- Retrive RoundOff Value from global setting 
             SELECT @RoundOffValue = FeatureValues
             FROM ZnodeGlobalSetting
             WHERE FeatureName = 'InventoryRoundOff';

             --@MessageDisplay will use to display validate message for input inventory value  

             DECLARE @sSql NVARCHAR(MAX);
             SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(NVARCHAR(200), @RoundOffValue)+'), 123.12345699 ) ';
             EXEC SP_EXecutesql
                  @sSql,
                  N'@MessageDisplay_new NVARCHAR(100) OUT',
                  @MessageDisplay_new = @MessageDisplay OUT;
             SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(NVARCHAR(200), @RoundOffValue)+'), 0.999999 ) ';
             EXEC SP_EXecutesql
                  @sSql,
                  N'@MessageDisplay_new NVARCHAR(100) OUT',
                  @MessageDisplay_new = @MessageDisplayForFloat OUT;
             DECLARE @InserInventoryForValidation TABLE
             (RowNumber    INT IDENTITY(1, 1),
              SKU          VARCHAR(MAX),
              Quantity     VARCHAR(MAX),
              ReOrderLevel VARCHAR(MAX),
              ListName     VARCHAR(MAX),
              ListCode     VARCHAR(MAX)
             );
             DECLARE @InsertInventory TABLE
             (RowNumber    INT,
              SKU          VARCHAR(300),
              Quantity     NUMERIC(28, 6),
              ReOrderLevel NUMERIC(28, 6),
              ListName     VARCHAR(200),
              ListCode     VARCHAR(600)
             );
             DECLARE @ErrorLogForInsertInventory TABLE
             (RowNumber      INT,
              SKU            VARCHAR(MAX),
              Quantity       VARCHAR(MAX),
              ReOrderLevel   VARCHAR(MAX),
              ListName       VARCHAR(MAX),
              ListCode       VARCHAR(MAX),
              ExpirationDate DATETIME,
              Exceptions     VARCHAR(MAX)
             );
             DECLARE @PimCategoryAttributeValueId TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              PimAttributeId              INT
             );
             --SET @CategoryXML =  REPLACE(@CategoryXML,'<?xml version="1.0" encoding="utf-16"?>','')

             DECLARE @InventoryListId INT;
             DECLARE @XML XML;
             SET @XML = @InventoryXML;
             INSERT INTO @InserInventoryForValidation
                    SELECT Tbl.Col.value('SKU[1]', 'VARCHAR(max)'),
                           Tbl.Col.value('Quantity[1]', 'VARCHAR(max)'),
                           Tbl.Col.value('ReOrderLevel[1]', 'VARCHAR(max)'),
                           Tbl.Col.value('ListName[1]', 'VARCHAR(MAX)'),
                           Tbl.Col.value('ListCode[1]', 'VARCHAR(MAX)')
                    FROM @xml.nodes('//ArrayOfImportInventoryModel/ImportInventoryModel') AS Tbl(Col);

             --Required Validation 
             --UomName should not be null 
             --Data for this Inventory list is already available  
             -- 
             -- 1)  Validation for SKU is pending Proper data not found and 
             --Discussion still open for Publish version where we create SKU and use thi SKU code for validation 
             --Select * from ZnodePimAttributeValue  where PimAttributeId =248
             --select * from View_ZnodePimAttributeValue Vzpa Inner join ZnodePimAttribute Zpa on Vzpa.PimAttributeId=Zpa.PimAttributeId where Zpa.AttributeCode = 'SKU'
             --Select * from ZnodePimAttribute where AttributeCode = 'SKU'
             --2)  Start Data Type Validation for XML Data  
             --SELECT * FROM ZnodeInventory
             --SELECT * FROM ZNodeInventoryList
             UPDATE @InserInventoryForValidation
               SET
                   ReOrderLevel = 0
             WHERE ReOrderLevel = '';

             --SELECT * FROM @InserInventoryForValidation

             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'SKU Should not be empty'
                    FROM @InserInventoryForValidation
                    WHERE ISNULL(SKU, '') = '';
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'Quantity Should not be empty'
                    FROM @InserInventoryForValidation
                    WHERE ISNULL(Quantity, '') = '';
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'ReOrderLevel Should not be empty'
                    FROM @InserInventoryForValidation
                    WHERE ISNULL(ReOrderLevel, '') = '';
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'ListName Should not be empty'
                    FROM @InserInventoryForValidation
                    WHERE ISNULL(ListName, '') = '';
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'ListCode Should not be empty'
                    FROM @InserInventoryForValidation
                    WHERE ISNULL(ListCode, '') = '';
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'You must enter a valid Quantity On Hand (ex. '+CONVERT( VARCHAR(30), @MessageDisplay)+' )'
                    FROM @InserInventoryForValidation
                    WHERE ISNUMERIC(Quantity) = 0;
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'You must enter a valid Re-Order Level  (ex. '+CONVERT( VARCHAR(30), @MessageDisplay)+' )'
                    FROM @InserInventoryForValidation
                    WHERE ISNUMERIC(ReOrderLevel) = 0;
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'Enter a Quantity between 0-'+CONVERT( VARCHAR(30), @MessageDisplay)
                    FROM @InserInventoryForValidation
                    WHERE(LEN(SUBSTRING(Quantity, 1,
                                                  CASE
                                                      WHEN CHARINDEX('.', Quantity) = 0
                                                      THEN 4000
                                                      ELSE CHARINDEX('.', Quantity)-1
                                                  END)) >= 7);
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'Enter a Quantity between 0-'+CONVERT( VARCHAR(30), @MessageDisplayForFloat)
                    FROM @InserInventoryForValidation
                    WHERE(CASE
                              WHEN Quantity LIKE '%.%'
                              THEN LEN(SUBSTRING(Quantity, CHARINDEX('.', Quantity)+1, 4000))
                              ELSE 0
                          END > (@RoundOffValue));
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'Enter a Re-Order Level between 0-'+CONVERT( VARCHAR(30), @MessageDisplay)
                    FROM @InserInventoryForValidation
                    WHERE(LEN(SUBSTRING(ReOrderLevel, 1,
                                                      CASE
                                                          WHEN CHARINDEX('.', ReOrderLevel) = 0
                                                          THEN 4000
                                                          ELSE CHARINDEX('.', ReOrderLevel)
                                                      END)) >= 7);
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'Enter a Re-Order Level between 0-'+CONVERT( VARCHAR(30), @MessageDisplayForFloat)
                    FROM @InserInventoryForValidation
                    WHERE(CASE
                              WHEN ReOrderLevel LIKE '%.%'
                              THEN LEN(SUBSTRING(ReOrderLevel, CHARINDEX('.', ReOrderLevel)+1, 4000))
                              ELSE 0
                          END > (@RoundOffValue));
             DELETE FROM @InserInventoryForValidation
             WHERE RowNumber IN
             (
                 SELECT DISTINCT
                        RowNumber
                 FROM @ErrorLogForInsertInventory
             );
             INSERT INTO @InsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode
                    FROM @InserInventoryForValidation;
					 
             -- start Functional Validation 
             -----------------------------------------------
             INSERT INTO @ErrorLogForInsertInventory
             (RowNumber,
              SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode,
              Exceptions
             )
                    SELECT RowNumber,
                           SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode,
                           'SKU Not Exists'
                    FROM @InsertInventory AS ii
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimAttributeValue AS a
                             INNER JOIN ZNodePimAttribute AS Zpa ON(zpa.PimAttributeId = a.PimAttributeId)
                             INNER JOIN ZnodePimAttributeValueLocale AS b ON a.PimAttributeValueId = b.PimAttributeValueId
                        WHERE zpa.Attributecode = 'SKU'
                              AND b.AttributeValue = ii.SKU
                    );   


			

             -- End Function Validation 	
             -----------------------------------------------
             --- Delete Invalid Data after functional validatin  
             DELETE FROM @InsertInventory
             WHERE RowNumber IN
             (
                 SELECT DISTINCT
                        RowNumber
                 FROM @ErrorLogForInsertInventory
             );
             DECLARE @TBL_ReadyToInsertInventory TABLE
             (SKU          VARCHAR(300),
              Quantity     NUMERIC(28, 6),
              ReOrderLevel NUMERIC(28, 6),
              ListName     VARCHAR(200),
              ListCode     VARCHAR(600)
             );
             INSERT INTO @TBL_ReadyToInsertInventory
             (SKU,
              Quantity,
              ReOrderLevel,
              ListName,
              ListCode
             )
                    SELECT SKU,
                           Quantity,
                           ReOrderLevel,
                           ListName,
                           ListCode
                    FROM @InsertInventory AS a
                    WHERE RowNumber IN
                    (
                        SELECT MAX(RowNumber)
                        FROM @InsertInventory
                        GROUP BY SKU,
                                 ListName,
                                 ListCode
                    );
             DECLARE @InventoryList TABLE
             (ListCode VARCHAR(300),
              ListId   INT
             );
             --INSERT INTO @InventoryList ( ListCode , ListId
             --                           )
             --       SELECT a.ListCode , b.InventoryListId
             --       FROM @TBL_ReadyToInsertInventory AS a INNER JOIN ZnodeInventoryList AS b ON ( a.ListCode = b.ListCode
             --                                                                                     AND
             --                                                                                     a.ListName = b.ListName );
             --INSERT INTO ZnodeInventoryList ( ListName , ListCode , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate
             --                               )
             --OUTPUT INSERTED.ListCode , inserted.InventoryListId
             --       INTO @InventoryList
             --       SELECT DISTINCT
             --              ListName , ListCode , @UserId , @GetDate , @UserId , @GetDate
             --       FROM @TBL_ReadyToInsertInventory AS IP
             --       WHERE NOT EXISTS ( SELECT TOP 1 1
             --                          FROM ZnodeInventoryList AS Zpl
             --                          WHERE Zpl.Listcode = IP.ListCode
             --                                AND
             --                                Zpl.ListName = IP.ListName
             --                        );
             --UPDATE zi
             --       SET Quantity = a.Quantity , ReOrderLevel = a.ReOrderLevel , ModifiedBy = @UserId , ModifiedDate = @GetDate
             --FROM ZNodeInventory zi INNER JOIN @InventoryList b ON ( b.ListId = zi.InventoryListId )
             --                       INNER JOIN @TBL_ReadyToInsertInventory a ON ( a.SKU = zi.SKU
             --                                                                     AND
             --                                                                     a.ListCode = b.ListCode );  
             ------ When sheet contain same sku with multiple quamtity then create two record in table so logic change on date 20/07/2016--- 
             --DELETE FROM ZNodeInventory   WHERE EXISTS (SELECT TOP 1 1 
             --FROM  @InsertInventory a 
             --INNER JOIN @InventoryList b ON (a.ListCode = b.ListCode) WHERE a.SKU =ZNodeInventory.SKU AND b.ListId = ZNodeInventory.InventoryListId   )
             --INSERT INTO ZNodeInventory ( InventoryListId , SKU , Quantity , ReOrderLevel , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate
             --                           )
             --       SELECT DISTINCT
             --              b.ListId , a.SKU , a.Quantity , a.ReOrderLevel , @UserId , @GetDate , @UserId , @GetDate
             --       FROM @TBL_ReadyToInsertInventory AS a INNER JOIN @InventoryList AS b ON ( a.ListCode = b.ListCode )
             --       WHERE NOT EXISTS ( SELECT TOP 1 1
             --                          FROM ZNodeInventory AS zi
             --                          WHERE zi.SKU = a.SKU
             --                                AND
             --                                b.ListId = zi.InventoryListId
             --                        );
             SELECT *
             FROM @ErrorLogForInsertInventory;

             ----SELECT @InventoryListId ID , cast(1 As Bit ) Status  
             --SELECT RowNumber,ErrorDescription,SKU ,	StartQuantity,EndQuantity,Inventory,SalesInventory ,TierInventory ,	Uom ,UnitSize ,	InventoryListCode ,
             --			InventoryListName ,	CurrencyId ,ActivationDate ,ExpirationDate FROM @ErrorLogForInsertInventory

             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH 
           
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInventory @InventoryXML = '+CAST(@InventoryXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportInventory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;