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
GO


--SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

--SET XACT_ABORT ON;
IF Exists(select * from sys.tables where name = 'Temp_xx_ZnodeImpersonationLog')
    drop table Temp_xx_ZnodeImpersonationLog
go
IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ZnodeImpersonificationLog'))
BEGIN
    CREATE TABLE [dbo].[Temp_xx_ZnodeImpersonationLog](
	[ImpersonationLogId] [int] IDENTITY(1,1) NOT NULL,
	[PortalId] [int] NOT NULL,
	[CSRId] [int] NULL,
	[WebstoreuserId] [int] NULL,
	[ActivityType] [varchar](100) NULL,
	[ActivityId] [int] NULL,
	[OperationType] [varchar](20) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Temp_xx_ZnodeImpersonationLog] PRIMARY KEY CLUSTERED 
(
	[ImpersonationLogId] ASC
)
);


IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[ZnodeImpersonificationLog])
    BEGIN
        SET IDENTITY_INSERT [dbo].[Temp_xx_ZnodeImpersonationLog] ON;
        INSERT INTO [dbo].[Temp_xx_ZnodeImpersonationLog]
           ([ImpersonationLogId]
           ,[PortalId]
           ,[CSRId]
           ,[WebstoreuserId]
           ,[ActivityType]
           ,[ActivityId]
           ,[OperationType]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
		   SELECT [ImpersonificationLogId]
           ,[PortalId]
           ,[CSRId]
           ,[WebstoreuserId]
           ,[ActivityType]
           ,[ActivityId]
           ,[OperationType]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
        FROM     [dbo].[ZnodeImpersonificationLog];
       
        SET IDENTITY_INSERT [dbo].[Temp_xx_ZnodeImpersonationLog] OFF;
    END

DROP TABLE [dbo].[ZnodeImpersonificationLog];

	IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ZnodeImpersonationLog'))

DROP TABLE [dbo].ZnodeImpersonationLog;


EXECUTE sp_rename N'[dbo].[Temp_xx_ZnodeImpersonationLog]', N'ZnodeImpersonationLog';

EXECUTE sp_rename N'[dbo].[PK_Temp_xx_ZnodeImpersonationLog]', N'PK_ZnodeImpersonationLog', N'OBJECT';



--SET TRANSACTION ISOLATION LEVEL READ COMMITTED;




END
GO

IF EXISTS (SELECT * FROM SYS.TABLES WHERE NAME='ZnodeProductFeed')
AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME='PortalId' AND TABLE_NAME='ZnodeProductFeed')
BEGIN
	TRUNCATE TABLE ZnodeProductFeed
END
