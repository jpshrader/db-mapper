--table query
SELECT 
	t.TABLE_SCHEMA,
	TABLE_NAME AS DBTableName, 
    isnull(q.epTableName,' ') as DBDescriptionName, 
    isnull(q.epExtendedProperty,' ') as DBDescription
FROM information_schema.tables AS t
    LEFT OUTER JOIN (SELECT OBJECT_NAME(ep.major_id) AS [epTableName],
        CAST(ep.Value AS nvarchar(500)) AS [epExtendedProperty]
        FROM sys.extended_properties ep
        WHERE ep.name = N'Description' AND ep.minor_id = 0) As q
    ON t.table_name = q.epTableName 
WHERE TABLE_TYPE = N'BASE TABLE'
ORDER BY TABLE_NAME