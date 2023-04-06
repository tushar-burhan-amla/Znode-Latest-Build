CREATE PROCEDURE [dbo].[Znode_GetVendorList]
(   @WhereClause NVARCHAR(3000),
    @Rows        INT            = 10,
    @PageNo      INT            = 1,
    @Order_BY    VARCHAR(1000)  = '',
    @RowsCount   INT OUT			 ,
	@LocaleId    INT 	)
AS 
/*
    Summary: This procedure is used to find the vendor list
	Unit Testing: 
     EXEC Znode_GetVendorList '' ,@RowsCount = 0  ,@LocaleId= 1 
	*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX), @RowsStart VARCHAR(50), @RowsEnd VARCHAR(50);
             SET @SQL = '
		DECLARE @TBL_VendorWithAddress TABLE (PimVendorId INT , VendorCode VARCHAR(600),VendorName NVARCHAR(max), CompanyName  NVARCHAR(2000), Email  VARCHAR(100),ExternalVendorNo Nvarchar(600),IsActive BIT ,PhoneNumber VARCHAR(50),FullName NVARCHAR(1000),RowId INT )
		DECLARE @PimAttributeId INT = [dbo].[Fn_GetProductVendorAttributeId]()
		DECLARE @TBL_DefaultValue TABLE (PimAttributeId INT ,AttributeDefaultValueCode NVARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT)

		INSERT INTO @TBL_DefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
		EXEC Znode_GetAttributeDefaultValueLocale @PimAttributeId,'+CAST(@LocaleId As VARCHAR(50))+'


		;with Cte_VendorWithAddress AS 
		(
		SELECT DISTINCT PimVendorId, VendorCode ,AttributeDefaultValue VendorName, ZPV.CompanyName , Email ,ExternalVendorNo,ZPV.IsActive ,ZA.PhoneNumber, CASE WHEN ZA.FirstName IS NULL THEN '''' ELSE ZA.FirstName END + CASE WHEN ZA.LastName IS NULL THEN '''' ELSE '' ''+ZA.LastName END FullName
		FROM [dbo].[ZnodePimVendor]  ZPV 
		INNER JOIN @TBL_DefaultValue TBDV ON (TBDV.AttributeDefaultValueCode = ZPV.VendorCode )
		LEFT JOIN ZnodeAddress ZA ON (ZPV.AddressId = ZA.AddressId)
		)
		INSERT INTO @TBL_VendorWithAddress
		SELECT *,DENSE_RANK()OVER(ORDER BY '+CASE
                                                   WHEN @Order_BY = ''
                                                   THEN ''
                                                   ELSE @Order_BY+','
                                               END+' PimVendorId DESC)   ROWId
		FROM  Cte_VendorWithAddress CTVWA
		'+CASE
                WHEN @WhereClause = ''
                THEN ''
                ELSE ' WHERE '+@WhereClause
            END+'
		SELECT @Count= COUNT (1) FROM @TBL_VendorWithAddress

		SELECT PimVendorId , VendorCode,VendorName, CompanyName  , Email  ,ExternalVendorNo,IsActive  ,FullName ,PhoneNumber   FROM   @TBL_VendorWithAddress '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows);
             EXEC Sp_Executesql
                  @SQL,
                  N' @Count INT OUT ',
                  @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetVendorList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetVendorList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;