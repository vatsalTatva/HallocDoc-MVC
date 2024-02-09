
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.mvc.Controllers
{
   public class PatientController : Controller
    {

        private readonly ILogger<PatientController> _logger;
        private readonly ILoginService _loginService;
        private readonly IPatientService _patientService;

        public PatientController(ILogger<PatientController> logger , ILoginService loginService,IPatientService patientService)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
        }




        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            var user = _loginService.Login(loginModel);
            if (user!=null)
            {
                
                return RedirectToAction("CreatePatientReq", "Patient");
            }
            else
            {
                ViewBag.AuthFailedMessage = "Please enter valid username and password !!";
            }
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel loginmodel)
        //{

        //    var result = await _loginService.Login(loginmodel);
        //    if (result)
        //    {
        //        return Ok("login successful");
        //    }
        //    return BadRequest("invalid credentials");
        //}

        [HttpPost]
        public IActionResult AddPatient(PatientInfoModel patientInfoModel)
        {

            _patientService.AddPatientInfo(patientInfoModel);

            return RedirectToAction("RequestScreen", "Patient");
        }

        public IActionResult RequestScreen()
        {
            return View();
        }

        public IActionResult CreatePatientReq()
        {
            return View();
        }

        public IActionResult CreateFamilyFrndReq()
        {
            return View();
        }

        public IActionResult CreateConciergeReq()
        {
            return View();
        }

        public IActionResult CreateBusinessPartnerReq()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }



    }
}
