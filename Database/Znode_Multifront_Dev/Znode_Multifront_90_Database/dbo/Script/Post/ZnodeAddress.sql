
--dt 09-03-2020 ZPD-9380 --> ZPD-9392
delete from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'CustomerAddress')
and ColumnName in ('IsDefaultBilling', 'IsDefaultShipping')

--------------------------------------->ZPD-14631
	UPDATE ZA
SET ZA.Address1 = ZA.Address2,ZA.Address2 = NULL
From ZnodeAddress ZA
INNER JOIN ZnodePortalAddress ZPA on ZA.AddressId = ZPA.AddressId
WHERE ZA.Address1 IS NULL AND ZA.Address2 IS NOT NULL

UPDATE ZA
SET ZA.Address2 = ZA.Address3,ZA.Address3 = NULL
From ZnodeAddress ZA
INNER JOIN ZnodePortalAddress ZPA on ZA.AddressId = ZPA.AddressId
WHERE ZA.Address2 IS NULL AND ZA.Address3 IS NOT NULL
