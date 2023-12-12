namespace ExcelGeneration.Models.DTO
{
    public class EntityListMetadataModelDTO
    {
        public int Id { get; set; }
        public string EntityName { get; set; }

        public static explicit operator EntityListMetadataModelDTO(EntityListMetadataModel data)
        {
            return new EntityListMetadataModelDTO
            {
                Id = data.Id,
                EntityName = data.EntityName,
            };
        }

        public static implicit operator EntityListMetadataModel(EntityListMetadataModelDTO data)
        {
            return new EntityListMetadataModel
            {
                Id = data.Id,
                EntityName = data.EntityName,
            };
        }
    }
}
