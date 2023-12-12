using DynamicTableCreation.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using DynamicTableCreation.Data;
using DynamicTableCreation.Models;


namespace DynamicTableCreation.Services
{
    public class ConnectionStringService
    {
        private readonly ApplicationDbContext _dbContext;
        public ConnectionStringService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private readonly List<string> TablesToExclude = new List<string>
        {
            "EntityColumnListMetadataModels",
            "logChilds",
            "UserRoleModel",
            "UserTableModel",
            "EntityListMetadataModels",
            "logParents",
            "__EFMigrationsHistory"
        };

        public string[] GetTableNames(string connectionString)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var tableNames = GetTableNames(connection);
                Console.WriteLine("Available User-Created Table Names:");
                foreach (var tableName in tableNames)
                {
                    Console.WriteLine(tableName);
                }
                return tableNames;
            }
        }


        private string[] GetTableNames(NpgsqlConnection connection)
        {
            using (var command = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema = 'public'", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var tableNames = new List<string>();
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        if (!TablesToExclude.Contains(tableName))
                        {
                            tableNames.Add(tableName);
                        }
                    }
                    return tableNames.ToArray();
                }
            }
        }

        public Dictionary<string, List<ColumnInfoDTO>> GetTableDetails(string connectionString)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                return GetTableDetails(connection);
            }
        }

        private Dictionary<string, List<ColumnInfoDTO>> GetTableDetails(NpgsqlConnection connection)
        {
            var tableDetails = new Dictionary<string, List<ColumnInfoDTO>>();

            foreach (var tableName in GetTableNames(connection))
            {
                tableDetails[tableName] = GetTableColumnsAndTypes(connection, tableName);
            }

            return tableDetails;
        }

        private List<ColumnInfoDTO> GetTableColumnsAndTypes(NpgsqlConnection connection, string tableName)
        {
            using (var command = new NpgsqlCommand($@"
            SELECT columns.column_name, columns.data_type, 
            CASE WHEN columns.column_name = kcu.column_name THEN 'PK' ELSE '' END AS key_type,
            CASE WHEN ccu.table_name IS NOT NULL THEN 'FK' ELSE '' END AS foreign_key
            FROM information_schema.columns
            LEFT JOIN information_schema.key_column_usage kcu 
            ON information_schema.columns.table_name = kcu.table_name AND information_schema.columns.column_name = kcu.column_name
            LEFT JOIN information_schema.constraint_column_usage ccu
            ON information_schema.columns.table_name = ccu.table_name AND information_schema.columns.column_name = ccu.column_name
            WHERE information_schema.columns.table_name = '{tableName}'", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var columnsAndTypes = new List<ColumnInfoDTO>();
                    while (reader.Read())
                    {
                        string columnName = reader.GetString(0);
                        string dataType = reader.GetString(1);
                        string keyType = reader.GetString(2);
                        //string foreignKey = reader.GetString(3);

                        columnsAndTypes.Add(new ColumnInfoDTO
                        {
                            Name = columnName,
                            Type = dataType,
                            keyType = keyType,
                            //foreignKey = foreignKey
                        });
                    }
                    return columnsAndTypes;
                }
            }
        }
        //public void AddTableDetailsToDatabase(Dictionary<string, List<ColumnInfoDTO>> tableDetails)
        //{
        //    foreach (var tableName in tableDetails.Keys)
        //    {
        //        _dbContext.EntityListMetadataModels.Add(new EntityListMetadataModel { EntityName = tableName });
        //        foreach (var columnInfo in tableDetails[tableName])
        //        {
        //            bool isPrimaryKey = columnInfo.keyType == "PK";
        //            //bool isForeignKey = columnInfo.foreignKey == "FK";

        //            _dbContext.EntityColumnListMetadataModels.Add(new EntityColumnListMetadataModel
        //            {
        //                EntityColumnName = columnInfo.Name,
        //                Datatype = columnInfo.Type,
        //                Length = 0,
        //                MinLength = 0,
        //                MaxLength = 0,
        //                MinRange = 0,
        //                MaxRange = 0,
        //                DateMinValue = "",
        //                DateMaxValue = "",
        //                Description = "",
        //                IsNullable = false,
        //                DefaultValue = "",
        //                ListEntityId = 0,
        //                ListEntityKey = 0,
        //                ListEntityValue = 0,
        //                True = "",
        //                False = "",
        //                ColumnPrimaryKey = isPrimaryKey,
        //                CreatedDate = DateTime.UtcNow,
        //                UpdatedDate = DateTime.UtcNow,
        //                EntityId = 0
        //            });
        //        }
        //    }
        //    _dbContext.SaveChanges();
        //}
        public void AddTableDetailsToDatabase(Dictionary<string, List<ColumnInfoDTO>> tableDetails)
        {
            foreach (var tableName in tableDetails.Keys)
            {
                // Save changes to EntityListMetadataModels to generate EntityId
                _dbContext.EntityListMetadataModels.Add(new EntityListMetadataModel { EntityName = tableName });
                _dbContext.SaveChanges();

                // Retrieve the generated EntityId
                int entityId = _dbContext.EntityListMetadataModels.Single(e => e.EntityName == tableName).Id;

                foreach (var columnInfo in tableDetails[tableName])
                {
                    bool isPrimaryKey = columnInfo.keyType == "PK";

                    _dbContext.EntityColumnListMetadataModels.Add(new EntityColumnListMetadataModel
                    {
                        EntityColumnName = columnInfo.Name,
                        Datatype = columnInfo.Type,
                        Length = 0,
                        MinLength = 0,
                        MaxLength = 0,
                        MinRange = 0,
                        MaxRange = 0,
                        DateMinValue = "",
                        DateMaxValue = "",
                        Description = "",
                        IsNullable = false,
                        DefaultValue = "",
                        ListEntityId = 0,
                        ListEntityKey = 0,
                        ListEntityValue = 0,
                        True = "",
                        False = "",
                        ColumnPrimaryKey = isPrimaryKey,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        EntityId = entityId // Use the retrieved EntityId
                    });
                }
            }

            _dbContext.SaveChanges(); // Save changes to EntityColumnListMetadataModels
        }

    }
}

