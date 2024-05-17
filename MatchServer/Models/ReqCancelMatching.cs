using System.ComponentModel.DataAnnotations;

namespace MatchServer.Models
{
    public class ReqCancelMatching
    {
        [Required]
        [Length(1, 50)]
        public string Id { get; set; } = null!;
    }
}
