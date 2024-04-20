using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models
{
    public class ReqLoginToHive
    {
        [Required]
        [Length(1, 50)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        public string Email { get; set; } = null!;
        [Required]
        [Length(1, 100)]
        public string Password { get; set; } = null!;
    }
}
