namespace DynamicTableCreation.Models
{
    public class TableCreationRequest
    {
        public string TableName { get; set; }
        public List<ColumnDefinition> Columns { get; set; }
    }
  
        public class UpdateEntityRequestModel
        {
        public int EntityId { get; set; }
        public string NewEntityName { get; set; }
    }

    public class ColumnDefinition
    {
        public string EntityColumnName { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
        public int? MinLength { set; get; }
        public int? MaxLength { set; get; }
        public int? MaxRange { set; get; }
        public int? MinRange { set; get; }
        public string DateMinValue { set; get; }
        public string DateMaxValue { set; get; }
        public string Description { get; set; }
        public int ListEntityId { get; set; }
        public int ListEntityKey { get; set; }
        public int ListEntityValue { get; set; }
        public bool IsNullable { get; set; }
        public string True { get; set; }
        public string False { get; set; }
        public string DefaultValue { get; set; }
        public bool ColumnPrimaryKey { get; set; }
    }
}
