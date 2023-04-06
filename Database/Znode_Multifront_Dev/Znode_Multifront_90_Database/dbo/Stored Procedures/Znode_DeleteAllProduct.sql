


CREATE Procedure [dbo].[Znode_DeleteAllProduct] (@IsAllProduct bit = 0 ,@DeleteProductId VARCHAR(MAX) = '')
	AS
	Begin
	       Declare @Status Bit
		  IF @IsAllProduct  =1 
		  BEGIN
		  WHILE 1=1
			BEGIN 
				SET @DeleteProductId = SUBSTRING((SELECT ','+CAST(PimProductId AS VARCHAR(50)) FROM ZnodePimProduct FOR XML PATH('')), 2, 8000);
				--Remove extra products from catalog
				EXEC Znode_DeletePimProducts @PimProductId = @DeleteProductId, @Status = @Status;
				
				IF ISnull(@DeleteProductId,'') = '' BREAK;
			END          
		 END
		  Else
		  Begin
			 EXEC Znode_DeletePimProducts @PimProductId = @DeleteProductId, @Status = @Status;
			 Select @DeleteProductId
		  END			 
	End