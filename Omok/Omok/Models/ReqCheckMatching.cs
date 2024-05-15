using System.ComponentModel.DataAnnotations;

namespace Omok.Models
{
    public class ReqCheckMatching
    {
        [Required]
        [Length(1, 50)]
        public string Id { get; set; } = null!;
    }
}
