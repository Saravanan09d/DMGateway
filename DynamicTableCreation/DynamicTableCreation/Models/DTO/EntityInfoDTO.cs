namespace DynamicTableCreation.Models.DTO
{
    public class EntityInfoDTO
    {
        public int EntityId { get; set; }
        public List<EntityColumnInfoDTO> Columns { get; set; }
    }
   
}
