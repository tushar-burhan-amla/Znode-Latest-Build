-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetDefaultPimProductFamilyId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultPimProductFamilyId INT;
         
		 SET @DefaultPimProductFamilyId = ( SELECT TOP 1 PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined = 1 AND IsCategory = 0  AND IsDefaultFamily = 1  )

                   
         RETURN @DefaultPimProductFamilyId;
     END;