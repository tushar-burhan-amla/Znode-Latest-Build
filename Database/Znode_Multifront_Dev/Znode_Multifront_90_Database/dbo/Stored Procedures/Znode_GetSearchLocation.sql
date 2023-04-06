CREATE PROCEDURE [dbo].[Znode_GetSearchLocation](@PortalId	int, @Keywords	nvarchar(max))
AS
	/*
	  Summary:  
	*/
BEGIN
	BEGIN TRY

	  SELECT ZA.*, 
	       Isnull(ZA.FirstName,'') + ' ' + Isnull(ZA.LastName,'') + ' ' + Isnull(	ZA.DisplayName	,'') + ' ' + Isnull(ZA.CompanyName	,'')
		+ ' ' + Isnull(ZA.Address1	,'') + ' ' + Isnull(ZA.Address2	,'') + ' '  + Isnull(ZA.Address3	,'') + ' ' + Isnull(ZA.CountryName	,'') 
		+ ' ' + Isnull(ZA.StateName	,'') + ' ' + Isnull(ZA.CityName	,'') + ' ' + Isnull(ZA.PostalCode	,'') 
		+ ' ' + Isnull(ZA.PhoneNumber	,'') + ' ' + Isnull(ZA.Mobilenumber	,'') + ' ' + Isnull(ZA.AlternateMobileNumber	,'') 
		+ ' ' + Isnull(ZA.FaxNumber,'') SeachColumn
	  INTO #PortalAddress1  
	  FROM ZnodeAddress ZA 
	  INNER JOIN ZnodePortalAddress ZPA ON ZA.AddressId = ZPA.AddressId
	  WHERE ZPA.PortalId = @PortalId 	AND ZA.IsActive = 1

	 DECLARE @SeachText VARCHAR(MAX), @SQL VARCHAR(1000)
	 SELECT @SeachText = COALESCE(@SeachText + ' or ', '') +'SeachColumn like ''%'+item+'%''' FROM dbo.Split (replace(@Keywords,' ' ,','),',') WHERE Item <> ''
	 SET @sql = 'Select AddressId,	FirstName,	LastName,	DisplayName,	CompanyName,	Address1,	Address2,	Address3,	CountryName,	StateName,	CityName,	
					    PostalCode,	PhoneNumber,	Mobilenumber,	AlternateMobileNumber,	FaxNumber,	IsDefaultBilling,	IsDefaultShipping,	IsActive,	ExternalId 
				 FROM #PortalAddress1  ZA WHERE 1=1 '+ CASE WHEN isnull(@SeachText,'') = '' THEN '' ELSE 'AND '+@SeachText END

	 EXEC (@sql)

		
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSearchLocation @PortalId = '+CAST(@PortalId AS VARCHAR(20))+',@Keywords='+CAST(@Keywords AS VARCHAR(50));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSearchLocation',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;