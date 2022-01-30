using Games.SharedModels.Requests.PlayerRequests;
using GamesApi.Logic.DataAccessLayer;
using GamesApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesApi.Logic
{
    public class GamePlayerLogic
    {
        private GamePlayerAccessLayer _gamePlayerAccessLayer;
        public GamePlayerLogic()
        {
            _gamePlayerAccessLayer = new GamePlayerAccessLayer();
        }
        public void AddGame(Game game)
        {
            _gamePlayerAccessLayer.AddGame(game);
        }
        public List<Game> GetGames()
        {
            return _gamePlayerAccessLayer.GetGames();
        }
        public Game GetGame(int id)
        {
            return _gamePlayerAccessLayer.GetGame(id);
        }
        public void DeleteGame(int id)
        {
            _gamePlayerAccessLayer.DeleteGame(id);
        }
        public bool UpdateGame(Game game)
        {
            return _gamePlayerAccessLayer.UpdateGame(game);
        }
        public void AddPlayer(Player player)
        {
            player.Password = Hash.CreateMD5(player.Password);
            _gamePlayerAccessLayer.AddPlayer(player);
        }
        public List<Player> GetPlayers()
        {
            return _gamePlayerAccessLayer.GetPlayers();
        }
        public Player GetPlayer(int id)
        {
            return _gamePlayerAccessLayer.GetPlayer(id);
        }
        public void DeletePlayer(int id)
        {
            _gamePlayerAccessLayer.DeletePlayer(id);
        }
        public void UpdatePlayer(Player player)
        {
            player.Password = Hash.CreateMD5(player.Password);
            _gamePlayerAccessLayer.UpdatePlayer(player);
        }
        public bool ValidateUsernameExists(string userName)
        {
            List<string> listOfPlayers = _gamePlayerAccessLayer.GetPlayers().Select(a => a.UserName).ToList();
            foreach (string name in listOfPlayers)
            {
                if (name.ToLower() == userName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValidateGameExists(string gameName)
        {
            List<Game> listOfGames = _gamePlayerAccessLayer.GetGames();
            foreach (Game game in listOfGames)
            {
                if (game.Name == gameName)
                {
                    return true;
                }
            }
            return false;
        }
        public void UpdatePlayerStatus(string userName, bool status, string token)
        {
            _gamePlayerAccessLayer.UpdatePlayerStatus(userName, status, token);
        }

        public bool IsLoggedIn(LoginRequest request)
        {
            List<Player> listOfPlayers = GetPlayers();
            bool isLoggedIn = listOfPlayers.Any(dbPlayer => dbPlayer.UserName == request.UserName && dbPlayer.IsLoggedIn);
            return isLoggedIn;
        }
        public bool IsPasswordCorrect(LoginRequest request)
        {
            List<Player> listOfPlayers = GetPlayers();
            foreach (Player dbPlayer in listOfPlayers)
            {
                if (dbPlayer.UserName == request.UserName && dbPlayer.Password == Hash.CreateMD5(request.Password))
                {
                    return true;
                }

            }
            return false;
        }
        public bool ValidateTokenMatches(string userName, string token)
        {
            List<Player> listOfPlayers = _gamePlayerAccessLayer.GetPlayers();
            foreach (Player player in listOfPlayers)
            {
                if (player.UserName == userName && player.Token == token)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValidateTokenExpiration(string userName, string token)
        {
            List<Player> listOfPlayers = _gamePlayerAccessLayer.GetPlayers();
            int expirationTimeInMinutes = int.Parse(ConfigurationManager.AppSettings["SessionExpireInMinutes"]);
            foreach (Player player in listOfPlayers)
            {
                if (player.UserName == userName && player.Token == token && player.LastSessionTime < DateTime.UtcNow.AddMinutes(expirationTimeInMinutes))
                {
                    return false;
                }
            }
            return true;
        }
        public bool ValidatePlayerIsAdmin(string userName)
        {
            Player player = GetPlayers().FirstOrDefault(a => a.UserName == userName);
            if (player == null)
            {
                return false;
            }
            return player.IsAdmin;
        }
        public void SuspendPlayer(int id)
        {
            _gamePlayerAccessLayer.SuspendPlayer(id);
        }
        public void UnsuspendPlayer(int id)
        {
            _gamePlayerAccessLayer.UnsuspendPlayer(id);
        }
        public void UserUpdatePassword(string password, int id)
        {
            _gamePlayerAccessLayer.UserUpdatePassword(password, id);
        }
        public void UserUpdateProfile(Player player)
        {
            _gamePlayerAccessLayer.UserUpdateProfile(player);
        }
    }
}
