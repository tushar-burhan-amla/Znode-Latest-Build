
CREATE PROCEDURE [dbo].[Znode_UpdateQuoteStatus]
(   
	@OmsQuoteId      VARCHAR(2000),
    @OmsOrderStateId INT           = NULL,
    @Status          INT           = 0 OUT, -- 1 for  sucessfull delete and update 
	@ExceptUpdateStatus VARCHAR(max) = '', -- in this status Quote are not updated 
	@ModifiedBy INT = 0 , 
	@IsAdminUser BIT = 0 
)
AS
  /* 
	Summary :- This Procedure is used to update the Quote status 
				returns 0 or 1 as result depends upon the status
	Unit Testing 
	begin tran
	DECLARE @OutIds INT = 0 
	EXEC Znode_UpdateQuoteStatus '306,281,263',80,@OutIds OUT,'Ordered,Draft'
	SELECT @OutIds
	rollback tran
	SELECT * FROM [ZNodeUserQuoteOrderLineItem]  WHERE omsQuoteId IN (306,281,263)
	SELECT * FROM ZnodeOmsOrderState
   */
BEGIN 
	BEGIN TRY
	SET NOCOUNT ON;
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
		DECLARE @TBL_NotUpdateStatus TABLE (value  VARCHAR(max))
		DECLARE @RejectOmsOrderStateId INT = (SELECT TOP 1 OmsOrderStateId FROM ZnodeOmsOrderState WHERE OrderStateName = 'REJECTED')
		DECLARE @ApprovedOmsOrderStateId INT = (SELECT TOP 1 OmsOrderStateId FROM ZnodeOmsOrderState WHERE OrderStateName = 'APPROVED')
		DECLARE @TBL_UpdateApprovalState TABLE (UserId INT ,ApprovalLevelId INT ,ApproverUserId INT, ApproverOrder INT   )
		DECLARE @TBL_OrderState TABLE
        (FirstName  VARCHAR(100),
        LastName   VARCHAR(100),
        Email      VARCHAR(50),
        [Status]   NVARCHAR(MAX),
        OmsQuoteId INT,
        UserId     INT,
        PortalId   INT,
		OmsOrderStateId INT
        );
             
		IF @ExceptUpdateStatus = '' 
		BEGIN 
			INSERT INTO @TBL_NotUpdateStatus 
			SELECT value 
			FROM dbo.[Fn_GetProcedureAttributeDefault]('OrderState') FNGP
		END 
		ELSE 
		BEGIN 
			INSERT INTO @TBL_NotUpdateStatus 
			SELECT item 
			FROM dbo.split(@ExceptUpdateStatus,',') FNGP
		END 

		IF EXISTS (SELECT TOP 1 1  FROM  ZnodeUser ZU 
							INNER JOIN AspNetUsers AU ON (AU.Id = ZU.AspNetUserId)
							INNER JOIN AspNetUserRoles RTY ON (RTY.UserId = AU.Id)
							INNER JOIN AspNetRoles TU ON (TU.Id = RTY.RoleId)
							WHERE ZU.UserId = @ModifiedBy  AND (ISNULL(TU.TypeOfRole,'') <> 'B2B' AND   TU.Name <> 'Customer'))
	BEGIN 
		SET @IsAdminUser = 1 
	END 


	DECLARE @OmsQuoteIds TABLE (OmsQuoteId INT )
	INSERT INTO  @OmsQuoteIds
	SELECT Item
    FROM dbo.Split(@OmsQuoteId, ',') SP

			 	 
		INSERT INTO @TBL_OrderState
        (FirstName,
        LastName,
        Email,
        [Status],
        OmsQuoteId,
        UserId,
        PortalId,
		OmsOrderStateId
        )
            SELECT ZU.FirstName,
                    Zu.LastName,
                    ZU.Email,
                    ZOOS.OrderStateName,
                    ZOQ.OmsQuoteId,
                    ZU.UserId,
                    ZOQ.PortalId,
					ZOOS.OmsOrderStateId
            FROM ZnodeUser ZU
                    INNER JOIN ZnodeOmsQuote ZOQ ON(ZOQ.UserId = ZU.UserId)
                    INNER JOIN ZnodeOmsOrderState ZOOS ON(ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId)
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @OmsQuoteIds SP
                WHERE ZOQ.OmsQuoteId = SP.OmsQuoteId
            );
         

	DECLARE @ApproverUserId TABLE (ApproverUserId INT )
	INSERT INTO @ApproverUserId
	SELECT  ApproverUserId 
	FROM ZnodeUserApprovers 
	WHERE ApproverOrder IN (SELECT ApproverOrder  FROM  ZnodeUserApprovers 
				WHERE ApproverUserId = @ModifiedBy 
				AND USERId  = ( SELECT TOP 1 UserId FROM ZnodeOMSQuoteApproval ZOQ WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @OmsQuoteIds SP
                WHERE ZOQ.OmsQuoteId = SP.OmsQuoteId
            ) )
	) AND USERId  = ( SELECT TOP 1 UserId FROM ZnodeOMSQuoteApproval 
	WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @OmsQuoteIds SP
                WHERE ZnodeOMSQuoteApproval.OmsQuoteId = SP.OmsQuoteId
            ) )
			 
	INSERT INTO @ApproverUserId
	SELECT ApproverUserId 
	FROM ZnodeUserApprovers a 
	INNER JOIN ZnodePortalApproval b ON (b.PortalApprovalId= a.PortalApprovalId)
	WHERE a.IsActive = 1 
	AND b.PortalId = (SELECT TOP 1 PortalId FROM ZnodeOmsQuote asd WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @OmsQuoteIds SP
                WHERE asd.OmsQuoteId = SP.OmsQuoteId
            ) )



	UPDATE ZnodeOMSQuoteApproval
        SET OmsOrderStateId = @OmsOrderStateId
		, ModifiedBy = @ModifiedBy,ModifiedDate = @GetDate
        WHERE EXISTS
        (
            SELECT 1
            FROM dbo.Split(@OmsQuoteId, ',') AS f
            WHERE f.item = ZnodeOMSQuoteApproval.OmsQuoteId
        )
        AND ( (NOT EXISTS 
        (
            SELECT TOP 1 1
            FROM @TBL_OrderState TBOS
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @TBL_NotUpdateStatus FNGP
                WHERE FNGP.Value = TBOS.[Status]
            )
            AND ZnodeOMSQuoteApproval.OmsQuoteId = TBOS.OmsQuoteId
        )
		AND( ApproverUserId = @ModifiedBy  OR ApproverUserId  
		IN (SELECT ApproverUserId FROM @ApproverUserId) OR @IsAdminUser = 1 ))) ; 
			 
		DECLARE @AmountOfQuote NUMERIC(28,8) = (SELECT TOP 1 QuoteOrderTotal FROM ZnodeOmsQuote WHERE OmsQuoteId = @OmsQuoteId  )

	INSERT INTO @TBL_UpdateApprovalState 
	SELECT UserId  ,ApproverLevelId  ,ApproverUserId , ApproverOrder
	FROM ZnodeUserApprovers a 
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOMSQuoteApproval TY 
	WHERE TY.UserId = a.UserId 
	AND TY.ApproverOrder = a.ApproverOrder  
	AND  TY.OmsOrderStateId = @ApprovedOmsOrderStateId
	AND EXISTS
        (
            SELECT 1
            FROM dbo.Split(@OmsQuoteId, ',') AS f
            WHERE f.item = TY.OmsQuoteId
        )
		)
	AND EXISTS (SELECT TOP 1 1 FROM ZnodeOMSQuoteApproval TY 
	WHERE TY.UserId = a.UserId 
	AND EXISTS
        (
            SELECT 1
            FROM dbo.Split(@OmsQuoteId, ',') AS f
            WHERE f.item = TY.OmsQuoteId
        ) )
		AND  a.FromBudgetAmount <= @AmountOfQuote
			
	UPDATE ZnodeOMSQuoteApproval 
	SET IsApprovalRoutingComplete = 1 
		, ModifiedBy = @ModifiedBy,ModifiedDate = @GetDate
        WHERE EXISTS
        (
            SELECT 1
            FROM dbo.Split(@OmsQuoteId, ',') AS f
            WHERE f.item = ZnodeOMSQuoteApproval.OmsQuoteId
        )
        AND (NOT EXISTS (SELECT TOP 1 1  FROM @TBL_UpdateApprovalState) OR @IsAdminUser = 1) 
		AND (@IsAdminUser = 1 OR ZnodeOMSQuoteApproval.ApproverUserId = @ModifiedBy)
		;

	--- SELECT @IsAdminUser 

		UPDATE ZnodeOmsQuote
        SET OmsOrderStateId = @OmsOrderStateId
		,IsConvertedToOrder = CASE WHEN @OmsOrderStateId IN (SELECT OmsOrderStateId FROM ZnodeOmsOrderState WHERE OrderStateName = 'REJECTED') 
								THEN 1  
								WHEN @OmsOrderStateId IN (SELECT OmsOrderStateId FROM ZnodeOmsOrderState WHERE OrderStateName IN ('APPROVED','PENDING APPROVAL')
										AND NOT EXISTS (SELECT * FROM ZnodeOmsOrder WHERE OMSQuoteId=ZnodeOmsQuote.OMSQuoteId)) 
								THEN 0
								ELSE IsConvertedToOrder END 
		, ModifiedBy = @ModifiedBy
		,ModifiedDate = @GetDate
        WHERE EXISTS
        (
            SELECT 1
            FROM dbo.Split(@OmsQuoteId, ',') AS f
            WHERE f.item = ZnodeOmsQuote.OmsQuoteId
        )
        AND ((
		NOT EXISTS
        (
            SELECT TOP 1 1
            FROM @TBL_OrderState TBOS
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @TBL_NotUpdateStatus FNGP
                WHERE FNGP.Value = TBOS.[Status]
            )
            AND ZnodeOmsQuote.OmsQuoteId = TBOS.OmsQuoteId
        )
		AND (EXISTS (SELECT TOP 1  1  FROM ZnodeOMSQuoteApproval TY WHERE TY.OmsQuoteId = ZnodeOmsQuote.OmsQuoteId AND TY.IsApprovalRoutingComplete = 1   )
		OR EXISTS (SELECT TOP 1 1   FROM ZnodeOMSQuoteApproval TY WHERE TY.OmsQuoteId = ZnodeOmsQuote.OmsQuoteId  AND TY.OmsOrderStateId = @RejectOmsOrderStateId)) 
		) OR @IsAdminUser = 1 ) ;
			
		IF NOT EXISTS
        (
            SELECT TOP 1 1
            FROM @TBL_OrderState TBOS
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @TBL_NotUpdateStatus FNGP
                WHERE FNGP.Value = TBOS.[Status]
            )
        )  AND EXISTS (SELECT TOP 1 1 FROM @TBL_OrderState TBOS) 
            BEGIN
            SELECT a.FirstName,
                    a.LastName,
                    a.Email,
                    ZOOS.OrderStateName [Status],
                    a.OmsQuoteId,
                    a.UserId,
                    a.PortalId, 
					ZOOS2.OrderStateName ChildOrderStatus,
					aa.QuoteOrderTotal,
					ZOOS2.OmsOrderStateId 
                FROM @TBL_OrderState a 
				INNER JOIN ZnodeOmsQuote aa ON (aa.OmsQuoteId =a.OmsQuoteId)
				INNER JOIN ZnodeOmsOrderState ZOOS ON(ZOOS.OmsOrderStateId =  aa.OmsOrderStateId )
				INNER JOIN ZnodeOmsOrderState ZOOS2 ON(ZOOS2.OmsOrderStateId =  @OmsOrderStateId )
                SET @Status = 1;
                  
            END;
        ELSE
            BEGIN
                SET @Status = 0;
            END;
    END TRY
    BEGIN CATCH
              
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdateQuoteStatus @OmsQuoteId = '+@OmsQuoteId+',@OmsOrderStateId='+CAST(@OmsOrderStateId AS VARCHAR(50))+',@ExceptUpdateStatus='+@ExceptUpdateStatus+',@Status='+CAST(@Status AS VARCHAR(10))+',@ModifiedBy='+@ModifiedBy;
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_UpdateQuoteStatus',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;