using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicTableCreation.Models
{
    public class UserTableModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [ForeignKey("RoleId")]
        public int  RoleId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
