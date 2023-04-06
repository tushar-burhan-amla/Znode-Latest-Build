-- DECLARE @DD INT = NULL  EXEC Znode_DeleteAccountDetails '19',@DD OUT SELECT @DD  

    
-- 
CREATE Procedure [dbo].[Znode_DeleteAccountDetails] 
(
@AccountId Varchar(100) = NULL
,@Status int oUt 
)
AS 
BEGIN 
  BEGIN TRY
  SET NOCOUNT ON 
  BEGIN TRAN A 
    DECLARE @V_table TABLE (USERID1 NVARCHAR(200))
    DECLARE @V_tabledeleted TABLE (AccountId1 int)
	   
    DELETE FROM ZnodeAccountProfile OUTPUT deleted.Accountid INTO @V_tabledeleted WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@AccountId,',') a WHERE a.item = ZnodeAccountProfile.AccountId )
	

	DELETE FROM ZnodeAccount OUTPUT deleted.UserId INTO @V_table
	WHERE  EXISTS (SELECT TOP 1 1 FROM @V_tabledeleted WHERE Accountid1 = Accountid) 

	DELETE FROM AspNetUserRoles WHERE EXISTS (SELECT TOP 1 1 FROM @V_table WHERE UserID1 = UserID) 

	DELETE  FROM AspNetUsers WHERE EXISTS (SELECT TOP 1 1 FROM @V_table WHERE UserID1 = id) 
	
	

    IF (SELECT COUNT (1) FROM @V_tabledeleted  ) = (SELECT COUNT(1) FROM dbo.split(@AccountId,','))
	BEGIN 
	SELECT 0 ID , CAST(1 AS BIT ) Status 
	
	END 
	ELSE 
	BEGIN 
		SELECT 0 ID , CAST(0 AS BIT ) Status 
	END 
	SET @Status = 1 
   COMMIT TRAN A 
  END TRY 
  BEGIN CATCH 
   SELECT ERROR_LINE () , ERROR_MESSAGE(),ERROR_PROCEDURE ()
   SET @Status = 0 
   ROLLBACK TRAN A 
  END CATCH 
END