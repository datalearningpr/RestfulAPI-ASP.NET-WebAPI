using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    class Comment
    {
        public int id { get; set; }
        public string body { get; set; }
        public int postId { get; set; }
        public String username { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class CommentViewModel
    {
        public int postId { get; set; }
        public string comment { get; set; }
    }

}
