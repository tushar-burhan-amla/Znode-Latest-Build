/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


 IF EXISTS (select top 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodeCMSFormWidgetConfiguration' AND COLUMN_NAME = 'RedirectUrl')
 BEGIN
	Alter table ZnodeCMSFormWidgetConfiguration Alter column RedirectUrl varchar(max) 
 END

 GO 

 IF NOT EXISTS (select top 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePortalAddress' AND COLUMN_NAME = 'StoreLocationCode')
 BEGIN
	 Alter table ZnodePortalAddress Add StoreLocationCode nvarchar(100)  Null
	 Declare @SSQL nvarchar(max)
	 SET @SSQL  = 'Update ZnodePortalAddress SET StoreLocationCode = PortalAddressId ' 
	 EXEC sys.sp_sqlexec @SSQL;
	 Alter table ZnodePortalAddress Add CONSTRAINT [UK_ZnodePortalAddress_StoreLocationCode] UNIQUE ([StoreLocationCode])
 END