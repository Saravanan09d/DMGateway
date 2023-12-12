using System.Data;

namespace DynamicTableCreation.Models.DTO
{
    public class DataTypeValidationResult
    {
        public List<string> BadRows { get; set; }
        public DataTable ValidDataTypesDataTable { get; set; }
    }
}
