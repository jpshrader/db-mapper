using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace db_mapper.Common {
	public class SqlDataAccess : DataAccess {
		public SqlDataAccess(string connectionString) : base(connectionString) { }

		public override DatabaseMap GetMap() {
			var tableMap = GetTableMap();
			if (tableMap.Rows.Count == 0) {
				return null;
			}

			var columnMap = GetColumnMap();
			var columnRelationships = GetColumnRelationships();
			var dbMap = new DatabaseMap {
				DbDescription = tableMap.Rows[0]["DBDescription"].ToString(),
				DbDescriptionName = tableMap.Rows[0]["DBDescriptionName"].ToString(),
				TableMaps = new List<TableMap>()
			};

			foreach (DataRow row in tableMap.Rows) {
				var tableEntry = new TableMap {
					TableName = row["DBTableName"].ToString(),
					TableSchema = row["TABLE_SCHEMA"].ToString(),
					ColumnMaps = new List<ColumnMap>()
				};

				var columns = columnMap.Rows.OfType<DataRow>()
					.Where(c => c["tablename"].ToString() == tableEntry.TableName);
				foreach (var col in columns) {
					var columnMapItem = new ColumnMap {
						ColumnName = col["Column Name"].ToString(),
						DataType = col["Data type"].ToString(),
						Description = col["description"].ToString(),
						IsNullable = col["is_nullable"].ToString(),
						IsPrimaryKey = col["IsPrimaryKey"].ToString(),
						DefaultValue = col["defaultvalue"].ToString(),
						MaxLength = col["Max Length"].ToString(),
						ColumnRelationships = new List<ColumnRelationship>()
					};
					var columnRelations = columnRelationships.Rows.OfType<DataRow>()
						.Where(r => r["PKColumn"].ToString() == columnMapItem.ColumnName && r["Primary Table"].ToString() == tableEntry.TableName);

					foreach (var colRel in columnRelations) {
						var rel = new ColumnRelationship {
							PrimaryTable = colRel["Primary Table"].ToString(),
							RelationshipType = colRel["RelationshipType"].ToString(),
							ForeignTable = colRel["Foreign Table"].ToString(),
							ForeignColumn = colRel["FKColumn"].ToString(),
							ForeignKeyName = colRel["FK Name"].ToString()
						};

						columnMapItem.ColumnRelationships.Add(rel);
					}

					tableEntry.ColumnMaps.Add(columnMapItem);
				}

				dbMap.TableMaps.Add(tableEntry);
			}


			return dbMap;
		}

		public DataTable GetTableMap() {
			var sql = @"
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
ORDER BY TABLE_NAME";

			return GetDataTable(sql);
		}

		public DataTable GetColumnMap() {
			var sql = @"
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
order by tablename";

			return GetDataTable(sql);
		}

		public DataTable GetColumnRelationships() {
			var sql = @"
SELECT
    tr.name 'Foreign Table',
	cr.name as FKColumn, cr.column_id as FKColumnID,
	'>-' as RelationshipType,
	tp.name 'Primary Table',
	cp.name as PKColumn, cp.column_id as PKColumnID,
    fk.name 'FK Name'
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
ORDER BY tp.name, cp.column_id";

			return GetDataTable(sql);
		}
	}
}