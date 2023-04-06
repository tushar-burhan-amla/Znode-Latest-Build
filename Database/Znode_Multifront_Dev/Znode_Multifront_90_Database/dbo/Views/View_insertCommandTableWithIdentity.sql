
CREATE view [dbo].[View_insertCommandTableWithIdentity] as 
select table_name=name,'Select  '+prefix+' ( '+substring(body1,1,len(body1)-1)+' ) values( '+substring(body2,1,len(body2)-1)+' ) SET IDENTITY_INSERT [dbo].['+ CAST( name AS nvarchar) +'] OFF '+postfix  Query
from (select db_name() as database_name
			,sysobjects.name
			,table_id = sysobjects.id,
			'''  SET IDENTITY_INSERT [dbo].['+ CAST( sysobjects.name AS nvarchar) +'] ON
            insert into '+ CAST( sysobjects.name AS nvarchar)   prefix,
			(SELECT DISTINCT STUFF((SELECT '['+ (CAST(syscolumns.name AS nvarchar)  +'],') 
									FROM syscolumns inner join systypes on syscolumns.xtype = systypes.xtype
									where systypes.name<>'sysname'
									and  sysobjects.id = syscolumns.id
									ORDER BY syscolumns.colid  FOR XML PATH('')),1,0,'')
			)body1,
		   (SELECT DISTINCT STUFF((SELECT ( case when systypes.name='int' then   ' ''+isnull(convert(nvarchar(max),['+CAST( syscolumns.name  AS nvarchar) +']),''null'')+'''
									else  '''+ case when convert(nvarchar(max),['+CAST( syscolumns.name  AS nvarchar) +']) is null then ''null'' else '''''''' + REPLACE(convert(nvarchar(max),['+CAST( syscolumns.name  AS nvarchar) +'] ),char(39),char(39)+char(39))+'''''' '' end +'''
									end +',' ) 
									FROM syscolumns inner join systypes on syscolumns.xtype = systypes.xtype
									where systypes.name<>'sysname'
									and  sysobjects.id = syscolumns.id
									ORDER BY syscolumns.colid  FOR XML PATH('')),1,0,'')
			) body2,
			'''  from '+ CAST( sysobjects.name AS nvarchar) postfix 
from sysobjects 
where sysobjects.xtype = 'U' )  da