using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    class Post
    {
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string category { get; set; }
        public string username { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class PostViewModel
    {
        public string title { get; set; }
        public string body { get; set; }
        public string category { get; set; }

    }
}
