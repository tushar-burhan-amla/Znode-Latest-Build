CREATE PROCEDURE [dbo].[Znode_DeleteAccount]
( 
	@AccountId VARCHAR(2000)='',
	@Status    BIT OUT,
	@AccountIds TransferId READONLY ,
	@IsForceFullyDelete BIT =0 ,  
	@IsDebug bit = 0 
)
AS
  /* 
     Summary : Remove account details with child table date through accountid
     Sequence For Delete Data   
     1. ZnodeDepartment          
     2. ZnodeAccountAddress		 
     3. ZnodeAccountRoles		 
     4. ZnodeUser				 
     5. ZnodeAccountPermission 
     6. ZnodeAccount	
     7. ZnodePriceListAccount
     8. ZnodePortalAccount
     9. ZnodeNote
     10.ZnodeAccountPromotion
     here check if all account ids which is comma separeted in @AccountId  parameter if deleted all then output dataset status is true other wise its false
     Also check account id not have any chaild account or any user 
     Unit Testing	
	 begin tran
     EXEC Znode_DeleteAccount '5',0
     rollback tran
*/
BEGIN
BEGIN TRAN DeleteAccount;
BEGIN TRY
    SET NOCOUNT ON; 
	DECLARE @StatusOut Table (Id INT ,Message NVARCHAR(max), Status BIT )
    
	--Table varible to store string formated accountids into record format
	IF OBJECT_ID('tempdb..#TBL_AccountId') IS NOT NULL
		DROP TABLE #TBL_AccountId;

	CREATE TABLE #TBL_AccountId (AccountId INT);

    INSERT INTO #TBL_AccountId
    SELECT item
    FROM dbo.Split(@AccountId, ',')
	where @AccountId <> '';
			
	INSERT INTO #TBL_AccountId
	SELECT id FROM @AccountIds 

	--Getting accounts recursive data(child parent data)
	;With Cte_RecursiveAccountId AS 
	(
	       
		SELECT AccountId ,ParentAccountId 
		FROM ZnodeAccount a WITH (NOLOCK)
		WHERE exists(select * from #TBL_AccountId b where a.AccountId = b.AccountId)
		UNION ALL 
		SELECT ZA.AccountId ,ZA.ParentAccountId 
		FROM ZnodeAccount ZA WITH (NOLOCK) 
		INNER JOIN Cte_RecursiveAccountId CTRA ON (ZA.ParentAccountId = CTRA.AccountId)	    
	) 
	SELECT * 
	INTO #ConvertTableData
	FROM Cte_RecursiveAccountId

	CREATE NONCLUSTERED INDEX NC_INdx_AccountId ON #TBL_AccountId(AccountId);

    DECLARE @DeletedCount INT= 0; -- Carry the how much records are need to delete 
    -- Table variable to store deleted accountid will be used to delete records from child tables
    -- to avoid multiple calling of ZnodeAccount tables
	IF OBJECT_ID('tempdb..#TLB_DeleteAccountId') IS NOT NULL
	DROP TABLE #TLB_DeleteAccountId;
    CREATE TABLE #TLB_DeleteAccountId (AccountId INT);
    DECLARE @TBL_AddressDetail TABLE(AddressId INT); 
	DECLARE @TBL_UserS TABLE(UserId INT);
		
	-- this table is used to hold the address id related to the account ids 
    -- Retrive validate data from account table 
    -- Any kind of validation / restriction can apply on following query for deletion
	IF OBJECT_ID('tempdb..#AccountUserWith') IS NOT NULL
		DROP TABLE #AccountUserWith;
		
	CREATE TABLE #AccountUserWith (AccountId INT , ParentAccountId INT );
	
	INSERT INTO #AccountUserWith
	SELECT ZA.AccountId,ZA.ParentAccountId
    FROM [dbo].ZnodeAccount AS ZA WITH (NOLOCK)
    INNER JOIN #ConvertTableData AS TBA ON  ZA.AccountId = TBA.AccountId
    WHERE  EXISTS
    (
        SELECT TOP 1 1
        FROM ZnodeUser AS ZU WITH (NOLOCK)
		INNER JOIN ZnodeOmsOrderDetails ZOOSD WITH (NOLOCK) ON (ZOOSD.UserId = ZU.UserId)
        WHERE ZU.AccountId = ZA.AccountId
										
    )
		
	CREATE NONCLUSTERED INDEX NC_Idx_AccountId_PAccountId ON #AccountUserWith (AccountId,ParentAccountId);	
				
	DECLARE @TBL_GetUserDeleted TABLE (ID INT , [Status] BIT )

	  		
	INSERT INTO #TLB_DeleteAccountId
	SELECT ZA.AccountId
	FROM [dbo].ZnodeAccount AS ZA WITH (NOLOCK)
	WHERE 
	EXISTS( SELECT TOP 1 1 FROM #ConvertTableData SP WHERE   ZA.AccountId = SP.AccountId)
	AND (NOT EXISTS
	(
		SELECT TOP 1 1
		FROM #AccountUserWith AS ZU
		WHERE ZU.AccountId = ZA.AccountId
		OR ZU.ParentAccountId= ZA.AccountId
										
	)  OR 	 @IsForceFullyDelete =1 )
				   
	CREATE NONCLUSTERED INDEX NC_Idx_AccountId ON #TLB_DeleteAccountId (AccountId);		

	INSERT INTO @TBL_UserS 
	SELECT UserId 
	FROM ZnodeUser WITH (NOLOCK)
	WHERE EXISTS
	(
		SELECT TOP 1 1
		FROM #TLB_DeleteAccountId AS TBDA
		WHERE TBDA.AccountId = ZnodeUSer.AccountId
	);


	SET @DeletedCount =
	(
		SELECT COUNT(1)
		FROM #TLB_DeleteAccountId
	);
		
	DELETE FROM ZnodeDepartmentUser 
	WHERE DepartmentId IN (SELECT DepartmentId FROM ZnodeDepartment
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeDepartment.AccountId
    ))
    DELETE FROM ZnodeDepartment
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeDepartment.AccountId
    ); -- check for delete 

			

    DELETE FROM ZnodeAccountAddress
    OUTPUT Deleted.AddressId
        INTO @TBL_AddressDetail -- Insert the deleted AddressedId 
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeAccountAddress.AccountId
    );

	DELETE FROM ZnodeAccountRoles
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeAccountRoles.AccountId
    );
          
    DELETE FROM ZnodePriceListAccount
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodePriceListAccount.AccountId
    );
    DELETE FROM ZnodePortalAccount
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodePortalAccount.AccountId
    );
    DELETE FROM ZnodeNote
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeNote.AccountId
    );
    DELETE FROM ZnodeAccountPromotion
    WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM #TLB_DeleteAccountId AS TBDA
        WHERE TBDA.AccountId = ZnodeAccountPromotion.AccountId
    );

			 
	DELETE FROM ZnodeAccountProfile 
	WHERE EXISTS
	(
	SELECT TOP 1 1
	FROM #TLB_DeleteAccountId AS TBDA
	WHERE TBDA.AccountId = ZnodeAccountProfile.AccountId
	)

	DELETE FROM ZnodeUserProfile 
	WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM @TBL_UserS AS TBDA
        WHERE TBDA.Userid = ZnodeUserProfile.UserId
    ) 
	DELETE FROM ZnodeUserPortal 
	WHERE EXISTS
    (
        SELECT TOP 1 1
        FROM @TBL_UserS AS TBDA
        WHERE TBDA.Userid = ZnodeUserPortal.UserId
    ) 
		
	DECLARE @UserId transferId 
	INSERT INTO @UserId 
	SELECT userId 
	FROM @TBL_UserS 

	DELETE FROM ZnodeAccountUserPermission 
	WHERE EXISTS (
            SELECT TOP 1 1
            FROM @TBL_UserS AS TBDA
            WHERE TBDA.UserId = ZnodeAccountUserPermission.UserId
        )
		
		DELETE FROM ZnodeAccountPermissionaccess
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM ZnodeAccountPermission
            WHERE ZnodeAccountPermission.AccountPermissionId = ZnodeAccountPermissionaccess.AccountPermissionId
                AND EXISTS
            (
                SELECT TOP 1 1
                FROM #TLB_DeleteAccountId AS TBDA
                WHERE TBDA.AccountId = ZnodeAccountPermission.AccountId
            )
        );
        DELETE FROM ZnodeAccountPermission
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM #TLB_DeleteAccountId AS TBDA
            WHERE TBDA.AccountId = ZnodeAccountPermission.AccountId
        );
	--	INSERT INTO @StatusOut(id ,Status)
	EXEC Znode_DeleteUserDetails @UserIds= @UserId,@Status=0,@IsForceFullyDelete = @IsForceFullyDelete,@IsCallInternal =1 
           
		DELETE FROM ZnodeAddress
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM @TBL_AddressDetail AS TBD
            WHERE TBD.AddressId = ZnodeAddress.AddressId
        );

		DELETE FROM ZnodeAccount
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM #TLB_DeleteAccountId AS TBDA
            WHERE TBDA.AccountId = ZnodeAccount.AccountId
        );

			
        SET @Status = 1;
        IF
        (
            SELECT COUNT(1)
            FROM #ConvertTableData
        ) = @DeletedCount    -- check for passed @AccountId ids is equal to the deleted ids with child accounts
            BEGIN
                SELECT 1 AS ID,
                    CAST(1 AS BIT) AS [Status]; 
            END;
        ELSE
            BEGIN
                SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
            END;
		    
COMMIT TRAN DeleteAccount;
END TRY
	BEGIN CATCH
	SELECT 0 AS ID,
	CAST(0 AS BIT) AS Status;
	SELECT ERROR_MESSAGE()
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteAccount @AccountId = '+@AccountId+',@Status='+CAST(@Status AS VARCHAR(200));
             			 
	SET @Status = 0;
            
	ROLLBACK TRAN DeleteAccount;
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_DeleteAccount',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
            
END CATCH;
END;