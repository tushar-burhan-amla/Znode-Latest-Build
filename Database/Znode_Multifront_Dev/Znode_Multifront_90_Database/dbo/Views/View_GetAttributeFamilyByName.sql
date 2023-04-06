CREATE VIEW [dbo].[View_GetAttributeFamilyByName]
AS
     SELECT MediaFamilyLocaleId,
            LocaleId,
            MediaAttributeFamilyId,
            AttributeFamilyName,
            Label,
            Description,
            CAST('' AS    VARCHAR(300)) AS Name,
            CreatedBy,
            CONVERT( DATE, CreatedDate) CreatedDate,
            ModifiedBy,
            CONVERT( DATE, ModifiedDate) ModifiedDate
     FROM dbo.ZnodeMediaFamilyLocale;

