CREATE  PROCEDURE [dbo].[Znode_GetWarehouseList]
(   @WhereClause  VARCHAR(MAX),
    @Rows         INT          = 100,
    @PageNo       INT          = 1,
    @Order_BY     VARCHAR(100) = '',
	@RowsCount    INT OUT
    )
AS
   
     BEGIN
         SET NOCOUNT ON;
		 
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_WarehouseDetail TABLE
			 (WarehouseId			INT,
			 WarehouseCode VARCHAR(100),
			 WarehouseName NVARCHAR(1000),
			 AddressId INT,
			 CountryName		VARCHAR(200) ,
			 StateName		VARCHAR(200) ,
			 CityName VARCHAR(200) ,
			 PostalCode VARCHAR(200) ,
			 IsActive			BIT,
			 RowId				INT,
			 CountNo			INT
			 )

             SET @SQL = '
						;with Cte_portalcountry AS 
						(
							SELECT        ZW.WarehouseCode, ZW.WarehouseName,ZW.WarehouseId , ZWA.AddressId, ZA.CountryName, ZA.StateName, ZA.CityName, ZA.PostalCode
							FROM           ZnodeAddress ZA INNER JOIN ZnodeWarehouseAddress ZWA ON ZA.AddressId = ZWA.AddressId INNER JOIN ZnodeWarehouse ZW ON ZWA.WarehouseId = ZW.WarehouseId
					     )
						,Cte_filterPortalCountry AS 
						(
						SELECT WarehouseId,WarehouseCode,CountryName,WarehouseName,AddressId,StateName,CityName,PostalCode
						,'+dbo.Fn_GetPagingRowId(@Order_By,'WarehouseId')+',Count(*)Over() CountNo 
						FROM Cte_portalcountry where 1=1  '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
						)

						SELECT  WarehouseId,WarehouseCode,CountryName,WarehouseName,AddressId,StateName,CityName,PostalCode,RowId,CountNo
						FROM Cte_filterPortalCountry
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@rows)
						print @SQL
			 INSERT INTO @TBL_WarehouseDetail (WarehouseId,WarehouseCode,CountryName,WarehouseName,AddressId,StateName,CityName,PostalCode,RowId,CountNo )
			 EXEC(@SQL)

			 SET @RowsCount =ISNULL((SELECT TOP 1 CountNo  FROM @TBL_WarehouseDetail),0)
			 SELECT WarehouseId,WarehouseCode,CountryName,WarehouseName,AddressId,StateName,CityName,PostalCode
			 FROM @TBL_WarehouseDetail
			 	 
         END TRY
         BEGIN CATCH
          DECLARE @Status BIT ;
		  SET @Status = 0;
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetWarehouseList @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  

          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetWarehouseList',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
     END;