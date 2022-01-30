using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesApi.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
        public DateTime? LastSessionTime { get; set; }
        public bool IsSuspended { get; set; }

    }
}
