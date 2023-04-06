

CREATE view [dbo].[View_ReturnBoolean]
AS 
SELECT isnull (null ,0) as Id , CAST(1 AS BIT ) Status