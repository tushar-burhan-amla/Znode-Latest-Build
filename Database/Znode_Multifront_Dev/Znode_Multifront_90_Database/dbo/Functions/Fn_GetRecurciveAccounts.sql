-- EXEC [dbo].[Znode_SplitWhereClause] 'ProductName LIKE ''%raj and suraj%'' AND SKU = ''po''',0


CREATE FUNCTION [dbo].[Fn_GetRecurciveAccounts]
(
       @AccountId      INT 
)
RETURNS  @ConvertTableData TABLE (AccountId INT ,ParentAccountId INT  )
AS
     BEGIN
     
	   With Cte_RecursiveAccountId AS (
	       
		   SELECT AccountId ,ParentAccountId 
		   FROM ZnodeAccount WITH (NOLOCK)
		   WHERE AccountId = @AccountId OR @AccountId = 0 

		   UNION ALL 

		   SELECT ZA.AccountId ,ZA.ParentAccountId 
		   FROM ZnodeAccount ZA WITH (NOLOCK) 
		   INNER JOIN Cte_RecursiveAccountId CTRA ON (ZA.ParentAccountId = CTRA.AccountId)
	    
	   )

	   INSERT INTO @ConvertTableData
	   SELECT * 
	   FROM Cte_RecursiveAccountId
	 
	     
     
		 RETURN 
     END;