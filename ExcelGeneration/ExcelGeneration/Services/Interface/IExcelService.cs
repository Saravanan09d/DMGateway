using ExcelGeneration.Models.DTO;
using ExcelGeneration.Models;
using System.Data;


namespace ExcelGeneration.Services.Interface
{
    public interface IExcelService
    {
        byte[] GenerateExcelFile(List<EntityColumnDTO> columns, int? parentId);
        List<Dictionary<string, string>> ReadDataFromExcel(Stream excelFileStream, int rowcount);
        public DataTable ReadExcelFromFormFile(IFormFile excelFile);
        public bool IsValidDataType(string data, string expectedDataType);
        public IEnumerable<EntityColumnDTO> GetColumnsForEntity(string entityName);
        Task<LogDTO> Createlog(string tableName, List<string> filedata, string fileName, int successdata, List<string> errorMessage, int total_count, List<string> ErrorRowNumber);
        public void InsertDataFromDataTableToPostgreSQL(DataTable data, string tableName, List<string> columns, IFormFile file);
        public int GetEntityIdByEntityNamefromui(string entityName);
        public List<EntityListMetadataModel> GetEntityListMetadataModelforlist();

        public int? GetEntityIdFromTemplate(IFormFile file, int sheetIndex);
        public Task<List<string>> GetAllIdsFromDynamicTable(string tableName);
        public bool TableExists(string tableName);
        public bool IsValidByteA(string data);
        public bool IsHexString(string input);

        public Task<ValidationResultData> ValidateNotNull(DataTable excelData, List<EntityColumnDTO> columnsDTO);
        public DataTypeValidationResult ValidateDataTypes(ValidationResultData validationResult, List<EntityColumnDTO> columnsDTO);
        public Task<ValidationResultData> ValidatePrimaryKeyAsync(ValidationResultData validationResult, List<EntityColumnDTO> columnsDTO, string tableName);
        public Task<ValidationResult> resultparams(ValidationResultData validationResult, string comma_separated_string);
        public Task<ValidationResult> resultparamsforprimary(ValidationResultData validationResult, string comma_separated_string, string tableName);
        public Task<(string TableName, List<dynamic> Rows)> GetTableDataByListEntityId(int listEntityId);

        public (int EntityId, string EntityColumnName) GetAllEntityColumnData(int checklistEntityValue);

        public (string TableName, List<dynamic> Rows) GetTableDataByChecklistEntityValue(int checklistEntityValue);
    }


}

