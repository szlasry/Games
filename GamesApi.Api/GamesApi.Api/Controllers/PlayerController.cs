using Games.SharedModels.Requests;
using Games.SharedModels.Requests.PlayerRequests;
using Games.SharedModels.Responses;
using Games.SharedModels.Responses.PlayerResponses;
using Games.SharedModels.ViewModel.PlayerViewModels;
using GamesApi.Api.Models;
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
        private GamePlayerLogic gamePlayerLogic = new GamePlayerLogic();
        private TokenValidator tokenValidator = new TokenValidator();
        [Route("api/Player/AddPlayer")]
        [HttpPost]
        //https://localhost:44322/Player/AddPlayer
        public BaseResponse AddPlayer(AddPlayerRequest request)
        {
            try
            {
                bool doesExist = gamePlayerLogic.ValidateUsernameExists(request.UserName);
                if (doesExist)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Username already exists in our DB",
                        IsSuccess = false
                    };
                }
                gamePlayerLogic.AddPlayer(new Player()
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
                List<Player> listOfPlayers = gamePlayerLogic.GetPlayers();
                return new GetPlayersResponse()
                {
                    Players = listOfPlayers.Select(a => new PlayerViewModel()
                    {
                        Id = a.Id,
                        UserName = a.UserName,
                        Country = a.Country,
                        Birthday = a.Birthday,
                        IsAdmin = a.IsAdmin,
                        IsSuspended = a.IsSuspended
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
                Player player = gamePlayerLogic.GetPlayer(id);
                return new GetPlayerResponse()
                {
                    Player = new PlayerViewModel()
                    {
                        Id = player.Id,
                        UserName = player.UserName,
                        Country = player.Country,
                        Birthday = player.Birthday,
                        IsAdmin = player.IsAdmin,
                        IsSuspended = player.IsSuspended
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
        public BaseResponse DeletePlayer([FromUri] DeletePlayerRequest request)
        {
            try
            {
                BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
                if (!baseResponse.IsSuccess)
                {
                    return baseResponse;
                }
                gamePlayerLogic.DeletePlayer(request.Id);
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
        //[Route("api/Player/UpdatePlayer")]
        //[HttpPost]
        ////https://localhost:44322/Player/UpdatePlayer
        //public BaseResponse UpdatePlayer(Player player)
        //{
        //    try
        //    {
        //        gamePlayerLogic.UpdatePlayer(player);
        //        return new BaseResponse()
        //        {
        //            ErrorCode = 0,
        //            ErrorMessage = null,
        //            IsSuccess = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BaseResponse
        //        {
        //            ErrorCode = 5000,
        //            ErrorMessage = ex.Message,
        //            IsSuccess = false
        //        };
        //    }
        //}
        //https://localhost:44322/api/Player/SuspendPlayer
        [Route("api/Player/SuspendPlayer")]
        [HttpPost]
        public BaseResponse SuspendPlayer(SuspendPlayerRequest request)
        {
            BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
            if (!baseResponse.IsSuccess)
            {
                return new BaseResponse
                {
                    ErrorCode = 100,
                    ErrorMessage = "Username and token have been deemed unworthy",
                    IsSuccess = false
                };
            }
            else
            {
                gamePlayerLogic.SuspendPlayer(request.TargetPlayerId);
                return new BaseResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
        }
        [Route("api/Player/UnsuspendPlayer")]
        [HttpPost]
        public BaseResponse UnsuspendPlayer(UnsuspendPlayerRequest request)
        {
            BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
            if (!baseResponse.IsSuccess)
            {
                return new BaseResponse
                {
                    ErrorCode = 100,
                    ErrorMessage = "Username and token have been deemed unworthy",
                    IsSuccess = false
                };
            }
            else
            {
                gamePlayerLogic.UnsuspendPlayer(request.TargetPlayerId);
                return new BaseResponse
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
        }
        [Route("api/Player/UserUpdateProfile")]
        [HttpPost]
        public BaseResponse UserUpdateProfile(UpdateProfileRequest request)
        {
            BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
            if (!baseResponse.IsSuccess)
            {
                return baseResponse;
            }
            gamePlayerLogic.UserUpdateProfile(new Player() 
            {
                Id = request.Player.Id,
                Country = request.Player.Country
            });
            return new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = null,
                IsSuccess = true
            };
        }
        [Route("api/Player/UserUpdatePassword")]
        [HttpPost]
        public BaseResponse UserUpdatePassword(UpdatePasswordRequest request)
        {
            BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
            if (!baseResponse.IsSuccess)
            {
                return baseResponse;
            }
            Player playerFromDB = gamePlayerLogic.GetPlayer(request.Id);
            if (Hash.CreateMD5(request.OldPassword) != playerFromDB.Password)
            {
                return new BaseResponse()
                {
                    ErrorCode = 600,
                    ErrorMessage = "You entered the wrong password",
                    IsSuccess = false
                };
            }
            else
            {
                gamePlayerLogic.UserUpdatePassword(Hash.CreateMD5(request.NewPassword), request.Id);
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }

        }
        //https://localhost:44322/api/Player/LogIn
        [Route("api/Player/LogIn")]
        [HttpPost]
        public LoginPlayerResponse LogIn(LoginRequest request)
        {
            try
            {
                bool doesNameExist = gamePlayerLogic.ValidateUsernameExists(request.UserName);
                if (!doesNameExist)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Username does not exist in our DB, get fucked",
                        IsSuccess = false
                    };
                }
                bool isPasswordCorrect = gamePlayerLogic.IsPasswordCorrect(request);
                if (!isPasswordCorrect)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Wrong Password, get fucked",
                        IsSuccess = false
                    };
                }
                bool isLoggedIn = gamePlayerLogic.IsLoggedIn(request);
                if (isLoggedIn)
                {
                    return new LoginPlayerResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "You are already signed in, what trickery is this.",
                        IsSuccess = false
                    };
                }
                List<Player> listOfPlayers = gamePlayerLogic.GetPlayers();
                Player dbPlayer = listOfPlayers.FirstOrDefault(a => a.UserName == request.UserName);
                string token = Guid.NewGuid().ToString();
                gamePlayerLogic.UpdatePlayerStatus(request.UserName, true, token);
                return new LoginPlayerResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true,
                    Player = new PlayerViewModel
                    {
                        Id = dbPlayer.Id,
                        UserName = dbPlayer.UserName,
                        Country = dbPlayer.Country,
                        Birthday = dbPlayer.Birthday,
                        IsAdmin = dbPlayer.IsAdmin,
                        Token = token,
                        IsSuspended = dbPlayer.IsSuspended
                    }
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
        public BaseResponse LogOut(BaseRequest request)
        {
            try
            {
                if (!gamePlayerLogic.ValidateUsernameExists(request.UserName))
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Your name does not exist in our database.",
                        IsSuccess = false
                    };
                }
                if (!gamePlayerLogic.IsLoggedIn(new LoginRequest()
                {
                    UserName = request.UserName
                }))
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "You are already signed out, what trickery is this.",
                        IsSuccess = false
                    };
                }
                gamePlayerLogic.UpdatePlayerStatus(request.UserName, false, null);
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
