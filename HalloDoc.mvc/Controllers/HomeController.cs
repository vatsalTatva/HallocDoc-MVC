using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using HalloDoc.mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HalloDoc.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;

        public HomeController(ILogger<HomeController> logger, IAdminService adminService)
        {
            _logger = logger;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

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

        public IActionResult ReviewAgreement(int reqId)
        {
            AgreementModel am = new AgreementModel();
            am.reqId = reqId;
            return View(am);
        }
        public IActionResult AgreeAgreement(AgreementModel agreementModal)
        {
            bool isSaved = _adminService.IAgreeAgreement(agreementModal);
            return RedirectToAction("AdminDashboard", "Admin");

        }

        public IActionResult CancelAgreement(int reqId)
        {
            var model = _adminService.ICancelAgreement(reqId);
            return PartialView("_cancelagreement", model);
        }

        [HttpPost]
        public IActionResult CancelAgreementSubmit(int ReqClientid, string Description)
        {
            AgreementModel model = new()
            {
                reqClientId = ReqClientid,
                reason = Description,
            };
            var obj = _adminService.SubmitCancelAgreement(model);
            return RedirectToAction("admin_dashboard", "Admin", obj);
        }


    }
}