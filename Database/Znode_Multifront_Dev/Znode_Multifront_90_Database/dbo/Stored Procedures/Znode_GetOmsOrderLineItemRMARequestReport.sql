
CREATE PROCEDURE [dbo].[Znode_GetOmsOrderLineItemRMARequestReport]
( @RMARequestId INT = NULL)

AS 
/*
 Summary :This Procedure is Used to get the User RMA request Details filtered by RmaRequestId
     Unit Testing
     EXEC  [dbo].[Znode_GetOmsOrderLineItemRMARequestReport] 21

*/   
     BEGIN
	     SET NOCOUNT ON
		 BEGIN TRY
         DECLARE @RmaConfigurationId INT=
         (
             SELECT TOP 1 RmaConfigurationId
             FROM ZNodeRMAConfiguration
         );
			
         SELECT ZOOLI.OmsOrderDetailsId,ZOOD.BillingFirstName+' '+ZOOD.BillingLastName CustomerName,ZOOD.BillingEmailId,ZOOLI.[ProductName],ZOOLI.[Description],ZRRI.[Quantity],ZOOLI.[Price] - ZOOLI.[Discountamount] Price,ZRRI.[Quantity] * (ZOOLI.[Price] - ZOOLI.[Discountamount]) Total,[SKU],ZRC.DisplayName DepartmentName,ZRC.Email DepartmentEmail,ZRC.[Address] DepartmentAddress,ZRS.CustomerNotification,zrc.IsEmailNotification				
         FROM [ZnodeOmsOrderLineItems] ZOOLI
              LEFT JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId)
              LEFT JOIN ZnodeRmaRequestItem ZRRI ON(ZOOLI.OmsOrderLineItemsId = ZRRI.OmsOrderLineItemsId)
              LEFT JOIN ZnodeRmaRequest ZRR ON(ZRR.RmaRequestId = ZRRI.RmaRequestId)
              LEFT JOIN ZnodeRmaConfiguration ZRC ON(ZRC.RmaConfigurationId = @RmaConfigurationId)
              LEFT JOIN ZnodeRmaRequestStatus ZRS ON(ZRS.RmaRequestStatusId = ZRR.RmaRequestStatusId)
         WHERE ZRRI.RmaRequestId = @RMARequestId;   
		 END TRY 
		 BEGIN CATCH     
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsOrderLineItemRMARequestReport @RMARequestId = '+CAST(@RMARequestId AS VARCHAR(max))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetOmsOrderLineItemRMARequestReport',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END;