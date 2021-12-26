--DBTable Query
SELECT distinct
	c.object_id,  
	(select name from sys.schemas where schema_id=(select schema_id from sys.objects where object_id=c.object_id)) as schemaName,
	(select name from sys.objects where object_id=c.object_id) as tablename,
    c.name 'Column Name',
	c.column_id,
    t.Name 'Data type',
	isnull(object_definition(c.default_object_id),' ') AS defaultvalue,
	ISNULL(i.is_primary_key, 0) 'IsPrimaryKey',
    c.max_length 'Max Length',
    c.is_nullable,
	isnull((select e.value from sys.extended_properties e where e.minor_id = c.column_id and e.major_id = c.object_id),' ') as [description]
FROM sys.columns c
inner JOIN sys.types t ON c.user_type_id = t.user_type_id
left join sys.extended_properties e on e.minor_id = c.column_id and e.major_id = c.object_id 
LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id and ic.object_id=e.minor_id
LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
WHERE c.object_id in (select object_id from sys.tables where type='U') AND c.name IS NOT NULL
order by tablename