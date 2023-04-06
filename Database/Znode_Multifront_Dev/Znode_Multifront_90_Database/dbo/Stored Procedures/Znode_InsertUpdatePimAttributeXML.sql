CREATE PROCEDURE [dbo].[Znode_InsertUpdatePimAttributeXML] 
(
 @PimAttributeId  INT
)
AS
 BEGIN 
  BEGIN TRY 
     
 SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate(); 
			-- DECLARE @PimAttributeId VARCHAR(max) = ''
			
			-- DECLARE @PimCatalogId int= ISNULL((SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId), 0);  --- this variable is used to carry y pim catalog id by using published catalog id
            -- This variable is used to carry the default locale which is globaly set

		
		
			DECLARE @TBL_AttributesDetails TABLE(PimAttributeId int , AttributeCode varchar(300), IsUseInSearch bit, IsHtmlTags bit
										, IsComparable bit, IsFacets bit, AttributeValue varchar(max)
										,PRIMARY KEY (AttributeCode,PimAttributeId)) 
			 -- This variable is used in loop to increment the counter
   		    DECLARE @TBL_PimAttributeIds TABLE(PimAttributeId int, ParentPimAttributeId int, AttributeTypeId int, AttributeCode varchar(300)
									, IsRequired bit, IsLocalizable bit, IsFilterable bit, IsSystemDefined bit, IsConfigurable bit, IsPersonalizable bit
									, DisplayOrder int, HelpDescription varchar(max), IsCategory bit, IsHidden bit, CreatedDate datetime, ModifiedDate datetime,
									 AttributeName nvarchar(max), AttributeTypeName varchar(300), IsCustomField bit,LocaleId INT ,IsSwatch BIT);
			    
				INSERT INTO @TBL_AttributesDetails (PimAttributeId,AttributeCode,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets )
				SELECT ZPA.PimAttributeId,ZPA.AttributeCode,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets
				FROM ZnodePimAttribute AS ZPA 
				LEFT JOIN ZnodePimFrontendProperties AS ZPFP ON ZPFP.PimAttributeId = ZPA.PimATtributeId
				GROUP BY ZPA.PimAttributeId,ZPA.AttributeCode,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets
	
				 ;WITH Cte_PimAttributeFilter
                  AS (SELECT ZPA.PimAttributeId,ZPA.ParentPimAttributeId,ZPA.AttributeTypeId,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,ZPA.IsFilterable,
					  ZPA.IsSystemDefined,ZPA.IsConfigurable,ZPA.IsPersonalizable,ZPA.DisplayOrder,ZPA.HelpDescription,ZPA.IsCategory,ZPA.IsHidden,ZPA.CreatedBy,
					  ZPA.CreatedDate,ZPA.ModifiedBy,ZPA.ModifiedDate,ZPAL.AttributeName,ZAT.AttributeTypeName,ZPAL.LocaleId,ZPA.IsSwatch
                      FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodePimAttributeLocale ZPAL ON(ZPAL.PimAttributeId = ZPA.PimAttributeId)
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
					--  WHERE AttributeCode = 'ShortDescription'
                      --    WHERE EXISTS(SELECT TOP 1 1 FROM @PimAttributeId SP WHERE SP.id = CTPADV.PimAttributeId)
					  )
       		INSERT INTO @TBL_PimAttributeIds ( PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
                      IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,LocaleId,IsSwatch )
			SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
				  IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,LocaleId,IsSwatch
            FROM Cte_PimAttributeFilter CTAF

			DECLARE @TBL_AttributevalueGet TABLE(PimAttributeId INT,AttributeCode VARCHAR(300),AttributeValue NVARCHAR(max),LocaleId INT)

			INSERT INTO @TBL_AttributevalueGet (PimAttributeId,AttributeCode,AttributeValue,LocaleId)
			 SELECT PimAttributeId  , AttributeCode ,
			        (SELECT  (
                                   SELECT  TBA.AttributeCode,
                                          AttributeName,
                                          ISNULL(IsUseInSearch, 0) AS IsUseInSearch,
                                          ISNULL(IsHtmlTags, 0) AS IsHtmlTags,
										  ISNULL(IsComparable, 0) AS IsComparable,
                                          ISNULL(IsFacets, 0) AS IsFacets,
                                          ISNULL(DisplayOrder,0) AS DisplayOrder,
										  AttributeTypeName,
                                          IsPersonalizable,
                                          ISNULL(TBA.IsCustomField, 0) AS IsCustomField,
                                          ISNULL(IsConfigurable, 0) AS IsConfigurable,
										 CASE WHEN  IsSwatch = 1 THEN 'true' 
										   WHEN IsSwatch = 0  THEN 'false' ELSE 'null' END    IsSwatch
										  
                                   FROM  @TBL_PimAttributeIds AS TBA 
								   INNER JOIN @TBL_AttributesDetails AS TBAD ON TBAD.PimAttributeId = TBA.PimAttributeId										
                                   WHERE TBA.PimAttributeId = TBAVI.PimAttributeId
								   AND TBA.LocaleId = TBAVI.LocaleId
								   FOR XML PATH('') 
                               )  ) AttributeValue, LocaleId
                  FROM @TBL_PimAttributeIds AS TBAVI
				  
				 		  	  
				  MERGE INTO ZnodePimAttributeXML TARGET 
				  USING (    SELECT * FROM @TBL_AttributevalueGet ) SOURCE 
				  ON (TARGET.PimAttributeId = SOURCE.PimAttributeId 
				  AND TARGET.LocaleId = Source.LocaleID  )
				  WHEN MATCHED THEN 
				  uPDATE 
				  SET TARGET.AttributeXml = SOURCE.AttributeValue
				      , TARGET.AttributeCode = Source.AttributeCode
				      ,TARGET.ModifiedDate = @GetDate
					  ,TARGET.ModifiedBY = 2 
                  WHEN NOT MATCHED THEN 
				  INSERT (PimAttributeId
				          , AttributeCode
							,AttributeXml
							,LocaleId
							,CreatedBy
							,CreatedDate
							,ModifiedBy
							,ModifiedDate)
				  VALUES (SOURCE.PimAttributeId,SOURCE.AttributeCode, SOURCE.AttributeValue ,Source.LocaleId ,2,@GetDate,2 ,@GetDate);

			

   
  END TRY 
  BEGIN CATCH
  SELECT ERROR_MESSAGE() 
  END CATCH 
 END