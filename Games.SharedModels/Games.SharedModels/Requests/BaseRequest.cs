using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests
{
    public class BaseRequest
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
