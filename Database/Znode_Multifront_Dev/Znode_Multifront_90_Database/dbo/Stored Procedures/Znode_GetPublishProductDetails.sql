-- EXEC Znode_GetPublishProductDetails 6 ,2
-- SELECT * FROM ZnodePimProduct
-- SELECT * FROM ZnodePublishProduct


CREATE PROCEDURE [dbo].[Znode_GetPublishProductDetails](
       @PimProductId INT ,
       @UserId       INT 				    -- To update Modifiedby filed in table ZnodeProductPublish
	
)
AS 
    ------------------------------------------------------------------------------
    --Summary : Publish Catalaog and Category with product detsils
    --          Retrive all category detsils with attributes and insert into ZnodePublishcategory tables 
    --		 Retrive all product details belongs to respective category and insert into ZnodePublishProduct table
    --		 Update Modifiedby and ModifiedDate in table ZnodeProductPublish -- pending
    --Unit Testing 
    -- EXEC Znode_GetPublishProductDetails  @PimProductId = 20051 ,@UserId =2
    -- SELECT * FROM ZnodePublishProduct WHERE PimProductid  = 4
    ------------------------------------------------------------------------------
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             SET NOCOUNT ON;  
             --DECLARE @V_PimCatalogId INT  = (SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId )

             DECLARE @Locale TABLE (
                                   RowId      INT IDENTITY(1 , 1) ,
                                   LocaleId   INT ,
                                   LocaleCode NVARCHAR(600) ,
                                   IsDefault  BIT
                                   );
             INSERT INTO @Locale ( LocaleId , LocaleCode , IsDefault
                                 )
                    SELECT LocaleId , Name , IsDefault
                    FROM ZnodeLocale
                    WHERE IsActive = 1;
             DECLARE @V_LocaleId INT , @V_LocaleDefaultId INT= ( SELECT FeatureValues
                                                                 FROM ZnodeGlobalSetting
                                                                 WHERE FeatureName = 'Locale'
                                                               );
             DECLARE @V_LocaleCode NVARCHAR(600);
             DECLARE @v_Count INT= 1;
             DECLARE @v_Count_forProduct INT= 1;
             DECLARE @Xmlreturn TABLE (
                                      ProductId  INT ,
                                      ProductXml NVARCHAR(MAX)
                                      );
             DECLARE @XmlFullreturn TABLE (
                                          ProductXml XML
                                          );
             DECLARE @PimProductIds TABLE (
                                          PimProductId     INT ,
                                          PublishCatalogId INT
                                          );
             DECLARE @PimParentProduct TABLE (
                                             PimProductId      INT ,
                                             PublishCategoryId INT
                                             ); 
	
             -- SELECT * FROM ZnodePublishCategory
             DECLARE @PublishPimProductId TABLE (
                                                RowId            INT IDENTITY(1 , 1) ,
                                                PublishProductId INT ,
                                                PimProductId     INT ,
                                                PublishCatalogId INT
                                                );
             INSERT INTO @PimProductIds
                    SELECT PimProductId , PublishCatalogId
                    FROM ZNodePublishProduct
                    WHERE PimProductId = @PimProductId
                    UNION ALL
                    SELECT awa.PimChildProductId , zpp.PublishCatalogId
                    FROM ZnodePimAddOnProductDetail AS awa INNER JOIN ZnodePimAddOnProduct AS awaa ON ( awaa.PimAddOnProductId = awa.PimAddOnProductId )
                                                           INNER JOIN ZNodePublishProduct AS zpp ON ( zpp.PimProductId = awaa.PimProductId )
                    WHERE awaa.PimProductId = @PimProductId
                    GROUP BY awa.PimChildProductId , zpp.PublishCatalogId
                    UNION ALL
                    SELECT ZPPTA.PimProductId , zpp.PublishCatalogId
                    FROM ZnodePimAttribute ( NOLOCK
                                           ) AS ZPA INNER JOIN ZnodePimAttributeValue ( NOLOCK
                                                                                      ) AS ZPAV ON ZPA.PimAttributeId = ZPAV.PimAttributeId
                                                    INNER JOIN ZnodePimAttributeValuelocale ( NOLOCK
                                                                                            ) AS ZPADVL ON ZPAV.PimAttributeValueId = ZPADVL.PimAttributeValueId
                                                    INNER JOIN ZnodePimProductTypeAssociation ( NOLOCK
                                                                                              ) AS ZPPTA ON ZPAV.PimProductId = ZPPTA.PimParentProductId
                                                    INNER JOIN ZNodePublishProduct AS zpp ON ( zpp.PimProductId = ZPPTA.PimParentProductId )
                    WHERE ZPA.AttributeCode = 'ProductType'
                          AND
                          ZPADVL.AttributeValue IN ( 'GroupedProduct' , 'BundleProduct' , 'ConfigurableProduct'
                                                   )
                          AND
                          ZPPTA.PimParentProductId = @PimProductId
                    GROUP BY ZPPTA.PimProductId , zpp.PublishCatalogId
                    UNION ALL
                    SELECT DISTINCT
                           awaa.PimProductId , zpp.PublishCatalogId
                    FROM ZnodePimLinkProductDetail AS awaa INNER JOIN ZNodePublishProduct AS zpp ON ( zpp.PimProductId = awaa.PimParentProductId )
                    WHERE awaa.PimParentProductId = @PimProductId
                    GROUP BY awaa.PimProductId , zpp.PublishCatalogId;            

	
             --SELECT * FROM @PimProductIds


             DECLARE @PimProductId1 INT , @PublishCatalogId INT; 

	
             --SELECT * FROM @PimProductIds

             DECLARE CUR_FORPublishProduct CURSOR
             FOR SELECT PimProductId , PublishCatalogId
                 FROM @PimProductIds
                 WHERE PimProductId = @PimProductId;
             OPEN CUR_FORPublishProduct;
             FETCH NEXT FROM CUR_FORPublishProduct INTO @PublishCatalogId , @PimProductId1;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     EXEC Znode_GetPublishProducts @PublishCatalogId = @PublishCatalogId , @UserId = @UserId , @StoreToTable = 1 , @PimProductId = @PimProductId1;
                     FETCH NEXT FROM CUR_FORPublishProduct INTO @PublishCatalogId , @PimProductId1;
                 END;
             CLOSE CUR_FORPublishProduct;
             DEALLOCATE CUR_FORPublishProduct;
             INSERT INTO @PublishPimProductId ( PublishProductId , PimProductId , PublishCatalogId
                                              )
                    SELECT PublishProductId , PimProductId , a.PublishCatalogId
                    FROM ZNodePublishProduct AS a
                    WHERE EXISTS ( SELECT TOP 1 1
                                   FROM @PimProductIds AS qqweq
                                   WHERE qqweq.PimProductId = a.PimProductId
                                         AND
                                         qqweq.PublishCatalogId = a.PublishCatalogId
                                 )
                          AND
                          PublishCatalogId IS NOT NULL;
             WHILE @v_Count <= ISNULL( ( SELECT MAX(RowId)
                                         FROM @Locale
                                       ) , 0)
                 BEGIN
                     WHILE @v_Count_forProduct <= ISNULL( ( SELECT MAX(RowId)
                                                            FROM @PublishPimProductId
                                                          ) , 0)
                         BEGIN
                             SET @V_LocaleId = ( SELECT LocaleId
                                                 FROM @Locale
                                                 WHERE RowID = @v_Count
                                               );
                             SET @V_LocaleCode = ( SELECT LocaleCode
                                                   FROM @Locale
                                                   WHERE RowID = @v_Count
                                                 );
                             SET @PublishCatalogId = ( SELECT TOP 1 PublishCatalogId
                                                       FROM @PublishPimProductId
                                                       WHERE RowId = @v_Count_forProduct
                                                     );
                             DECLARE @T_Attributes TABLE (
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
                                                         );
                             DECLARE @T_Attributes_Locale TABLE (
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
                                                                LocaleId                   INT ,
                                                                IsPersonalizable           BIT ,
                                                                IsCustomeField             BIT ,
                                                                IsConfigurable             BIT
                                                                );
                             INSERT INTO @T_Attributes_Locale ( PimProductId , PimAttributeFamilyId , PimAttributeId , AttributeCode , IsLayeredNavigation , IsHtmlTags , IsComparable , IsPromoRuleCondition , IsFacets , AttributeName , PimProductAttributeValueId , AttributeTypeID , AttributeTypeName , Localeid , IsPersonalizable , IsCustomeField , IsConfigurable
                                                              )
                                    SELECT a.PimProductId , a.PimAttributeFamilyId , d.PimAttributeId , q.AttributeCode , w.IsLayeredNavigation , w.IsHtmlTags , w.IsComparable , w.IsPromoRuleCondition , w.IsFacets , d.AttributeName , a.PimAttributeValueId , q.AttributeTypeID , ws.AttributeTypeName , d.LocaleId , IsPersonalizable , 0 AS IsCustomeField , IsConfigurable
                                    FROM ZnodePimAttributeLocale AS d INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = d.PimAttributeId
                                                                                                             AND
                                                                                                             D.LocaleId IN ( @V_LocaleDefaultId , @V_LocaleId
                                                                                                                           ) )
                                                                      INNER JOIN ZnodeAttributeType AS ws ON ( ws.AttributeTypeId = q.AttributeTypeId )
                                                                      LEFT JOIN ZnodePimFrontendProperties AS w ON ( q.PimAttributeId = w.PimAttributeId )
                                                                      INNER JOIN ZnodePimAttributeValue AS a ON ( a.PimAttributeId = d.PimAttributeId ) 
                                                                                                                --AND EXISTS (SELECT TOP 1 1 FROM ZnodePimCatalogCategory b WHERE a.PimProductId = b.PimProductId)
                                                                                                                AND
                                                                                                                EXISTS ( SELECT TOP 1 1
                                                                                                                         FROM @PublishPimProductId AS we
                                                                                                                         WHERE we.PimProductId = a.PimProductId
                                                                                                                               AND
                                                                                                                               we.PublishCatalogId = @PublishCatalogId
                                                                                                                       )
                                    UNION ALL
                                    SELECT DISTINCT
                                           a.PimParentProductId , 0 AS PimAttributeFamilyId , d.PimAttributeId , q.AttributeCode , w.IsLayeredNavigation , w.IsHtmlTags , w.IsComparable , w.IsPromoRuleCondition , IsFacets , d.AttributeName , 0 AS PimAttributeValueId , q.AttributeTypeID , ws.AttributeTypeName , d.LocaleId , q.IsPersonalizable , 0 AS IsCustomeField , 0 AS IsConfigurable
                                    FROM ZnodePimAttributeLocale AS d INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = d.PimAttributeId
                                                                                                             AND
                                                                                                             D.LocaleId IN ( @V_LocaleDefaultId , @V_LocaleId
                                                                                                                           ) )
                                                                      INNER JOIN ZnodeAttributeType AS ws ON ( ws.AttributeTypeId = q.AttributeTypeId )
                                                                      INNER JOIN ZnodePimLinkProductDetail AS a ON ( a.PimAttributeId = d.PimAttributeId
                                                                                                                     AND
                                                                                                                     EXISTS ( SELECT TOP 1 1
                                                                                                                              FROM @PublishPimProductId AS b
                                                                                                                              WHERE a.PimParentProductId = b.PimProductId
                                                                                                                                    AND
                                                                                                                                    b.PublishCatalogId = @PublishCatalogId
                                                                                                                            ) )
                                                                      LEFT JOIN ZnodePimFrontendProperties AS w ON ( q.PimAttributeId = w.PimAttributeId )



                                    --WHERE EXISTS
                                    --(
                                    --    SELECT TOP 1 1
                                    --    FROM ZnodePimCatalogCategory zsw
                                    --    WHERE zsw.PimProductId = a.PimProductId
                                    --          AND zsw.PimCatalogId = @V_PimCatalogId
                                    --)
                                    UNION ALL
                                    SELECT DISTINCT
                                           a.PimProductId , NULL AS PimAttributeFamilyId , NULL AS PimAttributeId , a.CustomCode AS AttributeCode , NULL AS IsLayeredNavigation , NULL AS IsHtmlTags , NULL AS IsComparable , NULL AS IsPromoRuleCondition , NULL AS IsFacets , b.CustomKey AS AttributeName , NULL AS PimAttributeValueId , NULL AS AttributeTypeID , NULL AS AttributeTypeName , b.LocaleId AS LocaleId , 0 AS IsPersonalizable , 1 AS IsCustomeField , 0 AS IsConfigurable
                                    FROM ZnodePimCustomField AS a INNER JOIN ZnodePimCustomFieldLocale AS b ON ( b.PimCustomFieldId = a.PimCustomFieldId
                                                                                                                 AND
                                                                                                                 b.LocaleId IN ( @V_LocaleDefaultId , @V_LocaleId
                                                                                                                               ) )
                                    WHERE EXISTS ( SELECT TOP 1 1
                                                   FROM @PublishPimProductId AS b
                                                   WHERE a.PimProductId = b.PimProductId
                                                         AND
                                                         b.PublishCatalogId = @PublishCatalogId
                                                 );
                             --UNION ALL 
                             --SELECT DISTINCT
                             --	   a.PimProductId,
                             --	   NULL PimAttributeFamilyId,
                             --	   NULL PimAttributeId,
                             --	   b.SupplierVendorCode AttributeCode,
                             --	   NULL IsLayeredNavigation,
                             --	   NULL IsHtmlTags,
                             --	   NULL IsComparable,
                             --	   NULL IsPromoRuleCondition,
                             --	   NULL IsFacets,
                             --	   c.SupplierVendorName AttributeName,
                             --	   NULL PimAttributeValueId,
                             --	   NULL AttributeTypeID,
                             --	   NULL AttributeTypeName,
                             --	   c.LocaleId LocaleId,
                             --	   0 IsPersonalizable,
                             --	   0 IsCustomeField,
                             --	   0 IsConfigurable
                             --FROM ZnodePimSupplierVendorProduct a
                             --INNER JOIN ZnodePimSupplierVendor b ON (b.PimSupplierVendorId = a.PimSupplierVendorId)
                             --INNER JOIN ZnodePimSupplierVendorLocale c ON (c.PimSupplierVendorId  = b.PimSupplierVendorId AND c.LocaleId IN (@V_LocaleDefaultId, @V_LocaleId) )
                             --  WHERE EXISTS
                             --(
                             --	SELECT TOP 1 1
                             --	FROM @PublishPimProductId bb
                             --	WHERE a.PimProductId = bb.PimProductId
                             --			AND bb.PublishCatalogId = @PublishCatalogId
                             --)
                             --UNION ALL 
                             --SELECT DISTINCT
                             --	   a.PimProductId,
                             --	   NULL PimAttributeFamilyId,
                             --	   NULL PimAttributeId,
                             --	   b.ManufacturerBrandCode AttributeCode,
                             --	   NULL IsLayeredNavigation,
                             --	   NULL IsHtmlTags,
                             --	   NULL IsComparable,
                             --	   NULL IsPromoRuleCondition,
                             --	   NULL IsFacets,
                             --	   c.BrandName AttributeName,
                             --	   NULL PimAttributeValueId,
                             --	   NULL AttributeTypeID,
                             --	   NULL AttributeTypeName,
                             --	    c.LocaleId LocaleId,
                             --	   0 IsPersonalizable,
                             --	   0  IsCustomeField,
                             --	   0 IsConfigurable
                             --FROM ZnodePimManufacturerBrandProduct a
                             --INNER JOIN ZnodePimManufacturerBrand b ON (b.PimManufacturerBrandId = a.PimManufacturerBrandId)
                             --INNER JOIN ZnodePimManufacturerBrandLocale c ON (c.PimManufacturerBrandId  = b.PimManufacturerBrandId AND c.LocaleId IN (@V_LocaleDefaultId, @V_LocaleId) )
                             --  WHERE EXISTS
                             --(
                             --	SELECT TOP 1 1
                             --	FROM @PublishPimProductId bb
                             --	WHERE a.PimProductId = bb.PimProductId
                             --			AND bb.PublishCatalogId = @PublishCatalogId
                             --)


                             INSERT INTO @T_Attributes
                                    SELECT a.PimProductId , a.PimAttributeFamilyId , a.PimAttributeId , a.AttributeCode , a.IsLayeredNavigation , a.IsHtmlTags , a.IsComparable , a.IsPromoRuleCondition , IsFacets , a.AttributeName , a.PimProductAttributeValueId , AttributeTypeID , a.AttributeTypeName , IsPersonalizable , IsCustomeField , IsConfigurable
                                    FROM @T_Attributes_Locale AS a
                                    WHERE a.LOcaleid = @V_LocaleId;
                             INSERT INTO @T_Attributes
                                    SELECT a.PimProductId , a.PimAttributeFamilyId , a.PimAttributeId , a.AttributeCode , a.IsLayeredNavigation , a.IsHtmlTags , a.IsComparable , a.IsPromoRuleCondition , IsFacets , a.AttributeName , a.PimProductAttributeValueId , AttributeTypeID , a.AttributeTypeName , IsPersonalizable , IsCustomeField , IsConfigurable
                                    FROM @T_Attributes_Locale AS a
                                    WHERE a.LOcaleid = @V_LocaleDefaultId
                                          AND
                                          NOT EXISTS ( SELECT TOP 1 1
                                                       FROM @T_Attributes AS s
                                                       WHERE s.AttributeCode = a.AttributeCode
                                                             AND
                                                             s.PimProductId = a.PimProductId
                                                     );
                             DECLARE @T_ProductAttributeValues TABLE (
                                                                     PimProductId    INT ,
                                                                     PimAttributeId  INT ,
                                                                     AttributeCode   VARCHAR(600) ,
                                                                     AttributeName   NVARCHAR(MAX) ,
                                                                     AttributeTypeId INT ,
                                                                     AttributeValues NVARCHAR(MAX)
                                                                     );
                             DECLARE @T_ProductAttributeValues_Locale TABLE (
                                                                            PimProductId      INT ,
                                                                            PimAttributeId    INT ,
                                                                            AttributeCode     VARCHAR(600) ,
                                                                            AttributeName     NVARCHAR(MAX) ,
                                                                            AttributeTypeId   INT ,
                                                                            AttributeTypeName VARCHAR(2000) ,
                                                                            AttributeValues   NVARCHAR(MAX) ,
                                                                            LocaleId          INT
                                                                            );
                             DECLARE @T_ProductAttributeValues_Name TABLE (
                                                                          PimProductId    INT ,
                                                                          PimAttributeId  INT ,
                                                                          AttributeCode   VARCHAR(600) ,
                                                                          AttributeName   NVARCHAR(MAX) ,
                                                                          AttributeTypeId INT ,
                                                                          AttributeValues NVARCHAR(MAX)
                                                                          );
                             INSERT INTO @T_ProductAttributeValues_Locale
                                    SELECT q.PimProductId , q.PimAttributeId , q.AttributeCode , q.AttributeName , q.AttributeTypeID , q.AttributeTypeName , d.AttributeValue , ISNULL(d.LocaleId , @V_LocaleDefaultId)
                                    FROM @T_Attributes AS q LEFT JOIN ZnodePimAttributeValueLocale AS d ON ( q.PimProductAttributeValueId = d.PimAttributeValueId
                                                                                                             AND
                                                                                                             d.LocaleId IN ( @V_LocaleDefaultId , @V_LocaleId
                                                                                                                           ) )
                                    WHERE q.PimAttributeId IS NOT NULL
                                    UNION ALL
                                    SELECT DISTINCT
                                           a.PimProductId , NULL AS PimAttributeId , a.CustomCode AS AttributeCode , b.CustomKey , NULL AS AttributeTypeID , NULL AS AttributeTypeName , b.CustomKeyValue AS AttributeValue , b.LocaleId
                                    FROM ZnodePimCustomField AS a INNER JOIN ZnodePimCustomFieldLocale AS b ON ( b.PimCustomFieldId = a.PimCustomFieldId
                                                                                                                 AND
                                                                                                                 b.LocaleId IN ( @V_LocaleDefaultId , @V_LocaleId
                                                                                                                               ) )
                                    WHERE EXISTS ( SELECT TOP 1 1
                                                   FROM @PublishPimProductId AS b
                                                   WHERE a.PimProductId = b.PimProductId
                                    --   AND b.PublishCatalogId = @PublishCatalogId
                                                 );
                             UPDATE a
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
                                                              ELSE 'no-image.png'
                                                          END
                             FROM @T_ProductAttributeValues_Locale a
                             WHERE a.AttributeTypeName IN ( 'Image' , 'Video' , 'Audio'
                                                          );


                             -- SELECT * FROM ZnodeAttributeType 
                             INSERT INTO @T_ProductAttributeValues
                                    SELECT a.PimProductId , a.PimAttributeId , a.AttributeCode , a.AttributeName , a.AttributeTypeId , a.AttributeValues
                                    FROM @T_ProductAttributeValues_Locale AS a
                                    WHERE LocaleId = @V_LocaleId;
                             INSERT INTO @T_ProductAttributeValues
                                    SELECT a.PimProductId , a.PimAttributeId , a.AttributeCode , a.AttributeName , a.AttributeTypeId , a.AttributeValues
                                    FROM @T_ProductAttributeValues_Locale AS a
                                    WHERE Localeid = @V_LocaleDefaultId
                                          AND
                                          NOT EXISTS ( SELECT TOP 1 1
                                                       FROM @T_ProductAttributeValues AS s
                                                       WHERE s.AttributeCode = a.AttributeCode
                                                             AND
                                                             s.PimProductId = a.PimProductId
                                                     );
                             
                             -- SELECT * FROM ZnodePimLinkProductDetail WHERE PimParentProductId = 6 AND PimAttributeId = 291
                             WITH LinkProducts
                                  AS (SELECT PimProductId , PimAttributeId , SUBSTRING( ( SELECT DISTINCT
                                                                                                 ','+CAST(zpp.PublishProductId AS VARCHAR(1000))
                                                                                          FROM ZnodePimLinkProductDetail AS df INNER JOIN ZNodePublishProduct AS zpp ON ( zpp.PimProductId = 6
                                                                                                                                                                          AND
                                                                                                                                                                          zpp.PublishCatalogId = @PublishCatalogId )
                                                                                          WHERE df.PimParentProductId = fd.PimProductId
                                                                                                AND
                                                                                                df.PimAttributeId = fd.PimAttributeId
                                                                                          FOR XML PATH('')
                                                                                        ) , 2 , 4000) AS AttributeValues
                                      FROM @T_ProductAttributeValues AS fd
                                      WHERE EXISTS ( SELECT TOP 1 1
                                                     FROM ZnodePimLinkProductDetail AS de
                                                     WHERE de.PimParentProductId = fd.PimProductId
                                                           AND
                                                           de.PimAttributeId = fd.PimAttributeId
                                                   ) )
                                  UPDATE a
                                         SET AttributeValues = b.AttributeValues
                                  FROM @T_ProductAttributeValues a INNER JOIN LinkProducts b ON ( a.PimProductId = b.PimProductId
                                                                                                  AND
                                                                                                  a.PimAttributeId = b.PimAttributeId );
                             INSERT INTO @T_ProductAttributeValues_Name
                                    SELECT a.PimProductId , a.PimAttributeId , a.AttributeCode , a.AttributeName , a.AttributeTypeId , a.AttributeValues
                                    FROM @T_ProductAttributeValues AS a
                                    WHERE a.AttributeCode IN ( 'ProductName' , 'SKU'
                                                             );
                             DECLARE @TBL_StoreTOXML TABLE (
                                                           PublishProductId     INT ,
                                                           PimProductId         INT ,
                                                           AttributeCode        VARCHAR(600) ,
                                                           AttributeName        NVARCHAR(MAX) ,
                                                           IsLayeredNavigation  BIT ,
                                                           IsHtmlTags           BIT ,
                                                           IsComparable         BIT ,
                                                           IsPromoRuleCondition BIT ,
                                                           AttributeTypeName    VARCHAR(600) ,
                                                           AttributeValues      NVARCHAR(MAX) ,
                                                           IsPersonalizable     BIT ,
                                                           IsCustomeField       BIT ,
                                                           IsConfigurable       BIT
                                                           );
                             INSERT INTO @TBL_StoreTOXML
                                    SELECT Wq.PublishProductId , a.PimProductId , a.AttributeCode , ISNULL(a.AttributeName , '') AS AttributeName , ISNULL(a.IsLayeredNavigation , 0) , ISNULL(a.IsHtmlTags , 0) , ISNULL(a.IsComparable , 0) , ISNULL(a.IsPromoRuleCondition , 0) , ISNULL(a.AttributeTypeName , '') , b.AttributeValues , a.IsPersonalizable , a.IsCustomeField , a.IsConfigurable
                                    FROM @T_Attributes AS a INNER JOIN ZNodePublishProduct AS wq ON ( wq.PimProductId = a.PimProductId )
                                                            LEFT JOIN @T_ProductAttributeValues AS b ON ( b.AttributeName = a.AttributeCode
                                                                                                          AND
                                                                                                          b.PimProductId = a.PimProductId ); 
	


                             -- SELECT * FROM @TBL_StoreTOXML
                             INSERT INTO @Xmlreturn
                                    SELECT PublishProductId , ( SELECT ( SELECT AttributeCode , AttributeName , IsLayeredNavigation , IsHtmlTags , IsComparable , IsPromoRuleCondition , IsFacets , AttributeValues , AttributeTypeName , a.IsPersonalizable , a.IsCustomeField , a.IsConfigurable
                                                                         FROM @TBL_StoreTOXML AS wd
                                                                         WHERE wd.PimProductId = a.PimProductId
                                                                               AND
                                                                               wd.PublishProductId = q.PublishProductId
                                                                               AND
                                                                               wd.AttributeCode = a.AttributeCode
                                                                         FOR XML PATH('AttributeEntity') , TYPE
                                                                       )
                                                                FROM @T_Attributes AS a
                                                                WHERE a.PimProductId = q.PimProductId
                                                                FOR XML PATH('') , ROOT('Attributes')
                                                              ) AS XMLGEM
                                    FROM @PublishPimProductId AS q
                                    GROUP BY q.PublishProductId , q.PimProductId;

	 
                             -- SELECT * FROM @Xmlreturn

                             INSERT INTO @XmlFullreturn
                                    SELECT '<ProductEntity>'+ISNULL( ( SELECT b.PublishProductId AS ZnodeProductId , SUBSTRING( ( SELECT DISTINCT
                                                                                                                                         ','+CAST(ds.PublishCategoryId AS VARCHAR(1000))
                                                                                                                                  FROM ZnodePublishCategoryProduct AS ds INNER JOIN ZNodePublishProduct AS dt ON ( dt.publishProductId = dt.PublishProductId )
                                                                                                                                                                         INNER JOIN ZnodePublishCatalog AS dg ON ( dg.PublishCataLogId = ds.PublishCatalogId )
                                                                                                                                                                         INNER JOIN ZnodePublishCategory AS dq ON ( dq.PublishCategoryId = ds.PublishcategoryId )
                                                                                                                                                                         INNER JOIN ZnodePimCatalogCategory AS df ON ( df.PimProductId = dt.PimProductId
                                                                                                                                                                                                                       AND
                                                                                                                                                                                                                       df.PimCatalogId = dg.PimCatalogId
                                                                                                                                                                                                                       AND
                                                                                                                                                                                                                       df.PimCategoryId = dq.PimCategoryId )
                                                                                                                                  WHERE ds.PublishProductId = b.PublishProductId
                                                                                                                                        AND
                                                                                                                                        ds.PublishCatalogId = b.PublishCatalogId
                                                                                                                                  FOR XML PATH('')
                                                                                                                                ) , 2 , 4000) AS TempZnodeCategoryIds , c.AttributeValues AS Name , d.AttributeValues AS SKU , b.PublishCatalogId AS ZnodeCatalogId , @V_LocaleId AS LocaleId
                                                                       FROM @PublishPimProductId AS b LEFT JOIN @T_ProductAttributeValues_Name AS c ON ( c.PimProductId = b.PimProductId
                                                                                                                                                         AND
                                                                                                                                                         c.AttributeCode = 'ProductName' )
                                                                                                      LEFT JOIN @T_ProductAttributeValues_Name AS d ON ( d.PimProductId = b.PimProductId
                                                                                                                                                         AND
                                                                                                                                                         d.AttributeCode = 'SKU' )
                                                                       WHERE b.PimProductId = a.PimProductId
                                                                             AND
                                                                             b.RowId = @v_Count_forProduct
                                                                       FOR XML PATH('')
                                                                     ) , '')+b.ProductXml+'</ProductEntity>'
                                    FROM @PublishPimProductId AS a INNER JOIN @Xmlreturn AS b ON ( a.PublishProductId = b.ProductId )
                                    WHERE RowId = @v_Count_forProduct;
                             --WHERE a.PublishCatalogId = @PublishCatalogId 


                             DELETE FROM @Xmlreturn;
                             DELETE FROM @TBL_StoreTOXML;
                             DELETE FROM @T_ProductAttributeValues_Name;
                             DELETE FROM @T_ProductAttributeValues;
                             DELETE FROM @T_ProductAttributeValues_Locale;
                             DELETE FROM @T_Attributes;
                             DELETE FROM @T_Attributes_Locale;
                             SET @v_Count_forProduct = @v_Count_forProduct + 1;
                         END;
                     SET @v_Count_forProduct = 1;
                     SET @v_Count = @v_Count + 1;
                 END;
             SELECT *
             FROM @XmlFullreturn;
             SELECT PublishProductId AS ProductXml
             FROM ZNodePublishProduct AS wq
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @PimProductIds AS sw
                            WHERE wq.PimProductId = sw.PimProductId
                                  AND
                                  sw.PublishCatalogId = wq.PublishCatalogId
                          );
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE();
             ROLLBACK TRAN A;
         END CATCH;
     END;