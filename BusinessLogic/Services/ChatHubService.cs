using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ChatHubService : Hub
    {

        private readonly ApplicationDbContext _db;
        private readonly IGenericService<Chat> _chatRepo;

        public ChatHubService(ApplicationDbContext db, IGenericService<Chat> chatRepo)
        {
            _db = db;
            _chatRepo = chatRepo;
        }

        public async Task SendMessage(string message, string RequestID, string ProviderID, string AdminID, string RoleID)
        {
            //if (RoleID != "1" && AdminID != "0")
            //{
            //    var adminData = _context.Admins.ToList();

            //    foreach (var item in adminData)
            //    {
            //        Chat chat = new Chat();
            //        chat.Message = message;
            //        chat.SentBy = Convert.ToInt32(RoleID);
            //        chat.AdminId = item.Adminid;
            //        chat.RequestId = Convert.ToInt32(RequestID);
            //        chat.PhyscainId = Convert.ToInt32(ProviderID);
            //        chat.SentDate = DateTime.Now;
            //        _chatRepo.Add(chat);
            //    }
            //}
            //else
            //{
            Chat chat = new Chat();
            chat.Message = message;
            chat.SentBy = Convert.ToInt32(RoleID);
            chat.AdminId = Convert.ToInt32(AdminID);
            chat.RequestId = Convert.ToInt32(RequestID);
            chat.PhyscianId = Convert.ToInt32(ProviderID);
            chat.SentDate = DateTime.Now;
            _chatRepo.Add(chat);
            //}

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
