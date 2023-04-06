CREATE PROCEDURE [dbo].[Znode_UpdateAttributeValue](@ProductId        VARCHAR(2000),      
                                                   @PimAttributeCode NVARCHAR(300),      
                                                   @LocaleId         INT,      
                                                   @AttributeValue   NVARCHAR(MAX),      
                                                   @UserId           INT,      
                                                   @Status           BIT OUT,      
                                                   @IsUnAssociated   BIT           = 0,      
                                                   @IsDebug          BIT           = 0)      
AS       
         
/* ---------------------------------------------------------------------------------------------------------------      
    --Summary : Update AttributeValue for specific product       
    --          Input parameter : @LocaleId , @PimAttributeCode,  @ProductId,@AttributeValue,@UserId      
    --Unit Testing :       
    BEGIN TRANSACTION       
    DECLARE @Status bit       
    EXEC [Znode_UpdateAttributeValue]      
    @ProductId        = 10637,      
    @PimAttributeCode = 'Brand' ,      
    @LocaleId         =1 ,      
    @AttributeValue   = 'Tropicana',      
    @UserId           =2,      
    @Status           =@Status OUT      
    SELECT @Status       
    --SELECT zpa.AttributeCode ,ZpAVL .AttributeValue  FROM ZnodePimAttributeValueLocale ZpAVL INNER JOIN ZnodePimAttributeValue zpav ON ZpAVL.PimAttributeValueId = zpav.PimAttributeValueId      
    --INNER JOIN dbo.ZnodePimAttribute zpa ON zpav.PimAttributeId = zpa.PimAttributeId      
    --WHERE zpav.PimProductId =12 AND ZpAVL.LocaleId = 1 AND zpa.AttributeCode = 'ProductName'       
    --ROLLBACK Transaction       
    ---------------------------------------------------------------------------------------------------------------      
*/      
      
     BEGIN    

		BEGIN TRAN UpdateAttributeValue;      
			BEGIN TRY      
				DECLARE @GetDate DATETIME= dbo.Fn_GetDate();      
				DECLARE @PimDefaultFamily INT= dbo.Fn_GetDefaultPimProductFamilyId();      
				DECLARE @TBL_PimProductId TABLE      
				(PimProductId               INT,      
				PimAttributeId             INT,      
				AttributeCode              VARCHAR(300),      
				AttributeValue             NVARCHAR(MAX),      
				PimAttributeDefaultValueId INT ,
				Attributetype VARCHAR(50)     
				); -- table holds the PimProductId   
				    
				DECLARE @TBL_DefaultAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))      
      
				INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)      
				SELECT PimAttributeId,AttributeCode FROM  [dbo].[Fn_GetDefaultAttributeId] ()      
      
				DECLARE @TBL_PimAttributeValueId TABLE      
				(PimAttributeValueId INT,      
				PimAttributeId      INT,      
				PimProductId        INT,      
				PimAttributeDefaultValueId int       
				);      
	  
  
				INSERT INTO @TBL_PimProductId      
				(PimProductId,      
				PimAttributeId,      
				AttributeCode,      
				AttributeValue,      
				PimAttributeDefaultValueId      
				)      
				SELECT item,      
					ZPA.PimAttributeId,      
					@PimAttributeCode,      
					@AttributeValue,      
					ZPADV.PimAttributeDefaultValueId      
				FROM dbo.Split(@ProductId, ',') SP      
					LEFT JOIN ZnodePimAttribute ZPA ON(ZPA.AttributeCode = @PimAttributeCode)      
					LEFT JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPA.PimAttributeId = ZPADV.PimAttributeId AND ( ZPADV.AttributeDefaultValueCode = @AttributeValue OR ISNULL(@AttributeValue, '') = ''  ))      
					--WHERE NOT EXISTS (SELECT * FROM @TBL_PimProductId_Simple WHERE PimProductId = SP.item AND PimAttributeId = ZPA.pimattributeid)-- AND AttributeCode = @AttributeValue AND AttributeValue = @AttributeValue AND PimAttributeDefaultValueId= ZPADV.PimAttributeDefaultValueId)


					Update TBL 
					SET Attributetype = 'Simple Select'
					FROM @TBL_PimProductId TBL
					WHERE EXISTS (SELECT * FROM ZnodePimAttribute WHERE AttributeCode = @PimAttributeCode AND AttributeTypeId = (SELECT Top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Simple Select'))
					--INNER JOIN ZnodePimAttribute ZPA ON(ZPA.AttributeCode = @PimAttributeCode and ZPA.AttributeTypeId = (SELECT AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Simple Select'))      


		  
      
				IF @IsUnAssociated = 1      
				BEGIN      
				INSERT INTO @TBL_PimAttributeValueId      
				(PimAttributeValueId,      
				PimAttributeId,      
				PimProductId,      
				PimAttributeDefaultValueId     
				)      
					SELECT ZPAV.PimAttributeValueId,      
							ZPAV.PimAttributeId,      
							ZPAV.PimProductId,      
							ZPADV.PimAttributeDefaultValueId      
					FROM ZnodePimAttributeValue ZPAV      
							INNER JOIN @TBL_PimProductId TBLP ON(TBLP.PimProductId = ZPAV.PimProductId AND TBLP.PimAttributeId = ZPAV.PimAttributeId AND TBLP.AttributeValue = @AttributeValue)
							INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId)
							INNER JOIN ZnodePimAttributeDefaultValue ZADV ON (ZPADV.PimAttributeDefaultValueId = ZADV.PimAttributeDefaultValueId AND ZADV.AttributeDefaultValueCode =@AttributeValue )

      
				DELETE FROM ZnodePimProductAttributeDefaultValue 
				WHERE EXISTS      
				(      
					SELECT TOP 1 1      
					FROM @TBL_PimAttributeValueId TBLAP      
					WHERE TBLAP.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId      
					AND ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId = TBLAP.PimAttributeDefaultValueId --@PimAttributeDefaultValueId      
				)      
				AND LocaleId = @LocaleId
					

				DELETE FROM ZnodePimAttributeValue    
				WHERE EXISTS      
				(      
					SELECT TOP 1 1      
					FROM @TBL_PimAttributeValueId TBLAP      
					WHERE TBLAP.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId      
						AND ZnodePimAttributeValue.PimAttributeDefaultValueId =TBLAP.PimAttributeDefaultValueId -- @PimAttributeDefaultValueId      
				)
				AND not exists (select * from   ZnodePimProductAttributeDefaultValue where PimAttributeValueId =ZnodePimAttributeValue.PimAttributeValueId )--AND PimAttributeDefaultValueId <>TBLAP.PimAttributeDefaultValueId ) 
				--AND LocaleId = @LocaleId     

				END;   

				---- here needs to handle which records will be updated and which records will be inserted 	

				--INSERT INTO @TBL_PimProductId 
				--(PimProductId,      
				--PimAttributeId,      
				--AttributeCode,      
				--AttributeValue,      
				--PimAttributeDefaultValueId      
				--)     
				--SELECT 	TBL.PimProductId,      
				--TBL.PimAttributeId,      
				--AttributeCode,      
				--TBL.AttributeValue,      
				--TBL.PimAttributeDefaultValueId 
				--FROM @TBL_PimProductId_Simple TBL 
				--INNER JOIN 	Znodepimattributevalue ZPA ON (ZPA.PimProductId = TBL.PimProductId AND ZPA.PimAttributeId = TBL.PimAttributeId)	 
			 

				UPDATE ZnodePimAttributeValue       
				SET  ModifiedBy = @UserId      
				, ModifiedDate = @GetDate      
				,PimAttributeDefaultValueId = ZAV.PimAttributeDefaultValueId      
				OUTPUT INSERTED.PimAttributeValueId,      
				INSERTED.PimAttributeId,      
				INSERTED.PimProductId,      
				INSERTED.PimAttributeDefaultValueId      
				INTO @TBL_PimAttributeValueId      
				FROM ZnodePimAttributeValue        
				INNER JOIN @TBL_PimProductId ZAV ON ( ZAV.PimProductId = ZnodePimAttributeValue.PimProductId      
						AND ZAV.PimAttributeId = ZnodePimAttributeValue.PimAttributeId)      
				WHERE @IsUnAssociated = 0  


				--INSERT INTO @TBL_PimProductId 
				--(PimProductId,      
				--PimAttributeId,      
				--AttributeCode,      
				--AttributeValue,      
				--PimAttributeDefaultValueId      
				--)     
				--SELECT 	PimProductId,      
				--PimAttributeId,      
				--AttributeCode,      
				--AttributeValue,      
				--PimAttributeDefaultValueId 
				--FROM @TBL_PimProductId_Simple TBL 
				--WHERE NOT EXISTS (SELECT * FROM Znodepimattributevalue WHERE PimAttributeId = TBL.pimattributeid AND PimProductId = TBL.PimProductId)
     
				INSERT INTO ZnodePimAttributeValue      
				(PimProductId,      
				PimAttributeId,      
				PimAttributeDefaultValueId,      
				AttributeValue,      
				CreatedBy,      
				CreatedDate,      
				ModifiedBy,      
				ModifiedDate,      
				PimAttributeFamilyId      
				)      
				OUTPUT INSERTED.PimAttributeValueId,      
				INSERTED.PimAttributeId,      
				INSERTED.PimProductId,      
				INSERTED.PimAttributeDefaultValueId      
				INTO @TBL_PimAttributeValueId      
				SELECT TBPP.PimProductId,      
					TBPP.PimAttributeId,      
					TBPP.PimAttributeDefaultValueId,      
					TBPP.AttributeValue,      
					@UserId,      
					@GetDate,      
					@UserId,      
					@GetDate,      
					@PimDefaultFamily      
				FROM @TBL_PimProductId TBPP      
				WHERE NOT EXISTS      
				(      
				SELECT TOP 1 1      
				FROM ZnodePimAttributeValue ZAV      
				WHERE ZAV.PimProductId = TBPP.PimProductId      
						AND ZAV.PimAttributeId = TBPP.PimAttributeId      
				)      
				AND @IsUnAssociated = 0;      
                   
				UPDATE A      
				SET      
				AttributeValue = C.AttributeValue      
				FROM ZnodePimAttributeValueLocale A      
				INNER JOIN ZnodePimAttributeValue B ON(B.PimAttributeValueId = A.PimAttributeValueId)      
				INNER JOIN @TBL_PimProductId C ON(B.PimAttributeId = C.PimAttributeId      
											AND B.PimProductId = C.PimProductId)      
				WHERE LocaleId = @LocaleId;  

 
				Declare @Pimattriutedefaultvalueid INT = 0
						 
				SELECT @Pimattriutedefaultvalueid =  A.PimAttributeDefaultValueId from ZnodePimAttributeDefaultValue A 
				INNER JOIN ZnodePimAttributeDefaultValueLocale B On (A.PimAttributeDefaultValueId = B.PimAttributeDefaultValueId AND A.PimAttributeId = (SELECT TOP 1 PimAttributeId from ZnodePimAttribute WHERE AttributeCode = @PimAttributeCode))
				WHERE A.AttributeDefaultValueCode = @AttributeValue AND B.LocaleId  = @LocaleId	


				-- UP ZnodePimProductAttributeDefaultValue 
				IF @Pimattriutedefaultvalueid <>0
				Update ZPDA
				SET ZPDA.ModifiedBy = @UserId,
				ZPDA.Modifieddate = @getdate,
				ZPDA.pimattributedefaultvalueid = ZPA.pimattributedefaultvalueid
				FROM @TBL_PimProductId TBL
				INNER JOIN @TBL_PimAttributeValueId ZPA ON (TBL.pimattributeid = ZPA.pimattributeid AND TBL.pimproductid = ZPA.PimProductId)
				INNER JOIN ZnodePimProductAttributeDefaultValue ZPDA ON (ZPDA.PimAttributeValueId = ZPA.pimattributevalueid)
				WHERE TBL.Attributetype = 'Simple Select'
				

				DELETE TBL
				FROM @TBL_PimAttributeValueId TBL
				INNER JOIN @TBL_PimProductId TBLP On (TBL.PimAttributeId = TBLP.PimAttributeId AND TBL.PimProductId = TBLP.pimproductid AND TBL.pimattributevalueid = TBLP.pimattributedefaultvalueid)
				WHERE TBLP.Attributetype = 'Simple Select'
				AND (EXISTS (SELECT * FROM ZnodePimProductAttributeDefaultValue WHERE PimAttributeValueId = TBL.pimattributevalueid )
				OR  EXISTS (SELECt * FROM ZnodePimAttributeValueLocale WHERE PimAttributeValueId = TBL.pimattributevalueid))
		   
				INSERT INTO ZnodePimProductAttributeDefaultValue      
				(PimAttributeValueId,      
				LocaleId,      
				PimAttributeDefaultValueId,      
				CreatedBy,      
				CreatedDate,      
				ModifiedBy,      
				ModifiedDate      
				)      
				SELECT DISTINCT TBPAV.PimAttributeValueId,      
					@LocaleId,      
					TBPAV.PimAttributeDefaultValueId,      
					@UserId,      
					@GetDate,      
					@UserId,      
					@GetDate      
				FROM @TBL_PimProductId TBPP      
					INNER JOIN @TBL_PimAttributeValueId TBPAV ON(TBPAV.PimProductId = TBPP.PimProductId      
																AND TBPAV.PimAttributeId = TBPP.PimAttributeId)      
																AND @IsUnAssociated = 0      
				AND EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
				AND NOT EXISTS (SELECT * FROM ZnodePimProductAttributeDefaultValue WHERE PimAttributeValueId =TBPAV.PimAttributeValueId AND PimAttributeDefaultValueId = TBPP.PimAttributeDefaultValueId )
				  
				UPDATE ZPAVL      
				SET AttributeValue =@AttributeValue      
				FROM ZnodePimAttributeValueLocale ZPAVL       
				INNER JOIN @TBL_PimAttributeValueId TBPAV ON( TBPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId )      
																AND @IsUnAssociated = 0  
   
      
				INSERT INTO ZnodePimAttributeValueLocale      
				(PimAttributeValueId,      
				LocaleId,      
				AttributeValue,      
				CreatedBy,      
				CreatedDate,      
				ModifiedBy,      
				ModifiedDate      
				)      
				SELECT TBPAV.PimAttributeValueId,      
					@LocaleId,      
					TBPP.AttributeValue,      
					@UserId,      
					@GetDate,      
					@UserId,      
					@GetDate      
				FROM @TBL_PimProductId TBPP      
					INNER JOIN @TBL_PimAttributeValueId TBPAV ON(TBPAV.PimProductId = TBPP.PimProductId      
																AND TBPAV.PimAttributeId = TBPP.PimAttributeId)      
																WHERE @IsUnAssociated = 0      
				AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)      
				AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale TBH WHERE TBH.PimAttributeValueId = TBPAV.PimAttributeValueId);      
				  
				 SELECT *  INTO #TBL_PimProductId
			 FROM @TBL_PimProductId


			  IF @LocaleId = 1
			 BEGIN 	 
	
DECLARE @sqlt NVARCHAr(max) = ''
DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr int 

DECLARE Cur_AttributeDataUpdate CURSOR FOR 



SELECT b.AttributeCode , PimAttributeId 
FROM INFORMATION_SCHEMA.COLUMNS a 
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )
WHERE TABLE_NAME = 'ZnodePimProduct'
AND IsCategory = 0 
AND IsShowOnGrid = 1 
AND EXISTS (SELECT TOP 1 1 FROM @TBL_PimProductId n  WHERE n.AttributeCode = b.AttributeCode  )
OPEN Cur_AttributeDataUpdate 
FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
WHILE @@FETCH_STATUS = 0 
BEGIN 

 SET @sqlt = 'UPDATE a  
 SET '+@AttributeCodeAtt+'= AttributeValue 
 FROM ZnodePimProduct a 
 INNER JOIN #TBL_PimProductId m ON(m.PimProductId = a.pimProductId ) 
 WHERE m.AttributeCode = '''+@AttributeCodeAtt+'''
 ' 

 EXEC (@sqlt)

FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
END 
CLOSE Cur_AttributeDataUpdate
DEALLOCATE Cur_AttributeDataUpdate 

END 

			






				SET @Status = 1;      
				SELECT 1 AS ID,      
				CAST(1 AS BIT) AS [Status];      
      
			COMMIT TRAN UpdateAttributeValue;      
		END TRY      
		BEGIN CATCH      
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),       
		@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdateAttributeValue @ProductId = '+      
		@ProductId+',@PimAttributeCode='+@PimAttributeCode+',@Status='+CAST(@Status AS VARCHAR(50))+      
		',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@AttributeValue='+CAST(@AttributeValue AS NVARCHAR(MAX))+',@UserId='+CAST(@UserId AS NVARCHAR(50));      
		SET @Status = 0;      
		SELECT 1 AS ID,      
		CAST(0 AS BIT) AS [Status],      
		@ErrorMessage;      
		ROLLBACK TRAN UpdateAttributeValue;      
		EXEC Znode_InsertProcedureErrorLog      
		@ProcedureName = 'Znode_UpdateAttributeValue',      
		@ErrorInProcedure = @Error_procedure,      
		@ErrorMessage = @ErrorMessage,      
		@ErrorLine = @ErrorLine,      
		@ErrorCall = @ErrorCall;      
		END CATCH;      
     END