using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userDash2.Models
{
    public class Message : BaseEntity
    {
        [Key]
        public int MessageId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int PostedOn { get; set; }
        public User Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Comment> Comments { get; set; }

        public Message()
        {
            Comments = new List<Comment>();
        }
    }
}