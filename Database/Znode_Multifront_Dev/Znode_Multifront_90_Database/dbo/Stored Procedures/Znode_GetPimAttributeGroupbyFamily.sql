
Create PROCEDURE [dbo].[Znode_GetPimAttributeGroupbyFamily]
( @PimAttributeFamilyID INT = 0,
  @IsCategory           BIT = 0,
  @LocaleId             INT = 0)
AS
/*
Summary: This procedure is used to get PimAttributeGroup order by GroupDisplayOrder
         Table variable @TBL_PimAttributeGroupbyFamily_locale consist of records related to GroupType 'Group'
		 Table variable @TBL_PimAttributeGroupbyFamily consist of records related to GroupType 'Link'
Unit Testing:
BEGIN TRAN
EXEC Znode_GetPimAttributeGroupbyFamily_New1 1,0,0
ROLLBACK TRAN

*/
     BEGIN
	    
         BEGIN TRY
			  BEGIN TRAN PimAttributeGroupbyFamily	
             SET NOCOUNT ON;
			
				 DECLARE @DefaultLocaleId INT;
				 SELECT @DefaultLocaleId = dbo.Fn_GetDefaultLocaleId();

				 IF @PimAttributeFamilyID = 0
					 BEGIN
						 SELECT @PimAttributeFamilyID = PimAttributeFamilyId FROM ZnodePimAttributeFamily  WHERE IsDefaultFamily = 1 AND IsCategory = @IsCategory;						 
					 END;

				 DECLARE @TBL_PimAttributeGroupbyFamily TABLE
				 (PimAttributeFamilyId INT,
				  GroupCode            VARCHAR(200),
				  AttributeGroupName   NVARCHAR(600),
				  PimAttributeGroupId  INT,
				  GroupDisplayOrder    INT,
				  GroupType            VARCHAR(100)
				 );

				 DECLARE @TBL_PimAttributeGroupbyFamily_locale TABLE
				 (PimAttributeFamilyId INT,
				  GroupCode            VARCHAR(200),
				  AttributeGroupName   NVARCHAR(600),
				  PimAttributeGroupId  INT,
				  GroupDisplayOrder    INT,
				  GroupType            VARCHAR(100),
				  LocaleId             INT
				 );
				 --ZPFGM.GroupDisplayOrder
				 INSERT INTO @TBL_PimAttributeGroupbyFamily_locale
				 SELECT DISTINCT ZPAF.PimAttributeFamilyId,GroupCode,AttributeGroupName,ZPAGL.PimAttributeGroupId, dbo.Fn_GetAttributeGroupDisplayOrder(ZPFGM.PimAttributeFamilyId,ZPFGM.PimAttributeGroupId) ,'Group',ZPAGL.LocaleId
				 FROM dbo.ZnodePimAttributeFamily AS ZPAF
				 INNER JOIN dbo.ZnodePimFamilyGroupMapper AS ZPFGM ON(ZPAF.PimAttributeFamilyId = ZPFGM.PimAttributeFamilyId)
				 INNER JOIN ZnodePimAttributeGroup AS ZPAG ON(ZPFGM.PimAttributeGroupId = ZPAG.PimAttributeGroupId)
				 INNER JOIN ZnodePimAttributeGroupLocale AS ZPAGL ON(ZPAG.PimAttributeGroupId = ZPAGL.PimAttributeGroupId
				 AND ZPAGL.LocaleId IN(@LocaleId, @DefaultLocaleId))
				 WHERE(ZPAF.PimAttributeFamilyId = @PimAttributeFamilyId)
				 AND ZPAF.IsCategory = @IsCategory
				 and   ZPAGL.PimAttributeGroupId  not in
						(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode in ( 'ProductSetting','ShippingSettings') and @IsCategory = 1)
						;
				  

				 INSERT INTO @TBL_PimAttributeGroupbyFamily
				 SELECT PimAttributeFamilyId,GroupCode,AttributeGroupName,PimAttributeGroupId,GroupDisplayOrder,GroupType
				 FROM @TBL_PimAttributeGroupbyFamily_locale AS a
				 WHERE LocaleId = @LocaleId
				 UNION ALL
				 SELECT 0,Zpa.AttributeCode,Zpl.AttributeName,Zpa.PimAttributeId,NULL,'Link'
				 FROM ZnodePimAttribute AS Zpa
				 INNER JOIN ZnodePimAttributeLocale AS Zpl ON(Zpa.PimAttributeId = Zpl.PimAttributeId
				 AND zpa.IsCategory = @IsCategory
				 AND Zpl.LocaleId = @LocaleId)
				 WHERE AttributeTypeId =
				 (
					SELECT TOP 1 AttributeTypeId
					FROM ZnodeAttributeType
					WHERE AttributeTypeName = 'Link'
				 );
			
				 INSERT INTO @TBL_PimAttributeGroupbyFamily
				 SELECT PimAttributeFamilyId,GroupCode,AttributeGroupName,PimAttributeGroupId,GroupDisplayOrder,GroupType
				 FROM @TBL_PimAttributeGroupbyFamily_locale AS a
				 WHERE LocaleId = @DefaultLocaleId
				 AND NOT EXISTS
				 (
						SELECT TOP 1 1 FROM @TBL_PimAttributeGroupbyFamily AS sse WHERE sse.PimAttributeGroupId = a.PimAttributeGroupId AND sse.PimAttributeFamilyId = a.PimAttributeFamilyId																	
				 )
						UNION ALL
						SELECT 0,Zpa.AttributeCode,Zpl.AttributeName,Zpa.PimAttributeId,NULL,'Link' FROM ZnodePimAttribute AS Zpa
						INNER JOIN ZnodePimAttributeLocale AS Zpl ON(Zpa.PimAttributeId = Zpl.PimAttributeId  AND zpa.IsCategory = @IsCategory AND Zpl.LocaleId = @DefaultLocaleId)																		 																		  
						WHERE zpa.AttributeTypeId =
						(
							SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Link'														
						)
							AND NOT EXISTS
						(
							SELECT TOP 1 1 FROM @TBL_PimAttributeGroupbyFamily AS sse WHERE sse.PimAttributeGroupId = zpa.PimAttributeId AND sse.PimAttributeFamilyId = 0
						);
					 
				 SELECT PimAttributeFamilyId,GroupCode,AttributeGroupName,PimAttributeGroupId,GroupType FROM @TBL_PimAttributeGroupbyFamily				 
				 ORDER BY CASE WHEN GroupDisplayOrder IS NULL THEN 1 ELSE 0  END, GroupDisplayOrder;	
					  											  
		 COMMIT TRAN PimAttributeGroupbyFamily;			   
         END TRY
         BEGIN CATCH
		    DECLARE @STATUS BIT
            DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributeGroupbyFamily @PimAttributeFamilyID = '+CAST(@PimAttributeFamilyID AS VARCHAR(200))+',@IsCategory= '+CAST(@IsCategory AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));           
			SET @Status = 0;
            SELECT 0 AS ID, CAST(0 AS BIT) AS Status;                  
 			ROLLBACK TRAN PimAttributeGroupbyFamily;				
            EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName    = 'Znode_GetPimAttributeGroupbyFamily',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage     = @ErrorMessage,
                  @ErrorLine        = @ErrorLine,
                  @ErrorCall        = @ErrorCall;   
				                 
         END CATCH;
     END;
