CREATE PROCEDURE [dbo].[Znode_InsertUpdateGlobalEntityAttributeValue]
(   
	@GlobalEntityValueXml NVARCHAR(max),
    @GlobalEntityValueId int,
	@EntityName varchar(200),
	@FamilyId  INT,
    @UserId     INT,
    @status     BIT OUT 
)
AS
/*
     Summary : To Insert / Update single Global Entity Value with multiple attribute values 
     Update Logic: 
*/
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY

		     DECLARE @ConvertedXML XML = REPLACE(REPLACE(REPLACE(@GlobalEntityValueXml,' & ', '&amp;'),'"', '&quot;'),'''', '&apos;')
              DECLARE @GlobalEntityValueDetail_xml GlobalEntityValueDetail;

             INSERT INTO @GlobalEntityValueDetail_xml
			 (GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeCode,AttributeValue
			 ,LocaleId,GlobalEntityValueId)
			SELECT Tbl.Col.value('GlobalAttributeId[1]', 'int') AS GlobalAttributeId,
			Tbl.Col.value('GlobalAttributeValueId[1]', 'int') AS GlobalAttributeValueId,
			Tbl.Col.value('GlobalAttributeDefaultValueId[1]', 'int') AS GlobalAttributeDefaultValueId,
			Tbl.Col.value('AttributeCode[1]', 'NVARCHAR(300)') AS AttributeCode,
			Tbl.Col.value('AttributeValue[1]', 'NVARCHAR(MAX)') AS AttributeValue,
			Tbl.Col.value('LocaleId[1]', 'INT') AS LocaleId,
			@GlobalEntityValueId AS GlobalEntityValueId
			FROM @ConvertedXML.nodes('//ArrayOfEntityAttributeDetailsModel/EntityAttributeDetailsModel') AS Tbl(Col);

			Declare @IsFamilyUnique BIT
			set @IsFamilyUnique = (select IsFamilyUnique from ZnodeGlobalEntity where EntityName = @EntityName)
			if(@IsFamilyUnique = 0)
			BEGIN
			   IF EXISTS(select top 1 1 from ZnodeGlobalEntityFamilyMapper where GlobalEntityId = (select GlobalEntityId from ZnodeGlobalEntity where EntityName = @EntityName)  and GlobalEntityValueId = @GlobalEntityValueId )
					update ZnodeGlobalEntityFamilyMapper set GlobalAttributeFamilyId= @FamilyId 
					where GlobalEntityValueId = @GlobalEntityValueId	and GlobalEntityId = (select GlobalEntityId from ZnodeGlobalEntity where EntityName = @EntityName)
			   ELSE
					insert into ZnodeGlobalEntityFamilyMapper values (@FamilyId,(select GlobalEntityId from ZnodeGlobalEntity where  EntityName = @EntityName),@GlobalEntityValueId)
			END

			If @EntityName='Store'
             EXEC [dbo].[Znode_ImportInsertUpdatePortalGlobalAttributeValue]
                  @GlobalEntityValueDetail_xml,
                  @UserId,
                  @status OUT,0 ; 
			else If @EntityName='User'
			EXEC [dbo].[Znode_ImportInsertUpdateUserGlobalAttributeValue]
                  @GlobalEntityValueDetail_xml,
                  @UserId,
                  @status OUT,0 ; 
			else If @EntityName='Account'
			EXEC [dbo].Znode_ImportInsertUpdateAccountGlobalAttributeValue
                  @GlobalEntityValueDetail_xml,
                  @UserId,
                  @status OUT,0 ; 
		    else if @EntityName = 'Content Containers'
			EXEC [dbo].Znode_ImportInsertUpdateWidgetGlobalAttributeValue
                  @GlobalEntityValueDetail_xml,
                  @UserId,
                  @status OUT,0 ; 
			else If @EntityName='FormBuilder'
			EXEC [dbo].Znode_ImportInsertUpdateFormBuilderGlobalAttributeValue
                  @GlobalEntityValueDetail_xml,
                  @UserId,
                  @status OUT,0 ; 
			
             SET @status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC [Znode_InsertUpdateGlobalEntityAttributeValue] @GlobalEntityValueXml= '+CAST(@GlobalEntityValueXml AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdateGlobalEntityAttributeValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;