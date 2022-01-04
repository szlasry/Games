using Games.SharedModels.ViewModel.GameViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.SharedModels.Responses.GameResponses
{
    public class GetGameResponse:BaseResponse
    {
        public GameViewModel Game { get; set; }
    }
}
