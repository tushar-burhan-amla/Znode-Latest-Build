IF EXISTS ( SELECT TOP 1 1 FROM ZnodeCurrency WHERE CurrencyName = 'Serbian Dinar' AND CurrencyCode = '')
	BEGIN
		UPDATE ZnodeCurrency 
		SET CurrencyCode = 'RSD'
		WHERE CurrencyName = 'Serbian Dinar' AND CurrencyCode = ''
	END