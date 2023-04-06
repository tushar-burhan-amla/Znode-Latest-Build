
CREATE VIEW [dbo].[View_GetAssociatedPortalToTaxClass]
AS
     SELECT a.PortalId,
            a.StoreName,
            CONVERT( DATE, b.CreatedDate) CreatedDate,
            CONVERT( DATE, b.ModifiedDate) ModifiedDate,
            CASE
                WHEN b.PortalId IS NULL
                THEN 0
                ELSE 1
            END IsAssociated,
            c.TaxClassId
     FROM ZnodePortal a
          CROSS JOIN ZnodeTaxClass c
                     LEFT JOIN ZnodePortalTaxClass b ON(a.PortalId = b.PortalId
                                                        AND b.TaxClassId = c.TaxClassId);