using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Model.DTO
{
    public class UserTableModelDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
