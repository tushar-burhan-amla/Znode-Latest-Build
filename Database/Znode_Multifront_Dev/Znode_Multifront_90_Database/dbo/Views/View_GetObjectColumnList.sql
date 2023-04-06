  Create VIEW [dbo].[View_GetObjectColumnList]
	AS 
  Select a.name ObjectName,column_id columnId,b.name ColumnName,t.name DataType
			from sys.objects a inner join sys.columns b on(a.object_id = b.object_id)
			inner join sys.types t ON b.user_type_id = t.user_type_id