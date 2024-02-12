
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

        public IActionResult Index()
        {
            return View();
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


        [HttpGet]
        public async Task<IActionResult> CheckEmailExists( string email)
            {
            var emailExists = await _patientService.IsEmailExists(email);
            return Json(new { emailExists });
        }

        [HttpPost]
        public IActionResult AddFamilyRequest(FamilyReqModel familyReqModel)
        {

            _patientService.AddFamilyReq(familyReqModel);

            return RedirectToAction("RequestScreen", "Patient");
        }

        [HttpPost]
        public IActionResult AddConciergeRequest(ConciergeReqModel conciergeReqModel)
        {

            _patientService.AddConciergeReq(conciergeReqModel);

            return RedirectToAction("RequestScreen", "Patient");
        }

        [HttpPost]
        public IActionResult AddBusinessRequest(BusinessReqModel businessReqModel)
        {

            _patientService.AddBusinessReq(businessReqModel);

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
        //public IActionResult CreatePatientReq(PatientInfoModel model)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        return RedirectToAction("RequestScreen");
        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //}

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
