using Dapper;
using DynamicTableCreation.Data;
using DynamicTableCreation.Models.DTO;
using DynamicTableCreation.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace DynamicTableCreation.Services
{
    public class ViewService : IViewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;


        public ViewService(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        public IEnumerable<EntityColumnDTO> GetColumnsForEntity(string entityName)
        {
            try
            {
                var entity = _context.EntityListMetadataModels.FirstOrDefault(e => e.EntityName == entityName);

                if (entity == null)
                {
                    return null;
                }
                var columnsDTO = _context.EntityColumnListMetadataModels
                    .Where(column => column.EntityId == entity.Id)
                    .Select(column => new EntityColumnDTO
                    {
                        Id = column.Id,
                        EntityId = column.EntityId,
                        EntityColumnName = column.EntityColumnName,
                        Datatype = column.Datatype,
                        Length = column.Length,
                        MinLength = column.MinLength,
                        MaxLength = column.MaxLength,
                        MinRange = column.MinRange,
                        MaxRange = column.MaxRange,
                        DateMinValue = column.DateMinValue,
                        DateMaxValue = column.DateMaxValue,
                        Description = column.Description,
                        IsNullable = column.IsNullable,
                        DefaultValue = column.DefaultValue,
                        ColumnPrimaryKey = column.ColumnPrimaryKey,
                        True = column.True,
                        False = column.False,
                        ListEntityId = column.ListEntityId,
                        ListEntityKey = column.ListEntityKey,
                        ListEntityValue = column.ListEntityValue                        

                    }).ToList();
                columnsDTO = columnsDTO.Select(column =>
                {
                    column.S_ListEntityId = GetEntityNameByEntityId(column.ListEntityId);
                    column.S_ListEntityKey = GetEntityColumnNameByEntityId(column.ListEntityKey);
                    column.S_ListEntityValue = GetEntityColumnNameByEntityId(column.ListEntityValue);
                    return column;
                }).ToList();

                if (columnsDTO.Count == 0)
                {
                    return null;
                }
          
                return columnsDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetColumnsForEntity: {ex.Message}");
                throw;
            }
        }

        public string GetEntityNameByEntityId( int listentityId)
        {
            // Query the EntityListMetadataModels DbSet to get the EntityName based on the listEntityId
            var entityNameEntity = _context.EntityListMetadataModels.FirstOrDefault(entity => entity.Id == listentityId);

            if (entityNameEntity == null)
            {
                // Handle the case where the EntityName is not found
                return null;
            }
            var tableName = entityNameEntity.EntityName;

            return tableName;
        }

        public string GetEntityColumnNameByEntityId(int listentitycolumnId)
        {
            // Query the EntityListMetadataModels DbSet to get the EntityName based on the listEntityId
            var entityNameEntity = _context.EntityColumnListMetadataModels.FirstOrDefault(x => x.Id == listentitycolumnId);

            if (entityNameEntity == null)
            {
                // Handle the case where the EntityName is not found
                return null;
            }
            var columnname = entityNameEntity.EntityColumnName;

            return columnname;
        }

        public async Task<(string TableName, List<dynamic> Rows)> GetTableDataByListEntityId(int listEntityId)
        {
            // Use Entity Framework Core to get the table name
            var tableNameEntity = _context.EntityColumnListMetadataModels.FirstOrDefault(mapping => mapping.ListEntityId == listEntityId);

            if (tableNameEntity == null)
            {
                // Handle the case where the table name is not found
                return (null, null);
            }

            // Query the EntityListMetadataModels DbSet to get the EntityName based on the listEntityId
            var entityNameEntity = _context.EntityListMetadataModels.FirstOrDefault(entity => entity.Id == tableNameEntity.ListEntityId);

            if (entityNameEntity == null)
            {
                // Handle the case where the EntityName is not found
                return (null, null);
            }

            string tableName = entityNameEntity.EntityName;

            try
            {
                using (IDbConnection dbConnection = new NpgsqlConnection(_dbConnection.ConnectionString))
                {
                    dbConnection.Open();

                    // Dynamically query the table based on the provided table name
                    string rowDataQuery = $"SELECT * FROM public.\"{tableName}\"";

                    // Use Dapper to execute the query and return the results
          
                    var rows = dbConnection.Query(rowDataQuery).ToList();


                    return (tableName, rows);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in GetTableDataByListEntityId: {ex.Message}");
                throw;
            }

        }

        

    }
}
