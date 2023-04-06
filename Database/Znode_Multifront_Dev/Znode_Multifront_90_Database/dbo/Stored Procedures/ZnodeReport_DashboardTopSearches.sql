CREATE PROCEDURE [dbo].[ZnodeReport_DashboardTopSearches]
(      
@PortalId       VARCHAR(MAX)  = '',
@BeginDate      DATE          = NULL,
@EndDate        DATE          = NULL
)
AS
/*
     Summary:- This procedure is used to get the order details
    Unit Testing:
     EXEC [ZnodeReport_DashboardTopSearches] @PortalId='1'
*/
     BEGIN
BEGIN TRY
       SET NOCOUNT ON;
 
  ----old query
--  SELECT TOP 5  Data1 Searches , Count(*) Times  FROM ZnodeActivityLog where ActivityLogTypeId = 9500
--AND ((EXISTS
--   (
--   SELECT TOP 1 1
--   FROM dbo.split(@PortalId, ',') SP
--   WHERE CAST(PortalId AS VARCHAR(100)) = SP.Item
-- OR @PortalId = ''
--   ))
--  )
--AND (CAST(ActivityCreateDate AS DATE) BETWEEN CASE
-- WHEN @BeginDate IS NULL
-- THEN CAST(ActivityCreateDate AS DATE)
-- ELSE @BeginDate
-- END AND CASE
-- WHEN @EndDate IS NULL
-- THEN CAST(ActivityCreateDate AS DATE)
-- ELSE @EndDate
-- END)
--Group by Data1 Order by Count(*)  desc


----new query without date filter
select TOP 5 ZSA.SearchKeyword Searches, COUNT(1) AS  Times
from ZnodeSearchActivity ZSA
WHERE ZSA.ResultCount > 0
AND  ((EXISTS
  (
  SELECT TOP 1 1
  FROM dbo.split(@PortalId, ',') SP
  WHERE CAST(ZSA.PortalId AS VARCHAR(100)) = SP.Item
OR @PortalId = ''
  ))
 )
--AND (CAST(ZSA.CreatedDate AS DATE) BETWEEN CASE
-- WHEN @BeginDate IS NULL
-- THEN CAST(ZSA.CreatedDate AS DATE)
-- ELSE @BeginDate
-- END AND CASE
-- WHEN ZSA.CreatedDate IS NULL
-- THEN CAST(ZSA.CreatedDate AS DATE)
-- ELSE @EndDate
-- END)
Group by ZSA.SearchKeyword Order by COUNT(1)  desc


END TRY

BEGIN CATCH
DECLARE @Status BIT ;
    SET @Status = 0;
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardTopSearches @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
             
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    

             EXEC Znode_InsertProcedureErrorLog
@ProcedureName = 'ZnodeReport_DashboardTopSearches',
@ErrorInProcedure = @Error_procedure,
@ErrorMessage = @ErrorMessage,
@ErrorLine = @ErrorLine,
@ErrorCall = @ErrorCall;
END CATCH
     END;