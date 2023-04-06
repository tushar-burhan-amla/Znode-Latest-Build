CREATE  Procedure [dbo].[Znode_DeleteAllCategory] (@IsAllCategory bit = 0 ,@DeleteCategoryId NVARCHAR(MAX) = '')
	AS
	Begin
	       Declare @Status Bit
		  IF @IsAllCategory  =1 
		  BEGIN
		 -- WHILE 1=1
			--BEGIN 
				SET @DeleteCategoryId = SUBSTRING((SELECT TOP 100 ','+CAST(PimCategoryId AS VARCHAR(50)) FROM ZnodePimCategory FOR XML PATH('')), 2, 4000);
				--Remove extra products from catalog
				EXEC Znode_DeletePimCategory @PimCategoryIds = @DeleteCategoryId, @Status = @Status;
				
			--	IF ISnull(@DeleteCategoryId,'') = '' BREAK;
			--END          
		  END
		  Else
		  Begin
			 EXEC Znode_DeletePimCategory @PimCategoryIds = @DeleteCategoryId, @Status = @Status;
		  END			 
	End