
INSERT INTO Znodeproductfeedtype (ProductFeedTypeCode,ProductFeedTypeName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'Xml','XML Product Feed',2,GETDATE(),2,GETDATE()  
where not exists(select * from Znodeproductfeedtype where ProductFeedTypeCode ='Xml')

Delete from Znodeproductfeedtype where ProductFeedTypeCode = 'Shoppingfeed'

--Ticket:-ZPD-20517
Update ZnodeProductFeedType
set ProductFeedTypeName='XML Site Map'
Where ProductFeedTypeName='Xml Site Map'