using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Responses.PlayerResponses
{
    public class LoginPlayerResponse : BaseResponse
    {
        public PlayerResponses P { get; set; }
        public string Token { get; set; }
    }
}
