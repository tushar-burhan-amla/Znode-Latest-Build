 CREATE PROCEDURE [dbo].[Znode_InsertUpdateDownloadableProductsOrderDetail]
(  
	@OMSDownloadableProduct  OMSDownloadableProduct READONLY,
	@UserId INT, 
	@OmsOrderDetailsId INT
)		
AS 
/*
    Summary: This procedure is used to insert data into table ZnodeOmsDownloadableProductKey
			 retrive data FROM ZnodePimDownloadableProductKey table for specific key of respective SKU
			 as per sold quantity with different key.
			  
	Unit Testing: 
	begin transaction 
	declare @p7 int
	set @p7=NULL
	declare @p12 dbo.OMSDownloadableProduct
	INSERT INTO @p12 values(123,'234234234234',4)
	INSERT INTO @p12 values(123,'WTR 07M4431',4)
		exec sp_executesql N'Znode_InsertUpdateDownloadableProductsOrderDetail  @OMSDownloadableProduct_local,@UserId',
		N'@OMSDownloadableProduct_local [dbo].[OMSDownloadableProduct] READONLY, @UserId int 
	',@OMSDownloadableProduct_local=@p12,@UserId =2 
	SELECT @p7
	rollback transaction 


*/
BEGIN
BEGIN TRY
    SET NOCOUNT ON;
	Declare @RowCount int, @StartWith int ,@ItemNo int ,  @Quantity int , @SQLQuery  nvarchar(max),@SKU nvarchar(300), @OmsOrderLineItemsId int 
	Declare @SKUSoldWithKeys TABLE (SKU nvarchar(300),ProductName nvarchar(300),PimDownloadableProductKeyId int,DownloadableProductKey nvarchar(250),DownloadableProductURL nvarchar(2000))
	Declare @SKUSoldWithKeysOutPut TABLE (SKU nvarchar(300),ProductName nvarchar(300), PimDownloadableProductKeyId int,DownloadableProductKey nvarchar(250),DownloadableProductURL nvarchar(2000),[OmsOrderLineItemsId] [int] )

	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	SET @StartWith  =1 

	Declare @OMSDownloadableProduct_local AS TABLE
	(
		[RowNum] [int]  Identity(1,1),
		[OmsOrderLineItemsId] [int] NULL,
		[SKU] [varchar](300) NULL,
		[Quantity] [numeric](26, 8) NULL
	)
	
	INSERT INTO @OMSDownloadableProduct_local
	SELECT * FROM @OMSDownloadableProduct

	---Updating lineitemid on basis of sku and order details id
	UPDATE a SET OmsOrderLineItemsId = b.OmsOrderLineItemsId
	FROM @OMSDownloadableProduct_local a
	INNER JOIN ZnodeOmsOrderLineItems b WITH (NOLOCK) ON a.SKU = b.Sku
	WHERE b.ParentOmsOrderLineItemsId IS NOT NULL AND B.OmsOrderDetailsId = @OmsOrderDetailsId

	SELECT @RowCount = MAx(RowNum) FROM @OMSDownloadableProduct_local 
	While @StartWith <= @RowCount 
	Begin
		SET @SKU = ''
		SET @Quantity =  0 
		SET @OmsOrderLineItemsId = 0 

		SELECT @Quantity = Quantity ,@SKU = SKU , @OmsOrderLineItemsId = [OmsOrderLineItemsId]  FROM @OMSDownloadableProduct_local where RowNum = @StartWith 

		SET @SQLQuery   = 
		'SELECT TOP ' + Convert(Varchar(10), @Quantity ) + ' ZPDP.SKU,ZPDP.ProductName, ZPDPK.PimDownloadableProductKeyId, ZPDPK.DownloadableProductKey,ZPDPK.DownloadableProductURL
		FROM ZnodePimDownloadableProduct ZPDP WITH (NOLOCK) INNER JOIN  ZnodePimDownloadableProductKey ZPDPK WITH (NOLOCK)  ON 
		ZPDP.PimDownloadableProductId =ZPDPK.PimDownloadableProductId where ZPDP.SKU  =''' +  @SKU + ''' AND ZPDPK.IsUsed = 0 '

		INSERT INTO @SKUSoldWithKeys 
		EXEC sys.sp_sqlexec @SQLQuery;

		INSERT INTO @SKUSoldWithKeysOutPut(SKU,ProductName,PimDownloadableProductKeyId,DownloadableProductKey,DownloadableProductURL,[OmsOrderLineItemsId]) 
		SELECT ssk.SKU,ProductName,PimDownloadableProductKeyId,DownloadableProductKey,DownloadableProductURL,@OmsOrderLineItemsId  
		FROM @SKUSoldWithKeys ssk
		INNER JOIN @OMSDownloadableProduct_local ODL ON (ssk.SKU = ODL.SKU AND ODL.OmsOrderLineItemsId = @OmsOrderLineItemsId)

		INSERT INTO ZnodeOmsDownloadableProductKey (OmsOrderLineItemsId,PimDownloadableProductKeyId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @OmsOrderLineItemsId, PimDownloadableProductKeyId,@UserId,@GetDate,@UserId,@GetDate  FROM @SKUSoldWithKeysOutPut ssko
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsDownloadableProductKey ODP WITH (NOLOCK) WHERE ODP.OmsOrderLineItemsId =  @OmsOrderLineItemsId 
		AND ODP.PimDownloadableProductKeyId = ssko.PimDownloadableProductKeyId)
										
		UPDATE PDP SET PDP.IsUsed =1 
		FROM @SKUSoldWithKeysOutPut SSWK 
		INNER JOIN ZnodePimDownloadableProductKey PDP ON SSWK.PimDownloadableProductKeyId = PDP.PimDownloadableProductKeyId

		SET @StartWith  = @StartWith  + 1 
	End   
	SELECT * FROM @SKUSoldWithKeysOutPut 	
END TRY
BEGIN CATCH
DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateDownloadableProductsOrderDetail @OMSDownloadableProduct_local = '
			 
              			 
    SELECT 0 AS ID,CAST(0 AS BIT) AS Status,ERROR_MESSAGE();                    
		  
    EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_InsertUpdateDownloadableProductsOrderDetail',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;

END CATCH

END