using Games.SharedModels.ViewModel.GameViewModels;
using GamesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Requests.GameRequests
{
    public class UpdateGameRequest: BaseRequest
    {
        public GameViewModel Game { get; set; }
    }
}
