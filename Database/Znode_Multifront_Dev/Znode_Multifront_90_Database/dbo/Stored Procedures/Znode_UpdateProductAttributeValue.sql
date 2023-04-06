CREATE PROCEDURE [dbo].[Znode_UpdateProductAttributeValue]
(
	@SKU				 NVARCHAR(MAX),
	@LocaleCode			 nvarchar(100),
	@AttributeCodeValues XML,
	@UserId				 INT = 0,
	@Status				 BIT OUT,
	@IsUnAssociated		 BIT = 0
)
AS    
/* ---------------------------------------------------------------------------------------------------------------
    --Summary : Update AttributeValue for specific product 
    --          Input parameter : @LocaleId , @PimAttributeCode,  @ProductId,@AttributeValue,@UserId
    --Unit Testing : 

     BEGIN TRANSACTION 
    DECLARE @Status bit 
    EXEC [Znode_UpdateProductAttributeValue]
    @SKU        = 10637,
    @LocaleCode         = 'en-US',
    @AttributeCodeValues   = 'Tropicana',
    @UserId           =2,
    @Status           =@Status OUT
    SELECT @Status 
    --SELECT zpa.AttributeCode ,ZpAVL .AttributeValue  FROM ZnodePimAttributeValueLocale ZpAVL INNER JOIN ZnodePimAttributeValue zpav ON ZpAVL.PimAttributeValueId = zpav.PimAttributeValueId
    --INNER JOIN dbo.ZnodePimAttribute zpa ON zpav.PimAttributeId = zpa.PimAttributeId
    --WHERE zpav.PimProductId =12 AND ZpAVL.LocaleId = 1 AND zpa.AttributeCode = 'ProductName' 
    ROLLBACK Transaction 
    ---------------------------------------------------------------------------------------------------------------
	*/

     BEGIN
         BEGIN TRAN UpdateAttributeValue;
         BEGIN TRY
             DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
             DECLARE @PimDefaultFamily INT= dbo.Fn_GetDefaultPimProductFamilyId();
			 DECLARE @ProductId INT, @LocaleId INT
			 
             DECLARE @TBL_PimProductId TABLE
             ( RowId INT IDENTITY(1,1),
				PimProductId INT,
				PimAttributeId INT,
                AttributeCode VARCHAR(300),
                AttributeValue NVARCHAR(MAX),
                PimAttributeDefaultValueId INT
             ); -- table holds the PimProductId 

			  DECLARE @TBL_DefaultAttributeId TABLE 
			  ( 
				PimAttributeId INT PRIMARY KEY , 
				AttributeCode VARCHAR(600)
			  )
			 SELECT TOP 1 @LocaleId  =  LocaleId  from ZnodeLocale where Code = @LocaleCode
			 If @LocaleId is null 
			 Begin
				SET @Status = 0 
				 Rollback TRAN UpdateAttributeValue;
				Return 0
			 End
		     INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM  ZnodePimAttribute a 
			 INNER JOIN ZnodeAttributeType r ON (r.AttributeTypeId = a.AttributeTypeId)
			 WHERE AttributeTypeName IN ('Simple Select', 'Multi Select')
			 		 AND IsCategory = 0 

			  DECLARE @TBL_TextAreaAttributeId TABLE 
			  ( 
				PimAttributeId INT PRIMARY KEY , 
				AttributeCode VARCHAR(600)
			  )

		     INSERT INTO @TBL_TextAreaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM  ZnodePimAttribute a 
			 INNER JOIN ZnodeAttributeType r ON (r.AttributeTypeId = a.AttributeTypeId)
			 WHERE AttributeTypeName IN ('Text Area')
			 AND IsCategory = 0 


             DECLARE @TBL_PimAttributeValueId TABLE
             (
				
				PimAttributeValueId INT,
				PimAttributeId      INT,
				PimProductId        INT,
				PimAttributeDefaultValueId int 
             );

			 SELECT @ProductId = PAV.PimProductId FROM ZnodePimAttributeValueLocale PAVL 
			 INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId = PAV.PimAttributeValueId
			 INNER JOIN ZnodePimAttribute PA ON PA.PimAttributeId = PAV.PimAttributeId
			 WHERE PA.AttributeCode = 'SKU' AND PAVL.AttributeValue = @SKU

			 INSERT INTO @TBL_PimProductId ( PimProductId, AttributeCode, AttributeValue )
			 SELECT @ProductId, Tbl.Col.value( 'AttributeCode[1]', 'NVARCHAR(max)' ) AS AttributeCode
				,Tbl.Col.value( 'AttributeValues[1]', 'NVARCHAR(max)' ) AS AttributeValue
			 FROM @AttributeCodeValues.nodes( '//ArrayOfPIMAttributeCodeValueModel/PIMAttributeCodeValueModel' ) AS Tbl(Col)

			 UPDATE @TBL_PimProductId
			 SET PimAttributeId = ZPA.PimAttributeId, PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId
			 FROM @TBL_PimProductId Tbl
			 inner JOIN ZnodePimAttribute AS ZPA ON( ZPA.AttributeCode = Tbl.AttributeCode )
			 LEFT JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPA.PimAttributeId = ZPADV.PimAttributeId and ZPADV.AttributeDefaultValueCode = Tbl.AttributeValue )
			 where ZPA.IsCategory <> 1

			 ;WITH Cte_DeleteDuplicate As
			 (
			   SELECT  Row_Number()over(Partition By PimProductId, PimAttributeId, PimAttributeDefaultValueId order by PimProductId, PimAttributeId, PimAttributeDefaultValueId, rowid )SRNO ,*
			   FROM @TBL_PimProductId 		 
			 ),
			 CTE_Last_Dataset as
			 (
				SELECT MAX(SRNO)SRNO, PimProductId, PimAttributeId, PimAttributeDefaultValueId from Cte_DeleteDuplicate
				GROUP BY PimProductId, PimAttributeId, PimAttributeDefaultValueId
			 )
			 DELETE A FROM Cte_DeleteDuplicate a
			 WHERE NOT EXISTS ( SELECT * FROM CTE_Last_Dataset b WHERE a.SRNO = b.SRNO and ISNULL(a.PimProductId,0) = ISNULL(B.PimProductId,0)  and ISNULL(a.PimAttributeId,0) = ISNULL(B.PimAttributeId,0) )
			 AND PimAttributeId IS NOT NULL

	
             IF @IsUnAssociated = 1
                 BEGIN
                     INSERT INTO @TBL_PimAttributeValueId ( PimAttributeValueId, PimAttributeId, PimProductId, PimAttributeDefaultValueId )
                     SELECT PimAttributeValueId, ZPAV.PimAttributeId, ZPAV.PimProductId, TBLP.PimAttributeDefaultValueId
                     FROM ZnodePimAttributeValue ZPAV
                     INNER JOIN @TBL_PimProductId TBLP ON(TBLP.PimProductId = ZPAV.PimProductId AND TBLP.PimAttributeId = ZPAV.PimAttributeId );

			
                     DELETE FROM ZnodePimProductAttributeDefaultValue
                     WHERE EXISTS
                     (
                         SELECT TOP 1 1
                         FROM @TBL_PimAttributeValueId TBLAP
                         WHERE TBLAP.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId
                               AND ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId = TBLAP.PimAttributeDefaultValueId --@PimAttributeDefaultValueId
                     )
                     AND LocaleId = @LocaleId;
					 

                     DELETE FROM ZnodePimAttributeValue
                     WHERE EXISTS
                     (
                         SELECT TOP 1 1
                         FROM @TBL_PimAttributeValueId TBLAP
                         WHERE TBLAP.PimAttributeValueId = ZnodePimAttributeValue.PimAttributeValueId
                               AND ZnodePimAttributeValue.PimAttributeDefaultValueId =TBLAP.PimAttributeDefaultValueId -- @PimAttributeDefaultValueId
                     );
                     --AND LocaleId = @LocaleId 					

                 END;

             INSERT INTO ZnodePimAttributeValue ( PimProductId, PimAttributeId, PimAttributeDefaultValueId, AttributeValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, PimAttributeFamilyId )
             OUTPUT INSERTED.PimAttributeValueId, INSERTED.PimAttributeId, INSERTED.PimProductId, INSERTED.PimAttributeDefaultValueId INTO @TBL_PimAttributeValueId
             SELECT DISTINCT TBPP.PimProductId, TBPP.PimAttributeId, NULL,NULL, @UserId, @GetDate, @UserId, @GetDate, @PimDefaultFamily
             FROM @TBL_PimProductId TBPP
             WHERE NOT EXISTS
             (
                SELECT TOP 1 1
                FROM ZnodePimAttributeValue ZAV
                WHERE ZAV.PimProductId = TBPP.PimProductId
                        AND ZAV.PimAttributeId = TBPP.PimAttributeId
             )
             AND @IsUnAssociated = 0 AND TBPP.PimProductId IS NOT NULL;
              
			 UPDATE ZnodePimAttributeValue 
			 SET  ModifiedBy = @UserId , 
				  ModifiedDate = @GetDate,
				  PimAttributeDefaultValueId = ZAV.PimAttributeDefaultValueId
			 OUTPUT INSERTED.PimAttributeValueId,
                    INSERTED.PimAttributeId,
                    INSERTED.PimProductId,
				    INSERTED.PimAttributeDefaultValueId
             INTO @TBL_PimAttributeValueId
			 FROM ZnodePimAttributeValue  
			 INNER JOIN @TBL_PimProductId ZAV ON ( ZAV.PimProductId = ZnodePimAttributeValue.PimProductId AND ZAV.PimAttributeId = ZnodePimAttributeValue.PimAttributeId) 
			 WHERE @IsUnAssociated = 0
			  
             UPDATE A
             SET PimAttributeDefaultValueId = C.PimAttributeDefaultValueId,
				 ModifiedBy = @UserId ,
			     ModifiedDate = @GetDate
             FROM ZnodePimProductAttributeDefaultValue A
             INNER JOIN ZnodePimAttributeValue B ON(B.PimAttributeValueId = A.PimAttributeValueId)
             INNER JOIN @TBL_PimProductId C ON(B.PimAttributeId = C.PimAttributeId AND B.PimProductId = C.PimProductId);
			
             UPDATE A
             SET AttributeValue = C.AttributeValue,
				 ModifiedBy = @UserId ,
			     ModifiedDate = @GetDate
             FROM ZnodePimAttributeValueLocale A
             INNER JOIN ZnodePimAttributeValue B ON(B.PimAttributeValueId = A.PimAttributeValueId)
             INNER JOIN @TBL_PimProductId C ON(B.PimAttributeId = C.PimAttributeId AND B.PimProductId = C.PimProductId)
			 WHERE LocaleId = @LocaleId;

             INSERT INTO ZnodePimProductAttributeDefaultValue ( PimAttributeValueId, LocaleId, PimAttributeDefaultValueId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
             SELECT TBPAV.PimAttributeValueId, @LocaleId, TBPP.PimAttributeDefaultValueId, @UserId, @GetDate, @UserId, @GetDate
             FROM @TBL_PimProductId TBPP 
			 INNER JOIN @TBL_PimAttributeValueId TBPAV ON(TBPAV.PimProductId = TBPP.PimProductId AND TBPAV.PimAttributeId = TBPP.PimAttributeId)
             WHERE @IsUnAssociated = 0
			 AND EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
			 AND NOT EXISTS ( SELECT * FROM ZnodePimProductAttributeDefaultValue Z WHERE Z.PimAttributeValueId = TBPAV.PimAttributeValueId AND Z.PimAttributeDefaultValueId = TBPP.PimAttributeDefaultValueId ) ;
			 

			  UPDATE ZPAVL
			  SET AttributeValue = PP.AttributeValue			      
			  FROM ZnodePimAttributeValueLocale ZPAVL 
			  INNER JOIN @TBL_PimAttributeValueId TBPAV ON( TBPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId AND  ZPAVL.LocaleId = @localeId )  AND @IsUnAssociated = 0
			  INNER JOIN @TBL_PimProductId PP ON ( TBPAV.PimProductId = PP.PimProductId AND TBPAV.PimAttributeId = PP.PimAttributeId ) 
																	-- AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)

             
			 INSERT INTO ZnodePimAttributeValueLocale ( PimAttributeValueId, LocaleId, AttributeValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
             SELECT DISTINCT TBPAV.PimAttributeValueId, @LocaleId, TBPP.AttributeValue, @UserId, @GetDate, @UserId, @GetDate
             FROM @TBL_PimProductId TBPP
             INNER JOIN @TBL_PimAttributeValueId TBPAV ON(TBPAV.PimProductId = TBPP.PimProductId AND TBPAV.PimAttributeId = TBPP.PimAttributeId)
             WHERE @IsUnAssociated = 0
			 AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
			 AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale TBH WHERE TBH.PimAttributeValueId = TBPAV.PimAttributeValueId AND TBH.LocaleId = @LocaleId )
			 AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_TextAreaAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
			
			  UPDATE ZPAVL
			  SET AttributeValue = PP.AttributeValue			      
			  FROM ZnodePimProductAttributeTextAreaValue ZPAVL 
			  INNER JOIN @TBL_PimAttributeValueId TBPAV ON( TBPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId AND  ZPAVL.LocaleId = @localeId )  AND @IsUnAssociated = 0
			  INNER JOIN @TBL_PimProductId PP ON ( TBPAV.PimProductId = PP.PimProductId AND TBPAV.PimAttributeId = PP.PimAttributeId ) 
																	-- AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
             
			 INSERT INTO ZnodePimProductAttributeTextAreaValue ( PimAttributeValueId, LocaleId, AttributeValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
             SELECT DISTINCT TBPAV.PimAttributeValueId, @LocaleId, TBPP.AttributeValue, @UserId, @GetDate, @UserId, @GetDate
             FROM @TBL_PimProductId TBPP
             INNER JOIN @TBL_PimAttributeValueId TBPAV ON(TBPAV.PimProductId = TBPP.PimProductId AND TBPAV.PimAttributeId = TBPP.PimAttributeId)
             WHERE @IsUnAssociated = 0
			 AND  EXISTS (SELECT TOP 1 1 FROM @TBL_TextAreaAttributeId TBL WHERE TBL.PimAttributeId = TBPP.PimAttributeId)
			 AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeTextAreaValue TBH WHERE TBH.PimAttributeValueId = TBPAV.PimAttributeValueId AND TBH.LocaleId = @LocaleId );

			 
			 SELECT @SKU SKU, AttributeCode, AttributeValue --PimProductId ,
			 FROM @TBL_PimProductId a
			 WHERE NOT EXISTS ( SELECT * FROM @TBL_PimAttributeValueId b WHERE isnull(a.PimProductId,0) = isnull(b.PimProductId,0) and a.PimAttributeId = b.PimAttributeId )--and a.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId )

			 IF NOT EXISTS ( SELECT PimProductId , AttributeCode, AttributeValue
							 FROM @TBL_PimProductId a
							 WHERE NOT EXISTS ( SELECT * FROM @TBL_PimAttributeValueId b WHERE a.PimProductId = b.PimProductId and a.PimAttributeId = b.PimAttributeId ))--and a.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId ) )
			 BEGIN
				SET @Status = 1;
				--SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];
			 END
			 ELSE 
			 BEGIN
				SET @Status = 0;
				--SELECT 1 AS ID, CAST(0 AS BIT) AS [Status];
			 END

             --SELECT 1 AS ID, CAST(1 AS BIT) AS [Status];
			 --SELECT @Status

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
 WHERE m.ProductAttributeCode = '''+@AttributeCodeAtt+'''
 ' 

 EXEC (@sqlt)

FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
END 
CLOSE Cur_AttributeDataUpdate
DEALLOCATE Cur_AttributeDataUpdate 

END 



             COMMIT TRAN UpdateAttributeValue;
         END TRY
         BEGIN CATCH

			 SELECT @SKU SKU, AttributeCode, AttributeValue --PimProductId ,
			 FROM @TBL_PimProductId a
			 WHERE NOT EXISTS ( SELECT * FROM @TBL_PimAttributeValueId b WHERE a.PimProductId = b.PimProductId and a.PimAttributeId = b.PimAttributeId )

		  --SELECT ERROR_MESSAGE ()
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			         @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdateProductAttributeValue @ProductId = '+ @ProductId+',@Status='+CAST(@Status AS VARCHAR(50))+
		             ',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@AttributeCodeValues='+CAST(@AttributeCodeValues AS NVARCHAR(MAX))+',@UserId='+CAST(@UserId AS NVARCHAR(50));
             SET @Status = 0;
             SELECT 1 AS ID,
                    CAST(0 AS BIT) AS [Status],
                    @ErrorMessage;
             ROLLBACK TRAN UpdateAttributeValue;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_UpdateProductAttributeValue',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;