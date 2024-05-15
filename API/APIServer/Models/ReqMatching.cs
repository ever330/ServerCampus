using System.ComponentModel.DataAnnotations;

namespace APIServer.Models
{
    public class ReqMatching
    {
        [Required]
        [Length(1, 50)]
        public string Id { get; set; } = null!;
    }
}
