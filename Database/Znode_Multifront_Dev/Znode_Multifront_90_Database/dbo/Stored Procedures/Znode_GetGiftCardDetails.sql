CREATE PROCEDURE [dbo].[Znode_GetGiftCardDetails]
(
	@VoucherCodes Varchar(max),
	@OmsOrderDetailsId   int
)
AS
BEGIN  
    BEGIN TRY  
        DECLARE @IsApplied as bit = 1;
        SET NOCOUNT ON;
        ;With Cte_GiftCart as
        (
            Select Gift.GiftCardId,GiftHistory.OmsOrderDetailsId,Gift.PortalId,Gift.Name as VoucherName,Gift.CardNumber as VoucherNumber,GiftHistory.TransactionAmount as VoucherAmountUsed
            , @IsApplied as IsVoucherApplied, ISNULL(Gift.RemainingAmount,0) as VoucherBalance, Gift.IsActive as IsActive,Gift.ExpirationDate, 
            ISNULL(Gift.RemainingAmount,0) as RemainingAmount, ZOD.OmsOrderId
            , ROW_NUMBER() OVER (ORDER BY ZOD.OmsOrderId) As Rn
            from ZnodeGiftCard Gift
            INNER JOIN ZnodeGiftCardHistory GiftHistory ON Gift.GiftCardId = GiftHistory.GiftCardId
            LEFT JOIN ZnodeOmsOrderDetails ZOD on (GiftHistory.OmsOrderDetailsId = ZOD.OmsOrderDetailsId AND ZOD.IsActive = 1)
            LEFT JOIN ZnodeOmsOrder ZO on (ZOD.OmsOrderId = ZO.OmsOrderId)
            Where Gift.CardNumber in (select ltrim(rtrim(Item)) From dbo.Split(@VoucherCodes,',')) AND GiftHistory.OmsOrderDetailsId = @OmsOrderDetailsId
        )
        ,Cte_GiftCart1 as
        (
           SELECT ZOD.OmsOrderId,GiftHistory.GiftCardId,ISNULL(Gift.RemainingAmount,0) as RemainingAmount, min(GiftHistory.OmsOrderDetailsId) as OmsOrderDetailsId
           FROM ZnodeGiftCardHistory GiftHistory
           inner join ZnodeGiftCard Gift ON GiftHistory.GiftCardId = Gift.GiftCardId
           LEFT JOIN ZnodeOmsOrderDetails ZOD on (GiftHistory.OmsOrderDetailsId = ZOD.OmsOrderDetailsId AND ZOD.IsActive = 1)
           LEFT JOIN ZnodeOmsOrder ZO on (ZOD.OmsOrderId = ZO.OmsOrderId)  
           WHERE EXISTS(select * from  Cte_GiftCart Gift where GiftHistory.GiftCardId = Gift.GiftCardId)
           GROUP BY ZOD.OmsOrderId,GiftHistory.GiftCardId,Gift.RemainingAmount
        )
        ,CTE_OrderVoucherAmount AS
       (
           SELECT ISNULL(cte.OmsOrderId,0) AS OmsOrderId,cte.OmsOrderDetailsId,Hist.GiftCardId, 
           CASE WHEN cte.OmsOrderId IS NOT NULL THEN Hist.TransactionAmount ELSE Cte.RemainingAmount END as OrderVoucherAmount, ROW_NUMBER() OVER (ORDER BY cte.OmsOrderId) As Rn
           FROM ZnodeGiftCardHistory Hist
           LEFT join Cte_GiftCart1 Cte on Hist.GiftCardId = Cte.GiftCardId and Hist.OmsOrderDetailsId = Cte.OmsOrderDetailsId
           WHERE EXISTS(select * from  Cte_GiftCart Gift where Hist.GiftCardId = Gift.GiftCardId AND Hist.OmsOrderDetailsId = Gift.OmsOrderDetailsId)
           AND CASE WHEN cte.OmsOrderId IS NOT NULL THEN Hist.TransactionAmount ELSE Cte.RemainingAmount END IS NOT NULL
        )
        Select a.GiftCardId, PortalId,VoucherName,VoucherNumber,VoucherAmountUsed,
          IsVoucherApplied, VoucherBalance, IsActive,ExpirationDate, RemainingAmount, OrderVoucherAmount
        from Cte_GiftCart a
        inner join CTE_OrderVoucherAmount b on a.GiftCardId = b.GiftCardId  AND isnull(a.OmsOrderId,0) = isnull(b.OmsOrderId,0) AND a.Rn=b.Rn
        Order By a.ExpirationDate, a.RemainingAmount

    END TRY  
    BEGIN CATCH  
   
        DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
        @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGiftCardDetails @VoucherCodes = '+CAST(@VoucherCodes AS VARCHAR(MAX))+',@OmsOrderDetailsId='+CAST(@OmsOrderDetailsId AS VARCHAR(50))                  
        EXEC Znode_InsertProcedureErrorLog  
        @ProcedureName = 'Znode_GetGiftCardDetails',  
        @ErrorInProcedure = @Error_procedure,  
        @ErrorMessage = @ErrorMessage,  
        @ErrorLine = @ErrorLine,  
        @ErrorCall = @ErrorCall;  
    END CATCH  
END