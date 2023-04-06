


CREATE   View [dbo].[View_GetPriceListAccounts] As
Select q.PriceListId, a.PriceListAccountId, b.AccountId,   (ISNULL(b.FirstName, '') + ' ' + ISNULL(b.LastName, '')) FullName,
a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate,CASE WHEN a.PriceListId IS NULL THEN 0 ELSE 1 END IsAssociated
 from ZnodePriceList q 
 CROSS JOIN ZnodeAccount b 
 LEFT JOIN ZnodePriceListAccount a ON ( a.AccountId = b.Accountid  AND a.PriceListId = q.PriceListId)