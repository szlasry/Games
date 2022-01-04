using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.GameRequests
{
    public class AddGameRequest : BaseRequest
    {
        public string Name { get; set; }
        public int NumberOfPlayers { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }
    }
}
