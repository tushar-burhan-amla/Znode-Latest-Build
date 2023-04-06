-- EXEC Znode_GetPimProductAttributeValues 0,1,0


CREATE PROCEDURE [dbo].[Znode_GetPimProductAttributeValues_old](
       @PimProductId INT = 0 ,
       @LocaleId     INT = 0 ,
       @IsCopy       BIT = 0)
AS
     BEGIN
         BEGIN TRY
             DECLARE @V_LocaleId INT= @LocaleId , @LocaleIdDefault INT= ( SELECT FeatureValues
                                                                          FROM ZnodeGlobalSetting
                                                                          WHERE FeatureName = 'Locale'
                                                                        ) , @DefaultfamilyId INT= ( SELECT PimAttributeFamilyId
                                                                                                    FROM ZnodePimAttributeFamily
                                                                                                    WHERE IsDefaultFamily = 1
                                                                                                          AND
                                                                                                          IsCategory = 0
                                                                                                  ) , @DefaultProductFamily INT; 
             --------------------------------------------------------------------------Find the default family of the product --------------------------------------------------------------------
             DECLARE @TBL_DefraultProductfamilyId TABLE (
                                                        PimProdcutFamilyId INT
                                                        );
             INSERT INTO @TBL_DefraultProductfamilyId ( PimProdcutFamilyId
                                                      )
             EXEC Znode_GetPimProductAttributeFamilyId @PimProductId;
             SET @DefaultProductFamily = ( SELECT TOP 1 PimProdcutFamilyId
                                           FROM @TBL_DefraultProductfamilyId
                                         );

             ------------------------------------------------------------ collect the Pim Attribute locale wise -------------------------------------------------------------------------------------

             DECLARE @TBL_Attributes TABLE (
                                           PimProductId         INT ,
                                           PimAttributeId       INT ,
                                           AttributeName        NVARCHAR(MAX) ,
                                           PimAttributeValueId  INT ,
                                           PimAttributeFamilyId INT ,
                                           AttributeTypeID      INT ,
                                           AttributeCode        VARCHAR(600) ,
                                           IsRequired           BIT ,
                                           IsLocalizable        BIT ,
                                           IsFilterable         BIT ,
                                           IsSystemDefined      BIT ,
                                           IsConfigurable       BIT ,
                                           IsPersonalizable     BIT ,
                                           DisplayOrder         INT ,
                                           HelpDescription      VARCHAR(MAX) ,
                                           IsCategory           BIT
                                           );
             DECLARE @TBL_Attributes_locale TABLE (
                                                  PimProductId         INT ,
                                                  PimAttributeId       INT ,
                                                  AttributeName        NVARCHAR(MAX) ,
                                                  PimAttributeValueId  INT ,
                                                  PimAttributeFamilyId INT ,
                                                  AttributeTypeID      INT ,
                                                  AttributeCode        VARCHAR(600) ,
                                                  IsRequired           BIT ,
                                                  IsLocalizable        BIT ,
                                                  IsFilterable         BIT ,
                                                  IsSystemDefined      BIT ,
                                                  IsConfigurable       BIT ,
                                                  IsPersonalizable     BIT ,
                                                  DisplayOrder         INT ,
                                                  HelpDescription      VARCHAR(MAX) ,
                                                  IsCategory           BIT
                                                  );
	       
             ---- Here declare the tables for finding the locale wise attributes 

             INSERT INTO @TBL_Attributes_locale ( PimProductId , PimAttributeId , AttributeName , PimAttributeValueId , PimAttributeFamilyId , AttributeTypeID , AttributeCode , IsRequired , IsLocalizable , IsFilterable , IsSystemDefined , IsConfigurable , IsPersonalizable , DisplayOrder , HelpDescription , IsCategory
                                                )
                    SELECT DISTINCT
                           ISNULL(a.PimProductId , @PimProductId) AS PimProductId , d.PimAttributeId , d.AttributeName , a.PimAttributeValueId , zpfgm.PimAttributeFamilyId , q.AttributeTypeID , q.AttributeCode , q.IsRequired , q.IsLocalizable , q.IsFilterable , q.IsSystemDefined , q.IsConfigurable , q.IsPersonalizable , q.DisplayOrder , q.HelpDescription , q.IsCategory
                    FROM ZnodePimAttributeLocale AS d INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = d.PimAttributeId
                                                                                             AND
                                                                                             d.LocaleId = @V_LocaleId )
                                                      INNER JOIN ZnodePimFamilyGroupMapper AS zpfgm ON ( zpfgm.PimAttributeId = q.PimAttributeId )
                                                      LEFT JOIN ZnodePimAttributeValue AS a ON ( a.PimAttributeId = d.PimAttributeId
                                                                                                 AND
                                                                                                 a.PimProductId = @PimProductId
                                                                                                 AND
                                                                                                 a.PimAttributeFamilyId = zpfgm.PimAttributeFamilyId )
                    WHERE zpfgm.PimAttributeFamilyId IN ( @DefaultProductFamily , @DefaultfamilyId
                                                        )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimConfigureProductAttribute AS sd
                                       WHERE sd.PimAttributeId = q.PimAttributeId
                                             AND
                                             zpfgm.PimAttributeFamilyId = sd.PimFamilyId
                                             AND
                                             sd.PimProductId = @PimProductId
                                     )
                    UNION ALL  --- this union for personalies attribute which is directly attached to the product 
				
                    SELECT ISNULL(zpav.PimProductId , 66) AS PimProductId , q.PimAttributeId , d.AttributeName , zpav.PimAttributeValueId , zpav.PimAttributeFamilyId , q.AttributeTypeID , q.AttributeCode , q.IsRequired , q.IsLocalizable , q.IsFilterable , q.IsSystemDefined , q.IsConfigurable , q.IsPersonalizable , q.DisplayOrder , q.HelpDescription , q.IsCategory
                    FROM ZnodePimAttributeValue AS zpav INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = zpav.PimAttributeId )
                                                        INNER JOIN ZnodePimAttributeLocale AS d ON ( d.PimAttributeId = q.PimAttributeId
                                                                                                     AND
                                                                                                     d.LocaleId = @LocaleIdDefault )
                    WHERE zpav.PimProductId = 66
                          AND
                          zpav.PimAttributeFamilyId IS NULL; 
		   
             --- Collect the data for localeid pass in parameter  

             INSERT INTO @TBL_Attributes ( PimProductId , PimAttributeId , AttributeName , PimAttributeValueId , PimAttributeFamilyId , AttributeTypeID , AttributeCode , IsRequired , IsLocalizable , IsFilterable , IsSystemDefined , IsConfigurable , IsPersonalizable , DisplayOrder , HelpDescription , IsCategory
                                         )
                    SELECT a.PimProductId , a.PimAttributeId , a.AttributeName , a.PimAttributeValueId , a.PimAttributeFamilyId , a.AttributeTypeID , a.AttributeCode , a.IsRequired , a.IsLocalizable , a.IsFilterable , a.IsSystemDefined , a.IsConfigurable , a.IsPersonalizable , a.DisplayOrder , a.HelpDescription , a.IsCategory
                    FROM @TBL_Attributes_locale AS a
                    UNION ALL
                    SELECT DISTINCT
                           ISNULL(a.PimProductId , @PimProductId) AS PimProductId , d.PimAttributeId , d.AttributeName , a.PimAttributeValueId , zpfgm.PimAttributeFamilyId , q.AttributeTypeID , q.AttributeCode , q.IsRequired , q.IsLocalizable , q.IsFilterable , q.IsSystemDefined , q.IsConfigurable , q.IsPersonalizable , q.DisplayOrder , q.HelpDescription , q.IsCategory
                    FROM ZnodePimAttributeLocale AS d INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = d.PimAttributeId
                                                                                             AND
                                                                                             d.LocaleId = @LocaleIdDefault )
                                                      INNER JOIN ZnodePimFamilyGroupMapper AS zpfgm ON ( zpfgm.PimAttributeId = q.PimAttributeId )
                                                      LEFT JOIN ZnodePimAttributeValue AS a ON ( a.PimAttributeId = d.PimAttributeId
                                                                                                 AND
                                                                                                 a.PimProductId = @PimProductId
                                                                                                 AND
                                                                                                 a.PimAttributeFamilyId = zpfgm.PimAttributeFamilyId )
                    WHERE zpfgm.PimAttributeFamilyId IN ( @DefaultProductFamily , @DefaultfamilyId
                                                        )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_Attributes_locale AS s
                                       WHERE s.PimAttributeId = d.PimAttributeId
                                     )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimConfigureProductAttribute AS sd
                                       WHERE sd.PimAttributeId = q.PimAttributeId
                                             AND
                                             sd.PimProductId = @PimProductId
                                     )
                    UNION ALL --- this union for personalies attribute which is directly attached to the product 
				
                    SELECT ISNULL(zpav.PimProductId , @PimProductId) AS PimProductId , q.PimAttributeId , d.AttributeName , zpav.PimAttributeValueId , zpav.PimAttributeFamilyId , q.AttributeTypeID , q.AttributeCode , q.IsRequired , q.IsLocalizable , q.IsFilterable , q.IsSystemDefined , q.IsConfigurable , q.IsPersonalizable , q.DisplayOrder , q.HelpDescription , q.IsCategory
                    FROM ZnodePimAttributeValue AS zpav INNER JOIN ZnodePimAttribute AS q ON ( q.PimAttributeId = zpav.PimAttributeId )
                                                        INNER JOIN ZnodePimAttributeLocale AS d ON ( d.PimAttributeId = q.PimAttributeId
                                                                                                     AND
                                                                                                     d.LocaleId = @LocaleIdDefault )
                    WHERE zpav.PimProductId = @PimProductId
                          AND
                          zpav.PimAttributeFamilyId IS NULL
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_Attributes_locale AS s
                                       WHERE s.PimAttributeId = d.PimAttributeId
                                     );


				
			
             --- collect the data for localeid in default  also check the attribnute not present in above table 
             --------------------------------------------------------------------------Collect the Attribute families Assign to the product  -----------------------------------------------------------------	

             DECLARE @TBL_PimFamily TABLE (
                                          PimAttributeFamilyId INT ,
                                          FamilyCode           VARCHAR(200) ,
                                          AttributeFamilyName  NVARCHAR(MAX)
                                          );
             DECLARE @TBL_PimFamilyLocale TABLE (
                                                PimAttributeFamilyId INT ,
                                                FamilyCode           VARCHAR(200) ,
                                                AttributeFamilyName  NVARCHAR(MAX)
                                                );
             INSERT INTO @TBL_PimFamilyLocale
                    SELECT zpaf.PimAttributeFamilyId , zpaf.FamilyCode , zpfl.AttributeFamilyName
                    FROM ZnodePimAttributeFamily AS zpaf INNER JOIN ZnodePimFamilyLocale AS zpfl ON ( zpaf.PimAttributeFamilyId = zpfl.PimAttributeFamilyId
                                                                                                      AND
                                                                                                      zpfl.LocaleId = @V_LocaleId )
                    WHERE EXISTS ( SELECT TOP 1 1
                                   FROM @TBL_Attributes AS ta
                                   WHERE ta.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
                                 );

             --- Collect the data for localeid pass in parameter  

             INSERT INTO @TBL_PimFamily
                    SELECT PimAttributeFamilyId , FamilyCode , AttributeFamilyName
                    FROM @TBL_PimFamilyLocale
                    UNION ALL
                    SELECT zpaf.PimAttributeFamilyId , zpaf.FamilyCode , zpfl.AttributeFamilyName
                    FROM ZnodePimAttributeFamily AS zpaf LEFT JOIN ZnodePimFamilyLocale AS zpfl ON ( zpaf.PimAttributeFamilyId = zpfl.PimAttributeFamilyId
                                                                                                     AND
                                                                                                     zpfl.LocaleId = @LocaleIdDefault )
                    WHERE EXISTS ( SELECT TOP 1 1
                                   FROM @TBL_Attributes AS ta
                                   WHERE ta.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
                                 )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_PimFamilyLocale AS tfl
                                       WHERE tfl.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
                                     );
			
             --- collect the data for localeid in default  also check the attribnute not present in above table 
             --------------------------------------------------------------------- Collect the data for Attribute group ------------------------------------------------------

             DECLARE @TBL_AttributeGroup TABLE (
                                               PimAttributeGroupId  INT ,
                                               GroupCode            VARCHAR(200) ,
                                               AttributeGroupName   NVARCHAR(MAX) ,
                                               PimAttributeFamilyId INT ,
                                               PimAttributeId       INT
                                               );
             DECLARE @TBL_AttributeGroupLocale TABLE (
                                                     PimAttributeGroupId  INT ,
                                                     GroupCode            VARCHAR(200) ,
                                                     AttributeGroupName   NVARCHAR(MAX) ,
                                                     PimAttributeFamilyId INT ,
                                                     PimAttributeId       INT
                                                     );
             INSERT INTO @TBL_AttributeGroupLocale
                    SELECT DISTINCT
                           Zpfg.PimAttributeGroupId , Zpfg.GroupCode , zpagl.AttributeGroupName , zpfgm.PimAttributeFamilyId , zpfgm.PimAttributeId
                    FROM @TBL_Attributes AS ta INNER JOIN ZnodePimFamilyGroupMapper AS zpfgm ON ( zpfgm.PimAttributeFamilyId = ta.PimAttributeFamilyId
                                                                                                  AND
                                                                                                  ISNULL(zpfgm.PimAttributeId , 0) = ta.PimAttributeid )
                                               INNER JOIN ZnodePimAttributeGroup AS Zpfg ON ( zpfg.PimAttributeGroupId = zpfgm.PimAttributeGroupId )
                                               INNER JOIN ZnodePimAttributeGroupLocale AS zpagl ON ( zpagl.PimAttributeGroupId = Zpfg.PimAttributeGroupId
                                                                                                     AND
                                                                                                     zpagl.LocaleId = @V_LocaleId );
		 
             --- Collect the data for localeid pass in parameter  

             INSERT INTO @TBL_AttributeGroup
                    SELECT PimAttributeGroupId , GroupCode , AttributeGroupName , PimAttributeFamilyId , PimAttributeId
                    FROM @TBL_AttributeGroupLocale
                    UNION ALL
                    SELECT DISTINCT
                           Zpfg.PimAttributeGroupId , Zpfg.GroupCode , zpagl.AttributeGroupName , zpfgm.PimAttributeFamilyId , zpfgm.PimAttributeId
                    FROM @TBL_Attributes AS ta INNER JOIN ZnodePimFamilyGroupMapper AS zpfgm ON ( zpfgm.PimAttributeFamilyId = ta.PimAttributeFamilyId
                                                                                                  AND
                                                                                                  ISNULL(zpfgm.PimAttributeId , 0) = ta.PimAttributeid )
                                               INNER JOIN ZnodePimAttributeGroup AS Zpfg ON ( zpfg.PimAttributeGroupId = zpfgm.PimAttributeGroupId )
                                               INNER JOIN ZnodePimAttributeGroupLocale AS zpagl ON ( zpagl.PimAttributeGroupId = Zpfg.PimAttributeGroupId
                                                                                                     AND
                                                                                                     zpagl.LocaleId = @LocaleIdDefault )
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_AttributeGroupLocale AS tgl
                                       WHERE tgl.PimAttributeGroupId = zpfgm.PimAttributeGroupId
                                     );

             --- collect the data for localeid in default  also check the attribnute not present in above table 
             ---------------------------------------------------------------------------Collect the values of Attribute for product -------------------------------------------
             DECLARE @TBL_PimAttributeValues TABLE (
                                                   PimProductId    INT ,
                                                   PimAttributeId  INT ,
                                                   AttributeName   NVARCHAR(MAX) ,
                                                   AttributeTypeId INT ,
                                                   AttributeValue  NVARCHAR(MAX) ,
                                                   AttributeCode   VARCHAR(600)
                                                   );
             DECLARE @TBL_PimAttributeValues_Locale TABLE (
                                                          PimProductId    INT ,
                                                          PimAttributeId  INT ,
                                                          AttributeName   NVARCHAR(MAX) ,
                                                          AttributeTypeId INT ,
                                                          AttributeValue  NVARCHAR(MAX) ,
                                                          AttributeCode   VARCHAR(600)
                                                          );
             INSERT INTO @TBL_PimAttributeValues_Locale ( PimProductId , PimAttributeId , AttributeName , AttributeTypeId , AttributeValue , AttributeCode
                                                        )
                    SELECT q.PimProductId , q.PimAttributeId , q.AttributeName , q.AttributeTypeID , d.AttributeValue AS AttributeValue , q.AttributeCode
                    FROM @TBL_Attributes AS q INNER JOIN ZnodePimAttributeValueLocale AS d ON ( d.PimAttributeValueId = q.PimAttributeValueId
                                                                                                AND
                                                                                                d.LocaleId = @V_LocaleId )
                                              LEFT JOIN View_PimDefaultValue AS w ON ( CAST(w.PimAttributeDefaultValueId AS VARCHAR(1000)) = d.AttributeValue
                                                                                       AND
                                                                                       w.PimAttributeId = q.PimAttributeId
                                                                                       AND
                                                                                       w.LocaleId = @V_LocaleId );
			
             --- Collect the data for localeid pass in parameter  

             INSERT INTO @TBL_PimAttributeValues ( PimProductId , PimAttributeId , AttributeName , AttributeTypeId , AttributeValue , AttributeCode
                                                 )
                    SELECT a.PimProductId , a.PimAttributeId , a.AttributeName , a.AttributeTypeId , a.AttributeValue , a.AttributeCode
                    FROM @TBL_PimAttributeValues_Locale AS a
                    UNION ALL
                    SELECT q.PimProductId , q.PimAttributeId , q.AttributeName , q.AttributeTypeID , d.AttributeValue AS AttributeValue , q.AttributeCode
                    FROM @TBL_Attributes AS q LEFT JOIN ZnodePimAttributeValueLocale AS d ON ( d.PimAttributeValueId = q.PimAttributeValueId
                                                                                               AND
                                                                                               d.LocaleId = @LocaleIdDefault )
                                              LEFT JOIN View_PimDefaultValue AS w ON ( CAST(w.PimAttributeDefaultValueId AS VARCHAR(1000)) = d.AttributeValue
                                                                                       AND
                                                                                       w.PimAttributeId = q.PimAttributeId
                                                                                       AND
                                                                                       w.LocaleId = @LocaleIdDefault )
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_PimAttributeValues_Locale AS s
                                       WHERE s.PimAttributeId = q.PimAttributeId
                                             AND
                                             s.PimProductId = q.PimProductId
                                     );
	   
             --- collect the data for localeid in default  also check the attribnute not present in above table 
             --SELECT * FROM @TBL_PimAttributeValues
             --------------------------------------------------------------------Collect the data for attribute default values ---------------------------------------------

             DECLARE @TBL_DefaultValue TABLE (
                                             PimAttributeId             INT ,
                                             PimAttributeDefaultValueId INT ,
                                             AttributeDefaultValueCode  VARCHAR(200) ,
                                             AttributeDefaultValue      NVARCHAR(MAX) ,
                                             IsEditable                 BIT
                                             );
             DECLARE @TBL_DefaultValueLocale TABLE (
                                                   PimAttributeId             INT ,
                                                   PimAttributeDefaultValueId INT ,
                                                   AttributeDefaultValueCode  VARCHAR(200) ,
                                                   AttributeDefaultValue      NVARCHAR(MAX) ,
                                                   IsEditable                 BIT
                                                   );
             INSERT INTO @TBL_DefaultValueLocale ( PimAttributeId , PimAttributeDefaultValueId , AttributeDefaultValueCode , AttributeDefaultValue , IsEditable
                                                 )
                    SELECT DISTINCT
                           ta.PimAttributeId , zpadv.PimAttributeDefaultValueId , zpadv.AttributeDefaultValueCode , AttributeDefaultValue , zpadv.IsEditable
                    FROM ZnodePimAttributeDefaultValue AS zpadv INNER JOIN ZnodePimAttributeDefaultValueLocale AS zpadvl ON ( zpadv.PimAttributeDefaultValueId = zpadvl.PimAttributeDefaultValueId
                                                                                                                              AND
                                                                                                                              zpadvl.LocaleId = @V_LocaleId )
                                                                INNER JOIN @TBL_Attributes AS ta ON ( ta.PimAttributeId = zpadv.PimAttributeId );

             --- Collect the data for localeid pass in parameter  
             INSERT INTO @TBL_DefaultValue ( PimAttributeId , PimAttributeDefaultValueId , AttributeDefaultValueCode , AttributeDefaultValue , IsEditable
                                           )
                    SELECT PimAttributeId , PimAttributeDefaultValueId , AttributeDefaultValueCode , AttributeDefaultValue , IsEditable
                    FROM @TBL_DefaultValueLocale AS tdvl
                    UNION ALL
                    SELECT DISTINCT
                           ta.PimAttributeId , zpadv.PimAttributeDefaultValueId , zpadv.AttributeDefaultValueCode , AttributeDefaultValue , zpadv.IsEditable
                    FROM ZnodePimAttributeDefaultValue AS zpadv INNER JOIN ZnodePimAttributeDefaultValueLocale AS zpadvl ON ( zpadv.PimAttributeDefaultValueId = zpadvl.PimAttributeDefaultValueId
                                                                                                                              AND
                                                                                                                              zpadvl.LocaleId = @LocaleIdDefault )
                                                                INNER JOIN @TBL_Attributes AS ta ON ( ta.PimAttributeId = zpadv.PimAttributeId )
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM @TBL_DefaultValueLocale AS tdv
                                       WHERE tdv.PimAttributeDefaultValueId = zpadv.PimAttributeDefaultValueId
                                     );

             --- collect the data for localeid in default  also check the attribnute not present in above table
             --------------------------------------------------------------Combain the all attributes data for product here  ---------------------------------------- 			 
             DECLARE @TBL_PimUniqueAttribute TABLE (
                                                   PimAttributeId INT
                                                   );
             INSERT INTO @TBL_PimUniqueAttribute ( PimAttributeId
                                                 )
                    SELECT DISTINCT
                           c.PimAttributeId
                    FROM ZnodePimAttributeValidation AS c INNER JOIN ZnodeAttributeInputValidation AS d ON ( c.InputValidationId = d.InputValidationId )
                    WHERE d.Name = 'UniqueValue'
                          AND
                          c.Name = 'true';
             --- This is for unique attribute show blank when copy product
             -----------------------------------------------------------------------------------------------------------------------------



             SELECT q.PimAttributeFamilyId , b.FamilyCode , q.PimAttributeId , g.PimAttributeGroupId , a.AttributeTypeId , j.AttributeTypeName , q.AttributeCode , q.IsRequired , q.IsLocalizable , q.IsFilterable , q.AttributeName ,
                                                                                                                                                                                                                       CASE
                                                                                                                                                                                                                           WHEN j.AttributeTypeName IN ( 'Image' , 'Audio' , 'Video' , 'File' , 'GalleryImages'
                                                                                                                                                                                                                                                       )
                                                                                                                                                                                                                           THEN dbo.FN_GetThumbnailMediaPath ( a.AttributeValue , 0
                                                                                                                                                                                                                                                             ) +'~'+a.AttributeValue
                                                                                                                                                                                                                           ELSE CASE
                                                                                                                                                                                                                                    WHEN @IsCopy = 1
                                                                                                                                                                                                                                         AND
                                                                                                                                                                                                                                         EXISTS ( SELECT TOP 1 1
                                                                                                                                                                                                                                                  FROM @TBL_PimUniqueAttribute AS tvl
                                                                                                                                                                                                                                                  WHERE tvl.PimAttributeId = q.PimAttributeId
                                                                                                                                                                                                                                                )
                                                                                                                                                                                                                                    THEN ''
                                                                                                                                                                                                                                    ELSE a.AttributeValue
                                                                                                                                                                                                                                END
                                                                                                                                                                                                                       END AS AttributeValue , q.PimAttributeValueId , tdv.PimAttributeDefaultValueId , tdv.AttributeDefaultValueCode , tdv.AttributeDefaultValue AS AttributeDefaultValue , ISNULL(NULL , 0) AS RowId , ISNULL(tdv.IsEditable , 1) AS IsEditable , d.ControlName , d.Name AS ValidationName , e.ValidationName AS SubValidationName , e.RegExp , c.Name AS ValidationValue , CAST(CASE
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       WHEN e.RegExp IS NULL
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       THEN 0
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       ELSE 1
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   END AS BIT) AS IsRegExp , q.HelpDescription
             FROM @TBL_Attributes AS q LEFT JOIN @TBL_PimFamily AS b ON ( b.PimAttributeFamilyId = q.PimAttributeFamilyId )
                                       LEFT JOIN @TBL_AttributeGroup AS g ON ( g.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                                               AND
                                                                               g.PimAttributeId = q.PimAttributeId )
                                       LEFT JOIN ZnodeAttributeType AS j ON ( j.AttributeTypeId = q.AttributeTypeId )
                                       LEFT JOIN @TBL_PimAttributeValues AS a ON ( q.PimAttributeId = a.PimAttributeId
                                                                                   AND
                                                                                   ISNULL(q.PimProductId , 0) = ISNULL(a.PimProductId , 0) )
                                       LEFT JOIN ZnodePimAttributeValidation AS c ON ( c.PimAttributeId = q.PimAttributeId )
                                       LEFT JOIN ZnodeAttributeInputValidation AS d ON ( c.InputValidationId = d.InputValidationId )
                                       LEFT JOIN ZnodeAttributeInputValidationRule AS e ON ( c.InputValidationRuleId = e.InputValidationRuleId )
                                       LEFT JOIN @TBL_DefaultValue AS tdv ON ( tdv.PimAttributeId = q.PimAttributeId )
             ORDER BY CASE
                          WHEN q.DisplayOrder IS NULL
                          THEN 0
                          ELSE 1
                      END , q.DisplayOrder , q.PimAttributeId;
         END TRY
         BEGIN CATCH
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
         END CATCH;
     END;