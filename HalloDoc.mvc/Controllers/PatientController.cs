
using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HalloDoc.mvc.Controllers
{
   public class PatientController : Controller
    {

        private readonly ILogger<PatientController> _logger;
        private readonly ILoginService _loginService;
        private readonly IPatientService _patientService;
        private readonly INotyfService _notyf;


        public PatientController(ILogger<PatientController> logger , ILoginService loginService,IPatientService patientService , INotyfService notyf)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _loginService.Login(loginModel);
                if (user != null)
                {
                    TempData["username"] = user.Username;
                    TempData["id"] = user.Id;
                    _notyf.Success("Logged In Successfully !!");
                    return RedirectToAction("PatientDashboard", "Patient");
                }
                else
                {
                    _notyf.Error("Invalid Credentials");

                    //ViewBag.AuthFailedMessage = "Please enter valid username and password !!";
                }
                return View();
            }
            else
            {
                return View(loginModel);
            }
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
            _notyf.Success("Submit Successfully !!");
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

        [HttpPost]
        public IActionResult CreatePatientReq( PatientInfoModel patientInfoModel)
        {
            if (ModelState.IsValid)
            {
                // Save patient data to database
                return RedirectToAction("RequestScreen");
            }
            return View(patientInfoModel);
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

        //public IActionResult PatientDashboard()
        //{
        //    var infos = _patientService.GetPatientInfos();
        //    var viewmodel = new PatientDashboardInfo { patientDashboardItems = infos };
        //    return View(viewmodel);
        //}
        public IActionResult PatientDashboard()
        {
            
            var infos = _patientService.GetMedicalHistory("abc@gmail.com");
            var viewmodel = new MedicalHistoryList { medicalHistoriesList = infos };
            return View(viewmodel);
        }
        public IActionResult SubmitMeInfo()
        {
            return View();
        }

        public IActionResult GetDcoumentsById(int requestId) 
        {
            var list = _patientService.GetAllDocById(requestId);
            return PartialView("_DocumentList",list.ToList());
        }
    }
}
