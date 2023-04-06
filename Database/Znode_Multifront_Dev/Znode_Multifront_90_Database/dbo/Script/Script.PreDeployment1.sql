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
USE Master 
GO
IF Exists (Select * from sys.databases where Name = 'Znode_Multifront_90_UAT' )
Begin
	ALTER DATABASE Znode_Multifront_90_UAT SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP Database Znode_Multifront_90_UAT 
End 
GO