
CREATE FUNCTION [dbo].[Fn_GetRecurciveUserId]
(@UserId INT,
 @ProcessType  varchar(50)='Quote'
)
RETURNS @ConvertTableData TABLE
(UserId       INT,
 ParentUserId INT
)
AS 
      BEGIN
         DECLARE @AccountId INT = 0 

		  IF @ProcessType in ('Order','Quote') AND @UserId <> 0  
		 BEGIN  
		  INSERT INTO @ConvertTableData 
		  SELECT  UserId,ApproverUserId 
		  FROM ZnodeUserApprovers a 
		  WHERE ApproverUserId = @UserId
		  AND @ProcessType  =  'Quote'
		  UNION ALL 
		  SELECT @UserId , NULL

		 END 
		 ELSE
		 
		 IF EXISTS (SELECT TOP 1 1 FROM View_CustomerUserDetail WHERE TypeOfRole = 'B2B' AND
		 ( @ProcessType='Template' or 
		 ( @ProcessType in ('Order','Quote') AND RoleName = 'Administrator' ) )
		 AND userId = @UserId  )
		 BEGIN 
		   
		   SET @AccountId = (SELECT  AccountId  FROM View_CustomerUserDetail WHERE  userId = @UserId )

		   INSERT INTO @ConvertTableData
		   SELECT @UserId,NULL
           UNION ALL
		   SELECT UserId ,NULL 
		   FROM ZnodeUser ZU 
		   WHERE EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetRecurciveAccounts] (@AccountId) FNGRA WHERE FNGRA.AccountId = ZU.AccountId )
		   
		 END 
		 ELSE 
		 INSERT INTO @ConvertTableData
                SELECT @UserId,
                       NULL
                UNION ALL
                SELECT UserId,
                       ApprovalUserId
                FROM ZnodeAccountUserOrderApproval
                WHERE ApprovalUserId = @UserId;
         RETURN;
     END;