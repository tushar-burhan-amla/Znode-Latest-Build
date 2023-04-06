-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetDefaultValue](
               @FilterBy NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultValue NVARCHAR(MAX)= '';
         IF @FilterBy = 'PimFamily'
             BEGIN
                 SELECT @DefaultValue = CAST(ZPAF.PimAttributeFamilyId AS VARCHAR(100))
                 FROM ZnodePimAttributeFamily AS ZPAF
                 WHERE ZPAF.IsCategory = 0
                       AND
                       ZPAF.IsDefaultFamily = 1;
             END;
         ELSE IF @FilterBy = 'CategoryFamily'
                     BEGIN
                         SELECT @DefaultValue = CAST(ZPAF.PimAttributeFamilyId AS VARCHAR(100))
                         FROM ZnodePimAttributeFamily AS ZPAF
                         WHERE ZPAF.IsCategory = 1
                               AND
                               ZPAF.IsDefaultFamily = 1;
                     END;
         ELSE IF @FilterBy = 'MediaFamily'
                             BEGIN
                                 SELECT @DefaultValue = CAST(ZMAF.MediaAttributeFamilyId AS VARCHAR(100))
                                 FROM ZnodeMediaAttributeFamily AS ZMAF
                                 WHERE ZMAF.IsDefaultFamily = 1;
                             END;
         ELSE IF @FilterBy = 'AttributeCode'
		     BEGIN 
			     
							SET @DefaultValue= 'ProductName,SKU,Price,Quantity,IsActive,ProductType,ProductImage,Assortment,DisplayOrder,OutOfStockOptions'

			 END 
		 ELSE 
         BEGIN
                                 SELECT @DefaultValue = ZGS.FeatureValues
                                 FROM ZnodeGlobalSetting AS ZGS
                                 WHERE ZGS.FeatureName = @FilterBy;
          END;
                   
         RETURN @DefaultValue;
     END;