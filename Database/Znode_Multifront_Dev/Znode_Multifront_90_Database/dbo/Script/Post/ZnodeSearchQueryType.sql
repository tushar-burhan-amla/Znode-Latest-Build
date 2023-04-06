-- dt 30-11-2021 ZPD-16534
Update ZnodeSearchQueryType set HelpDescription = 'When Multi-Match query type is configured, the application searches keywords in more than one searchable field to display results on the Web Store. Best: Should be used when the searched term must appear in any one searchable field. Cross: Should be used when the searched term can be spread across multiple searchable fields. Auto Correct is not applicable for Multi match -> Cross combination.' 
where QueryTypeName = 'Multi Match'

-- dt 16-12-2021 ZPD-16815
Update ZnodeSearchQueryType set QueryTypeName = 'Multi Match - Best' where QueryTypeName = 'Best' 
Update ZnodeSearchQueryType set QueryTypeName = 'Multi Match - Cross' where QueryTypeName = 'Cross'

Update ZnodeSearchQueryType
set HelpDescription = 'When Multi Match query type is configured, the application searches keywords in more than one searchable field to display results on the Web Store. Best: Should be used when the searched term must appear in any one searchable field'
where QueryTypeName = 'Multi Match - Best' and not exists (select * from ZnodeSearchQueryType where QueryTypeName = 'Multi Match - Best' and HelpDescription like 'When Multi Match%')

Update ZnodeSearchQueryType
set HelpDescription = 'When Multi Match query type is configured, the application searches keywords in more than one searchable field to display results on the Web Store. Cross: Should be used when the searched term can be spread across multiple searchable fields'
where QueryTypeName = 'Multi Match - Cross' and not exists (select * from ZnodeSearchQueryType where QueryTypeName = 'Multi Match - Cross' and HelpDescription like 'When Multi Match%')