 
CREATE PROCEDURE [dbo].[Znode_GetPimProductAttributeValues]  
(  
    @ChangeFamilyId INT = 0,  
    @PimProductId   INT = 0,  
    @LocaleId       INT = 0,  
    @IsCopy         BIT = 0  
)  
AS  
/*  
  Summary :- This procedure is used to get the Attribute and Product attribute value as per filter pass   
  Unit Testing   
  BEGIN TRAN  
  EXEC Znode_GetPimProductAttributeValues 0, 16,1,0  
  
  SELECT * fROM znodePimAttribute WHERE IsShowOnGrid = 1   
  ROLLBACK TRAN  
  
*/    
     BEGIN  
      BEGIN TRAN PimProductAttributeValues  
         BEGIN TRY  
             DECLARE @IsFilesNameRequired bit=1, @V_LocaleId INT= @LocaleId, @LocaleIdDefault INT= dbo.Fn_GetDefaultLocaleId(), @PimAttributeFamilyId  INT;   
              
    DECLARE @TBL_PimAttribute TABLE (PimAttributeId INT ,ParentPimAttributeId INT ,AttributeTypeId INT ,AttributeCode VARCHAR(600),IsRequired BIT,IsLocalizable BIT,IsFilterable BIT  
      ,IsSystemDefined BIT,IsConfigurable BIT ,IsPersonalizable BIT,DisplayOrder INT ,HelpDescription NVARCHAR(max),IsCategory BIT,IsHidden BIT,CreatedDate DATETIME ,ModifiedDate DATETIME   
      ,AttributeName NVARCHAR(max) ,AttributeTypeName VARCHAR(600) )  
      
    DECLARE @TBL_PimAttributeDefault TABLE (PimAttributeId INT,AttributeDefaultValueCode VARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder int,PimAttributeDefaultValueId INT, IsDefault BIT )  
    CREATE TABLE #TBL_AttributeValueDetail  (Id int identity,PimProductId INT , AttributeValue NVARCHAR(max),AttributeCode VARCHAR(300),PimAttributeId INT, AttributeDefaultValue NVARCHAR(max),FilesName NVARCHAR(max))  
    DECLARE @TBL_AttributeFamily TABLE (PimAttributeFamilyId INT ,FamilyCode VARCHAR(600),IsSystemDefined BIT ,IsDefaultFamily BIT ,IsCategory BIT ,AttributeFamilyName NVARCHAR(max))  
    DECLARE @TBL_MultiSelectAttribute TABLE (PimAttributeId INT , AttributeCode VARCHAR(600))  
    CREATE TABLE #TBL_AttributeValueFinale  (PimProductId INT , AttributeValue NVARCHAR(max),AttributeCode VARCHAR(300),PimAttributeId INT, AttributeDefaultValueCode NVARCHAR(MAX),FilesName NVARCHAR(MAX) )  
    INSERT INTO @TBL_MultiSelectAttribute (PimAttributeId,AttributeCode)  
    SELECT PimAttributeId,AttributeCode FROM [dbo].[Fn_GetProductMultiSelectAttributes] ()  
    DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))  
    INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)  
    SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()  
      --- Get the default family id   
    IF @ChangeFamilyId = 0   
      BEGIN          
    SET @PimAttributeFamilyId = ISNULL((SELECT TOP 1 PimAttributeFamilyId FROM ZnodePimProduct ZPP WHERE PimProductId = @PimProductId ), dbo.Fn_GetDefaultPimProductFamilyId() )  
      END     
       ELSE   
      BEGIN   
    SET @PimAttributeFamilyId = @ChangeFamilyId  
      END   

     DECLARE @PimAttributeId TransferId , @PimProductId_new TransferId  
  
    INSERT INTO @PimAttributeId(Id)  
    SELECT PimAttributeId FROM ZnodePimFamilyGroupMapper  ZPFGM   
    WHERE PimAttributeFamilyId = @PimAttributeFamilyId AND ISNULL(PimAttributeId,0) <> 0  
    AND NOT EXISTS  
    (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute ZPCPA WHERE ZPCPA.PimAttributeId = ZPFGM.PimAttributeId AND ZPCPA.PimProductId = @PimProductId)  
    UNION    
    SELECT PimAttributeId FROM ZnodePimAttributeValue ZPAV WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.PimAttributeId = ZPAV.PimAttributeId AND ZPA.IsPersonalizable = 1 )   
    AND ZPAV.PimProductId = @PimProductId AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute ZPCPA WHERE ZPCPA.PimAttributeId = ZPAV.PimAttributeId AND ZPCPA.PimProductId = @PimProductId)  
  
  
     INSERT INTO @TBL_PImAttribute (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable  
     ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName ,AttributeTypeName )  
     EXEC [dbo].[Znode_GetPimAttributesDetailsforPimProduct] @PimAttributeId,@LocaleId  
  
     INSERT INTO @PimProductId_new   
              SELECT @PimProductId  

     INSERT INTO #TBL_AttributeValueDetail (PimProductId,AttributeValue,AttributeCode,PimAttributeId,AttributeDefaultValue,FilesName)  
     EXEC [dbo].[Znode_GetProductsAttributeValue_newTesting]  @PimProductId_new , @PimAttributeId ,@LocaleId  ,0,@IsFilesNameRequired
  
     INSERT INTO @TBL_PimAttributeDefault (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId,IsDefault)  
     EXEC [dbo].[Znode_GetAttributeDefaultValueLocaleNew_TansferId] @PimAttributeId,@LocaleId  
  
  
     INSERT INTO @TBL_AttributeFamily (PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)  
     EXEC Znode_GetFamilyValueLocale  @PimAttributeFamilyId, @LocaleId   
   
    --  update the media path   
     SELECT TBA.PimProductId , TBA.PimAttributeId , SUBSTRING( ( SELECT ','+AttributeValue  
      FROM  #TBL_AttributeValueDetail AS TBAI  
      WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId 
	  ORDER BY TBAI.Id ASC  
      FOR XML PATH('')
	  
	  , TYPE).value('.', 'varchar(Max)') , 2 , 4000) As AttributeValue , 
	  SUBSTRING( (SELECT ','+CAST(MediaID AS VARCHAR(200))   
     FROM  ZnodePimAttributeValue tr    
     INNER JOIN ZnodePimProductAttributeMedia CTPM ON CTPM.PimAttributeValueId  = tr.PimAttributeValueId   
     WHERE tr.PimAttributeId = TBA.PimAttributeId AND tr.PimProductId = TBA.PimProductId   
     AND CTPM.LocaleId =@LocaleId 
	 ORDER BY CTPM.PimProductAttributeMediaId ASC
	 FOR XML PATH('')
	 
	  ) ,2, 4000) MediaIds,
	  SUBSTRING( ( SELECT ','+FilesName  
      FROM  #TBL_AttributeValueDetail AS TBAI  
      WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId 
	  ORDER BY TBAI.AttributeValue ASC  
      FOR XML PATH('')
	  
	  , TYPE).value('.', 'varchar(Max)') , 2 , 4000) As FilesName
	  into #Cte_ProductMedia    
      FROM #TBL_AttributeValueDetail AS TBA   
      INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId )
          
     UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue+'~'+CTPM.MediaIds,
	 FilesName=CTPM.FilesName
     FROM #TBL_AttributeValueDetail TBAV   
     INNER JOIN #Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId   
     AND CTPM.PimAttributeId = TBAV.PimAttributeId;   
    
    -- IF IsCopy Is True then Unique value are blank   
      ;With Cte_UniqueAttributeId AS    
      (  
      SELECT c.PimAttributeId   
      FROM ZnodePimAttributeValidation AS c   
      INNER JOIN ZnodeAttributeInputValidation AS d ON (c.InputValidationId = d.InputValidationId)  
      WHERE d.Name = 'UniqueValue' AND c.Name = 'true' AND @Iscopy = 1 GROUP BY c.PimAttributeId  
      )  
   
      UPDATE TBAV SET AttributeValue = '' FROM #TBL_AttributeValueDetail  TBAV INNER JOIN Cte_UniqueAttributeId CTUA ON (CTUA.PimAttributeId = TBAV.PimAttributeId)  
  
      INSERT INTO #TBL_AttributeValueFinale (PimProductId  , AttributeValue ,AttributeCode ,PimAttributeId,AttributeDefaultValueCode,FilesName)  
  
      SELECT DISTINCT  PimProductId  ,AttributeValue ,AttributeCode ,PimAttributeId,AttributeDefaultValue,FilesName  
      FROM #TBL_AttributeValueDetail TBLA   
  
     UPDATE #TBL_AttributeValueFinale  
     SET AttributeValue = AttributeDefaultValueCode  
     FROM #TBL_AttributeValueFinale AVF  
     WHERE EXISTS (SELECT TOP 1 1 FROM   
     Fn_GetDefaultAttributeId() GDA WHERE AVF.AttributeCode = GDA.AttributeCode)  
  
  
      SELECT TBAF.PimAttributeFamilyId,FamilyCode,TBPA.PimAttributeId,PimAttributeGroupId,TBPA.AttributeTypeId,
	  AttributeTypeName,TBPA.AttributeCode,  
      IsRequired,IsLocalizable,IsFilterable,AttributeName, TBAV.AttributeValue  ,PimAttributeValueId,TBADV.PimAttributeDefaultValueId,  
      TBADV.AttributeDefaultValueCode ,AttributeDefaultValue AS AttributeDefaultValue,ISNULL(NULL, 0) AS RowId,ISNULL(IsEditable, 1) AS IsEditable,  
      ZAIV.ControlName,ZAIV.Name AS ValidationName,ZAIVR.ValidationName AS SubValidationName,ZAIVR.RegExp,ZPAV.Name AS ValidationValue,  
      CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp,HelpDescription ,
	  TBAV.FilesName, TBADV.IsDefault
    FROM @TBL_PimAttribute  TBPA   
    LEFT JOIN #TBL_AttributeValueFinale  TBAV ON (TBAV.PimAttributeId = TBPA.PimAttributeId)  
    LEFT JOIN @TBL_PimAttributeDefault TBADV ON (TBADV.PimAttributeId = TBPA.PimAttributeId)  
    LEFT JOIN ZnodePimAttributeValidation AS ZPAV ON(ZPAV.PimAttributeId = TBPA.PimAttributeId)  
    LEFT JOIN ZnodeAttributeInputValidation AS ZAIV ON(ZPAV.InputValidationId = ZAIV.InputValidationId)  
    LEFT JOIN ZnodeAttributeInputValidationRule AS ZAIVR ON(ZPAV.InputValidationRuleId = ZAIVR.InputValidationRuleId)  
    LEFT JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = TBAV.PimProductId AND ZPV.PimAttributeId = TBAV.PimAttributeId)  
    LEFT JOIN @TBL_AttributeFamily TBAF ON (TBAF.PimAttributeFamilyId = @PimAttributeFamilyId)  
    LEFT JOIN ZnodePimFamilyGroupMapper ZPFG ON (ZPFG.PimAttributeFamilyId = TBAF.PimAttributeFamilyId AND ZPFG.PimAttributeId = TBPA.PimAttributeId)  
    WHERE TBPA.AttributeCode <> 'PublishStatus'  
    ORDER BY TBPA.DisplayOrder,TBPA.PimAttributeId, TBADV.DisplayOrder  
     
   COMMIT TRAN PimProductAttributeValues;  
         END TRY  
         BEGIN CATCH  
   SELECT ERROR_MESSAGE()  
             DECLARE @Status BIT ;  
    SET @Status = 0;  
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),  
     @ErrorLine VARCHAR(100)= ERROR_LINE(),  
      @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimProductAttributeValues @ChangeFamilyId='+cast (@ChangeFamilyId AS VARCHAR(50))  
      +',@PimProductId = '+cast (@PimProductId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@IsCopy='+CAST(@IsCopy AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
    ROLLBACK TRAN PimProductAttributeValues;  
  
          EXEC Znode_InsertProcedureErrorLog  
            @ProcedureName = 'Znode_GetPimProductAttributeValues',  
            @ErrorInProcedure = @Error_procedure,  
            @ErrorMessage = @ErrorMessage,  
            @ErrorLine = @ErrorLine,  
            @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;