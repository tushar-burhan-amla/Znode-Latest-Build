CREATE TYPE [dbo].[PimCategoryDetail] AS TABLE (
    [PimCategoryId]              INT            NULL,
    [PimAttributeId]             INT            NULL,
    [PimAttributeValueId]        INT            NULL,
    [PimAttributeDefaultValueId] INT            NULL,
    [PimAttributeFamilyId]       INT            NULL,
    [LocaleId]                   INT            NULL,
    [AttributeCode]              VARCHAR (500)  NULL,
    [AttributeValue]             NVARCHAR (MAX) NULL);

