namespace db_mapper.Common {
	public class ColumnRelationship {
		public string ForeignTable { get; set; }

		public string RelationshipType { get; set; }

		public string PrimaryTable { get; set; }

		public string ForeignKeyName { get; set; }

		public string ForeignColumn { get; set; }
	}
}
