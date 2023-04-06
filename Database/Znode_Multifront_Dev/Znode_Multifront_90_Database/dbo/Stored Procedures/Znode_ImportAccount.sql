CREATE PROCEDURE [dbo].[Znode_ImportAccount](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200),@PortalId int 
	  ,@CsvColumnString nvarchar(max) )
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max),@IsAllowGlobalLevelUserCreation nvarchar(10)

		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	
		-- Three type of import required three table varible for product , category and brand
		
		select A.CountryCode,B.StateName,B.StateId,B.StateCode
        into #ZnodeCountryState
		from ZnodePortalCountry A
		inner JOin ZnodeState B on B.CountryCode=A.CountryCode
		Where B.CountryCode in ('US','CA')
		
		CREATE TABLE #InsertAccount 
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int,ParentAccountCode nvarchar(max),AccountName nvarchar(max), AccountCode nvarchar(max),ExternalID nvarchar(max),
			CatalogCode nvarchar(max),AddressName nvarchar(max),FirstName varchar(max),LastName varchar(max),CompanyName varchar(max),
			Address1 varchar(max),Address2 varchar(max),CountryName varchar(max),StateName varchar(max),CityName varchar(max),
			PostalCode varchar(max),PhoneNumber varchar(max),IsDefaultBilling varchar(10),IsDefaultShipping varchar(10),GUID VARCHAR(100)
		);
	
		SET @SSQL = ' INSERT INTO #InsertAccount ( RowNumber, ParentAccountCode,AccountName ,AccountCode,ExternalID,CatalogCode,AddressName,FirstName,LastName,CompanyName
						,Address1,Address2,CountryName,StateName,CityName,PostalCode,PhoneNumber,IsDefaultBilling,IsDefaultShipping,GUID )
		SELECT RowNumber, ParentAccountCode,AccountName ,AccountCode,ExternalID,CatalogCode,AddressName,FirstName,LastName,CompanyName
						,Address1,Address2,CountryName,StateName,CityName,PostalCode,PhoneNumber,IsDefaultBilling,IsDefaultShipping,GUID FROM '+ @TableName;

		EXEC sys.sp_sqlexec @SSQL;

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.AccountCode, '')) >100 and ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '86', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''
		and not exists(select * from znodePortalAccount ZPA inner join ZnodeAccount ZA ON ZPA.AccountId = ZA.AccountId where ii.ParentAccountCode = ZA.AccountCode and ZPA.PortalId = @PortalId)
		and not exists(select * from #InsertAccount IA where ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') = ISnull(ltrim(rtrim(IA.AccountCode)), '') and ii.RowNumber > IA.RowNumber)
	
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '87', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''
		and exists(select * from ZnodeAccount ZA where ii.ParentAccountCode = ZA.AccountCode and ZA.ParentAccountId is not null)
		--and not exists(select * from #InsertAccount IA where ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') = ISnull(ltrim(rtrim(IA.AccountCode)), '') and ii.RowNumber > IA.RowNumber)

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '89', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''
		and exists(select * from ZnodeAccount ZA where ii.AccountCode = ZA.AccountCode and ZA.ParentAccountId is not null)
		and exists(select * from ZnodeAccount ZA 
				inner join ZnodeAccount ZA1 on ZA.ParentAccountId = ZA1.accountId 
				 where ii.ParentAccountCode <> ZA1.AccountCode)
		and not exists(select * from #InsertAccount IA where ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') = ISnull(ltrim(rtrim(IA.AccountCode)), '')
		and ii.RowNumber > IA.RowNumber )
		

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'AccountCode', AccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.AccountCode)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'AccountCode', AccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.AccountCode, '')) >100 and ISnull(ltrim(rtrim(ii.AccountCode)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '79', 'AccountCode', AccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ii.AccountCode like '%[^a-zA-Z0-9]%' and ISnull(ltrim(rtrim(ii.AccountCode)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'AccountName', AccountName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.AccountName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'AccountName', AccountName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.AccountName, '')) >100 and ISnull(ltrim(rtrim(ii.AccountName)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '52', 'AccountName', AccountName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE isnull(replace(ii.AccountName,' ',''),'') like '%[^a-Z0-9]%' and ISnull(ltrim(rtrim(ii.AccountName)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'StateName', StateName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.StateName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'StateName', StateName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ltrim(rtrim(ii.StateName)), '')) >100 and ISnull(ltrim(rtrim(ii.StateName)), '') <> ''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '116', 'StateName', StateName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE not exists(select * from ZnodeState ZS where ZS.StateName = ISnull(ltrim(rtrim(ii.StateName)), '')
		and  exists (select * from #ZnodeCountryState Y where Y.CountryCode=zs.CountryCode and ii.StateName=y.StateName))
		and ISnull(ltrim(rtrim(ii.StateName)), '') <> '' and ii.CountryName in ('United States','Canada')

        INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '83', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE not exists(select * from ZnodeCountry ZC where ZC.CountryName = ISnull(ltrim(rtrim(ii.CountryName)), '') ) AND ISnull(ltrim(rtrim(ii.CountryName)), '') <> ''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.CountryName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '90', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.CountryName)), '') <> ''
		and not exists(select * from ZnodePortalCountry ZPC inner join ZnodeCountry ZC ON ZPC.CountryCode = ZC.CountryCode
		    where PortalId = @PortalId and ltrim(rtrim(ii.CountryName)) = ltrim(rtrim(ZC.CountryName)))

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'CityName', CityName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.CityName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'CityName', CityName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ltrim(rtrim(ii.CityName)), '')) >100 and ISnull(ltrim(rtrim(ii.CityName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'PostalCode', PostalCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.PostalCode)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'PostalCode', PostalCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ltrim(rtrim(ii.PostalCode)), '')) >100 and ISnull(ltrim(rtrim(ii.PostalCode)), '') <> ''
				
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'PhoneNumber', PhoneNumber, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.PhoneNumber)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '78', 'PhoneNumber', PhoneNumber, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ltrim(rtrim(ii.PhoneNumber)), '')) >100 and ISnull(ltrim(rtrim(ii.PhoneNumber)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'IsDefaultBilling', IsDefaultBilling, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.IsDefaultBilling)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'IsDefaultBilling', IsDefaultBilling, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertAccount AS ii  
		WHERE ISnull(ltrim(rtrim(ii.IsDefaultBilling)),'') not in ('True','1','FALSE','0') and ISnull(ltrim(rtrim(ii.IsDefaultBilling)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '91', 'IsDefaultBilling', IsDefaultBilling, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertAccount AS ii  
		WHERE exists(select * from ZnodeAccount ZA where ii.AccountCode = ZA.AccountCode)  
		and ISnull(ltrim(rtrim(ii.IsDefaultBilling)), '') <> '' and ISnull(ltrim(rtrim(ii.IsDefaultBilling)),'') in ('FALSE','0')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'IsDefaultShipping', IsDefaultShipping, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.IsDefaultShipping)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'IsDefaultShipping', IsDefaultShipping, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertAccount AS ii  
		WHERE ISnull(ltrim(rtrim(ii.IsDefaultShipping)),'') not in ('True','1','FALSE','0') and ISnull(ltrim(rtrim(ii.IsDefaultShipping)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '91', 'IsDefaultShipping', IsDefaultShipping, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertAccount AS ii  
		WHERE exists(select * from ZnodeAccount ZA where ii.AccountCode = ZA.AccountCode)  
		and ISnull(ltrim(rtrim(ii.IsDefaultShipping)), '') <> '' and ISnull(ltrim(rtrim(ii.IsDefaultShipping)),'') in ('FALSE','0')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'AddressName', AddressName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.AddressName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '81', 'AddressName', AddressName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.AddressName, '')) >200 and ISnull(ltrim(rtrim(ii.AddressName)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'Address1', Address1, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.Address1)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '81', 'Address1', Address1, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.Address1, '')) >200 and ISnull(ltrim(rtrim(ii.Address1)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '82', 'CompanyName', CompanyName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.CompanyName, '')) >300 and ISnull(ltrim(rtrim(ii.CompanyName)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'LastName', LastName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.LastName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '82', 'LastName', LastName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ltrim(rtrim(ii.LastName)), '')) >300 and ISnull(ltrim(rtrim(ii.LastName)), '') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'FirstName', FirstName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.FirstName)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '82', 'FirstName', FirstName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.FirstName, '')) >300 and ISnull(ltrim(rtrim(ii.FirstName)), '') <> ''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '80', 'CatalogCode', CatalogCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE not exists(select * from ZnodePimCatalog ZS where ZS.CatalogCode = ISnull(ltrim(rtrim(ii.CatalogCode)), '') )
		and ISnull(ltrim(rtrim(ii.CatalogCode)), '') <> ''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'CatalogCode', CatalogCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.CatalogCode)), '') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '82', 'ExternalID', ExternalID, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE len(ISnull(ii.ExternalID, '')) >300 and ISnull(ltrim(rtrim(ii.ExternalID)), '') <> ''

		-- -- error log when atleast db have 
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '85', 'IsDefaultBilling/IsDefaultShipping', IsDefaultBilling +'/'+ IsDefaultShipping, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount IC where not exists (
		SELECT TOP 1 1  from ZnodeAccount ZAA where IC.AccountCode = ZAA.AccountCode )
		and (IC.IsDefaultBilling not in ('1', 'true') or (IsDefaultShipping not in ('1', 'true')))  
		
		UPDATE ZIL
		SET ZIL.ColumnName =   ZIL.ColumnName + ' [ AccountCode - ' + ISNULL(AccountCode,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InsertAccount IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		DELETE FROM #InsertAccount
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber IS NOT NULL 
		);

		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertAccount
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End
		CREATE TABLE #InsertedAccount (AccountId int, Accountcode nvarchar(100)) 
		
		INSERT INTO ZnodeAccount(AccountCode,ParentAccountId,Name,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		OUTPUT INSERTED.AccountId, INSERTED.Accountcode INTO  #InsertedAccount (AccountId, Accountcode) 
		select IC.AccountCode, null as ParentAccountId, AccountName, IC.ExternalID, @UserId, @GetDate, @UserId, @GetDate
		from #InsertAccount IC
		where not exists(select * from ZnodeAccount ZA1 where ZA1.AccountCode = IC.AccountCode)
		and ISnull(ltrim(rtrim(IC.ParentAccountCode)), '') = '' 

		INSERT INTO ZnodePortalAccount(PortalId,AccountId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @PortalId, IA.AccountId,  @UserId, @GetDate, @UserId, @GetDate
		FROM #InsertedAccount IA 
		WHERE not exists(select * from ZnodePortalAccount ZPA where IA.AccountId = ZPA.AccountId )

		----Import Child Account where Parent account details having in same CSV ****Start
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '86', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''
		and not exists(select * from znodePortalAccount ZPA inner join ZnodeAccount ZA ON ZPA.AccountId = ZA.AccountId where ii.ParentAccountCode = ZA.AccountCode and ZPA.PortalId = @PortalId)
		and not exists(select * from #InsertAccount IA where ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') = ISnull(ltrim(rtrim(IA.AccountCode)), '') and ii.RowNumber > IA.RowNumber)
	
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '87', 'ParentAccountCode', ParentAccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertAccount AS ii
		WHERE ISnull(ltrim(rtrim(ii.ParentAccountCode)), '') <> ''
		and exists(select * from ZnodeAccount ZA where ii.ParentAccountCode = ZA.AccountCode and ZA.ParentAccountId is not null)

		UPDATE ZIL
		SET ZIL.ColumnName =   ZIL.ColumnName + ' [ AccountCode - ' + ISNULL(AccountCode,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InsertAccount IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		DELETE FROM #InsertAccount
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber IS NOT NULL 
		);

		INSERT INTO ZnodeAccount(AccountCode,ParentAccountId,Name,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		OUTPUT INSERTED.AccountId, INSERTED.Accountcode INTO  #InsertedAccount (AccountId, Accountcode) 
		select IC.AccountCode, za.AccountId as ParentAccountId, AccountName, IC.ExternalID, @UserId, @GetDate, @UserId, @GetDate
		from #InsertAccount IC
		left join ZnodeAccount za on IC.ParentAccountCode = za.AccountCode
		where not exists(select * from ZnodeAccount ZA1 where ZA1.AccountCode = IC.AccountCode)
		and ISnull(ltrim(rtrim(IC.ParentAccountCode)), '') <> ''

		INSERT INTO ZnodePortalAccount(PortalId,AccountId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @PortalId, IA.AccountId,  @UserId, @GetDate, @UserId, @GetDate
		FROM #InsertedAccount IA 
		WHERE not exists(select * from ZnodePortalAccount ZPA where IA.AccountId = ZPA.AccountId )
		--------
		update ZA set ZA.ParentAccountId =  ZA1.AccountId
		from ZnodeAccount ZA
		inner join #InsertAccount IA ON ZA.AccountCode = IA.AccountCode
		inner join ZnodeAccount ZA1 ON ZA1.AccountCode = IA.ParentAccountCode 
		where exists(select * from #InsertedAccount IC where IA.AccountCode = IC.AccountCode)
		and ZA.ParentAccountId is null

		----updating AccountName for respective account
		update ZA set Name = IC.AccountName, AccountCode = IC.AccountCode
		OUTPUT INSERTED.AccountId, INSERTED.Accountcode INTO  #InsertedAccount (AccountId, Accountcode)
		from ZnodeAccount ZA
		inner join #InsertAccount IC ON ZA.AccountCode = IC.AccountCode

		----updating publishcatalogin to respective account
		update ZA set PublishCatalogId = ZPC1.PublishCatalogId, AccountCode = IC.AccountCode
		OUTPUT INSERTED.AccountId, INSERTED.Accountcode INTO  #InsertedAccount (AccountId, Accountcode) 
		from ZnodeAccount ZA
		inner join #InsertAccount IC ON ZA.AccountCode = IC.AccountCode
		inner join ZnodePimCatalog ZPC ON IC.CatalogCode = ZPC.CatalogCode
		inner join ZnodePublishCatalog ZPC1 ON ZPC.PimCatalogId = ZPC1.PimCatalogId
		where not exists(select * from ZnodePortalCatalog ZPCa where ZPCa.PublishCatalogId = ZPC1.PublishCatalogId and ZPCa.PortalId = @PortalId)

		----------update ZnodeAddress
		DECLARE @AddressColumnString VARCHAR(1000), @WhereConditionString VARCHAR(1000), @UpdateColumnString VARCHAR(1000)

		SELECT @AddressColumnString = COALESCE(@AddressColumnString + ',', '')+a.ColumnName --COALESCE(@CsvColumnString + ' and ', '') +'ZA.'+ColumnName+' =  IC.'+ColumnName
		FROM ZnodeImportUpdatableColumns a
		INNER JOIN INFORMATION_SCHEMA.COLUMNS b on a.ColumnName = b.COLUMN_NAME  
		INNER JOIN dbo.Split(@CsvColumnString,',')C on b.COLUMN_NAME = c.Item
		WHERE b.TABLE_NAME = 'ZnodeAddress' 
		AND EXISTS(SELECT * FROM ZnodeImportHead IH where a.ImportHeadId = IH.ImportHeadId and IH.Name= 'Account')

		SELECT @UpdateColumnString = COALESCE(@UpdateColumnString + ' , ', '') +'ZA.'+a.COLUMN_NAME+' =  IC.'+a.COLUMN_NAME  
		FROM INFORMATION_SCHEMA.COLUMNS a
		INNER JOIN dbo.Split(@CsvColumnString,',')b on a.COLUMN_NAME = b.Item
		WHERE NOT EXISTS (SELECT * FROM dbo.Split(@AddressColumnString,',') c WHERE a.COLUMN_NAME = c.Item )
		AND a.TABLE_NAME = 'ZnodeAddress'

		SELECT @WhereConditionString = COALESCE(@WhereConditionString + ' AND ', '') +'ZA.'+item+' =  IC.'+item from dbo.split(@AddressColumnString,',')
		
		CREATE TABLE #InsertedAccountAddress (AddressId  int, AccountCode varchar(100)) 

		SET @SSQL = '
			UPDATE ZA set ModifiedBy = '+CONVERT(VARCHAR(10), @UserId)+', ModifiedDate = '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' '+CASE WHEN ISNULL(@UpdateColumnString,'') = '' THEN '' ELSE ','+@UpdateColumnString END+' 
			FROM ZnodeAddress ZA
			INNER JOIN #InsertAccount IC ON '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END

		EXEC (@SSQL)

		SET @SSQL = '
		Insert into ZnodeAddress (FirstName,LastName,DisplayName,Address1,Address2,Address3,CountryName,
								StateName,CityName,PostalCode,PhoneNumber,
								IsDefaultBilling,IsDefaultShipping,IsActive,ExternalId,CompanyName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
		OUTPUT INSERTED.AddressId, INSERTED.Address3 INTO  #InsertedAccountAddress (AddressId, AccountCode) 			 
		SELECT IC.FirstName,IC.LastName,IC.AddressName,IC.Address1,IC.Address2,IC.AccountCode,ZC.CountryCode,
		ZS.StateCode,IC.CityName,IC.PostalCode,IC.PhoneNumber,
		isnull(IC.IsDefaultBilling,0),isnull(IC.IsDefaultShipping,0),1,IC.ExternalId,IC.CompanyName, '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' , '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+'''
		FROM  #InsertAccount IC
		inner join ZnodeState ZS on IC.StateName = ZS.StateName
		inner join ZnodeCountry ZC ON IC.CountryName = ZC.CountryName and ZS.CountryCode = ZC.CountryCode
		WHERE NOT EXISTS(SELECT * FROM ZnodeAddress ZA WHERE '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END +')'

		EXEC (@SSQL)

		update ZAA set AddressId = UA.AddressId
		FROM #InsertedAccountAddress UA
		INNER JOIN #InsertedAccount IA ON UA.AccountCode = IA.AccountCode
		inner join ZnodeAccount ZA ON IA.AccountCode = ZA.AccountCode
		inner join ZnodeAccountAddress ZAA ON ZA.AccountId = ZAA.AccountId
		
		INSERT INTO ZnodeAccountAddress ( AccountId, AddressId, CreatedBy, CreatedDate,	ModifiedBy,	ModifiedDate )
		SELECT distinct IA.AccountId, AddressId ,  @UserId , @GetDate, @UserId , @GetDate 
		FROM #InsertedAccountAddress UA
		INNER JOIN #InsertedAccount IA ON UA.AccountCode = IA.AccountCode
		WHERE NOT EXISTS ( SELECT * FROM ZnodeAccountAddress AA WHERE AA.AccountId = IA.AccountId )

		update ZnodeAddress set Address3 = null
		where exists(select * from #InsertedAccountAddress IAA where IAA.AddressId = ZnodeAddress.AddressId )
		and Address3 is null
		
		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		
		 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAccount @TableName = '+CAST(@TableName AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@PortalId='+CAST(@PortalId AS VARCHAR(10))+',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max));

		 ---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;
              			
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportAccount',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;