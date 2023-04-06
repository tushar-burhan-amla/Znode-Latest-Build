-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetAttributeGroupDisplayOrder]
(
  @PimAttributeFamilyId INT,
  @PimAttributeGroupId int 
			   
)

RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @GroupDisplayOrder INT;
         
		 SET @GroupDisplayOrder = ( SELECT TOP 1 GroupDisplayOrder FROM ZnodePimFamilyGroupMapper WHERE PimAttributeFamilyId = @PimAttributeFamilyId AND PimAttributeGroupId = @PimAttributeGroupId )

                   
         RETURN @GroupDisplayOrder;
     END;