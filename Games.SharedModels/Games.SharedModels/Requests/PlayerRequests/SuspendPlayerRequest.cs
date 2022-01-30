using Games.SharedModels.ViewModel.PlayerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.PlayerRequests
{
    public class SuspendPlayerRequest : PlayerViewModel
    {
        public int TargetPlayerId { get; set; }
    }
}
