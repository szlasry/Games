using Games.SharedModels.ViewModel.PlayerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Responses.PlayerResponses
{
    public class GetPlayersResponse:BaseResponse
    {
        public List<PlayerViewModel> Players { get; set; }
    }
}
