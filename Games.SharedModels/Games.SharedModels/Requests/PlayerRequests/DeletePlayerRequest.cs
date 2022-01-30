using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.PlayerRequests
{
    public class DeletePlayerRequest:BaseRequest
    {
        public int Id { get; set; }
    }
}
