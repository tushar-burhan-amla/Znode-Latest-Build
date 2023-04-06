CREATE  view [dbo].[View_OrderDetails] As  
SELECT zood.UserId, zooli.OmsOrderLineItemsId,zood.OmsOrderId, zood.OmsOrderStateId, ZP.StoreName ,zooli.OmsOrderShipmentId,
zooli.Sku,zooli.ProductName,zooli.Quantity,zooli.Price,zooli.PromoDescription,zooli.DiscountAmount,'' OrderDateKey
,OrderDate , zood.ModifiedDate,zood.OmsOrderDetailsId
,zood.CouponCode, zood.BillingCountry,zc.CountryName,
zood.BillingCity, zood.BillingPostalCode,zood.BillingStateCode,
zood.ShippingCost,
zood.TaxCost,
zood.BillingEmailId,

						CONVERT (char(10),OrderDate,103) as FullDateUK,
						CONVERT (char(10),OrderDate,101) as FullDateUSA,
						DATEPART(DD, OrderDate) AS DayOfMonth,
						--Apply Suffix values like 1st, 2nd 3rd etc..
						CASE 
							WHEN DATEPART(DD,OrderDate) IN (11,12,13) 
							THEN CAST(DATEPART(DD,OrderDate) AS VARCHAR) + 'th'
							WHEN RIGHT(DATEPART(DD,OrderDate),1) = 1 
							THEN CAST(DATEPART(DD,OrderDate) AS VARCHAR) + 'st'
							WHEN RIGHT(DATEPART(DD,OrderDate),1) = 2 
							THEN CAST(DATEPART(DD,OrderDate) AS VARCHAR) + 'nd'
							WHEN RIGHT(DATEPART(DD,OrderDate),1) = 3 
							THEN CAST(DATEPART(DD,OrderDate) AS VARCHAR) + 'rd'
							ELSE CAST(DATEPART(DD,OrderDate) AS VARCHAR) + 'th' 
							END AS DaySuffix,
		
						DATENAME(DW, OrderDate) AS DayName,
						DATEPART(DW, OrderDate) AS DayOfWeekUSA,

						-- check for day of week as Per US and change it as per UK format 
						CASE DATEPART(DW, OrderDate)
							WHEN 1 THEN 7
							WHEN 2 THEN 1
							WHEN 3 THEN 2
							WHEN 4 THEN 3
							WHEN 5 THEN 4
							WHEN 6 THEN 5
							WHEN 7 THEN 6
							END 
							AS DayOfWeekUK,
		
					
						DATEPART(DY, OrderDate) AS DayOfYear,
						DATEPART(WW, OrderDate) + 1 - DATEPART(WW, CONVERT(VARCHAR, 
						DATEPART(MM, OrderDate)) + '/1/' + CONVERT(VARCHAR, 
						DATEPART(YY, OrderDate))) AS WeekOfMonth,
						(DATEDIFF(DD, DATEADD(QQ, DATEDIFF(QQ, 0, OrderDate), 0), 
						OrderDate) / 7) + 1 AS WeekOfQuarter,
						DATEPART(WW, OrderDate) AS WeekOfYear,
						DATEPART(MM, OrderDate) AS Month,
						DATENAME(MM, OrderDate) AS MonthName,
						CASE
							WHEN DATEPART(MM, OrderDate) IN (1, 4, 7, 10) THEN 1
							WHEN DATEPART(MM, OrderDate) IN (2, 5, 8, 11) THEN 2
							WHEN DATEPART(MM, OrderDate) IN (3, 6, 9, 12) THEN 3
							END AS MonthOfQuarter,
						DATEPART(QQ, OrderDate) AS Quarter,
						CASE DATEPART(QQ, OrderDate)
							WHEN 1 THEN 'First'
							WHEN 2 THEN 'Second'
							WHEN 3 THEN 'Third'
							WHEN 4 THEN 'Fourth'
							END AS QuarterName,
						DATEPART(YEAR, OrderDate) AS Year,
						'CY ' + CONVERT(VARCHAR, DATEPART(YEAR, OrderDate)) AS YearName,
						LEFT(DATENAME(MM, OrderDate), 3) + '-' + CONVERT(VARCHAR, 
						DATEPART(YY, OrderDate)) AS MonthYear,
						RIGHT('0' + CONVERT(VARCHAR, DATEPART(MM, OrderDate)),2) + 
						CONVERT(VARCHAR, DATEPART(YY, OrderDate)) AS MMYYYY,
						CONVERT(DATETIME, CONVERT(DATE, DATEADD(DD, - (DATEPART(DD, 
						OrderDate) - 1), OrderDate))) AS FirstDayOfMonth,
						CONVERT(DATETIME, CONVERT(DATE, DATEADD(DD, - (DATEPART(DD, 
						(DATEADD(MM, 1, OrderDate)))), DATEADD(MM, 1, 
						OrderDate)))) AS LastDayOfMonth,
						DATEADD(QQ, DATEDIFF(QQ, 0, OrderDate), 0) AS FirstDayOfQuarter,
						DATEADD(QQ, DATEDIFF(QQ, -1, OrderDate), -1) AS LastDayOfQuarter,
						CONVERT(DATETIME, '01/01/' + CONVERT(VARCHAR, DATEPART(YY, 
						OrderDate))) AS FirstDayOfYear,
						CONVERT(DATETIME, '12/31/' + CONVERT(VARCHAR, DATEPART(YY, 
						OrderDate))) AS LastDayOfYear,
						NULL AS IsHolidayUSA,
						CASE DATEPART(DW, OrderDate)
							WHEN 1 THEN 0
							WHEN 2 THEN 1
							WHEN 3 THEN 1
							WHEN 4 THEN 1
							WHEN 5 THEN 1
							WHEN 6 THEN 1
							WHEN 7 THEN 0
							END AS IsWeekday,Zood.PortalId,
							Substring( CONVERT(varchar(10),OrderDate, 24) ,1,2) 
							OrderTime
						 FROM dbo.ZnodeOmsOrderDetails zood 
 INNER JOIN dbo.ZnodeOmsOrderLineItems zooli ON zood.OmsOrderDetailsId = zooli.OmsOrderDetailsId
 Left OUTER JOIN dbo.ZnodeCountry zc ON zood.BillingCountry = zc.CountryCode
 Left OUTER JOIN dbo.ZnodePortal zp ON zood.PortalId= zp.PortalId


GO