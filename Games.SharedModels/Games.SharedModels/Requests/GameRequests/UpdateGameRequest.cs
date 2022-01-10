using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.GameRequests
{
    public class UpdateGameRequest: AddGameRequest
    {
        public int Id { get; set; }
    }
}
