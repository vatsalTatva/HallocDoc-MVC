using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class LoginService : ILoginService
    {

        private readonly ApplicationDbContext _db;

        public LoginService(ApplicationDbContext db)
        {
            _db = db;
        }

        public User Login(LoginModel loginModel)
        {

            var obj = _db.Aspnetusers.ToList();

            User user = new User();
            user = null;

            foreach (var item in obj)
            {
                if (item.Email == loginModel.email && item.Passwordhash == loginModel.password)
                {
                    user = _db.Users.FirstOrDefault(u => u.Aspnetuserid == item.Id);
                    return user;
                }
            }
            return user;



        }
      
    }
}
