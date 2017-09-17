using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class UserViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
    
    }
}
