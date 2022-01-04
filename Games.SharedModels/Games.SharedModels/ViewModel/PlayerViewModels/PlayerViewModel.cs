using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.ViewModel.PlayerViewModels
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Country { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsAdmin { get; set; }
    }
}
