
---dt- 10/03/2019 ZPD-7394
if not exists(select * from sys.indexes where name = 'Ind_ZnodePimAttributeDefaultValueLocale_LocaleId_PimAttributeDefaultValueId')
begin 
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeDefaultValueLocale_LocaleId_PimAttributeDefaultValueId]
ON [dbo].[ZnodePimAttributeDefaultValueLocale] ([LocaleId])
INCLUDE ([PimAttributeDefaultValueId])
end

go

if not exists(select * from sys.indexes where name = 'Ind_ZnodePimAttributeDefaultValueLocale_AttributeDefaultValue')
begin
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeDefaultValueLocale_AttributeDefaultValue]
ON [dbo].[ZnodePimAttributeDefaultValueLocale] ([PimAttributeDefaultValueId],[LocaleId])
INCLUDE ([AttributeDefaultValue])
end

--dt 20-08-2020 ZPD-12056
if exists(select * from sys.indexes where name = 'IX_ZnodePimAttributeDefaultJson_PimAttributeDefaultValueId_5A075')
	drop index IX_ZnodePimAttributeDefaultJson_PimAttributeDefaultValueId_5A075 ON [dbo].[ZnodePimAttributeDefaultJson]
go
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultJson_PimAttributeDefaultValueId_5A075]
    ON [dbo].[ZnodePimAttributeDefaultJson]([PimAttributeDefaultValueId] ASC)
    INCLUDE([DefaultValueJson]) WITH (FILLFACTOR = 90);