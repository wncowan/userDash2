using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userDash2.Models
{
    public class Comment : BaseEntity
    {
        [Key]
        public int CommentId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}