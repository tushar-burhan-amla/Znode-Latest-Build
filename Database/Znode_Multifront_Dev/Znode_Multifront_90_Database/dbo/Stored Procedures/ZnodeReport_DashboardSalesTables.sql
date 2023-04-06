CREATE  PROCEDURE [dbo].[ZnodeReport_DashboardSalesTables]    
(   @BeginDate      DATE          = NULL,    
 @EndDate        DATE          = NULL,    
 @PortalId       VARCHAR(MAX)  = ''    
)    
AS     
  /* Summary:- This procedure is used to get the order details     
    Unit Testing:    
     EXEC ZnodeReport_DashboardSalesTables    
 */    
     BEGIN    
  BEGIN TRY    
         SET NOCOUNT ON;    
   Declare @Tbl_UserData TABLE (UserId int , RoleName  nvarchar(100),FullName nvarchar(300),Email nvarchar(300), UserName nvarchar(300))    
  
   DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))    
   INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)    
   SELECT Symbol,CultureCode from  ZnodeCulture ZC         -- Changed table name from ZnodeCurrency to ZnodeCulture  
   -- LEFT JOIN ZnodeCulture ZCC ON (ZC.CurrencyId = ZCC.CurrencyId)   
          
   Insert into @Tbl_UserData (UserId, RoleName,FullName, Email, UserName )    
   select zu.UserId,znr.Name, ISnull(zu.FirstName,'') + ' ' + Isnull(MiddleName,'') + ' ' + Isnull(LastName,'') FullName, zu.Email,  anzu.UserName from AspNetUsers  anu inner join AspNetZnodeUser anzu on anu.UserName = anzu.AspNetZnodeUserId    
   inner join ZnodeUser zu on anu.id = zu.AspNetUserId    
   inner join AspNetUserRoles anur on anur.UserId = zu.AspNetUserId    
   inner join AspNetRoles znr on anur.RoleId = znr.Id    
    
   SELECT distinct ZOOD.OmsOrderDetailsId, P.StoreName,ZU.*,    
                Convert(Date, ZOOD.OrderDate) OrderDate,     
    ZOOD.Total  
 ,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol    
 --,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol    
         FROM ZnodeOmsOrderDetails ZOOD INNER JOIN ZNodePortal P ON P.PortalID = ZOOD.PortalId    
              LEfT  JOIN @Tbl_UserData  ZU ON ZOOD.UserId = ZU.UserId    
                        LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )      
         WHERE ZOOD.IsActive =1 AND     
      ((EXISTS    
               (    
                   SELECT TOP 1 1    
                   FROM dbo.split(@PortalId, ',') SP    
                   WHERE CAST(P.PortalId AS VARCHAR(100)) = SP.Item    
                         OR @PortalId = ''    
               ))    
    )    
   AND (CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE    
            WHEN @BeginDate IS NULL    
            THEN CAST(ZOOD.OrderDate AS DATE)    
            ELSE @BeginDate    
            END AND CASE    
                WHEN @EndDate IS NULL    
                THEN CAST(ZOOD.OrderDate AS DATE)    
                ELSE @EndDate    
                END)    
  END TRY    
  BEGIN CATCH    
  DECLARE @Status BIT ;    
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),    
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardSalesTables @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
             EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'ZnodeReport_DashboardSalesTables',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;    
  END CATCH    
      
  END;