-- ZPD-16760 dt: 13Dec2021
update ZnodeSearchProfile set SearchSubQueryTypeId=(select SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName='Cross'), Operator='AND'
where IsDefault=1 and ProfileName='ZnodeSearchProfile'
and not exists (select * from ZnodeSearchProfile where ProfileName='ZnodeSearchProfile' and SearchSubQueryTypeId = 7)