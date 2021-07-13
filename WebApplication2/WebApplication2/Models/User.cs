using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } // имя пользователя
        public string Password { get; set; } // возраст пользователя
        public string Email { get; set; }
    }
}
