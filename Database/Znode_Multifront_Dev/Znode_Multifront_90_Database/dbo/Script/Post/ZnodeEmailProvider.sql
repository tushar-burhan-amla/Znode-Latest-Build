INSERT [dbo].[ZnodeEmailProvider] ( [ProviderCode], [ProviderName], [ClassName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Description]) 
SELECT 'Klaviyo_Email', N'Klaviyo', N'KlaviyoProvider', 2,getdate(), 2, getdate() , N'Klaviyo_Email'
WHERE NOT EXISTS(SELECT * FROM [ZnodeEmailProvider] WHERE [ProviderCode] = 'Klaviyo_Email')
