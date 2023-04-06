
CREATE  view [dbo].[View_ReturnBooleanWithMessage]
AS 
SELECT isnull (null ,0) as Id,CAST('Message' AS NVARCHAR(MAX))  MessageDetails, CAST(1 AS BIT ) Status