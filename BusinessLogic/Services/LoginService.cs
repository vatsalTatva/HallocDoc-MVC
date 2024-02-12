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

        public Aspnetuser Login(LoginModel loginModel)
        {

            var user = _db.Aspnetusers.FirstOrDefault(x => x.Email == loginModel.email && x.Passwordhash == loginModel.password);
            return user;
            //var usr = _db.Aspnetusers.Where(x => x.Email == loginModel.email).FirstOrDefault();

            //if (usr.Email == loginModel.email && usr != null)
            //{
            //    if (usr.Passwordhash == loginModel.password)
            //    {
            //        return usr;
            //    }

            //    return usr;
            //}

            //return usr;




        }
      
    }
}
