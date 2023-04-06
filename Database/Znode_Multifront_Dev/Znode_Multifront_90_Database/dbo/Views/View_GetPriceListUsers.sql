CREATE VIEW [dbo].[View_GetPriceListUsers]
AS
     SELECT DISTINCT
            q.PriceListId,
            a.PriceListUserId,
            b.UserId,
            (ISNULL(b.FirstName, '')+' '+ISNULL(b.LastName, '')) FullName,
            a.CreatedBy,
            CONVERT( DATE, a.CreatedDate) CreatedDate,
            a.ModifiedBy,
            CONVERT( DATE, a.ModifiedDate) ModifiedDate,
            CASE
                WHEN a.PriceListId IS NULL
                THEN 0
                ELSE 1
            END IsAssociated,
            b.AspNetUserId,
            a.Precedence,
            za.Name AccountName,
            b.Email EmailId,
            AspU.UserName
     FROM ZnodePriceList q
          CROSS JOIN ZnodeUser b
                     LEFT JOIN ZnodePriceListPortal ZPLP ON(ZPLP.PriceListId = q.PriceListId)
                     LEFT JOIN AspNetUsers Asp ON(Asp.Id = b.AspNetUserId)
                     LEFT JOIN AspNetZnodeUser AspU ON(ASpU.AspNetZnodeUserId = ASP.UserName)
                     LEFT JOIN ZnodePriceListUser a ON(a.UserId = b.UserId
                                                       AND a.PriceListId = q.PriceListId)
                     LEFT JOIN ZnodeAccount za ON(za.AccountId = b.AccountId);