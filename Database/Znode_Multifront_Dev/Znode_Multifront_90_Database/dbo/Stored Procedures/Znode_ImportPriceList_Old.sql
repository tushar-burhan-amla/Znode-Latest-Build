
CREATE PROCEDURE [dbo].[Znode_ImportPriceList_Old](
      @PriceXML XML ,
      @Status   BIT OUT ,
      @UserId   INT)
AS 
    -----------------------------------------------------------------------------
    ----Summary:  Import RetailPrice List 
    ----		  Input XML data extracted in table format (table variable name:  @InsertPriceForValidation) by using  @xml.nodes 
    ----		  Validate data column wise and store error log into @ErrorLogForInsertPrice table 
    ----          Remove wrong data from table @InsertPriceForValidation and inserted correct data into @InsertPrice table for 
    ----		  further processing (Importing to target database )
    ---- Version 1 : Required Validation 
    ---- UomName should not be null 
    ---- Data for this RetailPrice list is already available  
    ---- Version 2 : Required Validation 
    ---- If UomName will be null then insert first record from UomTable and If UomName is wrong then raise error
    ---- SKU with retailprice data is available with price list id will insert 
    ---- multiple SKU with retail price is available then updated last sku details to price table and price tier table for respective price list
    ----1. Import functionality should be provided only for single price list (Validate - Pending) 
    ----  Tier price : TierStartQuantity should not between TierStartQuantity and TierEndQuantity for already existing SKU 
    ----  In case of update details for SKU if any kind of price value will null then avoid it to update on existing value. 
    ----2. From XML only SKU and RetailPrice is mandatory
    ----3. SKUActivation date sholud be less than SKUExpriration date
    ----4. Activation date sholud be less than Expiration date
    ----5. If Tier RetailPrice has values and TierSartQuantity /TierEndQuantity or both has null value then it should not get updated/created.
    ----6. ActivationDate and ExpirationDate value for tier price will be SKUActivationDate SKUExprirationDate 
    --- Change History : 
    --Remove column which is used to store range of qunatity by single column Quantity from table ZnodeTierProduct 
    --Manditory Retail price in Znodepricetable 
    -- SKUActivationfrom date and to date will used for tier price will store in single table ZnodePrice
    --Unit Testing   
    ----------------------------------------------------------------------------
    ----Begin transaction
    ----DEclare @status int 
    ----exec [Znode_ImportPriceList] @PriceXML =	
    ----'<ArrayOfImportPriceModel>
    ---- <ImportPriceModel>
    ----   <SKU>SGHJ150</SKU>
    ----   <RetailPrice>1</RetailPrice>
    ----   <SalesPrice></SalesPrice>
    ----   <SKUActivationDate>2016-01-01</SKUActivationDate>
    ----   <SKUExpirationDate>2016-01-01</SKUExpirationDate>
    ----   <TierStartQuantity></TierStartQuantity>
    ----   <TierEndQuantity>1</TierEndQuantity>
    ----   <TierPrice></TierPrice>
    ----   <UOM></UOM>
    ----   <UnitSize>1</UnitSize>
    ----   <RowNumber>1</RowNumber>
    ----   <SequenceNumber>2</SequenceNumber>
    ----   <PriceListCode>FlipKart</PriceListCode>
    ----   <PriceListName>FlipKart</PriceListName>
    ----   <CurrencyId>12</CurrencyId>
    ----   <ActivationDate>2016-01-01</ActivationDate>
    ----   <ExpirationDate>2016-01-01</ExpirationDate>
    ---- </ImportPriceModel>
    ----</ArrayOfImportPriceModel>'
    ----,@status =@status  out , @UserId=2
    ----select @status  
    ----select * from ZnodePrice where sKU = 'SGHJ150' and PriceId > 427 
    ----Rollback transaction
    --2) 
    --Begin transaction
    --DEclare @status int 
    --exec [Znode_ImportPriceList] @PriceXML =	
    --'<ArrayOfImportPriceModel>
    -- <ImportPriceModel>
    --   <SKU>GP1</SKU>
    --   <RetailPrice>12</RetailPrice>
    --   <SalesPrice></SalesPrice>
    --   <SKUActivationDate></SKUActivationDate>
    --   <SKUExpirationDate></SKUExpirationDate>
    --   <TierStartQuantity>5</TierStartQuantity>
    --   <TierPrice>10</TierPrice>
    --   <UOM>Item</UOM>
    --   <UnitSize>1</UnitSize>
    --   <RowNumber>1</RowNumber>
    --   <SequenceNumber>2</SequenceNumber>
    --   <PriceListCode>FlipKart</PriceListCode>
    --   <PriceListName>FlipKart</PriceListName>
    --   <CurrencyId>13</CurrencyId>
    --   <ActivationDate>2016-01-01</ActivationDate>
    --   <ExpirationDate>2016-01-01</ExpirationDate>
    -- </ImportPriceModel>
    -- <ImportPriceModel>
    --   <SKU>GP1</SKU>
    --   <RetailPrice>12</RetailPrice>
    --   <SalesPrice></SalesPrice>
    --   <SKUActivationDate>2016-01-01</SKUActivationDate>
    --   <SKUExpirationDate>2016-01-01</SKUExpirationDate>
    --   <TierStartQuantity>18</TierStartQuantity>
    --   <TierPrice>8</TierPrice>
    --   <UOM>Item</UOM>
    --   <UnitSize>1</UnitSize>
    --   <RowNumber>1</RowNumber>
    --   <SequenceNumber>32</SequenceNumber>
    --   <PriceListCode>FlipKart</PriceListCode>
    --   <PriceListName>FlipKart</PriceListName>
    --   <CurrencyId>13</CurrencyId>
    --   <ActivationDate>2016-01-01</ActivationDate>
    --   <ExpirationDate>2016-01-01</ExpirationDate>
    -- </ImportPriceModel>
    -- <ImportPriceModel>
    --   <SKU>GP1</SKU>
    --   <RetailPrice>12</RetailPrice>
    --   <SalesPrice></SalesPrice>
    --   <SKUActivationDate>2016-01-01</SKUActivationDate>
    --   <SKUExpirationDate>2016-01-01</SKUExpirationDate>
    --   <TierStartQuantity>18</TierStartQuantity>
    --   <TierPrice>80</TierPrice>
    --   <UOM>Item</UOM>
    --   <UnitSize>1</UnitSize>
    --   <RowNumber>1</RowNumber>
    --   <SequenceNumber>33</SequenceNumber>
    --   <PriceListCode>FlipKart</PriceListCode>
    --   <PriceListName>FlipKart</PriceListName>
    --   <CurrencyId>13</CurrencyId>
    --   <ActivationDate>2016-01-01</ActivationDate>
    --   <ExpirationDate>2016-01-01</ExpirationDate>
    -- </ImportPriceModel>
    --</ArrayOfImportPriceModel>'
    --,@status =@status  out , @UserId=2
    --select @status  
    --select * from ZnodePriceList where ListCode = 'FlipKart'
    --select * from ZnodePricetier where sKU = 'GP1'-- and PriceTierId > 3 
    --Rollback transaction
    ----------------------------------------------------------------------------- 

     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             DECLARE @InsertPriceForValidation TABLE (
                                                     RowNumber         INT IDENTITY(1 , 1) ,
                                                     SKU               VARCHAR(300) ,
                                                     TierStartQuantity VARCHAR(300) ,
                                                     RetailPrice       VARCHAR(300) ,
                                                     SalesPrice        VARCHAR(300) ,
                                                     TierPrice         VARCHAR(300) ,
                                                     Uom               VARCHAR(300) ,
                                                     UnitSize          VARCHAR(300) ,
                                                     PriceListCode     VARCHAR(200) ,
                                                     PriceListName     VARCHAR(600) ,
                                                     CurrencyId        VARCHAR(300) ,
                                                     ActivationDate    VARCHAR(300) NULL ,
                                                     ExpirationDate    VARCHAR(300) NULL ,
                                                     SKUActivationDate VARCHAR(300) NULL ,
                                                     SKUExpirationDate VARCHAR(300) NULL ,
                                                     SequenceNumber    VARCHAR(300)
                                                     );
             DECLARE @InsertPrice TABLE (
                                        RowNumber         INT IDENTITY(1 , 1) ,
                                        SKU               VARCHAR(300) ,
                                        TierStartQuantity NUMERIC(38 , 6) NULL ,
                                        RetailPrice       NUMERIC(38 , 6) ,
                                        SalesPrice        NUMERIC(38 , 6) NULL ,
                                        TierPrice         NUMERIC(38 , 6) NULL ,
                                        Uom               VARCHAR(100) ,
                                        UnitSize          NUMERIC(38 , 6) ,
                                        PriceListCode     VARCHAR(200) ,
                                        PriceListName     VARCHAR(600) ,
                                        CurrencyId        INT ,
                                        ActivationDate    DATETIME NULL ,
                                        ExpirationDate    DATETIME NULL ,
                                        SKUActivationDate VARCHAR(300) NULL ,
                                        SKUExpirationDate VARCHAR(300) NULL ,
                                        SequenceNumber    VARCHAR(300)
                                        );
             DECLARE @ErrorLogForInsertPrice TABLE (
                                                   RowNumber         INT ,
                                                   SKU               VARCHAR(300) ,
                                                   TierStartQuantity VARCHAR(300) ,
                                                   RetailPrice       VARCHAR(300) ,
                                                   SalesPrice        VARCHAR(300) ,
                                                   TierPrice         VARCHAR(300) ,
                                                   Uom               VARCHAR(300) ,
                                                   UnitSize          VARCHAR(300) ,
                                                   PriceListCode     VARCHAR(200) ,
                                                   PriceListName     VARCHAR(600) ,
                                                   CurrencyId        VARCHAR(300) ,
                                                   ActivationDate    VARCHAR(300) NULL ,
                                                   ExpirationDate    VARCHAR(300) NULL ,
                                                   SKUActivationDate VARCHAR(300) NULL ,
                                                   SKUExpirationDate VARCHAR(300) NULL ,
                                                   SequenceNumber    VARCHAR(300) ,
                                                   ErrorDescription  VARCHAR(MAX)
                                                   );
             DECLARE @PimCategoryAttributeValueId TABLE (
                                                        PimCategoryAttributeValueId INT ,
                                                        PimCategoryId               INT ,
                                                        PimAttributeId              INT
                                                        );
             --SET @CategoryXML =  REPLACE(@CategoryXML,'<?xml version="1.0" encoding="utf-16"?>','')

             DECLARE @RoundOffValue INT , @MessageDisplay NVARCHAR(100); 
             -- Retrive RoundOff Value from global setting 

             SELECT @RoundOffValue = FeatureValues
             FROM ZnodeGlobalSetting
             WHERE FeatureName = 'PriceRoundOff';

             --@MessageDisplay will use to display validate message for input inventory value  

             DECLARE @sSql NVARCHAR(MAX);
             SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(12, '+CONVERT(NVARCHAR(200) , @RoundOffValue)+'), 999999.000000000 ) ';
             EXEC SP_EXecutesql @sSql , N'@MessageDisplay_new NVARCHAR(100) OUT' , @MessageDisplay_new = @MessageDisplay OUT;
             DECLARE @PriceListId INT;
             DECLARE @XML XML;
             SET @XML = @PriceXML;
             INSERT INTO @InsertPriceForValidation
                    SELECT Tbl.Col.value ( 'SKU[1]' , 'VARCHAR(300)'
                                         ) , Tbl.Col.value ( 'TierStartQuantity[1]' , 'VARCHAR(300)'
                                                           ) , Tbl.Col.value ( 'RetailPrice[1]' , 'VARCHAR(300)'
                                                                             ) , Tbl.Col.value ( 'SalesPrice[1]' , 'VARCHAR(300)'
                                                                                               ) , Tbl.Col.value ( 'TierPrice[1]' , 'VARCHAR(300)'
                                                                                                                 ) , Tbl.Col.value ( 'UOM[1]' , 'VARCHAR(300)'
                                                                                                                                   ) , Tbl.Col.value ( 'UnitSize[1]' , 'VARCHAR(300)'
                                                                                                                                                     ) , Tbl.Col.value ( 'PriceListCode[1]' , 'VARCHAR(200)'
                                                                                                                                                                       ) , Tbl.Col.value ( 'PriceListName[1]' , 'VARCHAR(600)'
                                                                                                                                                                                         ) , Tbl.Col.value ( 'CurrencyId[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                           ) , Tbl.Col.value ( 'ActivationDate[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                                             ) , Tbl.Col.value ( 'ExpirationDate[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                                                               ) , Tbl.Col.value ( 'SKUActivationDate[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                                                                                 ) , Tbl.Col.value ( 'SKUExpirationDate[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                                                                                                   ) , Tbl.Col.value ( 'SequenceNumber[1]' , 'VARCHAR(300)'
                                                                                                                                                                                                                                                                                                     )
                    FROM @xml.nodes ( '//ArrayOfImportPriceModel/ImportPriceModel'
                                    ) AS Tbl(Col);

             -- 1)  Validation for SKU is pending Proper data not found and 
             --Discussion still open for Publish version where we create SKU and use the SKU code for validation 
             --Select * from ZnodePimAttributeValue  where PimAttributeId =248
             --select * from View_ZnodePimAttributeValue Vzpa Inner join ZnodePimAttribute Zpa on Vzpa.PimAttributeId=Zpa.PimAttributeId where Zpa.AttributeCode = 'SKU'
             --Select * from ZnodePimAttribute where AttributeCode = 'SKU'
             --------------------------------------------------------------------------------------
             --2)  Start Data Type Validation for XML Data  
             --------------------------------------------------------------------------------------			
             ---------------------------------------------------------------------------------------
             ---------If UOM will blank then retrive top -- Finctionality pending 
             ---Validate 



             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'UnitSize should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(UnitSize , '') != ''
                          AND
                          ISNUMERIC(UnitSize) = 0;
             DELETE FROM @InsertPriceForValidation
             WHERE RowNumber IN ( SELECT DISTINCT
                                         RowNumber
                                  FROM @ErrorLogForInsertPrice
                                );
             UPDATE @InsertPriceForValidation
                    SET UOM = ( SELECT TOP 1 Uom
                                FROM ZnodeUOM
                              ) , UnitSize = CASE
                                                 WHEN ISNULL(UnitSize , '0') = '0'
                                                      OR
                                                      ISNULL(UnitSize , '') = ''
                                                 THEN 1
                                                 ELSE UnitSize
                                             END
             WHERE ISNULL(UOM , '') = '';

             ---------------------------------------------------------------------------------------
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'CurrencyId should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNUMERIC(CurrencyId) = 0;
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SequenceNumber Should not be empty'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(SequenceNumber , '') = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SKU Should not be empty'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(SKU , '') = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'UOM Should not be empty'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(UOM , '') = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'PriceListCode should not be empty'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(PriceListCode , '') = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'PriceListName should not be empty'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(PriceListName , '') = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierStartQuantity should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ( ISNUMERIC(TierStartQuantity) = 0
                            AND
                            TierStartQuantity <> '' )
                          OR
                          TierStartQuantity = ''
                          AND
                          TierPrice <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierPrice should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNUMERIC(TierPrice) = 0
                          AND
                          TierPrice <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SalesPrice should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNUMERIC(SalesPrice) = 0
                          AND
                          SalesPrice <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'RetailPrice should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNUMERIC(RetailPrice) = 0;
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SequenceNumber should be numeric'
                    FROM @InsertPriceForValidation
                    WHERE ISNUMERIC(SequenceNumber) = 0;
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'ActivationDate Is Invalid'
                    FROM @InsertPriceForValidation AS IP
                    WHERE ISDATE(ActivationDate) = 0
                          AND
                          ISNULL(ActivationDate , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'ExpirationDate Is Invalid'
                    FROM @InsertPriceForValidation AS IP
                    WHERE ISDATE(ExpirationDate) = 0
                          AND
                          ISNULL(ExpirationDate , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SKUActivationDate Is Invalid'
                    FROM @InsertPriceForValidation AS IP
                    WHERE ISDATE(SKUActivationDate) = 0
                          AND
                          ISNULL(SKUActivationDate , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SKUExpirationDate Is Invalid'
                    FROM @InsertPriceForValidation AS IP
                    WHERE ISDATE(SKUExpirationDate) = 0
                          AND
                          ISNULL(SKUExpirationDate , '') <> '';
				     
             -- End  DataType Validation -- 
             ------------------------------
             --------------------------------------------------------------------------------------
             --- Insert Correct Data with Proper validating datatype 
             --------------------------------------------------------------------------------------
             DELETE FROM @InsertPriceForValidation
             WHERE RowNumber IN ( SELECT DISTINCT
                                         RowNumber
                                  FROM @ErrorLogForInsertPrice
                                );
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierStartQuantity should be less than '+CONVERT(VARCHAR(100) , @MessageDisplay)
                    FROM @InsertPriceForValidation
                    WHERE ( CONVERT(NUMERIC(38 , 6) , TierStartQuantity) > 999999 )
                          AND
                          TierStartQuantity <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'RetailPrice should be less than '+CONVERT(VARCHAR(100) , @MessageDisplay)
                    FROM @InsertPriceForValidation
                    WHERE ( CONVERT(NUMERIC(38 , 6) , RetailPrice) > 999999 );
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SalesPrice should be less than '+CONVERT(VARCHAR(100) , @MessageDisplay)
                    FROM @InsertPriceForValidation
                    WHERE ( CONVERT(NUMERIC(38 , 6) , SalesPrice) > 999999 )
                          AND
                          SalesPrice <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierPrice should be less than '+CONVERT(VARCHAR(100) , @MessageDisplay)
                    FROM @InsertPriceForValidation
                    WHERE ( CONVERT(NUMERIC(38 , 6) , TierPrice) > 999999 )
                          AND
                          TierPrice <> '';
             --DELETE FROM @InsertPriceForValidation
             --WHERE RowNumber IN ( SELECT DISTINCT
             --                            RowNumber
             --                     FROM @ErrorLogForInsertPrice
             --                   );
             INSERT INTO @InsertPrice ( SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber
                                      )
                    SELECT SKU ,
                           CASE
                               WHEN TierStartQuantity = ''
                               THEN 0
                               ELSE CONVERT(NUMERIC(38 , 6) , TierStartQuantity)
                           END , CONVERT(NUMERIC(38 , 6) , RetailPrice) ,
                                                                      CASE
                                                                          WHEN SalesPrice = ''
                                                                          THEN NULL
                                                                          ELSE CONVERT(NUMERIC(38 , 6) , SalesPrice)
                                                                      END ,
                                                                      CASE
                                                                          WHEN TierPrice = ''
                                                                          THEN NULL
                                                                          ELSE CONVERT(NUMERIC(38 , 6) , TierPrice)
                                                                      END , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber
                    FROM @InsertPriceForValidation;
				
             --------------------------------------------------------------------------------------
             --- start Functional Validation 
             --------------------------------------------------------------------------------------
             --- Verify SKU is present or not 

             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SKU Not Exists'
                    FROM @InsertPrice
                    WHERE SKU NOT IN ( SELECT ZPAVL.AttributeValue
                                       FROM ZnodePimAttribute AS ZPA INNER JOIN ZnodePimAttributeValue AS ZPAV ON ZPA.PimAttributeId = ZPAV.PimAttributeId
                                                                     INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
                                       WHERE ZPA.AttributeCode = 'SKU'
                                     );
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'RetailPrice Should be greater than 0'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(CAST(RetailPrice AS NUMERIC(38 , 6)) , 0) <= 0;
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , SequenceNumber , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SequenceNumber Should be greater than 0'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(SequenceNumber , 0) <= 0;
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , SequenceNumber , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SequenceNumber Should be unique'
                    FROM @InsertPriceForValidation
                    WHERE SequenceNumber IN ( SELECT SequenceNumber
                                              FROM @InsertPriceForValidation
                                              GROUP BY SequenceNumber
                                              HAVING COUNT(ISNULL(SequenceNumber , 0)) > 1
                                            );
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'CurrencyId Is Invalid (Not Exist in ZnodeCurrency table)'
                    FROM @InsertPrice AS IP
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeCurrency
                                       WHERE CurrencyId = IP.CurrencyId
                                     );
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'UOM Name Is Invalid (Not Exist in ZnodeUom table)'
                    FROM @InsertPrice AS IP
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeUOM
                                       WHERE LTRIM(RTRIM(UOM)) = LTRIM(RTRIM(IP.UOM))
                                     )
                          AND
                          ISNULL(LTRIM(RTRIM(UOM)) , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'ActivationDate should be less than ExpirationDate'
                    FROM @InsertPrice AS IP
                    WHERE ActivationDate > ExpirationDate
                          AND
                          ISNULL(ExpirationDate , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'SKUActivationDate should be less than SKUExpirationDate'
                    FROM @InsertPrice AS IP
                    WHERE SKUActivationDate > SKUExpirationDate
                          AND
                          ISNULL(SKUExpirationDate , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'UnitSize Should be greater than 0'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(CAST(UnitSize AS NUMERIC(38 , 6)) , 0) <= 0
                          AND
                          ISNULL(LTRIM(RTRIM(UOM)) , '') <> '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierStartQuantity is not valid'
                    FROM @InsertPriceForValidation
                    WHERE ( TierPrice IS NULL
                            OR
                            TierPrice = '0' )
                          AND
                          TierStartQuantity = '';
             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierPrice is not valid'
                    FROM @InsertPriceForValidation
                    WHERE ( TierPrice IS NULL
                            OR
                            TierPrice = '' )
                            AND
                            TierStartQuantity <> 0;
             --INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
             --                                    )
             --       SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierPrice is not valid'
             --       FROM @InsertPriceForValidation
             --       WHERE ( TierPrice IS NULL
             --               OR
             --               TierPrice = '0'
             --               OR
             --               TierPrice = '' )
             --             AND
             --             TierStartQuantity <> '';

             INSERT INTO @ErrorLogForInsertPrice ( RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , ErrorDescription
                                                 )
                    SELECT RowNumber , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate , SKUActivationDate , SKUExpirationDate , SequenceNumber , 'TierStartQuantity should be greater than 0'
                    FROM @InsertPriceForValidation
                    WHERE ISNULL(CAST(TierStartQuantity AS NUMERIC(38 , 6)) , 0) <= 0
                          AND
                          TierPrice <> '';

 
							
             --IF exists (Select top  1 1 from @InsertPriceForValidation where  (TierStartQuantity <>  '')  and (TierPrice <>  ''))
             --Begin
             --		INSERT INTO @ErrorLogForInsertPrice (RowNumber,SKU,TierStartQuantity ,RetailPrice ,SalesPrice,TierPrice,Uom ,UnitSize,PriceListCode,PriceListName,CurrencyId ,ActivationDate,ExpirationDate,SKUActivationDate,SKUExpirationDate,SequenceNumber,ErrorDescription) 
             --		Select RowNumber,SKU,TierStartQuantity ,RetailPrice ,SalesPrice,TierPrice,Uom ,UnitSize,PriceListCode,PriceListName,CurrencyId 
             --		,ActivationDate,ExpirationDate,SKUActivationDate,SKUExpirationDate,SequenceNumber,'TierPrice Should be greater than 0' from @InsertPriceForValidation 
             --		where Isnull(CAST(TierPrice AS Numeric(38,6)),0) <= 0 and CAST(TierStartQuantity AS Numeric(38,6)) > 0 
             --		INSERT INTO @ErrorLogForInsertPrice (RowNumber,SKU,TierStartQuantity ,RetailPrice ,SalesPrice,TierPrice,Uom ,UnitSize,PriceListCode,PriceListName,CurrencyId ,ActivationDate,ExpirationDate,SKUActivationDate,SKUExpirationDate,SequenceNumber,ErrorDescription) 
             --		Select RowNumber,SKU,TierStartQuantity ,RetailPrice ,SalesPrice,TierPrice,Uom ,UnitSize,PriceListCode,PriceListName,CurrencyId 
             --		,ActivationDate,ExpirationDate,SKUActivationDate,SKUExpirationDate,SequenceNumber,'TierStartQuantity Should be greater than 0' from @InsertPriceForValidation 
             --		where Isnull(CAST(TierPrice AS Numeric(38,6)),0) > 0   and CAST(TierStartQuantity AS Numeric(38,6)) < 0 
             --END 			
             -- End Function Validation 	
             ---------------------------
             --- Delete Invalid Data after functional validation 
             DELETE FROM @InsertPrice
             WHERE RowNumber IN ( SELECT DISTINCT
                                         RowNumber
                                  FROM @ErrorLogForInsertPrice
                                );
             UPDATE ZPL
                    SET ZPL.CurrencyId = IP.CurrencyId , ZPL.ActivationDate = IP.ActivationDate , ZPL.ExpirationDate = IP.ExpirationDate , Zpl.ListName = IP.PriceListName
             FROM ZnodePriceList ZPL INNER JOIN @InsertPrice IP ON Zpl.Listcode = IP.PriceListcode;
             INSERT INTO ZnodePriceList ( ListCode , ListName , CurrencyId , ActivationDate , ExpirationDate , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate
                                        )
                    SELECT DISTINCT
                           IP.PriceListcode , IP.PriceListName , IP.CurrencyId ,
                                                                    CASE
                                                                        WHEN ISNULL(IP.ActivationDate , '') = ''
                                                                        THEN NULL
                                                                        ELSE IP.ActivationDate
                                                                    END ,
                                                                    CASE
                                                                        WHEN ISNULL(IP.ExpirationDate , '') = ''
                                                                        THEN NULL
                                                                        ELSE IP.ExpirationDate
                                                                    END , @UserId , GETUTCDATE() , @UserId , GETUTCDATE()
                    FROM @InsertPrice AS IP
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePriceList AS Zpl
                                       WHERE Zpl.Listcode = IP.PriceListcode
                                     );
			

             --Validate StartQuantity and EndQuantity from PriceTier : This validation only for existing data 
             --INSERT INTO @ErrorLogForInsertPrice (RowNumber,SKU,TierStartQuantity ,RetailPrice ,SalesPrice,TierPrice,Uom ,UnitSize,PriceListCode,PriceListName,CurrencyId ,ActivationDate,ExpirationDate,SKUActivationDate,SKUExpirationDate,SequenceNumber,ErrorDescription) 
             --Select IP.RowNumber,IP.SKU,IP.TierStartQuantity ,IP.RetailPrice ,IP.SalesPrice,IP.TierPrice,IP.Uom ,IP.UnitSize,IP.PriceListCode,IP.PriceListName,IP.CurrencyId ,IP.ActivationDate,IP.ExpirationDate,IP.SKUActivationDate,IP.SKUExpirationDate,IP.SequenceNumber,
             --'TierStartQuantity already exists in PriceTier table for SKU '
             --From @InsertPrice IP  Inner join
             --ZnodePriceList Zpl ON Zpl.Listcode = IP.PriceListcode and Zpl.ListName = IP.PriceListName
             --INNER JOIN ZnodeUOM Zu ON ltrim(rtrim(IP.Uom)) = ltrim(rtrim(Zu.Uom)) 
             --INNER JOIN ZnodePriceTier ZPT  ON ZPT.PriceListId = Zpl.PriceListId 
             --AND ZPT.SKU = IP.SKU
             --Where IP.TierStartQuantity  = ZPT.Quantity  
             --- Delete Invalid Data after  Validate StartQuantity and EndQuantity from PriceTier
             DELETE FROM @InsertPrice
             WHERE RowNumber IN ( SELECT DISTINCT
                                         RowNumber
                                  FROM @ErrorLogForInsertPrice
                                );
			
             --INSERT INTO ZnodeUOM (Uom,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             --Select distinct ltrim(rtrim(Uom)) , @UserId,GETUTCDATE(),@UserId,GETUTCDATE()  from @InsertPrice 
             --where ltrim(rtrim(Uom)) not in (Select ltrim(rtrim(UOM)) From ZnodeUOM where UOM  is not null )


             UPDATE ZP
                    SET ZP.SalesPrice = IP.SalesPrice , ZP.RetailPrice = CASE
                                                                             WHEN CONVERT(VARCHAR(100) , ISNULL(IP.RetailPrice , '')) <> ''
                                                                             THEN IP.RetailPrice
                                                                         END , ZP.ActivationDate = CASE
                                                                                                       WHEN ISNULL(IP.SKUActivationDate , '') <> ''
                                                                                                       THEN IP.SKUActivationDate
                                                                                                       ELSE NULL
                                                                                                   END , ZP.ExpirationDate = CASE
                                                                                                                                 WHEN ISNULL(IP.SKUExpirationDate , '') <> ''
                                                                                                                                 THEN IP.SKUExpirationDate
                                                                                                                                 ELSE NULL
                                                                                                                             END , ZP.ModifiedBy = @UserId , ZP.ModifiedDate = GETUTCDATE()
             FROM @InsertPrice IP INNER JOIN ZnodePriceList Zpl ON Zpl.Listcode = IP.PriceListcode
                                  LEFT OUTER JOIN ZnodeUOM Zu ON LTRIM(RTRIM(IP.Uom)) = LTRIM(RTRIM(Zu.Uom))
                                  INNER JOIN ZnodePrice ZP ON ZP.PriceListId = Zpl.PriceListId
                                                              AND
                                                              ZP.SKU = IP.SKU  
             --Retrive last record from prince list of specific SKU ListCode and Name 									
             WHERE IP.SequenceNumber IN ( SELECT MAX(IPI.SequenceNumber)
                                          FROM @InsertPrice AS IPI
                                          WHERE IPI.SKU = IP.SKU
                                                AND
                                                IPI.PriceListcode = IP.PriceListcode
                                        );
             INSERT INTO ZnodePrice ( PriceListId , SKU , SalesPrice , RetailPrice , UomId , UnitSize , ActivationDate , ExpirationDate , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate
                                    )
                    SELECT Zpl.PriceListId , IP.SKU , IP.SalesPrice , IP.RetailPrice , Zu.UomId , IP.UnitSize ,
                                                                                                     CASE
                                                                                                         WHEN ISNULL(IP.SKUActivationDate , '') = ''
                                                                                                         THEN NULL
                                                                                                         ELSE IP.SKUActivationDate
                                                                                                     END ,
                                                                                                     CASE
                                                                                                         WHEN ISNULL(IP.SKUExpirationDate , '') = ''
                                                                                                         THEN NULL
                                                                                                         ELSE IP.SKUExpirationDate
                                                                                                     END , @UserId , GETUTCDATE() , @UserId , GETUTCDATE()
                    FROM @InsertPrice AS IP INNER JOIN ZnodePriceList AS Zpl ON Zpl.Listcode = IP.PriceListcode
                                            LEFT OUTER JOIN ZnodeUOM AS Zu ON LTRIM(RTRIM(IP.Uom)) = LTRIM(RTRIM(Zu.Uom))
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePrice
                                       WHERE ZnodePrice.PriceListId = Zpl.PriceListId
                                             AND
                                             ZnodePrice.SKU = IP.SKU
                                             AND
                                             ISNULL(ZnodePrice.SalesPrice , 0) = ISNULL(IP.SalesPrice , 0)
                                             AND
                                             ZnodePrice.RetailPrice = IP.RetailPrice
                                     )
                          AND
                          IP.SequenceNumber IN ( SELECT MAX(IPI.SequenceNumber)
                                                 FROM @InsertPrice AS IPI
                                                 WHERE IPI.SKU = IP.SKU
                                                       AND
                                                       IPI.PriceListcode = IP.PriceListcode
                                               );
             IF EXISTS ( SELECT TOP 1 1
                         FROM @InsertPrice
                         WHERE CONVERT(VARCHAR(100) , TierStartQuantity) <> ''
                               AND
                               ( CONVERT(VARCHAR(100) , TierPrice) <> '' )
                       )
                 BEGIN
                     UPDATE ZPT
                            SET ZPT.Price = IP.TierPrice , ZPT.ModifiedBy = @UserId , ZPT.ModifiedDate = GETUTCDATE()
                     FROM @InsertPrice IP INNER JOIN ZnodePriceList Zpl ON Zpl.Listcode = IP.PriceListcode
                                          LEFT OUTER JOIN ZnodeUOM Zu ON LTRIM(RTRIM(IP.Uom)) = LTRIM(RTRIM(Zu.Uom))
                                          INNER JOIN ZnodePriceTier ZPT ON ZPT.PriceListId = Zpl.PriceListId
                                                                           AND
                                                                           ZPT.SKU = IP.SKU
                                                                           AND
                                                                           ZPT.Quantity = IP.TierStartQuantity 
                     --Retrive last record from prince list of specific SKU ListCode and Name 
                     WHERE IP.SequenceNumber IN ( SELECT MAX(IPI.SequenceNumber)
                                                  FROM @InsertPrice AS IPI
                                                  WHERE IPI.SKU = IP.SKU
                                                        AND
                                                        IPI.PriceListcode = IP.PriceListcode
                                                );
                     INSERT INTO ZnodePriceTier ( PriceListId , SKU , Price , Quantity , UomId , UnitSize , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate
                                                )
                            SELECT Zpl.PriceListId , IP.SKU , IP.TierPrice , IP.TierStartQuantity , Zu.UomId , IP.UnitSize , @UserId , GETUTCDATE() , @UserId , GETUTCDATE()
                            FROM @InsertPrice AS IP INNER JOIN ZnodePriceList AS Zpl ON Zpl.Listcode = IP.PriceListcode
                                                    LEFT OUTER JOIN ZnodeUOM AS Zu ON LTRIM(RTRIM(IP.Uom)) = LTRIM(RTRIM(Zu.Uom))
                            WHERE NOT EXISTS ( SELECT TOP 1 1
                                               FROM ZnodePriceTier
                                               WHERE ZnodePriceTier.PriceListId = Zpl.PriceListId
                                                     AND
                                                     ZnodePriceTier.SKU = IP.SKU
                                                     AND
                                                     ZnodePriceTier.Quantity = IP.TierStartQuantity
                                             )
                                  --AND ZnodePriceTier.Price = IP.TierPrice )
                                  AND
                                  IP.SequenceNumber IN ( SELECT MAX(IPI.SequenceNumber)
                                                         FROM @InsertPrice AS IPI
                                                         WHERE IPI.SKU = IP.SKU
                                                               AND
                                                               IPI.PriceListcode = IP.PriceListcode
                                                               AND
                                                               IPI.TierStartQuantity = IP.TierStartQuantity
                                                       );
                 END;  
             --SELECT @PriceListId ID , cast(1 As Bit ) Status  

             SELECT RowNumber , ErrorDescription , SKU , TierStartQuantity , RetailPrice , SalesPrice , TierPrice , Uom , UnitSize , PriceListCode , PriceListName , CurrencyId , ActivationDate , ExpirationDate
             FROM @ErrorLogForInsertPrice;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH 
             --SELECT 0 ID , cast(0 As Bit ) Status  
             SET @Status = 0;
             SELECT ERROR_LINE() , ERROR_MESSAGE() , ERROR_PROCEDURE();
             ROLLBACK TRAN A;
         END CATCH;
     END;