
CREATE PROCEDURE [dbo].[Znode_GetPublishProducts_2](
       @PublishCatalogId  INT           = NULL ,
       @PublishCategoryId VARCHAR(1000) = NULL ,
       @UserId            INT ,
       @NotReturnXML      INT           = NULL ,
       @PimProductId      INT           = 0)
AS 
    ------------------------------------------------------------------------------
    --Summary : Publish Catalaog and Product with product detsils
    --          Retrive all Product detsils with attributes and insert into ZnodePublishProduct tables 
    --		    Retrive all product details belongs to respective Product and insert into ZnodePublishProduct table
    -- here is the check statements 
    -- SELECT * FROM ZnodePimCustomField WHERE CustomCode = 'Test'
    -- SELECT * FROM ZnodePimCatalogCategory WHERE pimCatalogId = 1031
    -- SELECT * FROM ZnodePimCustomFieldLocale WHERE PimCustomFieldId = 29
    -- select * from znodepublishcatalog
    -- SELECT * FROM ZnodePublishProduct WHERE publishproductId = 569
    -- EXEC Znode_GetPublishProducts  @PublishCatalogId =9,@UserId= 2 ,@RequiredXML= 1		
    ------------------------------------------------------------------------------
     BEGIN
         BEGIN TRAN B;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @PimCatalogId INT= ( SELECT PimCatalogId
                                          FROM ZnodePublishcatalog
                                          WHERE PublishCatalogId = @PublishCatalogId
                                        );  --- this variable is used to carry y pim catalog id by using published catalog id 


             DECLARE @LocaleId INT -- this variable used to carry the locale in loop 

             , @LocaleDefaultId INT= ( SELECT FeatureValues
                                       FROM ZnodeGlobalSetting
                                       WHERE FeatureName = 'Locale'
                                     )  --- this variable is used to carry the default locale which is globaly set  

             , @Count INT= 1;  -- this variable is used in loop to increment the counter

             DECLARE @Tbl_Locale TABLE (
                                       RowId      INT IDENTITY(1 , 1) ,
                                       LocaleId   INT ,
                                       LocaleCode NVARCHAR(600) ,
                                       IsDefault  BIT
                                       ); --- this table will used to hold the all currently active locale ids  

             DECLARE @Tbl_ProductAttributeXml TABLE (
                                                    ProductId  INT ,
                                                    ProductXml NVARCHAR(MAX)
                                                    );  -- this table is used to hold the attribute xml product wise


             DECLARE @Tbl_CompleteProductXml TABLE (
                                                   ProductXml XML
                                                   ); -- this table hold the complete xml of product with other information like category and catalog

             DECLARE @Tbl_PimProductswillPublished TABLE (
                                                         PimProductId        INT ,
                                                         PublishCategoryId   INT ,
                                                         IsAssociatedProduct BIT
                                                         );  -- this table is used to hold the product which publish in current process 


             DECLARE @Tbl_PublishProductIds TABLE (
                                                  PublishProductId INT ,
                                                  PimProductId     INT ,
                                                  PublishCatalogId INT
                                                  ); -- this table is used hold the published products for finding the attributes details 



             DECLARE @Tbl_Attributes TABLE (
                                           PimProductId               INT ,
                                           PimAttributeFamilyId       INT ,
                                           PimAttributeId             INT ,
                                           AttributeCode              VARCHAR(600) ,
                                           IsLayeredNavigation        BIT ,
                                           IsHtmlTags                 BIT ,
                                           IsComparable               BIT ,
                                           IsPromoRuleCondition       BIT ,
                                           IsFacets                   BIT ,
                                           AttributeName              NVARCHAR(MAX) ,
                                           PimProductAttributeValueId INT ,
                                           AttributeTypeID            INT ,
                                           AttributeTypeName          VARCHAR(600) ,
                                           IsPersonalizable           BIT ,
                                           IsCustomeField             BIT ,
                                           IsConfigurable             BIT
                                           ); --  this table is used to hold the filter the locale id data 

             DECLARE @Tbl_ProductAttributeValues TABLE (
                                                       PimProductId      INT ,
                                                       PimAttributeId    INT ,
                                                       AttributeCode     VARCHAR(600) ,
                                                       AttributeName     NVARCHAR(MAX) ,
                                                       AttributeTypeName VARCHAR(2000) ,
                                                       AttributeTypeId   INT ,
                                                       AttributeValues   NVARCHAR(MAX)
                                                       ); -- this table is used to store the filtered locale attribute values 

             DECLARE @Tbl_ProductAttributeValues_Name TABLE (
                                                            PimProductId    INT ,
                                                            PimAttributeId  INT ,
                                                            AttributeCode   VARCHAR(600) ,
                                                            AttributeName   NVARCHAR(MAX) ,
                                                            AttributeTypeId INT ,
                                                            AttributeValues NVARCHAR(MAX)
                                                            ); -- this extra table is used to separate out the product name and sku value for XML out	


             INSERT INTO @Tbl_Locale ( LocaleId , LocaleCode , IsDefault
                                     )
                    SELECT LocaleId , [Name] , IsDefault
                    FROM ZnodeLocale
                    WHERE IsActive = 1  --- select all currently active locals 
                          AND
                          @NotReturnXML IS NULL; -- this check is used when this procedure is call by internal procedure to publish only product and no need to return publish xml; 
             ---------------------------------- publish product are find here ---------------------------------------------------------------------

             INSERT INTO @Tbl_PimProductswillPublished
                    SELECT PimProductId , ZPC.PublishCategoryId , 0 -- define the main product not a grouped or bundle product
                    FROM ZnodePimCatalogCategory AS ZPCC INNER JOIN ZnodePublishCategory AS ZPC ON ( ZPC.PimCategoryId = ZPCC.PimCategoryId
                                                                                                     AND
                                                                                                     ZPC.PublishCataLogId = @PublishCatalogId ) -- check the only product which associated with publish category and publish catalog 
                    WHERE EXISTS ( SELECT TOP 1 1
                                   FROM ZnodePimAttributeValue AS ZPAV
                                   WHERE ZPAV.PimProductId = ZPCC.PimProductId
                                 ) --- this will check the product have attributes 
                          AND
                          ZPCC.PimCatalogId = @PimCatalogId
                          AND
                          ( ZPCC.PimProductId = @PimProductId
                            OR
                            @PimProductId = 0 ); --- this where clause is used when single product publish in publish product details procedure 

             INSERT INTO @Tbl_PimProductswillPublished -- insert the link product which is associated with main products 
                    SELECT DISTINCT
                           ZPLD.PimProductId , NULL AS PublishCategoryId , 1 -- define the  associated product with main product 
                    FROM ZnodePimLinkProductDetail AS ZPLD INNER JOIN ZnodePimCatalogCategory AS ZPCC ON ( ZPCC.PimProductId = ZPLD.PimProductId )
                                                           INNER JOIN ZnodePublishCategory AS ZPC ON ( ZPC.PimCategoryId = ZPCC.PimCategoryId
                                                                                                       AND
                                                                                                       ZPC.PublishCataLogId = @PublishCatalogId )
                                                           INNER JOIN @Tbl_PimProductswillPublished AS PPP ON ( ZPLD.PimParentProductId = PPP.PimProductId
                                                                                                                AND
                                                                                                                PPP.PublishCategoryId = ZPC.PublishCategoryId )
                    -- here check the parent product of link product is equal to main product in this @Tbl_PimProductswillPublished table 
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM @Tbl_PimProductswillPublished AS PPPI
                                       WHERE PPPI.PimProductId = ZPLD.PimProductId
                                     ) -- this check is used for distinct product if already in publish product list
                          AND
                          ( ZPLD.PimParentProductId = @PimProductId
                            OR
                            @PimProductId = 0 ); -- this check will used when single product publish

             INSERT INTO @Tbl_PimProductswillPublished -- insert the addon products with main products 
                    SELECT DISTINCT
                           awa.PimChildProductId , NULL AS PublishCategoryId , 1 -- define the  associated product with main product 
                    FROM ZnodePimAddOnProductDetail AS awa INNER JOIN ZnodePimAddOnProduct AS awaa ON ( awaa.PimAddOnProductId = awa.PimAddOnProductId )
                                                           INNER JOIN @Tbl_PimProductswillPublished AS zq ON ( awaa.PimProductId = zq.PimProductId )
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM @Tbl_PimProductswillPublished AS sw
                                       WHERE sw.PimProductId = awa.PimChildProductId
                                     )
                          AND
                          ( awaa.PimProductId = @PimProductId
                            OR
                            @PimProductId = 0 );
             INSERT INTO @Tbl_PimProductswillPublished -- insert the grouped , bundle , configurable products with main products 
                    SELECT DISTINCT
                           ZPPTA.PimProductId , NULL AS PublishCategoryId , 1 -- define the  associated product with main product 
                    FROM ZnodePimAttribute ( NOLOCK
                                           ) AS ZPA INNER JOIN ZnodePimAttributeValue ( NOLOCK
                                                                                      ) AS ZPAV ON ZPA.PimAttributeId = ZPAV.PimAttributeId
                                                    INNER JOIN ZnodePimAttributeValuelocale ( NOLOCK
                                                                                            ) AS ZPADVL ON ZPAV.PimAttributeValueId = ZPADVL.PimAttributeValueId
                                                    INNER JOIN ZnodePimProductTypeAssociation ( NOLOCK
                                                                                              ) AS ZPPTA ON ZPAV.PimProductId = ZPPTA.PimParentProductId
                                                    INNER JOIN @Tbl_PimProductswillPublished AS zq ON ZPPTA.PimParentProductId = zq.PimProductId
                    WHERE ZPA.AttributeCode = 'ProductType'
                          AND
                          ZPADVL.AttributeValue IN ( 'GroupedProduct' , 'BundleProduct' , 'ConfigurableProduct'
                                                   ) -- this check will check the product type of product 
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @Tbl_PimProductswillPublished AS sw
                                       WHERE sw.PimProductId = ZPPTA.PimProductId
                                     )  -- this check will check product alresdy not in publish product list 
                          AND
                          ( ZPPTA.PimParentProductId = @PimProductId
                            OR
                            @PimProductId = 0 ); -- this check will used when single product publish
             ----------------------------- product are published from here to sql table  ----------------------------------------------------------------------------------------

             MERGE INTO ZnodePublishProduct TARGET -- this merge statement is used for crude oprtaion with publisgh product table  
             USING ( SELECT PimProductId
                     FROM @Tbl_PimProductswillPublished
                     GROUP BY PimProductId
                   ) SOURCE
             ON ( TARGET.PimProductId = SOURCE.PimProductId
                  AND
                  TARGET.PublishCatalogId = @PublishCataLogId ) -- check for if already exists then just update otherwise insert the product  
                 WHEN MATCHED
                 THEN UPDATE SET TARGET.CreatedBy = @UserId , TARGET.CreatedDate = GETUTCDATE() , TARGET.ModifiedBy = @UserId , TARGET.ModifiedDate = GETUTCDATE()
                 WHEN NOT MATCHED
                 THEN INSERT(PimProductId , PublishCatalogId , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate) VALUES ( SOURCE.PimProductId , @PublishCatalogId , @UserId , GETUTCDATE() , @UserId , GETUTCDATE()
                                                                                                                           )
             OUTPUT INSERTED.PublishProductId , INSERTED.PimProductId , INSERTED.PublishCatalogId
                    INTO @Tbl_PublishProductIds; -- here used the ouput clause to catch what data inserted or updated into variable table 



             MERGE INTO ZnodePublishCategoryProduct TARGET -- this merge staetment is used for crude opration with  ZnodePublishCategoryProduct table 
             USING ( SELECT PublishProductId , PublishCategoryId , PublishCatalogId
                     FROM @Tbl_PublishProductIds AS a INNER JOIN @Tbl_PimProductswillPublished AS b ON ( a.PimProductId = b.PimProductId )
                   ) SOURCE
             ON ( TARGET.PublishProductId = SOURCE.PublishProductId
                  AND
                  ISNULL(TARGET.PublishCategoryId , 0) = ISNULL(SOURCE.PublishCategoryId , 0)
                  AND
                  TARGET.PublishCatalogId = SOURCE.PublishCatalogId )
                 WHEN NOT MATCHED
                 THEN INSERT(PublishProductId , PublishCategoryId , PublishCatalogId , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate) VALUES ( SOURCE.PublishProductId , SOURCE.PublishCategoryId , SOURCE.PublishCatalogId , @UserId , GETUTCDATE() , @userId , GETUTCDATE()
                                                                                                                                                   );
           

             ------------------------------------------------- while loop locale ids wise start here ------------------------------------------------------------------

             WHILE @Count <= ISNULL( ( SELECT MAX(RowId)
                                       FROM @Tbl_Locale
                                     ) , 0)
                 BEGIN -- here is begin the loop with while on active locale ids  

                     SET @LocaleId = ( SELECT LocaleId
                                       FROM @Tbl_Locale
                                       WHERE RowID = @Count
                                     ); -- set the locale id as per loop 
                     ---------------------------------------------------------- product attribute collect here -------------------------------------------------------------------------

                     WITH Cte_AttributesLocale
                          AS (  -- this CTE holds the both locale attribute details 

                          SELECT ZPAV.PimProductId , ZPAV.PimAttributeFamilyId , ZPAL.PimAttributeId , ZPA.AttributeCode , ZPFP.IsLayeredNavigation , ZPFP.IsHtmlTags , ZPFP.IsComparable , ZPFP.IsPromoRuleCondition , IsFacets , ZPAL.AttributeName , ZPAV.PimAttributeValueId , ZPA.AttributeTypeID , ZAT.AttributeTypeName , ZPA.IsPersonalizable , 0 AS IsCustomeField , ZPA.IsConfigurable , ZPAL.LocaleId
                          FROM ZnodePimAttributeLocale AS ZPAL INNER JOIN ZnodePimAttribute AS ZPA ON ( ZPA.PimAttributeId = ZPAL.PimAttributeId
                                                                                                        AND
                                                                                                        ZPAL.LocaleId IN ( @LocaleDefaultId , @LocaleId
                                                                                                                         ) ) -- for both locales 
                                                               INNER JOIN ZnodeAttributeType AS ZAT ON ( ZAT.AttributeTypeId = ZPA.AttributeTypeId )
                                                               INNER JOIN ZnodePimAttributeValue AS ZPAV ON ( ZPAV.PimAttributeId = ZPAL.PimAttributeId
                                                                                                              AND
                                                                                                              EXISTS ( SELECT TOP 1 1
                                                                                                                       FROM @Tbl_PublishProductIds AS TBPP
                                                                                                                       WHERE ZPAV.PimProductId = TBPP.PimProductId
                                                                                                                             AND
                                                                                                                             TBPP.PublishCatalogId = @PublishCatalogId
                                                                                                                     ) ) -- this check only those product whoes ready to publish  
                                                               LEFT JOIN ZnodePimFrontendProperties AS ZPFP ON ( ZPA.PimAttributeId = ZPFP.PimAttributeId )
                          UNION ALL 
                          --- add the link product attribute details 
                          SELECT DISTINCT
                                 ZPLPD.PimParentProductId , 0 AS PimAttributeFamilyId , ZPAL.PimAttributeId , ZPA.AttributeCode , ZPFP.IsLayeredNavigation , ZPFP.IsHtmlTags , ZPFP.IsComparable , ZPFP.IsPromoRuleCondition , IsFacets , ZPAL.AttributeName , 0 AS PimAttributeValueId , ZPA.AttributeTypeID , ZAT.AttributeTypeName , ZPA.IsPersonalizable , 0 AS IsCustomeField , 0 AS IsConfigurable , ZPAL.LocaleId
                          FROM ZnodePimAttributeLocale AS ZPAL INNER JOIN ZnodePimAttribute AS ZPA ON ( ZPA.PimAttributeId = ZPAL.PimAttributeId
                                                                                                        AND
                                                                                                        ZPAL.LocaleId IN ( @LocaleDefaultId , @LocaleId
                                                                                                                         ) ) -- for both locales 
                                                               INNER JOIN ZnodeAttributeType AS ZAT ON ( ZAT.AttributeTypeId = ZPA.AttributeTypeId )
                                                               INNER JOIN ZnodePimLinkProductDetail AS ZPLPD ON ( ZPLPD.PimAttributeId = ZPAL.PimAttributeId
                                                                                                                  AND
                                                                                                                  EXISTS -- this check only those product whoes ready to publish 
                                                                                                                  ( SELECT TOP 1 1
                                                                                                                    FROM @Tbl_PublishProductIds AS TBPP
                                                                                                                    WHERE ZPLPD.PimParentProductId = TBPP.PimProductId
                                                                                                                          AND
                                                                                                                          TBPP.PublishCatalogId = @PublishCatalogId
                                                                                                                  ) )
                                                               LEFT JOIN ZnodePimFrontendProperties AS ZPFP ON ( ZPA.PimAttributeId = ZPFP.PimAttributeId )
                          UNION ALL
                          --- here add the custome filed details 
                          SELECT DISTINCT
                                 ZPCF.PimProductId , NULL AS PimAttributeFamilyId , NULL AS PimAttributeId , ZPCF.CustomCode AS AttributeCode , NULL AS IsLayeredNavigation , NULL AS IsHtmlTags , NULL AS IsComparable , NULL AS IsPromoRuleCondition , NULL AS IsFacets , ZPCFL.CustomKey AS AttributeName , NULL AS PimAttributeValueId , NULL AS AttributeTypeID , NULL AS AttributeTypeName , 0 AS IsPersonalizable , -- this flag is used for personalies attribute 
                                 1 AS IsCustomeField , -- this flag for custome filed attribute
                                 0 AS IsConfigurable , --- this flag for configurable attributes 
                                 ZPCFL.LocaleId
                          FROM ZnodePimCustomField AS ZPCF INNER JOIN ZnodePimCustomFieldLocale AS ZPCFL ON ( ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId
                                                                                                              AND
                                                                                                              ZPCFL.LocaleId IN ( @LocaleDefaultId , @LocaleId
                                                                                                                                ) ) -- for both locales 
                          WHERE EXISTS ( SELECT TOP 1 1
                                         FROM @Tbl_PublishProductIds AS TBPP
                                         WHERE ZPCF.PimProductId = TBPP.PimProductId
                                               AND
                                               TBPP.PublishCatalogId = @PublishCatalogId
                                       ) -- this check only those product whoes ready to publish 
                          ) ,
                          Cte_FilterForFirstLocale
                          AS  -- this is nested CTE table for filter data for one locale 
                          (SELECT *
                           FROM Cte_AttributesLocale AS CTAL
                           WHERE CTAL.LocaleId = @LocaleId) ,
                          Cte_FilterAttributeLocale
                          AS  -- this is nested CTE for filter data for second locale 
                          (
                          SELECT *
                          FROM Cte_FilterForFirstLocale
                          UNION ALL
                          SELECT *
                          FROM Cte_AttributesLocale AS CTALI
                          WHERE CTALI.LocaleId = @LocaleDefaultId
                                AND
                                NOT EXISTS ( SELECT TOP 1 1
                                             FROM Cte_FilterForFirstLocale AS CTFFL
                                             WHERE CTFFL.AttributeCode = CTALI.AttributeCode
                                           ) -- check the data already present for first locale if not then take the default locale data 
                          )
                          INSERT INTO @Tbl_Attributes -- put the CTE data into table variable for futher use 
                                 SELECT CTFAL.PimProductId , CTFAL.PimAttributeFamilyId , CTFAL.PimAttributeId , CTFAL.AttributeCode , CTFAL.IsLayeredNavigation , CTFAL.IsHtmlTags , CTFAL.IsComparable , CTFAL.IsPromoRuleCondition , CTFAL.IsFacets , CTFAL.AttributeName , CTFAL.PimAttributeValueId , CTFAL.AttributeTypeId , CTFAL.AttributeTypeName , CTFAL.IsPersonalizable , CTFAL.IsCustomeField , CTFAL.IsConfigurable
                                 FROM Cte_FilterAttributeLocale AS CTFAL
                                 WHERE CTFAL.LOcaleid = @LocaleId;

                  
                     
                     ------------------------------------------------------------- Attribute values collect here -----------------------------------------            

                     WITH Cte_ProductAttributeValuesLocale
                          AS (
                          SELECT TBA.PimProductId , TBA.PimAttributeId , TBA.AttributeCode , TBA.AttributeName , TBA.AttributeTypeID , TBA.AttributeTypeName , ZPAVL.AttributeValue ,
                                                                                                                                                                     CASE
                                                                                                                                                                         WHEN ZPAVL.LocaleId IS NULL
                                                                                                                                                                         THEN @LocaleDefaultId
                                                                                                                                                                         ELSE ZPAVL.LocaleId
                                                                                                                                                                     END AS LocaleId
                          FROM @Tbl_Attributes AS TBA LEFT JOIN ZnodePimAttributeValueLocale AS ZPAVL ON ( TBA.PimProductAttributeValueId = ZPAVL.PimAttributeValueId
                                                                                                           AND
                                                                                                           ZPAVL.LocaleId IN ( @LocaleDefaultId , @LocaleId
                                                                                                                             ) )
                          WHERE TBA.PimAttributeId IS NOT NULL  --- check the attribute value is not null 
                           
                          UNION ALL
                          SELECT DISTINCT -- add the custome filed value locale wise attribute 
                                 ZPCF.PimProductId , NULL AS PimAttributeId , ZPCF.CustomCode AS AttributeCode , ZPCFL.CustomKey , NULL AS AttributeTypeID , NULL AS AttributeTypeName , ZPCFL.CustomKeyValue AS AttributeValue , ZPCFL.LocaleId
                          FROM ZnodePimCustomField AS ZPCF INNER JOIN ZnodePimCustomFieldLocale AS ZPCFL ON ( ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId
                                                                                                              AND
                                                                                                              ZPCFL.LocaleId IN ( @LocaleDefaultId , @LocaleId
                                                                                                                                ) )
                          WHERE EXISTS ( SELECT TOP 1 1
                                         FROM ZnodePimCatalogCategory AS ZPCC
                                         WHERE ZPCF.PimProductId = ZPCC.PimProductId
                                               AND
                                               ZPCC.PimCatalogId = @PimCatalogId
                                       ) -- check the 
                          ) ,
                          Cte_ProductAttributeValueForFirstLocale
                          AS (SELECT *
                              FROM Cte_ProductAttributeValuesLocale AS CTPAVL
                              WHERE CTPAVL.LocaleId = @LocaleId) ,
                          Cte_ProductAttributeValue
                          AS (
                          SELECT *
                          FROM Cte_ProductAttributeValueForFirstLocale
                          UNION ALL
                          SELECT *
                          FROM Cte_ProductAttributeValuesLocale AS CTPAVL
                          WHERE CTPAVL.LocaleId = @LocaleDefaultId
                                AND
                                NOT EXISTS ( SELECT TOP 1 1
                                             FROM Cte_ProductAttributeValueForFirstLocale AS CTPAVFL
                                             WHERE CTPAVFL.AttributeCode = CTPAVL.AttributeCode
                                                   AND
                                                   CTPAVFL.PimProductId = CTPAVL.PimProductId
                                           ) -- this check for if record is already present for first locale 
 					  
                          )
                          INSERT INTO @Tbl_ProductAttributeValues -- insert the CTE data of attribute values in  variable table 
                                 SELECT CTPAV.PimProductId , CTPAV.PimAttributeId , CTPAV.AttributeCode , CTPAV.AttributeName , CTPAV.AttributeTypeName , CTPAV.AttributeTypeId , CTPAV.AttributeValue
                                 FROM Cte_ProductAttributeValue AS CTPAV;
                     UPDATE a  --- this will update the media file comma separeted in  table 
                            SET AttributeValues = CASE
                                                      WHEN EXISTS ( SELECT TOP 1 1
                                                                    FROM ZnodeMedia AS zm
                                                                    WHERE EXISTS ( SELECT TOP 1 1
                                                                                   FROM dbo.Split ( a.AttributeValues , ','
                                                                                                  ) AS cq
                                                                                   WHERE cq.Item = CAST(zm.MediaId AS VARCHAR(100))
                                                                                 )
                                                                  )
                                                      THEN SUBSTRING( ( SELECT ','+zm.Path
                                                                        FROM ZnodeMedia AS zm
                                                                        WHERE EXISTS ( SELECT TOP 1 1
                                                                                       FROM dbo.Split ( a.AttributeValues , ','
                                                                                                      ) AS cq
                                                                                       WHERE cq.Item = CAST(zm.MediaId AS VARCHAR(100))
                                                                                     )
                                                                        FOR XML PATH('')
                                                                      ) , 2 , 4000)
                                                      ELSE ' '
                                                  END
                     FROM @Tbl_ProductAttributeValues a
                     WHERE a.AttributeTypeName IN ( 'Image' , 'Video' , 'Audio'
                                                  );
                     INSERT INTO @Tbl_ProductAttributeValues_Name --- this insert find the product name and sku from attributes 
                            SELECT TBPAV.PimProductId , TBPAV.PimAttributeId , TBPAV.AttributeCode , v.AttributeCode , TBPAV.AttributeTypeId , TBPAV.AttributeValues
                            FROM @Tbl_ProductAttributeValues AS TBPAV INNER JOIN ZnodePimAttribute AS v ON ( v.AttributeCode = TBPAV.AttributeCode
                                                                                                             AND
                                                                                                             v.AttributeCode IN ( 'ProductName' , 'SKU'
                                                                                                                                ) );   --- filter the attribute code only name and sku for outer xml   

                     WITH Cte_LinkProducts
                          AS (SELECT PimProductId , PimAttributeId , SUBSTRING( ( SELECT DISTINCT
                                                                                         ','+CAST(zpp.PublishProductId AS VARCHAR(1000))
                                                                                  FROM ZnodePimLinkProductDetail AS ZPLPDI INNER JOIN ZNodePublishProduct AS zpp ON ( zpp.PimProductId = ZPLPDI.PimProductId
                                                                                                                                                                      AND
                                                                                                                                                                      zpp.PublishCatalogId = @PublishCatalogId )
                                                                                  WHERE ZPLPDI.PimParentProductId = TBPAV.PimProductId
                                                                                        AND
                                                                                        ZPLPDI.PimAttributeId = TBPAV.PimAttributeId
                                                                                  FOR XML PATH('')
                                                                                ) , 2 , 4000) AS AttributeValues -- convert the link product ids into comma separated  
                              FROM @Tbl_ProductAttributeValues AS TBPAV
                              WHERE EXISTS ( SELECT TOP 1 1
                                             FROM ZnodePimLinkProductDetail AS ZPLPD
                                             WHERE ZPLPD.PimParentProductId = TBPAV.PimProductId
                                                   AND
                                                   ZPLPD.PimAttributeId = TBPAV.PimAttributeId
                                           ) -- this will check the product in link table 
                          )
                          UPDATE TBPAV -- here is update the comma separated link products into table 
                                 SET AttributeValues = CTLP.AttributeValues
                          FROM @Tbl_ProductAttributeValues TBPAV INNER JOIN Cte_LinkProducts CTLP ON ( TBPAV.PimProductId = CTLP.PimProductId
                                                                                                       AND
                                                                                                       TBPAV.PimAttributeId = CTLP.PimAttributeId );
					  
                     -------------------------------- XML creation start from here -----------------------------------------------------------------

                     WITH Cte_ProductAttributeForAttributeNode
                          AS  -- this CTE is used to find the attributes which is in attribute node of xml 
                          (SELECT TBA.PimProductId , TBA.AttributeCode , ISNULL(TBA.AttributeName , '') AS AttributeName , ISNULL(TBA.IsLayeredNavigation , 0) AS IsLayeredNavigation , ISNULL(TBA.IsHtmlTags , 0) AS IsHtmlTags , ISNULL(TBA.IsComparable , 0) AS IsComparable , ISNULL(TBA.IsPromoRuleCondition , 0) AS IsPromoRuleCondition , ISNULL(TBA.AttributeTypeName , '') AS AttributeTypeName , TBA.IsPersonalizable , TBA.IsCustomeField , TBA.IsConfigurable , TBPAV.AttributeValues
                           FROM @Tbl_Attributes AS TBA LEFT JOIN @Tbl_ProductAttributeValues AS TBPAV ON ( TBPAV.AttributeCode = TBA.AttributeCode )
                                                                                                         AND
                                                                                                         TBPAV.PimProductId = TBA.PimProductId)
                          INSERT INTO @Tbl_ProductAttributeXml -- this table is used to store the attribute XML of product 
                                 SELECT TBPP.PimProductId , ( SELECT ( SELECT CTPAN.AttributeCode , CTPAN.AttributeName , CTPAN.IsLayeredNavigation , CTPAN.IsHtmlTags , CTPAN.IsComparable , CTPAN.IsPromoRuleCondition , TBA.IsFacets , CTPAN.AttributeValues , CTPAN.AttributeTypeName , CTPAN.IsPersonalizable , CTPAN.IsCustomeField , CTPAN.IsConfigurable
                                                                       FROM Cte_ProductAttributeForAttributeNode AS CTPAN
                                                                       WHERE CTPAN.PimProductId = TBA.PimProductId
                                                                             AND
                                                                             CTPAN.AttributeCode = TBA.AttributeCode
                                                                       FOR XML PATH('AttributeEntity') , TYPE -- this will create inner attribute entity noe within attribute node
                                                                     )
                                                              FROM @Tbl_Attributes AS TBA
                                                              WHERE TBA.PimProductId = TBPP.PimProductId
                                                              FOR XML PATH('') , ROOT('Attributes') -- this will create the attribute node 
                                                            ) AS XMLGEM
                                 FROM @Tbl_PublishProductIds AS TBPP
                                 GROUP BY TBPP.PublishProductId , TBPP.PimProductId;
                     INSERT INTO @Tbl_CompleteProductXml -- this table is used to store the complete XML of product 
                            SELECT '<ProductEntity>'+ISNULL( ( SELECT DISTINCT
                                                                      TBPP.PublishProductId AS ZnodeProductId , SUBSTRING( ( SELECT DISTINCT
                                                                                                                                    ','+CAST(ds.PublishCategoryId AS VARCHAR(1000))
                                                                                                                             FROM @Tbl_PimProductswillPublished AS ds
                                                                                                                             WHERE ds.PimProductId = TBPP.Pimproductid
                                                                                                                             FOR XML PATH('')
                                                                                                                           ) , 2 , 4000) AS TempZnodeCategoryIds , TBPAVN.AttributeValues AS Name , TBPAVNS.AttributeValues AS SKU , TBPP.PublishCatalogId AS ZnodeCatalogId , TBPWP.IsAssociatedProduct , @LocaleId AS LocaleId
                                                               FROM @Tbl_PublishProductIds AS TBPP LEFT JOIN @Tbl_ProductAttributeValues_Name AS TBPAVN ON ( TBPAVN.PimProductId = TBPP.PimProductId
                                                                                                                                                             AND
                                                                                                                                                             TBPAVN.AttributeCode = 'ProductName' )
                                                                                                   LEFT JOIN @Tbl_ProductAttributeValues_Name AS TBPAVNS ON ( TBPAVNS.PimProductId = TBPP.PimProductId
                                                                                                                                                              AND
                                                                                                                                                              TBPAVNS.AttributeCode = 'SKU' )
                                                                                                   LEFT JOIN @Tbl_PimProductswillPublished AS TBPWP ON ( TBPWP.PimProductId = TBPP.PimProductId )
                                                               WHERE TBPP.PimProductId = TBPP.PimProductId
                                                                     AND
                                                                     TBPP.PublishCatalogId = @PublishCatalogId
                                                               FOR XML PATH('') -- here create the product detail XML like name , sku ,category ,catalog and locale
                                                             ) , '')+ -- here is bind the product detail xml and attribute xml with product 
                            ( SELECT TOP 1 TBAVX.ProductXml
                              FROM @Tbl_ProductAttributeXml AS TBAVX
                              WHERE ( TBPP.PimProductId = TBAVX.ProductId )
                            ) +'</ProductEntity>' -- this is main xml node 
                            FROM @Tbl_PublishProductIds AS TBPP
                            GROUP BY TBPP.PimProductId;
                   

                     --- below statement is used to blank the  variable table in loop --- 
                     DELETE FROM @Tbl_ProductAttributeXml;
                     DELETE FROM @Tbl_ProductAttributeValues_Name;
                     DELETE FROM @Tbl_ProductAttributeValues;
                     DELETE FROM @Tbl_Attributes;
                     SET @Count = @Count + 1; -- increment the counter 
                 END;
             IF @NotReturnXML IS NULL -- check the flag return the xml details 
                 BEGIN
                     SELECT *
                     FROM @Tbl_CompleteProductXml; -- return the XML 
                 END;
             COMMIT TRAN B;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE() , ERROR_PROCEDURE();
             ROLLBACK TRAN B;
         END CATCH;
     END;