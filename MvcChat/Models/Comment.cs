using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcChat.Models
{
    public class Comment
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        public string CommenterId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
        [DataType(DataType.DateTime)]
        public DateTime SendTime { get; set; } = DateTime.Now;
        public ICollection<Reply>? Replies { get; set; }
    }
}
