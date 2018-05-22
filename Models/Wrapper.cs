using System;
using System.Collections.Generic;

namespace userDash2.Models
{
    public class Wrapper : BaseEntity
    {
        // ADD LISTS OF EVERY MODEL TYPE AS NECESSARY
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }
        public List<Comment> Comments { get; set; }

        public Wrapper(List<User> users, List<Message> messages, List<Comment> comments)
        {
            Users = users;
            Messages = messages;
            Comments = comments;
        }
    }
}