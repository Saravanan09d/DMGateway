using System.Data;

namespace DynamicTableCreation.Models.DTO
{
    public class ValidationResultData
    {
        public List<string> BadRows { get; set; }
        public List<string> errorcolumns { get; set; }
        public DataTable SuccessData { get; set; }
        public string Column_Name { get; set; }
    }
}
