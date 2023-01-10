using PTRC.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PTRC.API.Service.UsersIdentity
{
    public interface IUserService
    {
        User Validate(string email, string password);
        List<User> GetUserList();
        User GetUserById(int id);
        List<User> SearchByName(string name);
    }

    // UserService concrete class
    public class UserService : IUserService
    {

        private List<User> userList = new List<User>();
        public UserService()
        {
            userList.Add(new User
            {
                Id = 1,
                Name =ConfigurationManager.AppSettings["UserName"].ToString(),
                Password = ConfigurationManager.AppSettings["Password"].ToString(),                
                Roles = new string[] { 1 % 2 == 0 ? "Admin" : "User" }
            });
        }

        public User Validate(string email, string password)
            => userList.FirstOrDefault(x => x.Name == email && x.Password == password);

        public List<User> GetUserList() => userList;

        public User GetUserById(int id)
            => userList.FirstOrDefault(x => x.Id == id);

        public List<User> SearchByName(string name)
            => userList.Where(x => x.Name.Contains(name)).ToList();
    }
}