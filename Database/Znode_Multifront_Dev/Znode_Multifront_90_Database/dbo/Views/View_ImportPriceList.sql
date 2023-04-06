Create View [dbo].[View_ImportPriceList]
AS 
SELECT  0  RowNumber ,
CAST('' AS  varchar(max)   )	ErrorDescription ,
			CAST('' AS varchar(300) )		SKU ,
			CAST('' AS  varchar(300)  )			StartQuantity  ,
				CAST('' AS  varchar(300)  )		EndQuantity  ,
				CAST('' AS  varchar(300)  )		Price  ,
				CAST('' AS  varchar(300)  )		SalesPrice   ,
				CAST('' AS  varchar(300)  )		TierPrice      ,
				CAST('' AS  varchar(300)  )		Uom  ,
				CAST('' AS  varchar(300)  )		UnitSize ,
				CAST('' AS   VARCHAR(200)  )		PriceListCode,
				CAST('' AS   VARCHAR(600)  )	PriceListName ,
				CAST('' AS  varchar(300)  )		CurrencyId  ,
				CAST('' AS  varchar(300)  )		ActivationDate  ,
				CAST('' AS  varchar(300)  )		ExpirationDate