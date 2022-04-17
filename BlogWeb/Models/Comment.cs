using System;
using System.Collections.Generic;

namespace BlogWeb.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public string? Contents { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserAccountId { get; set; }
        public int? PostId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual UserAccount? UserAccount { get; set; }
    }
}
