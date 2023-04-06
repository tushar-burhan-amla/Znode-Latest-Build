CREATE TYPE [dbo].[PimProductDetail] AS TABLE (
    [PimAttributeId]                 INT             NULL,
    [PimAttributeFamilyId]           INT             NULL,
    [ProductAttributeCode]           VARCHAR (300)   NULL,
    [ProductAttributeDefaultValueId] INT             NULL,
    [PimAttributeValueId]            INT             NULL,
    [LocaleId]                       INT             NULL,
    [PimProductId]                   INT             NULL,
    [AttributeValue]                 NVARCHAR (MAX)  NULL,
    [AssociatedProducts]             NVARCHAR (4000) NULL,
    [ConfigureAttributeIds]          VARCHAR (2000)  NULL,
    [ConfigureFamilyIds]             VARCHAR (2000)  NULL);

