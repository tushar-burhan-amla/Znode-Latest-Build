
UPDATE ZOQ SET FirstName = ZU.FirstName
FROM ZnodeOmsQuote ZOQ
INNER JOIN ZnodeUser ZU ON ZOQ.UserId = ZU.UserId
where ZOQ.FirstName IS NULL AND ZU.FirstName IS NOT NULL

UPDATE ZOQ SET MiddleName = ZU.MiddleName
FROM ZnodeOmsQuote ZOQ
INNER JOIN ZnodeUser ZU ON ZOQ.UserId = ZU.UserId
where ZOQ.MiddleName is null  and ZU.MiddleName IS NOT NULL

UPDATE ZOQ SET LastName = ZU.LastName
FROM ZnodeOmsQuote ZOQ
INNER JOIN ZnodeUser ZU ON ZOQ.UserId = ZU.UserId
where ZOQ.LastName IS NULL AND ZU.LastName IS NOT NULL

UPDATE ZOQ SET Email = ZU.Email
FROM ZnodeOmsQuote ZOQ
INNER JOIN ZnodeUser ZU ON ZOQ.UserId = ZU.UserId
where ZOQ.Email IS NULL AND ZU.Email IS NOT NULL

UPDATE ZOQ SET PhoneNumber = ZU.PhoneNumber
FROM ZnodeOmsQuote ZOQ
INNER JOIN ZnodeUser ZU ON ZOQ.UserId = ZU.UserId
where ZOQ.PhoneNumber IS NULL AND ZU.PhoneNumber IS NOT NULL


Update ZO set ZO.InitialPrice = ZO.Price
from ZnodeOmsQuoteLineItem ZO
Where InitialPrice is null and price is not null

Update  ZnodeOmsQuote set IsOldQuote = 1
where IsOldQuote is null 
and  OmsOrderStateId in (select OmsOrderStateId from ZnodeOmsOrderState where OrderStateName ='IN REVIEW' or OrderStateName ='SUBMITTED')

Update ZnodeOmsQuote set IsOldQuote = 0  
Where  IsOldQuote is null


IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsOldQuote' AND TABLE_NAME = 'ZnodeOmsQuote')
BEGIN
		ALTER TABLE ZnodeOmsQuote ALTER COLUMN IsOldQuote BIT NOT NULL
END
go
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsOldQuote' AND TABLE_NAME = 'ZnodeOmsQuote')
BEGIN
	IF NOT EXISTS(SELECT * FROM SYS.default_constraints WHERE Name = 'DF_ZnodeOmsQuote_IsOldQuote')
	BEGIN
		ALTER TABLE ZnodeOmsQuote ADD CONSTRAINT DF_ZnodeOmsQuote_IsOldQuote DEFAULT 0 FOR IsOldQuote
	END
END

