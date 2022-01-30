using Games.SharedModels.Requests.PlayerRequests;
using Games.SharedModels.Responses;
using GamesApi.Logic;
using GamesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GamesApi.Api.Models
{
    public class TokenValidator
    {
        GamePlayerLogic _gamePlayerLogic = new GamePlayerLogic();
        public BaseResponse ValidateToken(string userName, string token)
        {

            bool doesExist = _gamePlayerLogic.ValidateUsernameExists(userName);
            if (!doesExist)
            {
                return new BaseResponse()
                {
                    ErrorCode = 6000,
                    ErrorMessage = "Username does not exist in our DB, get fucked",
                    IsSuccess = false
                };
            }
            Player player = _gamePlayerLogic.GetPlayers().FirstOrDefault(a => a.UserName == userName);
            if (!_gamePlayerLogic.IsLoggedIn(new LoginRequest()
            {
                UserName = userName
            }))
            {
                return new BaseResponse
                {
                    ErrorCode = 6000,
                    ErrorMessage = "You ain't logged in bruh.",
                    IsSuccess = false
                };
            }
            bool doesTokenMatch = _gamePlayerLogic.ValidateTokenMatches(userName, token);
            if (!doesTokenMatch)
            {
                return new BaseResponse()
                {
                    ErrorCode = 6000,
                    ErrorMessage = "Token does not exist in our DB, get fucked",
                    IsSuccess = false
                };
            }
            bool didTokenExpire = _gamePlayerLogic.ValidateTokenExpiration(userName, token);
            if (!didTokenExpire)
            {
                return new BaseResponse()
                {
                    ErrorCode = 6000,
                    ErrorMessage = "Session expired, get fucked",
                    IsSuccess = false
                };
            }
            return new BaseResponse()
            {
                IsSuccess = true
            };
        }
    }
}