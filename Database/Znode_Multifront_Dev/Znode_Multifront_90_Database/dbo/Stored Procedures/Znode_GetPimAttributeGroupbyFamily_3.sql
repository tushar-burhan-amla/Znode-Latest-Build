-- exec Znode_GetPimAttributeGroupbyFamily 0,0,1
-- SELECT * FROM ZnodePimAttributeGroupLocale


CREATE PROCEDURE [dbo].[Znode_GetPimAttributeGroupbyFamily_3]
(
       @PimAttributeFamilyID INT = 0 ,
       @IsCategory           BIT = 0 ,
       @LocaleId             INT = 0
)
AS
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT;
             SELECT @DefaultLocaleId = dbo.Fn_GetDefaultLocaleId();
             IF @PimAttributeFamilyID = 0
                 BEGIN
                     SELECT @PimAttributeFamilyID = PimAttributeFamilyId
                     FROM ZnodePimAttributeFamily
                     WHERE IsDefaultFamily = 1
					 AND IsCategory  = @IsCategory ;
                 END;
             DECLARE @PimAttributeGroupbyFamily TABLE (
                                                      PimAttributeFamilyId INT ,
                                                      GroupCode            VARCHAR(200) ,
                                                      AttributeGroupName   NVARCHAR(600) ,
                                                      PimAttributeGroupId  INT ,
                                                      GroupDisplayOrder    INT ,
                                                      GroupType            VARCHAR(100)
                                                      );
             DECLARE @PimAttributeGroupbyFamily_locale TABLE (
                                                             PimAttributeFamilyId INT ,
                                                             GroupCode            VARCHAR(200) ,
                                                             AttributeGroupName   NVARCHAR(600) ,
                                                             PimAttributeGroupId  INT ,
                                                             GroupDisplayOrder    INT ,
                                                             GroupType            VARCHAR(100) ,
                                                             LocaleId             INT
                                                             );
             INSERT INTO @PimAttributeGroupbyFamily_locale
                    SELECT DISTINCT
                           qq.PimAttributeFamilyId , GroupCode , AttributeGroupName , c.PimAttributeGroupId , q.DisplayOrder , 'Group' , c.LocaleId
                    FROM dbo.ZnodePimAttributeFamily AS qq INNER JOIN dbo.ZnodePimFamilyGroupMapper AS w ON ( qq.PimAttributeFamilyId = w.PimAttributeFamilyId )
                                                           INNER JOIN ZnodePimAttributeGroup AS q ON ( w.PimAttributeGroupId = q.PimAttributeGroupId )
                                                           INNER JOIN ZnodePimAttributeGroupLocale AS c ON ( q.PimAttributeGroupId = c.PimAttributeGroupId
                                                                                                             AND
                                                                                                             c.LocaleId IN ( @LocaleId , @DefaultLocaleId
                                                                                                                           ) )
                    WHERE ( qq.PimAttributeFamilyId = @PimAttributeFamilyId
                            )
                          AND
                          qq.IsCategory = @IsCategory;


             INSERT INTO @PimAttributeGroupbyFamily
                    SELECT PimAttributeFamilyId , GroupCode , AttributeGroupName , PimAttributeGroupId , GroupDisplayOrder , GroupType
                    FROM @PimAttributeGroupbyFamily_locale AS a
                    WHERE LocaleId = @LocaleId
                    UNION ALL
                    SELECT 0 , Zpa.AttributeCode , Zpl.AttributeName , Zpa.PimAttributeId , NULL , 'Link'
                    FROM ZnodePimAttribute AS Zpa INNER JOIN ZnodePimAttributeLocale AS Zpl ON ( Zpa.PimAttributeId = Zpl.PimAttributeId
                                                                                                 AND
                                                                                                 zpa.IsCategory = @IsCategory
                                                                                                 AND
                                                                                                 Zpl.LocaleId = @LocaleId )
                    WHERE AttributeTypeId = ( SELECT TOP 1 AttributeTypeId
                                              FROM ZnodeAttributeType
                                              WHERE AttributeTypeName = 'Link'
                                            );



             INSERT INTO @PimAttributeGroupbyFamily
                    SELECT PimAttributeFamilyId , GroupCode , AttributeGroupName , PimAttributeGroupId , GroupDisplayOrder , GroupType
                    FROM @PimAttributeGroupbyFamily_locale AS a
                    WHERE LocaleId = @DefaultLocaleId
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @PimAttributeGroupbyFamily AS sse
                                       WHERE sse.PimAttributeGroupId = a.PimAttributeGroupId
                                             AND
                                             sse.PimAttributeFamilyId = a.PimAttributeFamilyId
                                     )
                    UNION ALL
                    SELECT 0 , Zpa.AttributeCode , Zpl.AttributeName , Zpa.PimAttributeId , NULL , 'Link'
                    FROM ZnodePimAttribute AS Zpa INNER JOIN ZnodePimAttributeLocale AS Zpl ON ( Zpa.PimAttributeId = Zpl.PimAttributeId
                                                                                                 AND
                                                                                                 zpa.IsCategory = @IsCategory
                                                                                                 AND
                                                                                                 Zpl.LocaleId = @DefaultLocaleId )
                    WHERE zpa.AttributeTypeId = ( SELECT TOP 1 AttributeTypeId
                                                  FROM ZnodeAttributeType
                                                  WHERE AttributeTypeName = 'Link'
                                                )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @PimAttributeGroupbyFamily AS sse
                                       WHERE sse.PimAttributeGroupId = zpa.PimAttributeId
                                             AND
                                             sse.PimAttributeFamilyId = 0
                                     );

	
             --INSERT INTO @PimAttributeGroupbyFamily
             --(PimAttributeFamilyId,GroupCode,AttributeGroupName,PimAttributeGroupId,GroupDisplayOrder,GroupType)
             --SELECT 0 , Zpa.AttributeCode , Zpl.AttributeName, Zpa.PimAttributeId,null,'Link' FROM ZnodePimAttribute Zpa 
             --inner join ZnodePimAttributeLocale Zpl on ( Zpa.PimAttributeId = Zpl.PimAttributeId AND zpa.IsCategory= @IsCategory AND Zpl.LocaleId IN( @LocaleId,@DefaultLocaleId)) where AttributeTypeId = 19    -- Link type attribute ( Default attribute)


             SELECT PimAttributeFamilyId , GroupCode , AttributeGroupName , PimAttributeGroupId , GroupType
             FROM @PimAttributeGroupbyFamily
             ORDER BY CASE
                          WHEN GroupDisplayOrder IS NULL
                          THEN 1
                          ELSE 0
                      END , GroupDisplayOrder;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE() , ERROR_LINE();
         END CATCH;
         END;