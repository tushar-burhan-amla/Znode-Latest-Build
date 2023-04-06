CREATE PROCEDURE [dbo].[Znode_ManageLinkProductList]
(   @WhereClause      XML,
    @Rows             INT          = 100,
    @PageNo           INT          = 1,
    @Order_BY         VARCHAR(100) = '',
    @RowsCount        INT OUT,
    @LocaleId         INT          = 1,
    @RelatedProductId INT          = 0,
    @PimAttributeId   INT          = 0)
AS
   /*  Summary :- This Procedure is used to find the link product Detail
     Unit Testing
     EXEC Znode_ManageLinkProductList '' , @RowsCount = 0 ,@RelatedProductId=128
   */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @PimProductIds TransferId, --VARCHAR(MAX),
					 @PimAttributeIds VARCHAR(MAX),
					 @OutPimProductIds VARCHAR(max);
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TransferPimProductId TransferId
			 DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
			 INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()

		     DECLARE @TBL_LinkProductDetail TABLE
             (PimProductId           INT,
              PimLinkProductDetailId INT,
              RelatedProductId       INT,
              PimAttributeId         INT,
			  DisplayOrder			 INT
             );
             DECLARE @TBL_AttributeDefaultValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder INT
             );
             DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
             DECLARE @FamilyDetails TABLE
             (PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(3000)
             );
             DECLARE @DefaultAttributeFamily INT=
             (
                 SELECT PimAttributeFamilyId
                 FROM ZnodePimAttributeFamily
                 WHERE IsCategory = 0
                       AND IsDefaultFamily = 1
             );
             DECLARE @ProductIdTable TABLE
             (PimProductId INT,
              CountId      INT,
              RowId        INT IDENTITY(1,1)
             );
             INSERT INTO @TBL_LinkProductDetail
             (PimProductId,
              PimLinkProductDetailId,
              RelatedProductId,
              PimAttributeId,
			  DisplayOrder
             )
                    SELECT PimProductId,
                           PimLinkProductDetailId,
                           PimParentProductId,
                           PimAttributeId,
						   DisplayOrder
                    FROM ZnodePimLinkProductDetail
                    WHERE PimParentProductId = @RelatedProductId
                          AND PimAttributeId = @PimAttributeId;

				INSERT INTO @TransferPimProductId
                SELECT PimProductId
                FROM @TBL_LinkProductDetail

			 IF NOT EXISTS (SELECT TOP 1 1 FROM @TransferPimProductId)
			 BEGIN
			  INSERT INTO @TransferPimProductId
			  SELECT '-1'

             END
			 DECLARE @AttributeCode NVARCHAR(max)
			 SET @AttributeCode = SUBSTRING ((SELECT ','+AttributeCode FROM [dbo].[Fn_GetProductGridAttributes]()  WHERE AttributeCode NOT IN ('AttributeFamily') FOR XML PATH('') ),2,4000)

			 EXEC Znode_GetProductIdForPaging
                  @whereClauseXML = @WhereClause,
                  @Rows = @Rows,
                  @PageNo = @PageNo,
                  @Order_BY = @Order_BY,
                  @RowsCount = @RowsCount OUT,
                  @LocaleId = @LocaleId,
                  @AttributeCode = @AttributeCode,
                  @PimProductId = @TransferPimProductId,
                  @IsProductNotIn = 0,
				  @OutProductId = @OutPimProductIds OUT;


			 INSERT INTO @ProductIdTable
             (PimProductId)
			 SELECT item
			 FROM dbo.split(@OutPimProductIds,',') SP

			 --SET @PimProductIds = SUBSTRING(
    --                                       (
    --                                           SELECT ','+CAST(PimProductId AS VARCHAR(100))
    --                                           FROM @ProductIdTable
    --                                           FOR XML PATH('')
    --                                       ), 2, 4000);

			INSERT INTO @PimProductIds ( Id )
			SELECT item
			FROM dbo.split(@OutPimProductIds,',') SP

             SET @PimAttributeIds = SUBSTRING( (SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM [dbo].[Fn_GetProductGridAttributes]() FOR XML PATH ('') ),2,4000);

			 INSERT INTO @TBL_AttributeDefaultValue
             (PimAttributeId,
              AttributeDefaultValueCode,
              IsEditable,
              AttributeDefaultValue,
			  DisplayOrder
             )
             EXEC Znode_GetAttributeDefaultValueLocale
                  @PimAttributeIds,
                  @LocaleId;
             INSERT INTO @TBL_AttributeDetails
             (PimProductId,
              AttributeValue,
              AttributeCode,
              PimAttributeId
             )
             EXEC [Znode_GetProductsAttributeValue]
                  @PimProductIds,
                  @PimAttributeIds,
                  @localeId;
             WITH Cte_UpdateDefaultAttributeValue
                  AS (SELECT PimProductId,
                             AttributeCode,
                             AttributeValue,
                             SUBSTRING(
                                      (
                                          SELECT ','+TBADV.AttributeDefaultValue
                                          FROM @TBL_AttributeDefaultValue TBADV
                                               INNER JOIN ZnodePimAttribute TBAC ON(TBADV.PimAttributeId = TBAC.PimAttributeId)
                                          WHERE TBAC.AttributeCode = TBAD.AttributeCode
                                                AND EXISTS
                                          (
                                              SELECT TOP 1 1
                                              FROM dbo.split(TBAD.AttributeValue, ',') SP
                                              WHERE Sp.item = TBADV.AttributeDefaultValueCode
                                          )
                                          FOR XML PATH('')
                                      ), 2, 4000) AttributeDefaultValue
                      FROM @TBL_AttributeDetails TBAD)
                  UPDATE TBAD
                    SET AttributeValue = CTUDAV.AttributeDefaultValue
                  FROM @TBL_AttributeDetails TBAD
                       INNER JOIN Cte_UpdateDefaultAttributeValue CTUDAV ON(CTUDAV.PimProductId = TBAD.PimProductId
                                                                            AND CTUDAV.AttributeCode = TBAD.AttributeCode)
                  WHERE AttributeDefaultValue IS NOT NULL;
             INSERT INTO @FamilyDetails
             (PimAttributeFamilyId,
              PimProductId
             )
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId]
                  @PimProductIds,
                  1;
             UPDATE a
               SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                       AND LocaleId = @LocaleId);
             UPDATE a
               SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                       AND LocaleId = @DefaultLocaleId)
             WHERE a.FamilyName IS NULL
                   OR a.FamilyName = '';

			 ;WITH Cte_ProductMedia
               AS (SELECT TBA.PimProductId , TBA.PimAttributeId
			  , SUBSTRING( ( SELECT ','+ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'+ zm.PATH  
			   FROM ZnodeMedia AS ZM
               INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
			   INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
			   INNER JOIN @TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)
			   WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId
			   FOR XML PATH('') ), 2 , 4000) AS AttributeValue
			   FROM @TBL_AttributeDetails AS TBA
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId ))

		      UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue
			  FROM @TBL_AttributeDetails TBAV
			  INNER JOIN Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId
			  AND CTPM.PimAttributeId = TBAV.PimAttributeId;



			INSERT INTO @TBL_AttributeDetails             (PimProductId,              AttributeValue,              AttributeCode,              PimAttributeId             )
			SELECT PimProductId ,FamilyName, 'AttributeFamily',NULL
			FROM @FamilyDetails

				-- LEFT JOIN @FamilyDetails AS zf ON(zf.PimProductId = zpp.PimProductId)
             --- Update the  product families name locale wise
        UPDATE  @TBL_AttributeDetails SET PimAttributeId = 0 WHERE PimAttributeId IS nULL
	     DECLARE @ProductXML XML

		--SELECT * FROM @TBL_AttributeDetails

	   	 SET @ProductXML =   '<MainProduct>'+ STUFF( (  SELECT '<Product>'
		                                                    +'<PimLinkProductDetailId>'+CAST(ISNULL(TBLPD.PimLinkProductDetailId,'') AS VARCHAR(50))+'</PimLinkProductDetailId>'
															+'<PimProductId>'+CAST(zpp.PimProductId AS VARCHAR(50))+'</PimProductId>'
															+'<RelatedProductId>'+CAST(ISNULL(TBLPD.RelatedProductId,'') AS VARCHAR(50))+'</RelatedProductId>'
															+'<DisplayOrder>'+CAST(ISNULL(TBLPD.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'
		 + STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST( (SELECT ''+TBADI.AttributeValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'
															FROM @TBL_AttributeDetails TBADI
															 WHERE TBADI.PimProductId = zpp.PimProductId
															 ORDER BY TBADI.PimProductId DESC
															 FOR XML PATH (''), TYPE
																).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Product>'

			 FROM @ProductIdTable AS zpp
             LEFT JOIN @TBL_LinkProductDetail AS TBLPD ON(TBLPD.PimProductId = ZPP.PimProductId)
             ORDER BY zpp.RowId
			FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainProduct>'
			--FOR XML PATH ('MainProduct'))


			SELECT  @ProductXML  ProductXMl

		     SELECT AttributeCode ,  ZPAL.AttributeName
			 FROM ZnodePimAttribute ZPA
			 LEFT JOIN ZnodePiMAttributeLOcale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )
             WHERE LocaleId = 1
			 AND  IsCategory = 0
			 AND ZPA.IsShowOnGrid = 1
			 UNION ALL
			 SELECT 'PublishStatus','Publish Status'


			SELECT @RowsCount AS RowsCount;













			 --SELECT zpp.PimProductid AS ProductId,
    --                [ProductName],
    --                ProductType,
    --                ISNULL(zf.FamilyName, '') AS AttributeFamily,
    --                [SKU],
    --                [Price],
    --                [Quantity],
    --                CASE
    --                    WHEN [IsActive] IS NULL
    --                    THEN CAST(0 AS BIT)
    --                    ELSE CAST([IsActive] AS BIT)
    --                END AS [IsActive],
    --                PimLinkProductDetailId,
    --                RelatedProductId,
    --                TBLPD.PimAttributeId,
    --                [dbo].FN_GetMediaThumbnailMediaPath(zm.Path) AS ImagePath,
    --                [Assortment],
    --                @LocaleId AS LocaleId,
    --                [DisplayOrder]
    --         FROM @ProductIdTable AS zpp
    --              INNER JOIN @TBL_LinkProductDetail AS TBLPD ON(TBLPD.PimProductId = ZPP.PimProductId)
    --              LEFT JOIN @FamilyDetails AS zf ON(zf.PimProductId = zpp.PimProductId)
    --              INNER JOIN
    --         (
    --             SELECT PimProductId,
    --                    AttributeValue,
    --                    AttributeCode
    --             FROM @TBL_AttributeDetails
    --         ) TB PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName],
    --                                                             [SKU],
    --                                                             [Price],
    --                                                             [Quantity],
    --                                                             [IsActive],
    --                                                             [ProductType],
    --                                                             [ProductImage],
    --                                                             [Assortment],
    --                                                             [DisplayOrder])) AS Piv ON(Piv.PimProductId = zpp.PimProductid)
    --              LEFT JOIN ZnodeMedia AS zm ON(zm.MediaId = piv.[ProductImage])
    --         ORDER BY zpp.RowId;
         END TRY
         BEGIN CATCH

              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageLinkProductList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@RelatedProductId='+CAST(@RelatedProductId AS VARCHAR(50))+',@PimAttributeId='+CAST(@PimAttributeId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));

             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageLinkProductList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;