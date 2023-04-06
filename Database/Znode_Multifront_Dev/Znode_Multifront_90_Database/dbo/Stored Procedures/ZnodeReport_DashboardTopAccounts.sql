CREATE PROCEDURE [dbo].[ZnodeReport_DashboardTopAccounts]              
(            
	@PortalId  bigint  = null,        
	@AccountId bigint  = null,    
	@SalesRepUserId int = 0                
)              
AS              
/*              
     Summary:- This procedure is used to get the order details              
    Unit Testing:              
     EXEC [ZnodeReport_DashboardTopAccounts]  @PortalId=7, @AccountId=0 ,@SalesRepUserId=0         
*/              
BEGIN              
BEGIN TRY              
    SET NOCOUNT ON;    
	----Verifying that the @SalesRepUserId is having 'Sales Rep' role
	IF NOT EXISTS
	(
	SELECT * FROM ZnodeUser ZU
	INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
	INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
	INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
	Where Exists(select * from AspNetRoles ANR Where Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id)
	AND ZU.UserId = @SalesRepUserId
	)  
	Begin
	SET @SalesRepUserId = 0
	End            
 
	DECLARE @TopItem TABLE (ItemName nvarchar(100),CustomerName nvarchar(200),ItemId nvarchar(10),ItemCode nvarchar(100), Total numeric(28,6),ItemDate datetime,Symbol NVARCHAR(10))            
             
    INSERT INTO @TopItem(ItemId, ItemName,CustomerName,ItemCode,ItemDate,Total,Symbol)          
       
	select ZA.AccountId,ZA.[Name], isnull(ZU.FirstNAme,'')+' '+isnull(ZU.LastName,'') as UserName,ZA.AccountCode, ZU.CreatedDate,0,''        
	from ZnodeAccount ZA      
	left join ZnodeUser ZU on ZU.AccountId = ZA.AccountId      
	left join  ZnodePortalAccount ZUP on ZUP.AccountId = ZA.AccountId          
	WHERE (ZUP.PortalId = @PortalId OR ISNULL(@PortalId,0) = 0) AND (ZA.AccountId = @AccountId OR  ISNULL(@AccountId,0) = 0)            
	and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)        
   
   SELECT TOP 5 ItemId,ItemCode, ItemName,CustomerName,ItemDate,Total,Symbol FROM @TopItem Order by  Convert(numeric,Total )  desc                
   
END TRY              
             
BEGIN CATCH              
	DECLARE @Status BIT ;              
	SET @Status = 0;              
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),              
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardTopAccount @PortalId = '+@PortalId;            
                               
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                  
                 
	EXEC Znode_InsertProcedureErrorLog              
	@ProcedureName = 'ZnodeReport_DashboardTopAccount',              
	@ErrorInProcedure = @Error_procedure,              
	@ErrorMessage = @ErrorMessage,              
	@ErrorLine = @ErrorLine,              
	@ErrorCall = @ErrorCall;              
END CATCH              
             
END;

