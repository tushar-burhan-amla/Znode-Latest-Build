-- =============================================
-- Create the Order by clase for dyanamic statement  
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetDefaultConvertedDate]
(
  @ColumName Nvarchar(1000)
  
)
RETURNS NVARCHAR(MAX)
AS
-- Summary :- This procedure is used to find the default value for date column to be modified 
BEGIN
	-- Declare the return variable here
	DECLARE @DotAlies VARCHAR(100) =''
	
	IF @ColumName LIKE '%.%'
	BEGIN 
	 

	 SET @DotAlies = SUBSTRING (@ColumName,1,CHARINDEX('.',@ColumName))

	END 



	DECLARE @DateColumn NVARCHAR(1000)= ' CONVERT(VARCHAR, '+@ColumName+','+(
    SELECT FeatureSubValues  FROM ZnodeGlobalSetting WHERE FeatureName = 'DateFormat' )+') AS '+REPLACE(@ColumName,@DotAlies,'')
	

    RETURN @DateColumn
 END