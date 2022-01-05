using Games.Client.Models;
using Games.Helpers;
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
            else
            {

                return View("ErrorView");
            }
        }
        public ActionResult AddGame()
        {
            return View("AddGameView", new GameViewModel());
        }
        public async Task<ActionResult> SendGameRequest(GameViewModel game)
        {
            string serializedGame = JsonConvert.SerializeObject(game);
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
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendDeleteRequest($"{BaseUri}Game/DeleteGame?id={Id}");
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
            HttpHelper httpHelper = new HttpHelper();
            string serializedResponse = await httpHelper.SendGetRequest($"{BaseUri}Game/GetGame?id={Id}");
            GetGameResponse response = JsonConvert.DeserializeObject<GetGameResponse>(serializedResponse);
            if (response.IsSuccess)
            {
                return View("UpdateGameView", response.Game);
            }
            else
            {
                return View("ErrorView");
            }
        }
        public async Task<ActionResult> UpdateGameRequest(GameViewModel game)
        {
            HttpHelper httpHelper = new HttpHelper();
            string serializedGame = JsonConvert.SerializeObject(game);
            string response = await httpHelper.SendPutRequest($"{BaseUri}Game/UpdateGame", serializedGame);
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
            string serializedPlayer = JsonConvert.SerializeObject(myPlayer);
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/AddPlayer", serializedPlayer);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {

                return RedirectToAction("Login", new {  UserName = myPlayer.UserName, Password = myPlayer.Password  });
            }
            else
            {
                return View("ErrorView");
            }

        }
        public async Task<ActionResult> Login(LoginViewModel player)
        {
            string serializedPlayer = JsonConvert.SerializeObject(player);
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/LogIn", serializedPlayer);
            LoginPlayerResponse loginPlayerResponse = JsonConvert.DeserializeObject<LoginPlayerResponse>(response);
            if (loginPlayerResponse.IsSuccess)
            {
                AddCookie(player.UserName, loginPlayerResponse.Token);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Errorview");
            }
        }

        public async Task<ActionResult> LogOut()
        {
            string serializedPlayer = JsonConvert.SerializeObject(new
            {
                UserName = Request.Cookies["user"]["UserName"]
            });
            HttpHelper httpHelper = new HttpHelper();
            string response = await httpHelper.SendPostRequest($"{BaseUri}Player/LogOut", serializedPlayer);
            BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(response);
            if (baseResponse.IsSuccess)
            {
                DeleteCookie();
                return RedirectToAction("Index");
            }
            else
            {
                return View("ErrorView");
            }

        }
        private void AddCookie(string userName, string token)
        {
            HttpCookie myCookie = new HttpCookie("user");
            myCookie["UserName"] = userName;
            myCookie["Token"] = token;
            myCookie.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Add(myCookie);

        }
        private void DeleteCookie()
        {
            if (Request.Cookies["user"] != null)
            {
                HttpCookie myCookie = new HttpCookie("user");
                myCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(myCookie);
            }
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


    }
}
