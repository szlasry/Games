using Games.SharedModels.Requests.GameRequests;
using Games.SharedModels.Responses;
using Games.SharedModels.Responses.GameResponses;
using Games.SharedModels.ViewModel.GameViewModels;
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
    public class GameController : ApiController
    {
        private GamePlayerLogic _gamePlayerLogic;
        private TokenValidator tokenValidator = new TokenValidator();
        public GameController()
        {
            _gamePlayerLogic = new GamePlayerLogic();
        }
        [Route("api/Game/AddGame")]
        [HttpPost]
        //https://localhost:44322/api/Game/AddGame
        public BaseResponse AddGame(AddGameRequest request)
        {
            try
            {
                BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
                if (!baseResponse.IsSuccess)
                {
                    return baseResponse;
                }
                bool doesExist = _gamePlayerLogic.ValidateGameExists(request.Game.Name);
                if (doesExist)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Game already exists in our DB",
                        IsSuccess = false
                    };
                }
                bool isUserAdmin = _gamePlayerLogic.ValidatePlayerIsAdmin(request.UserName);
                if (!isUserAdmin)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 7000,
                        ErrorMessage = "You ain't no admin",
                        IsSuccess = false
                    };
                }
                _gamePlayerLogic.AddGame(new Game()
                {
                    Name = request.Game.Name,
                    NumberOfPlayers = request.Game.NumberOfPlayers,
                    Image = request.Game.Image,
                    URL = request.Game.URL
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
        [Route("api/Game/GetGames")]
        //https://localhost:44322/api/Game/GetGames
        public GetGamesResponse GetGames()
        {// at some point will be a limitation for banned players/ validate that the player isn't suspended.
            try
            {

                List<Game> listOfGames = _gamePlayerLogic.GetGames();
                return new GetGamesResponse()
                {
                    Games = listOfGames.Select(a => new GameViewModel()
                    {
                        Id = a.Id,
                        Name = a.Name,
                        NumberOfPlayers = a.NumberOfPlayers,
                        Image = a.Image,
                        URL = a.URL
                    }).ToList(),
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new GetGamesResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }


        }
        [HttpGet]
        //https://localhost:44322/api/Game/GetGame?id=
        public GetGameResponse GetGame(int id)
        {
            try
            {
                Game game = _gamePlayerLogic.GetGame(id);
                return new GetGameResponse()
                {
                    Game = new GameViewModel
                    {
                        Id = game.Id,
                        Name = game.Name,
                        NumberOfPlayers = game.NumberOfPlayers,
                        Image = game.Image,
                        URL = game.URL
                    },
                    ErrorCode = 0,
                    ErrorMessage = null,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new GetGameResponse()
                {
                    ErrorCode = 5000,
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
        [HttpDelete]
        //https://localhost:44322/api/Game/DeleteGame?id=&username=&token=
        public BaseResponse DeleteGame([FromUri] DeleteGameRequest request)
        {
            try
            {
                bool isAdmin = _gamePlayerLogic.ValidatePlayerIsAdmin(request.UserName);
                if (!isAdmin)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 200,
                        ErrorMessage = "You ain't admin bitch",
                        IsSuccess = false
                    };
                }
                BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
                if (baseResponse.IsSuccess)
                {
                    _gamePlayerLogic.DeleteGame(request.Id);
                    return baseResponse;
                }
                return new BaseResponse()
                {
                    ErrorCode = 0,
                    ErrorMessage = "Failure",
                    IsSuccess = false
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
        [Route("api/Game/UpdateGame")]
        [HttpPost] 
        //https://localhost:44322/api/Game/UpdateGame
        public BaseResponse UpdateGame(UpdateGameRequest request)
        {
            try
            {
                bool isAdmin = _gamePlayerLogic.ValidatePlayerIsAdmin(request.UserName);
                if (!isAdmin)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 200,
                        ErrorMessage = "You ain't admin bitch",
                        IsSuccess = false
                    };
                }
                BaseResponse baseResponse = tokenValidator.ValidateToken(request.UserName, request.Token);
                if (!baseResponse.IsSuccess)
                {
                    return baseResponse;
                }
                bool didSucceed = _gamePlayerLogic.UpdateGame(new Game() 
                {
                    Id = request.Game.Id,
                    Name = request.Game.Name,
                    NumberOfPlayers = request.Game.NumberOfPlayers,
                    Image = request.Game.Image,
                    URL = request.Game.URL
                });
                if(!didSucceed)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 4000,
                        ErrorMessage = $"Did not Find any Game with the ID : {request.Game.Id}",
                        IsSuccess = false
                    };
                }
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
