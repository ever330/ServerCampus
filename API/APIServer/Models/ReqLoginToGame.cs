using System.ComponentModel.DataAnnotations;

namespace APIServer.Models
{
    public class ReqLoginToGame
    {
        [Required]
        [Length(1, 50)]
        public string Id { get; set; } = null!;
        [Required]
        [Length(1, 50)]
        public string AuthToken { get; set; } = null!;
    }
}
