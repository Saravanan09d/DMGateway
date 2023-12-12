using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicTableCreation.Models.DTO
{
    public class UpdateEntityColumnRequestModel
    {
        public  int EntityId { get; set; }
        public string EntityName { get; set; }
        public UpdateColumnList Update { get; set; }
    }

    public class UpdateColumnList
    {
        public List<EntityColumnProperties> PropertiesList { get; set; }
    }
    public class EntityColumnProperties
    {
        public string EntityColumnName { get; set; }
        public string Datatype { get; set; }
        public int Length { get; set; }
        public int? MinLength { set; get; }
        public int? MaxLength { set; get; }
        public int? MaxRange { set; get; }
        public int? MinRange { set; get; }
        public string DateMinValue { set; get; }
        public string DateMaxValue { set; get; }
        public string Description { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int ListEntityId { get; set; }
        public int ListEntityKey { get; set; }
        public int ListEntityValue { get; set; }
        public string True { get; set; }
        public string False { get; set; }
        public bool ColumnPrimaryKey { get; set; }
    }
    public class TableCheckResult
    {
        public string TableName { get; set; }
        public bool HasValues { get; set; }
    }



}
