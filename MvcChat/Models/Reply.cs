using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcChat.Models
{
    public class Reply
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        public string? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; } = null!;
        [DataType(DataType.DateTime)]
        public DateTime ReplyTime { get; set; } = DateTime.Now;
    }
}
