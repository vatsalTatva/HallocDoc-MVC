﻿using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly INotyfService _notyf;
        private readonly IAdminService _adminService;


        public AdminController(ILogger<AdminController> logger ,INotyfService notyfService , IAdminService adminService)
        {
            _logger = logger;
            _notyf = notyfService;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if(ModelState.IsValid)
            {
                
                return RedirectToAction("AdminDashboard","Admin");
            }
            return View(adminLoginModel);
            
        }
        public IActionResult AdminDashboard()
        {
            var list = _adminService.GetRequestsByStatus();
            return View(list);
        }

        public  IActionResult ViewCase()
        {
            return View();
        }
    }
}