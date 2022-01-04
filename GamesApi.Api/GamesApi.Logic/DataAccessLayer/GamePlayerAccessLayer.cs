using Dapper;
using GamesApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesApi.Logic.DataAccessLayer
{
    public class GamePlayerAccessLayer : BasicAccessLayer
    {
        public void AddGame(Game game)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Name", game.Name);
                p.Add("@NumberOfPlayers", game.NumberOfPlayers);
                p.Add("Image", game.Image);
                p.Add("URL", game.URL);
                db.Execute("USP_AddGame", p, commandType: CommandType.StoredProcedure);
            });
        }
        public List<Game> GetGames()
        {
            return WithConnection((db) =>
            {
                var p = new DynamicParameters();
                return db.Query("USP_GetGames", p, commandType: CommandType.StoredProcedure).Select(a => new Game()
                {
                    Id = a.Id,
                    Name = a.Name,
                    NumberOfPlayers = a.NumberOfPlayers,
                    Image = a.Image,
                    URL = a.URL
                }).ToList();
            });
        }
        public Game GetGame(int id)
        {
            return WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                return db.Query("USP_GetGame", p, commandType: CommandType.StoredProcedure).Select(a => new Game()
                {
                    Id = a.Id,
                    Name = a.Name,
                    NumberOfPlayers = a.NumberOfPlayers,
                    Image = a.Image,
                    URL = a.URL
                }).FirstOrDefault();
            });
        }
        public void DeleteGame(int id)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                db.Execute("USP_DeleteGame", p, commandType: CommandType.StoredProcedure);
            });
        }
        public bool UpdateGame(Game game)
        {
            int value = WithConnection((db) =>
             {
                 var p = new DynamicParameters();
                 p.Add("@Id", game.Id);
                 p.Add("@Name", game.Name);
                 p.Add("@NumberOfPlayers", game.NumberOfPlayers);
                 p.Add("@Image", game.Image??"");
                 p.Add("@URL", game.URL);
                 return db.QueryFirst<int>("USP_UpdateGame", p, commandType: CommandType.StoredProcedure);
             });
            if (value == 1)
            {
                return false;
            }
            if (value == 2)
            {
                return true;
            }
            return false;
        }
        public void AddPlayer(Player player)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@UserName", player.UserName);
                p.Add("@Password", player.Password);
                p.Add("@Country", player.Country);
                p.Add("@Birthday", player.Birthday);
                p.Add("@IsAdmin", player.IsAdmin);
                db.Execute("USP_AddPlayer", p, commandType: CommandType.StoredProcedure);
            });
        }
        public List<Player> GetPlayers()
        {
            return WithConnection((db) =>
            {
                var p = new DynamicParameters();
                return db.Query("USP_GetPlayers", p, commandType: CommandType.StoredProcedure).Select(a => new Player()
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Password = a.Password,
                    Country = a.Country,
                    Birthday = a.Birthday,
                    IsLoggedIn = a.IsLoggedIn,
                    IsAdmin = a.IsAdmin??false
                }).ToList();
            });
        }
        public Player GetPlayer(int id)
        {
            return WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                return db.Query("USP_GetPlayer", p, commandType: CommandType.StoredProcedure).Select(a => new Player()
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Country = a.Country,
                    Birthday = a.Birthday,
                    IsLoggedIn = a.IsLoggedIn,
                    IsAdmin = a.IsAdmin??false
                }).FirstOrDefault();
            });
        }
        public void DeletePlayer(int id)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                db.Execute("USP_DeletePlayer", p, commandType: CommandType.StoredProcedure);
            });
        }
        public void UpdatePlayer(Player player)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@Id", player.Id);
                p.Add("@UserName", player.UserName);
                p.Add("@Password", player.Password);
                p.Add("@Country", player.Country);
                p.Add("@Birthday", player.Birthday);
                p.Add("@IsAdmin", player.IsAdmin);
                db.Execute("USP_UpdatePlayer", p, commandType: CommandType.StoredProcedure);
            });
        }
        public void UpdatePlayerStatus(string userName, bool status, string token)
        {
            WithConnection((db) =>
            {
                var p = new DynamicParameters();
                p.Add("@IsLoggedIn", status);
                p.Add("@UserName", userName);
                p.Add("@Token", token);
                db.Execute("USP_UpdatePlayerStatus", p, commandType: CommandType.StoredProcedure);
            });
        }

    }
}
