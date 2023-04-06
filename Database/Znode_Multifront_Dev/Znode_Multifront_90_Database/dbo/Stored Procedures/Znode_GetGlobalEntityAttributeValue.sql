

CREATE   PROCEDURE [dbo].[Znode_GetGlobalEntityAttributeValue]
(
    @EntityName       nvarchar(200) = 0,
    @GlobalEntityValueId   INT = 0,
	@LocaleCode       nvarchar(200) = '',
	@GroupCode nvarchar(200)  = null,
	@SelectedValue bit = 0
)
AS
/*
	 Summary :- This procedure is used to get the Attribute and EntityValue attribute value as per filter pass 
	 Unit Testing 
	 BEGIN TRAN
	 EXEC [Znode_GetGlobalEntityAttributeValue] 'Store',1
	 ROLLBACK TRAN

*/	 
     BEGIN
         BEGIN TRY
 


		 IF @EntityName='Store'
			 Exec [dbo].[Znode_GetPortalGlobalAttributeValue] 
			 @EntityName=@EntityName,
			 @GlobalEntityValueId=@GlobalEntityValueId,@LocaleCode=@LocaleCode,
			 @GroupCode =@GroupCode,
			 @SelectedValue = @SelectedValue

		 Else IF @EntityName='User'
			 Exec [dbo].[Znode_GetUserGlobalAttributeValue] 
			 @EntityName=@EntityName,
			 @GlobalEntityValueId=@GlobalEntityValueId,
			 @LocaleCode=@LocaleCode,
			 @GroupCode =@GroupCode,
			 @SelectedValue = @SelectedValue

		Else IF @EntityName='Account'
			 Exec [dbo].[Znode_GetAccountGlobalAttributeValue] 
			 @EntityName=@EntityName,
			 @GlobalEntityValueId=@GlobalEntityValueId,
			 @LocaleCode=@LocaleCode,
			 @GroupCode =@GroupCode,
			 @SelectedValue = @SelectedValue
		--Else IF @EntityName='FormBuilder'
		--	 Exec [dbo].[Znode_GetFormBuilderGlobalAttributeValue] 
		--	 @EntityName=@EntityName,
		--	 @GlobalEntityValueId=@GlobalEntityValueId

		
			 Else IF @EntityName='Content Containers'
			 Exec [dbo].[Znode_GetWidgetGlobalAttributeValue] 
			 @EntityName=@EntityName,
			 @GlobalEntityValueId=@GlobalEntityValueId,
			 @LocaleCode=@LocaleCode,
			 @GroupCode =@GroupCode,
			 @SelectedValue = @SelectedValue
   
		  END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE()
             DECLARE @Status BIT ;
		  SET @Status = 0;
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		   @ErrorLine VARCHAR(100)= ERROR_LINE(),
		    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGlobalEntityAttributeValue @EntityName = '''+ISNULL(@EntityName,'''''')+''',@GlobalEntityValueId='+ISNULL(CAST(@GlobalEntityValueId AS VARCHAR(50)),'''')+
			''',@LocaleCode='+ISNULL(CAST(@LocaleCode AS VARCHAR(100)),'''')+''',@GroupCode='+ISNULL(CAST(@GroupCode AS VARCHAR(100)),'''')+''',@SelectedValue='+ISNULL(CAST(@SelectedValue AS VARCHAR(50)),'''')      			 
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                     
		 
          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetGlobalEntityAttributeValue',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
     END;