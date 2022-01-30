using Games.SharedModels.ViewModel.PlayerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.PlayerRequests
{
    public class UpdateProfileRequest : BaseRequest
    {
        public PlayerViewModel Player { get; set; }
    }
}
