using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiteDB;

namespace Dashboard.Models.MyDatabase
{
    public class MyUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }
    }
    public class MyDatabase
    {
        LiteDatabase _db = new LiteDatabase(@"MyDatabase.db");
        LiteCollection<MyUser> _users;

        public MyDatabase()
        {
            _users = _db.GetCollection<MyUser>("myusers");
        }

        public void AddUser(string name, string email, string password)
        {
            var user = new MyUser
            {
                UserName = name,
                Email = email,
                IsAdmin = false,
                Password = password
            };
            _users.Insert(user);
        }

        public void UpdateUserInfo(MyUser user, KeyValuePair<string, string> toModify)
        {
            if (toModify.Key == "UserName")
                user.UserName = toModify.Value;
            else if (toModify.Key == "Email")
                user.Email = toModify.Value;
            else if (toModify.Key == "Password")
                user.Password = toModify.Value;
            else
            {
                Console.WriteLine($"Wrong pair (Key = {toModify.Key})");
                return;
            }
            _users.Update(user);
        }

        public MyUser GetUserByName(string userName)
        {
            var results = _users.Find(x => x.UserName == userName);

            /**
             * Methode du bled pour trouver le user
             * (a ne pas faire)
             */
            var user = results.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine($"User {userName} does not exist");
            }

            return user;
        }
    }
}