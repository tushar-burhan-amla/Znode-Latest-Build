

CREATE VIEW [dbo].[View_GetManageMessageForEdit]
AS
	SELECT ISNULL(NULL,0) [CMSMessageKeyId] ,
	CAST('' AS [nvarchar](100))[MessageKey] ,
	0 [CMSMessageId] ,
	CAST('' AS [nvarchar](max)) [Message] ,
	0 [LocaleId] ,
	CAST('' AS [nvarchar](max)) [CMSAreaId] ,
	CAST('' AS [nvarchar](max))[Area] ,
	CAST('' AS [nvarchar](max))[PortalId] ,
	CAST('' AS [nvarchar](max))[StoreName]