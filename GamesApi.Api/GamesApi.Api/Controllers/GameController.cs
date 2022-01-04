using Games.SharedModels.Requests.GameRequests;
using Games.SharedModels.Responses;
using Games.SharedModels.Responses.GameResponses;
using Games.SharedModels.ViewModel.GameViewModels;
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
        public GameController()
        {
            _gamePlayerLogic = new GamePlayerLogic();
        }

        [HttpPost]
        //https://localhost:44322/api/Game/AddGame
        public BaseResponse AddGame(AddGameRequest request)
        {
            try
            {
                bool doesExist = _gamePlayerLogic.ValidateGameExists(request.Name);
                if (doesExist)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 6000,
                        ErrorMessage = "Game already exists in our DB",
                        IsSuccess = false
                    };
                }
                _gamePlayerLogic.AddGame(new Game()
                {
                    Name = request.Name,
                    NumberOfPlayers = request.NumberOfPlayers,
                    Image = request.Image,
                    URL = request.URL
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
        {
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
        //https://localhost:44322/api/Game/DeleteGame?id=
        public BaseResponse DeleteGame(int id)
        {
            try
            {
                _gamePlayerLogic.DeleteGame(id);
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
        //https://localhost:44322/api/Game/UpdateGame
        public BaseResponse UpdateGame(Game game)
        {
            try
            {
                bool didSucceed = _gamePlayerLogic.UpdateGame(game);
                if(!didSucceed)
                {
                    return new BaseResponse()
                    {
                        ErrorCode = 4000,
                        ErrorMessage = $"Did not Find any Game with the ID : {game.Id}",
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
