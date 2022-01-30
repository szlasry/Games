using Games.Client.Models;
using Games.Helpers;
using Games.SharedModels;
using Games.SharedModels.Requests;
using Games.SharedModels.Requests.GameRequests;
using Games.SharedModels.Requests.PlayerRequests;
using Games.SharedModels.Responses;
using Games.SharedModels.Responses.GameResponses;
using Games.SharedModels.Responses.PlayerResponses;
using Games.SharedModels.ViewModel.GameViewModels;
using Games.SharedModels.ViewModel.PlayerViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Games.Client.Controllers
{
    public class HomeController : Controller
    {
        public string BaseUri = "https://localhost:44367/api/";
        // GET: Home
        public async Task<ActionResult> Index()
        {
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendGetRequest($"{BaseUri}Game/GetGames");
            GetGamesResponse gamesResponse = JsonConvert.DeserializeObject<GetGamesResponse>(serializedResponse);
            if (gamesResponse.IsSuccess)
            {
                ViewBag.Games = GetGamesByGroups(gamesResponse.Games);
                return View("Index");
            }
            return View("ErrorView");
        }
        public ActionResult AddGame()
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (myPlayer.IsAdmin)
            {
                return View("AddGameView", new GameViewModel());
            }
            return View("NonAdminErrorView");

        }
        public async Task<ActionResult> AddGameRequest(GameViewModel game)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            string serializedGame = JsonConvert.SerializeObject(new AddGameRequest()
            {
                Game = new AddGameViewModel()
                {
                    Image = game.Image,
                    Name = game.Name,
                    NumberOfPlayers = game.NumberOfPlayers,
                    URL = game.URL,
                },
                Token = myPlayer.Token,
                UserName = myPlayer.UserName
            });
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Game/AddGame", serializedGame);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> DeleteGameRequest(int Id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendDeleteRequest($"{BaseUri}Game/DeleteGame?id={Id}&username={myPlayer.UserName}&token={myPlayer.Token}");
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }

        }
        public async Task<ActionResult> UpdateGame(int Id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendGetRequest($"{BaseUri}Game/GetGame?id={Id}");
            GetGameResponse response = JsonConvert.DeserializeObject<GetGameResponse>(serializedResponse);
            PlayerViewModel sessionPlayer = JsonConvert.DeserializeObject<PlayerViewModel>(Request.Cookies["User"]["Player"]);
            if (response.IsSuccess)
            {
                if (!sessionPlayer.IsAdmin)
                {
                    return View("NonAdminErrorView");
                }
                else
                {
                    return View("UpdateGameView", response.Game);
                }
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> UpdateGameRequest(GameViewModel game)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedGame = JsonConvert.SerializeObject(new UpdateGameRequest()
            {
                Game = new GameViewModel
                {
                    Id = game.Id,
                    Image = game.Image,
                    Name = game.Name,
                    NumberOfPlayers = game.NumberOfPlayers,
                    URL = game.URL,
                },
                Token = myPlayer.Token,
                UserName = myPlayer.UserName
            });
            string response = await httpHelper.SendPostRequest($"{BaseUri}Game/UpdateGame", serializedGame);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }
        }
        public ActionResult AddPlayer()
        {
            return View("AddPlayerView");
        }
        public async Task<ActionResult> AddPlayerRequest(AddPlayerRequest myPlayer)
        {
            if(myPlayer.UserName.Length < 3 || myPlayer.Password.Length <= 3 || myPlayer.Password != myPlayer.ConfirmPassword)
            {
                return View("ErrorView");
            }
            string serializedPlayer = JsonConvert.SerializeObject(myPlayer);
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/AddPlayer", serializedPlayer);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                return RedirectToAction("Login", new { UserName = myPlayer.UserName, Password = myPlayer.Password });
            }
            else
            {
                return View("ErrorView");
            }

        }
        public ActionResult AboutUs()
        {
            return View("AboutUsView");
        }
        public async Task<ActionResult> Login(LoginRequest request)
        {
            string serializedPlayer = JsonConvert.SerializeObject(request);
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/LogIn", serializedPlayer);
            LoginPlayerResponse loginPlayerResponse = JsonConvert.DeserializeObject<LoginPlayerResponse>(response);
            if (loginPlayerResponse.IsSuccess)
            {
                AddSession(loginPlayerResponse.Player);
                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }
        }

        public async Task<ActionResult> LogOut()
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            string serializedPlayer = JsonConvert.SerializeObject(new BaseRequest()
            {
                UserName = myPlayer.UserName,
                Token = myPlayer.Token
            });
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/LogOut", serializedPlayer);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                DeleteSession();

                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }

        }
        public async Task<ActionResult> ManagePlayers()
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendGetRequest($"{BaseUri}Player/GetPlayers");
            GetPlayersResponse response = JsonConvert.DeserializeObject<GetPlayersResponse>(serializedResponse);
            return View("ManagePlayersView", response.Players);
        }
        //27th jan
        public async Task<ActionResult> AdminDeletePlayer(int id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendDeleteRequest($"{BaseUri}Player/DeletePlayer?id={id}&username={myPlayer.UserName}&token={myPlayer.Token}");
            BaseResponse response = JsonConvert.DeserializeObject<BaseResponse>(serializedResponse);
            if (response.IsSuccess)
            {
                return RedirectToAction("ManagePlayers");
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> AdminSuspendPlayer(int id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            string serializedPlayer = JsonConvert.SerializeObject(new SuspendPlayerRequest()
            {
                UserName = myPlayer.UserName,
                Token = myPlayer.Token,
                TargetPlayerId = id
            });
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendPostRequest($"{BaseUri}Player/SuspendPlayer", serializedPlayer);
            BaseResponse response = JsonConvert.DeserializeObject<BaseResponse>(serializedResponse);
            if (response.IsSuccess)
            {
                return RedirectToAction("ManagePlayers");
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> AdminUnsuspendPlayer(int id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            if (!myPlayer.IsAdmin)
            {
                return View("NonAdminErrorView");
            }
            string serializedPlayer = JsonConvert.SerializeObject(new UnsuspendPlayerRequest()
            {
                TargetPlayerId = id,
                UserName = myPlayer.UserName,
                Token = myPlayer.Token
            });
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendPostRequest($"{BaseUri}Player/UnsuspendPlayer", serializedPlayer);
            BaseResponse response = JsonConvert.DeserializeObject<BaseResponse>(serializedResponse);
            if (response.IsSuccess)
            {
                return RedirectToAction("ManagePlayers");
            }
            else
            {
                return View("ErrorView");
            }
        }
        //27th jan
        public async Task<ActionResult> UserUpdateProfile(int id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedPlayer = await httpHelper.SendGetRequest($"{BaseUri}Player/GetPlayer?id={id}");
            GetPlayerResponse response = JsonConvert.DeserializeObject<GetPlayerResponse>(serializedPlayer);
            if (response.IsSuccess)
            {
                return View("UserUpdateProfileView", response.Player);
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> UserUpdateProfileRequest(PlayerViewModel player)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            string serializedPlayer = JsonConvert.SerializeObject(new UpdateProfileRequest()
            {
                Player = new PlayerViewModel()
                {
                    Id = player.Id,
                    Country = player.Country
                },
                UserName = myPlayer.UserName,
                Token = myPlayer.Token
            });
            string serializedResponse = await httpHelper.SendPostRequest($"{BaseUri}Player/UserUpdateProfile", serializedPlayer);
            BaseResponse response = JsonConvert.DeserializeObject<BaseResponse>(serializedResponse);
            if (response.IsSuccess)
            {
                return View("UpdateSuccessView");
            }
            else
            {
                return View("ErrorView");
            }

        }
        public ActionResult UserUpdatePassword(int id)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            return View("UserUpdatePasswordView", new UpdatePasswordRequest()
            {
                Id = id
            });
        }
        public async Task<ActionResult> UserUpdatePasswordRequest(UpdatePasswordRequest request)
        {
            PlayerViewModel myPlayer = GetPlayerFromCookie();
            if (myPlayer == null)
            {
                return View("ErrorView");
            }
            HttpHelper httpHelper = new HttpHelper();
            if (request.NewPassword == request.ConfirmNewPassword)
            {
                string serializedRequest = JsonConvert.SerializeObject(new UpdatePasswordRequest() 
                {
                    Id = myPlayer.Id,
                    UserName = myPlayer.UserName,
                    Token = myPlayer.Token,
                    OldPassword = request.OldPassword,
                    NewPassword = request.NewPassword,
                    ConfirmNewPassword = request.ConfirmNewPassword
                });
                string serializedResponse = await httpHelper.SendPostRequest($"{BaseUri}Player/UserUpdatePassword", serializedRequest);
                BaseResponse deserializedResponse = JsonConvert.DeserializeObject<BaseResponse>(serializedResponse);
                if (deserializedResponse.IsSuccess)
                {
                    return View("UpdateSuccessView");
                }
                else
                {
                    return View("Errorview");
                }
            }
            return RedirectToAction("Index");// need to return errorview that passwords dont match
        }
        private void AddSession(PlayerViewModel player)
        {
            HttpCookie myCookie = new HttpCookie("User");
            myCookie["Player"] = JsonConvert.SerializeObject(player);
            myCookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(myCookie);
        }
        private void DeleteSession()
        {
            HttpCookie myCookie = new HttpCookie("User");
            myCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(myCookie);
        }
        private Dictionary<int, List<GameViewModel>> GetGamesByGroups(List<GameViewModel> games)
        {
            int groupId = 1;
            int counter = 0;
            int amountOfRows = games.Count / 4;
            int gamesLeft = games.Count % 4;
            var totalNormalGamesInRow = games.Count - gamesLeft;
            if (amountOfRows == 0)
            {
                foreach (var item in games)
                {
                    item.GroupId = 1;
                }
            }
            else
            {
                foreach (var item in games)
                {
                    if (counter == totalNormalGamesInRow)
                    {
                        break;
                    }
                    if (counter > 0 && counter % 4 == 0)
                    {
                        groupId++;
                    }
                    item.GroupId = groupId;
                    counter++;
                }
                var itemsWithOutGroupId = games.Where(a => a.GroupId == 0).ToList();

                if (itemsWithOutGroupId.Count > 0)
                {
                    var groupIds = games.Where(a => a.GroupId > 0).Select(a => a.GroupId).Distinct().ToList();
                    var maxGroupId = groupIds.Max();
                    if (itemsWithOutGroupId.Count == 1)
                    {
                        itemsWithOutGroupId[0].GroupId = maxGroupId;
                    }
                    else if (itemsWithOutGroupId.Count == 2)
                    {
                        if (groupIds.Count == 1)
                        {
                            foreach (var item in itemsWithOutGroupId)
                            {
                                item.GroupId = 1;
                            }
                        }
                        else
                        {
                            foreach (var item in itemsWithOutGroupId)
                            {
                                item.GroupId = maxGroupId;
                                maxGroupId--;
                            }
                        }

                    }
                    else
                    {
                        if (groupIds.Count == 1)
                        {
                            foreach (var item in itemsWithOutGroupId)
                            {
                                item.GroupId = 1;
                            }
                        }
                        else if (groupIds.Count == 2)
                        {
                            itemsWithOutGroupId[0].GroupId = maxGroupId;
                            maxGroupId--;
                            itemsWithOutGroupId[1].GroupId = maxGroupId;
                            maxGroupId++;
                            itemsWithOutGroupId[2].GroupId = maxGroupId;
                        }
                        else
                        {
                            foreach (var item in itemsWithOutGroupId)
                            {
                                item.GroupId = maxGroupId;
                                maxGroupId--;
                            }
                        }
                    }
                }
            }
            return games.GroupBy(a => a.GroupId).ToDictionary(g => g.Key, g => g.ToList());
        }
        public PlayerViewModel GetPlayerFromCookie()
        {
            PlayerViewModel myPlayer = null;
            if (Request.Cookies["User"] != null && !string.IsNullOrEmpty(Request.Cookies["User"]["Player"]))
            {
                myPlayer = JsonConvert.DeserializeObject<PlayerViewModel>(Request.Cookies["User"]["Player"]);
                return myPlayer;
            }
            return null;
        }

    }
}
