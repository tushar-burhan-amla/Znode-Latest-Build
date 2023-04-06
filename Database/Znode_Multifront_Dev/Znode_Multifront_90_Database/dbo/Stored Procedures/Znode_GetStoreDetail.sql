CREATE PROCEDURE [dbo].[Znode_GetStoreDetail]
(   @WhereClause NVARCHAR(MAX),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(100)  = '',
	@StateCode   NVARCHAR(200),										 
	@RowsCount   INT OUT)
AS 
    /*
		 Summary :- this procedure is used to find the store details 
	 
		 Unit Testing 
		 EXEC Znode_GetStoreDetail '',@RowsCount = 0, @StateCode = ''
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
              DECLARE @TBL_PortalDetails TABLE (StoreName NVARCHAR(MAX),CityName VARCHAR(3000),StateName VARCHAR(3000),PhoneNumber NVARCHAR(100),PostalCode VARCHAR(50),
			 [Status] BIT,PortalAddressId INT,AddressId INT,PortalId INT,IsActive BIT,DisplayOrder INT,Address1 VARCHAR(3000),Address2 VARCHAR(3000),
             Address3 VARCHAR(3000) ,CountryName VARCHAR(3000),RowId INT,CountNo INT,PortalName NVARCHAR(MAX),MediaPath VARCHAR(MAX),FileName VARCHAR(MAX), Latitude Decimal(9,6), Longitude Decimal(9,6),StoreLocationCode NVARCHAR(200));
             SET @SQL = '
				;with Cte_GetStoreDetails AS 
				(
				 SELECT DISTINCT ZPA.StoreName,ZA.CityName,ZA.StateName,ZA.PostalCode,ZA.IsActive Status,ZA.PhoneNumber,ZPA.PortalAddressId,ZA.AddressId,
				 Zp.PortalId,ZA.IsActive,ZPA.DisplayOrder,ZA.Address1,ZA.Address2,ZA.Address3,ZA.CountryName,ZP.Storename AS PortalName,[dbo].[Fn_GetMediaThumbnailMediaPath](ZM.path) MediaPath,ZM.path FileName
				 ,zpa.Latitude, zpa.Longitude,ZPA.StoreLocationCode
				 FROM ZnodePortal ZP 
				 INNER JOIN ZnodePortalAddress ZPA ON (ZPA.portalId = ZP.PortalId) 
				 INNER JOIN ZnodeAddress ZA ON (Za.AddressId = ZPA.AddressId) 
				 LEFT JOIN ZnodeState ZS on(ZA.StateName = ZS.StateName OR ZA.StateName = ZS.StateCode)
				 --LEFT JOIN ZnodePortalAddress ZPDS ON (ZPDS.PortalId = ZP.PortalId)
				 LEFT JOIN ZnodeMedia ZM ON (ZPA.MediaId = ZM.MediaId)
				 WHERE  (ZS.StateCode =  '''+@statecode+''' OR ZS.StateName = '''+@statecode+''' OR  '''+@statecode+ ''' = ''''	)			 				
				 ) 
				 , Cte_GetStore AS 
				 (
				  SELECT  * ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo
				  FROM Cte_GetStoreDetails
				  WHERE 1=1 
				  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				 )

				 SELECT DISTINCT StoreName,CityName,StateName,PhoneNumber,PostalCode,Status,PortalAddressId,AddressId,PortalId,IsActive,DisplayOrder,Address1,Address2,Address3,CountryName,RowId,CountNo,PortalName,MediaPath,FileName,Latitude, Longitude,StoreLocationCode
				 FROM Cte_GetStore
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
          
             INSERT INTO @TBL_PortalDetails(StoreName,CityName,StateName,PhoneNumber,PostalCode,[Status],PortalAddressId,AddressId,PortalId,IsActive,
			 DisplayOrder,Address1,Address2,Address3,CountryName,RowId,CountNo,PortalName,MediaPath,FileName,Latitude, Longitude,StoreLocationCode)
             EXEC (@sql);

             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_PortalDetails), 0);

             SELECT StoreName,CityName,StateName,PostalCode,Status,PhoneNumber,PortalAddressId,AddressId,PortalId,IsActive,DisplayOrder,Address1,Address2,PortalName,
			 Address3,CountryName,MediaPath,FileName, Latitude, Longitude,StoreLocationCode FROM @TBL_PortalDetails;
			
         END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE();
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetStoreDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@StateCode = '+CAST(@StateCode AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetStoreDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;