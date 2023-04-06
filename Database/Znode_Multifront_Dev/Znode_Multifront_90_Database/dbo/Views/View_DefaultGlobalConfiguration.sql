
CREATE view [dbo].[View_DefaultGlobalConfiguration]
as
select CurrencyId Id ,'Currency' Name ,CurrencyName Value from ZnodeCurrency
where isdefault=1 and isactive=1
union all 
select CountryId Id ,'Country' Name ,CountryName Value  from ZnodeCountry
where isdefault=1 and isactive=1
union all 
select LocaleId Id ,'Locale' Name ,Name Value   from ZnodeLocale
where isdefault=1 and isactive=1
union all 
select TimeZoneId Id ,'TimeZone' Name ,TimeZoneDetailsCode Value  from ZnodeTimeZone
where isdefault=1 and isactive=1
union all 
select DateFormatId Id ,'DateFormat' Name ,DateFormat Value from ZnodeDateFormat
where isdefault=1 and isactive=1
union all 
select WeightUnitId Id ,'WeightUnit' Name ,WeightUnitCode Value from ZnodeWeightUnit
where isdefault=1 and isactive=1