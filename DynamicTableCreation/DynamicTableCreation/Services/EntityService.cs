using DynamicTableCreation.Data;
using DynamicTableCreation.Models;
using DynamicTableCreation.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace DynamicTableCreation.Services
{
    public class EntityService
    {
        private readonly ApplicationDbContext _dbContext;
        public EntityService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> TableExistsAsync(string tableName)
        {
            try
            {
                var lowerCaseTableName = tableName.ToLower();
                var existingEntity = await _dbContext.EntityListMetadataModels
                    .AnyAsync(e => e.EntityName.ToLower() == lowerCaseTableName);
                return existingEntity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking if table '{tableName}' exists: {ex.Message}");
                return false; 
            }
        }

        public TableCreationRequest MapToModel(TableCreationRequestDTO dto)
        {
            try
            {
                return new TableCreationRequest
                {
                    TableName = dto.TableName,
                    Columns = dto.Columns.Select(columnDto => new ColumnDefinition
                    {
                        EntityColumnName = columnDto.EntityColumnName,
                        DataType = columnDto.DataType,
                        Length = columnDto.Length,
                        MinLength = columnDto.MinLength,
                        MaxLength = columnDto.MaxLength,
                        MaxRange = columnDto.MaxRange,
                        MinRange = columnDto.MinRange,
                        DateMaxValue = columnDto.DateMaxValue,
                        DateMinValue = columnDto.DateMinValue,
                        Description = columnDto.Description,
                        ListEntityId = columnDto.ListEntityId,
                        ListEntityKey = columnDto.ListEntityKey,
                        ListEntityValue = columnDto.ListEntityValue,
                        True = columnDto.True,
                        False = columnDto.False,
                        IsNullable = columnDto.IsNullable,
                        DefaultValue = columnDto.DefaultValue,
                        ColumnPrimaryKey = columnDto.ColumnPrimaryKey
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in MapToModel: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CreateDynamicTableAsync(TableCreationRequest request)
        {
            try
            {
                var createTableSql = GenerateCreateTableSql(request);
                await _dbContext.Database.ExecuteSqlRawAsync(createTableSql);
                var entityList = await CreateTableMetadataAsync(request);
                if (entityList == null)
                {
                    return false;
                }
                await BindColumnMetadataAsync(request, entityList);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private async Task<EntityListMetadataModel> CreateTableMetadataAsync(TableCreationRequest request)
        {
            var lowerCaseTableName = request.TableName.ToLower();
            var existingEntity = await _dbContext.EntityListMetadataModels
                .FirstOrDefaultAsync(e => e.EntityName.ToLower() == lowerCaseTableName);
            if (existingEntity != null)
            {
                return existingEntity;
            }
            var entityList = new EntityListMetadataModel
            {
                EntityName = request.TableName,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };
            try
            {
                _dbContext.EntityListMetadataModels.Add(entityList);
                await _dbContext.SaveChangesAsync();
                return entityList;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private async Task BindColumnMetadataAsync(TableCreationRequest request, EntityListMetadataModel entityList)
        {
            try
            {
                foreach (var column in request.Columns)
                {
                    var existingColumn = await _dbContext.EntityColumnListMetadataModels
                        .FirstOrDefaultAsync(c => c.EntityColumnName.ToLower() == column.EntityColumnName.ToLower() && c.EntityId == entityList.Id);
                    if (existingColumn != null)
                    {
                        continue;
                    }
                    var entityColumn = new EntityColumnListMetadataModel
                    {
                        EntityColumnName = column.EntityColumnName,
                        Datatype = column.DataType,
                        Length = column.Length,
                        MinLength = column.MinLength|0,
                        MaxLength = column.MaxLength|0,
                        MinRange = column.MinRange | 0,
                        MaxRange = column.MaxRange | 0,
                        DateMinValue = column.DateMinValue,
                        DateMaxValue = column.DateMaxValue,
                        Description = column.Description,
                        IsNullable = column.IsNullable,
                        DefaultValue = column.DefaultValue,
                        ListEntityId = column.ListEntityId | 0,
                        ListEntityKey = column.ListEntityKey | 0,
                        ListEntityValue = column.ListEntityValue | 0,
                        True = column.True,
                        False = column.False,
                        ColumnPrimaryKey = column.ColumnPrimaryKey,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        EntityId = entityList.Id
                    };

                    _dbContext.EntityColumnListMetadataModels.Add(entityColumn);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string logFilePath = "error.log";
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.UtcNow}] An error occurred: {ex.Message}");
                    writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                    writer.WriteLine();
                }
                throw;
            }
        }

        private string GetTableNameForListEntityId(int entityId)
        {
            try
            {
                var entity = _dbContext.EntityListMetadataModels
                    .FirstOrDefault(e => e.Id == entityId);

                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(entity.EntityName))
                    {
                        return entity.EntityName;
                    }
                }
                return "TableNotFound";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting table name: {ex.Message}");
                return "TableNotFound";
            }
        }
        private string GetColumnNameForListKeyId(int listEntityKey)
        {
            try
            {
                var column = _dbContext.EntityColumnListMetadataModels
                    .FirstOrDefault(e => e.ListEntityKey == listEntityKey);

                if (column != null)
                {
                    if (!string.IsNullOrEmpty(column.EntityColumnName))
                    {
                        return column.EntityColumnName;
                    }
                }

                return "ColumnNotFound";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting column name: {ex.Message}");
                return "ColumnNotFound";
            }
        }
    
        public EntityInfoDTO GetEntityInfo(string entityName)
        {
            try
            {
                var entity = _dbContext.EntityListMetadataModels
                    .FirstOrDefault(e => e.EntityName == entityName);

                if (entity == null)
                {
                    return null; // or throw an exception, handle as needed
                }

                var entityId = entity.Id;

                var columns = _dbContext.EntityColumnListMetadataModels
                    .Where(c => c.EntityId == entityId)
                    .Select(c => new EntityColumnInfoDTO
                    {
                        Id = c.Id,
                        EntityColumnName = c.EntityColumnName,
                        // Add other properties as needed
                    })
                    .ToList();

                return new EntityInfoDTO
                {
                    EntityId = entityId,
                    Columns = columns
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; // or throw an exception, handle as needed
            }
        }

        private string GetDatatypeForListEntityKey(int listEntityKey)
        {
            try
            {
                // Assuming EntityColumnListMetadataModels is the DbSet in your DbContext
                var column = _dbContext.EntityColumnListMetadataModels
                    .FirstOrDefault(e => e.ListEntityKey == listEntityKey);

                if (column != null)
                {
                    // Check if Datatype is not null or empty before returning
                    if (!string.IsNullOrEmpty(column.Datatype))
                    {
                        // Convert the datatype to a standardized form
                        return ConvertToStandardDatatype(column.Datatype);
                    }
                }

                return "DatatypeNotFound";
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"An error occurred while getting datatype: {ex.Message}");
                return "DatatypeNotFound";
            }
        }

        private string ConvertToStandardDatatype(string originalDatatype)
        {
            switch (originalDatatype.ToLower())
            {
                case "int":
                case "integer":
                    return "integer";

                case "string":
                case "varchar":
                    return "varchar";

                // Add more cases as needed for other datatypes

                default:
                    return "UnknownDatatype";
            }
        }
        private string GenerateCreateTableSql(TableCreationRequest request)
        {
            try
            {
                var createTableSql = $"CREATE TABLE \"{request.TableName}\" (";
                bool hasColumns = false;

                foreach (var column in request.Columns)
                {
                    if (hasColumns)
                    {
                        createTableSql += ",";
                    }

                    createTableSql += $"\"{column.EntityColumnName}\" ";

                    switch (column.DataType.ToLower())
                    {
                        case "int":
                            createTableSql += "integer";
                            break;
                        case "date":
                            createTableSql += "date";
                            break;
                        case "string":
                            createTableSql += $"varchar({(column.MaxLength > 0 ? column.MaxLength : 255)})";
                            break;
                        case "char":
                            createTableSql += $"char({(column.Length == 1 ? column.Length : 255)})";
                            break;
                        case "listofvalue":
                            var referencedTableName = GetTableNameForListEntityId(column.ListEntityId);
                            var referanceKeyName = GetColumnNameForListKeyId(column.ListEntityKey);
                            var referanceEntityValue = GetDatatypeForListEntityKey(column.ListEntityKey);
                            if (!string.IsNullOrEmpty(referencedTableName))
                            {
                                createTableSql += $"{referanceEntityValue} REFERENCES \"{referencedTableName}\"(\"{referanceKeyName}\") NOT NULL";
                            }
                            else
                            {
                                createTableSql += "varchar";
                            }
                            break;
                        case "boolean":
                            createTableSql += "boolean";
                            break;
                        case "time":
                            createTableSql += "time";
                            break;
                        case "timestamp":
                            createTableSql += "timestamp";
                            break;
                        default:
                            createTableSql += "varchar";
                            break;
                    }

                    if (!column.IsNullable)
                    {
                        createTableSql += " NOT NULL";
                    }

                    if (!string.IsNullOrEmpty(column.DefaultValue))
                    {
                        createTableSql += $" DEFAULT '{column.DefaultValue}'";
                    }
                    if (column.ColumnPrimaryKey)
                    {
                        createTableSql += " PRIMARY KEY";
                    }
                    hasColumns = true;
                }
                createTableSql += hasColumns ? "," : "";
                createTableSql += "\"createddate\" timestamp DEFAULT CURRENT_TIMESTAMP";
                createTableSql += ");";
                return createTableSql;
            }
            catch (Exception ex)
            {
                string logFilePath = "error.log";
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.UtcNow}] An error occurred: {ex.Message}");
                    writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                    writer.WriteLine();
                }
                throw;
            }
        }

        public string GetOldEntityName(int entityId)
        {
            var oldEntityName = _dbContext.EntityListMetadataModels
                .Where(e => e.Id == entityId)
                .Select(e => e.EntityName)
                .FirstOrDefault();
            return oldEntityName;
        }

        public int GetEntityIdForTableName(string entityName)
        {
            var entity = _dbContext.EntityListMetadataModels
                   .FirstOrDefault(e => e.EntityName == entityName);

            return entity?.Id ?? 0;
        }
        public void UpdateEntityColumn(int entityId, string newEntityName, List<EntityColumnProperties> newEntityColumns)
        {
            string oldEntityName = GetOldEntityName(entityId);
            DropTable(oldEntityName);
            var existingEntity = _dbContext.EntityListMetadataModels
                .Include(e => e.EntityColumns)
                .FirstOrDefault(e => e.Id == entityId);
            if (existingEntity == null)
            {
                return;
            }
            existingEntity.EntityName = newEntityName;
            DeleteOldValues(entityId);
            foreach (var newColumn in newEntityColumns)
            {
                var existingColumn = existingEntity.EntityColumns
                    .FirstOrDefault(c => c.EntityColumnName == newColumn.EntityColumnName);
                if (existingColumn != null)
                {
                    UpdateExistingColumn(existingColumn, newColumn);
                }
                else
                {
                    AddNewColumn(existingEntity, newColumn);
                }
            }
            _dbContext.SaveChanges();
            var createTableSql = GenerateCreateTableSql(new TableCreationRequest
            {
                TableName = newEntityName,
                Columns = newEntityColumns.Select(ConvertToColumnDefinition).ToList()
            });
            _dbContext.Database.ExecuteSqlRaw(createTableSql);
        }
        private void DeleteOldValues(int entityId)
        {
            try
            {
                var recordsToDelete = _dbContext.EntityColumnListMetadataModels
                    .Where(e => e.EntityId == entityId)
                    .ToList();
                _dbContext.EntityColumnListMetadataModels.RemoveRange(recordsToDelete);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting old values: {ex.Message}");
            }
        }
        private void UpdateExistingColumn(EntityColumnListMetadataModel existingColumn, EntityColumnProperties newColumn)
        {
            existingColumn.Datatype = newColumn.Datatype;
            existingColumn.Length = newColumn.Length;
            existingColumn.MinLength = newColumn.MinLength|0;
            existingColumn.MaxLength = newColumn.MaxLength|0;
            existingColumn.MaxRange = newColumn.MaxRange|0;
            existingColumn.MinRange = newColumn.MinRange|0;
            existingColumn.DateMinValue = newColumn.DateMinValue;
            existingColumn.DateMaxValue = newColumn.DateMaxValue;
            existingColumn.Description = newColumn.Description;
            existingColumn.IsNullable = newColumn.IsNullable;
            existingColumn.DefaultValue = newColumn.DefaultValue;
            existingColumn.ListEntityId = newColumn.ListEntityId|0;
            existingColumn.ListEntityKey = newColumn.ListEntityKey|0;
            existingColumn.ListEntityValue = newColumn.ListEntityValue|0;
            existingColumn.True = newColumn.True;
            existingColumn.False = newColumn.False;
            existingColumn.ColumnPrimaryKey = newColumn.ColumnPrimaryKey;
            existingColumn.UpdatedDate = DateTime.UtcNow;
        }

        private void AddNewColumn(EntityListMetadataModel existingEntity, EntityColumnProperties newColumn)
        {
            var entityColumn = new EntityColumnListMetadataModel
            {
                EntityColumnName = newColumn.EntityColumnName,
                Datatype = newColumn.Datatype,
                Length = newColumn.Length,
                MinLength = newColumn.MinLength,
                MaxLength = newColumn.MaxLength,
                MaxRange = newColumn.MaxRange,
                MinRange = newColumn.MinRange,
                DateMinValue = newColumn.DateMinValue,
                DateMaxValue = newColumn.DateMaxValue,
                Description = newColumn.Description,
                IsNullable = newColumn.IsNullable,
                DefaultValue = newColumn.DefaultValue,
                ListEntityId = newColumn.ListEntityId,
                ListEntityKey = newColumn.ListEntityKey,
                ListEntityValue = newColumn.ListEntityValue,
                True = newColumn.True,
                False = newColumn.False,
                ColumnPrimaryKey = newColumn.ColumnPrimaryKey,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            };
            existingEntity.EntityColumns.Add(entityColumn);
        }
        private ColumnDefinition ConvertToColumnDefinition(EntityColumnProperties entityColumn)
        {
            return new ColumnDefinition
            {
                EntityColumnName = entityColumn.EntityColumnName,
                DataType = entityColumn.Datatype,
                Length = entityColumn.Length,
                MinLength = entityColumn.MinLength|0,
                MaxLength = entityColumn.MaxLength|0,
                MaxRange = entityColumn.MaxRange | 0,
                MinRange = entityColumn.MinRange | 0,
                DateMinValue = entityColumn.DateMinValue,
                DateMaxValue = entityColumn.DateMaxValue,
                Description = entityColumn.Description,
                IsNullable = entityColumn.IsNullable,
                DefaultValue = entityColumn.DefaultValue,
                ListEntityId = entityColumn.ListEntityId | 0,
                ListEntityKey = entityColumn.ListEntityKey | 0,
                ListEntityValue = entityColumn.ListEntityValue | 0,
                True = entityColumn.True,
                False = entityColumn.False,
                ColumnPrimaryKey = entityColumn.ColumnPrimaryKey,
            };
        }

        public void DropTable(string oldEntityName)
        {
            var tableExists = _dbContext.EntityListMetadataModels.FromSqlRaw("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", oldEntityName).Any();
            if (tableExists)
            {
                var dropTableSql = $"DROP TABLE \"{oldEntityName}\"";
                _dbContext.Database.ExecuteSqlRaw(dropTableSql);
            }
            else
            {
                // Log or handle the case where the table doesn't exist
            }
        }


        public async Task<IDictionary<string, bool>> TablesHaveValuesAsync(List<string> tableNames)
        {
            var tablesWithValues = new Dictionary<string, bool>();

            try
            {
                foreach (var tableName in tableNames)
                {
                    var tableExists = await TableExistsAsync(tableName);
                    if (!tableExists)
                    {
                        tablesWithValues.Add(tableName, false);
                        continue;
                    }

                    var sql = $"SELECT 1 FROM \"{tableName}\" LIMIT 1";
                    var tableHasValues = await _dbContext.EntityListMetadataModels
                        .FromSqlRaw(sql)
                        .AnyAsync();
                    tablesWithValues.Add(tableName, tableHasValues);
                }

                return tablesWithValues;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking if tables have values: {ex.Message}");
                return new Dictionary<string, bool>();
            }
        }


        public async Task<(string EntityName, string EntityKeyColumnName, string EntityValueColumnName)> GetEntityData(int ListEntityId, int ListEntityKey, int ListEntityValue)
        {
            try
            {
                var entityName = await _dbContext.EntityListMetadataModels
                    .Where(entity => entity.Id == ListEntityId)
                    .Select(entity => entity.EntityName)
                    .FirstOrDefaultAsync();
                var entityKeyColumnName = await _dbContext.EntityColumnListMetadataModels
                    .Where(column => column.Id == ListEntityKey)
                    .Select(column => column.EntityColumnName)
                    .FirstOrDefaultAsync();
                var entityValueColumnName = await _dbContext.EntityColumnListMetadataModels
                    .Where(column => column.Id == ListEntityValue)
                    .Select(column => column.EntityColumnName)
                    .FirstOrDefaultAsync();
                if (entityName != null && entityKeyColumnName != null && entityValueColumnName != null)
                {
                    return (entityName, entityKeyColumnName, entityValueColumnName);
                }
                return (string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching entity data: {ex.Message}");
                return (string.Empty, string.Empty, string.Empty);
            }
        }
        public void UpdateEntityListMetadataModels()
        {
            try
            {
                // Use raw SQL to get all table names from INFORMATION_SCHEMA.TABLES
                var allTableNames = _dbContext.EntityListMetadataModels
                    .FromSqlRaw("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
                    .Select(e => e.EntityName)
                    .ToList();

                // Assuming EntityListMetadataModels has an associated DbSet in YourDbContext
                var entityList = _dbContext.EntityListMetadataModels;

                foreach (var tableName in allTableNames)
                {
                    // Check if the table name already exists in EntityListMetadataModels
                    if (!entityList.Any(e => e.EntityName == tableName))
                    {
                        // Add a new record to EntityListMetadataModels for the new table
                        entityList.Add(new EntityListMetadataModel { EntityName = tableName });
                    }
                }

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"An error occurred while updating EntityListMetadataModels: {ex.Message}");
            }
        }

    }
}
