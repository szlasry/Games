using Games.SharedModels.Requests.PlayerRequests;
using Games.SharedModels.Responses;
using Games.SharedModels.Responses.PlayerResponses;
using Games.SharedModels.ViewModel.PlayerViewModels;
using GamesApi.Logic;
using GamesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GamesApi.Api.Controllers
{
    public class PlayerController : ApiController
    {
        [HttpPost]
        //https://localhost:44322/Player/AddPlayer
        public BaseResponse AddPlayer(AddPlayerRequest request)
        {
            try
            {
                GamePlayerLogic gameLogic = new GamePlayerLogic();
                bool doesExist = gameLogic.ValidateUsernameExists(request.UserName);
                if (doesExist)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Username already exists in our DB",
                        IsSuccess = false
                    };
                }
                gameLogic.AddPlayer(new Player()
                {
                    UserName = request.UserName,
                    Password = request.Password,
                    Country = request.Country,
                    Birthday = request.Birthday,
                });
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        [Route("api/Player/GetPlayers")]
        //https://localhost:44322/api/Player/GetPlayers
        public BaseResponse GetPlayers()
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                List<Player> listOfPlayers = gamePlayerLogic.GetPlayers();
                return new GetPlayersResponse()
                {
                    Players = listOfPlayers.Select(a => new PlayerViewModel()
                    {
                        Id = a.Id,
                        UserName = a.UserName,
                        Country = a.Country,
                        Birthday = a.Birthday,
                        IsAdmin = a.IsAdmin
                    }).ToList(),
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new GetPlayersResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        //https://localhost:44322/api/Player/GetPlayer
        public BaseResponse GetPlayer(int id)
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                Player player = gamePlayerLogic.GetPlayer(id);
                return new GetPlayerResponse()
                {
                    Player = new PlayerViewModel()
                    {
                        Id = player.Id,
                        UserName = player.UserName,
                        Country = player.Country,
                        Birthday = player.Birthday,
                        IsAdmin = player.IsAdmin
                    },
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        [HttpDelete]
        //https://localhost:44322/Player/DeletePlayer?id=
        public BaseResponse DeletePlayer(int id)
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                gamePlayerLogic.DeletePlayer(id);
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        [HttpPut]
        //https://localhost:44322/Player/UpdatePlayer
        public BaseResponse UpdatePlayer(Player player)
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                gamePlayerLogic.UpdatePlayer(player);
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        //https://localhost:44322/api/Player/LogIn
        [Route("api/Player/LogIn")]
        [HttpPost]
        public LoginPlayerResponse LogIn(Player player)
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                bool doesNameExist = gamePlayerLogic.ValidateUsernameExists(player.UserName);
                if (!doesNameExist)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Username does not exist in our DB, get fucked",
                        IsSuccess = false
                    };
                }
                bool isPasswordCorrect = gamePlayerLogic.IsPasswordCorrect(player);
                if (!isPasswordCorrect)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Wrong Password, get fucked",
                        IsSuccess = false
                    };
                }
                bool isLoggedIn = gamePlayerLogic.IsLoggedIn(player);
                if (isLoggedIn)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "You are already signed in, what trickery is this.",
                        IsSuccess = false
                    };
                }
                string token = Guid.NewGuid().ToString();
                gamePlayerLogic.UpdatePlayerStatus(player.UserName, true, token);
                return new LoginPlayerResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true,
                    Token = token

                };
            }
            catch (Exception ex)
            {
                return new LoginPlayerResponse
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        [Route("api/Player/LogOut")]
        [HttpPost]
        //https://localhost:44322/api/Player/LogOut
        public BaseResponse LogOut(Player player)
        {
            try
            {
                GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
                if (!gamePlayerLogic.ValidateUsernameExists(player.UserName))
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Your name does not exist in our database.",
                        IsSuccess = false
                    };
                }
                if (!gamePlayerLogic.IsLoggedIn(player))
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "You are already signed out, what trickery is this.",
                        IsSuccess = false
                    };
                }
                gamePlayerLogic.UpdatePlayerStatus(player.UserName, false, null);
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }


        }
    }
}
