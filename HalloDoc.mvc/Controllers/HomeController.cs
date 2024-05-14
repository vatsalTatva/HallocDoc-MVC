using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.CustomModels;
using DataAccess.Enums;
using HalloDoc.mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace HalloDoc.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;
        private readonly INotyfService _notyf;
        private readonly IJwtService _jwtService;

        public HomeController(ILogger<HomeController> logger, IAdminService adminService,INotyfService notyfService, IJwtService jwtService)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyfService;
            _jwtService = jwtService;
        }

        #region Login
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    adminLoginModel.password = GenerateSHA256(adminLoginModel.password);
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                       
                        int role = aspnetuser.Aspnetuserroles.Where(x => x.Userid == aspnetuser.Id).Select(x => x.Roleid).First();
                        if (role == 1)
                        {
                            var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                            Response.Cookies.Append("jwt", jwtToken);
                            _notyf.Success("Logged in Successfully");
                            return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else
                        {
                            var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                            Response.Cookies.Append("jwt", jwtToken);
                            _notyf.Success("Logged in Successfully");
                            return RedirectToAction("ProviderDashboard", "Provider");

                        }
                    }
                    else
                    {
                        _notyf.Error("Password is incorrect");

                        return View();
                    }
                }
                _notyf.Error("Email is incorrect");
                return View();
            }
            else
            {
                return View(adminLoginModel);
            }
        }

        [HttpGet]
        public IActionResult ReviewAgreement(int reqId)
        {
            var status = _adminService.GetStatusForReviewAgreement(reqId);
            if (status == (int)StatusEnum.MDEnRoute)
            {
                TempData["ReviewStatus"]= "Review Agreement is Already Accepted !!";
                return RedirectToAction("AgreementStatus");
            }
            else if(status == (int)StatusEnum.CancelledByPatient)
            {
                TempData["ReviewStatus"] = "Review Agreement is Already Cancelled by patient !!";
                return RedirectToAction("AgreementStatus");
            }
            else
            {
                AgreementModel model = new()
                {
                    reqId = reqId,
                };

                return View(model);
            }
        }

        public IActionResult AgreementStatus()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReviewAgreement(AgreementModel agreementModal)
        {
            bool isSaved = _adminService.AgreeAgreement(agreementModal);
            if (isSaved)
            {
                _notyf.Success("Agreement Accepted");
                return RedirectToAction("Login", "Patient");
            }
            _notyf.Error("Something went wrong");
            return RedirectToAction("ReviewAgreement", new {reqId= agreementModal.reqId});

        }

        [HttpGet]
        public IActionResult CancelAgreement(int reqId)
        {
            var model = _adminService.CancelAgreement(reqId);
            return PartialView("_cancelagreement", model);
        }

        [HttpPost]
        public IActionResult CancelAgreement(AgreementModel model)
        {
            bool isCancelled = _adminService.SubmitCancelAgreement(model);
            if (isCancelled)
            {
                _notyf.Success("Agreement Cancelled");
                return RedirectToAction("Login", "Patient");
            }
            _notyf.Error("Something went wrong");
            return RedirectToAction("ReviewAgreement", new { reqId = model.reqId });
        }


        public IActionResult Chat(int RequestId, int AdminID, int ProviderId)
        {
            int? roleMain = HttpContext.Session.GetInt32("roleId");
            ChatViewModel model = _adminService.GetChats(RequestId, AdminID, ProviderId, 1);
            return PartialView("_Chat", model);
        }

    }
}