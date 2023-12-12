using ExcelGeneration.Data;
using Microsoft.EntityFrameworkCore;
using ExcelGeneration.Models;


namespace ExcelGeneration.Services
{
    public class ExportExcelService
    {
        private readonly ApplicationDbContext _dbContext;
        public ExportExcelService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LogChild>> GetAllLogChildsByParentIDAsync(int parentID)
        {
            try
            {
                return await _dbContext.logChilds
                    .Where(c => c.ParentID == parentID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
