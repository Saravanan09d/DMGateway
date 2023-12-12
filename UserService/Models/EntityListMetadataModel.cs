namespace UserService.Models
{
    public class EntityListMetadataModel : BaseModel
    {
        public EntityListMetadataModel()
        {
            // Initialize the EntityColumns collection in the constructor
            EntityColumns = new List<EntityColumnListMetadataModel>();
        }
        public int Id { get; set; }
        public string EntityName { get; set; }
        public List<EntityColumnListMetadataModel> EntityColumns { get; set; }
    }
}
