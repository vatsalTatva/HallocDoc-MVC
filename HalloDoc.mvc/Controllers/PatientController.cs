
using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Security.Cryptography;
using System.Text;

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

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                string passwordhash = GenerateSHA256(loginModel.password);
                loginModel.password = passwordhash;
                var user = _loginService.Login(loginModel);
         
                //the above data is coming from user table and storing in user object
                if (user != null)
                {
                    TempData["username"] = user.Firstname;
                    TempData["id"] = user.Lastname;
                    _notyf.Success("Logged In Successfully !!");
                    return RedirectToAction("PatientDashboard",user);
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


        [HttpPost]
        public IActionResult AddPatient(PatientInfoModel patientInfoModel)
        {

            if (ModelState.IsValid)
            {
                _patientService.AddPatientInfo(patientInfoModel);
                _notyf.Success("Submit Successfully !!");
                return RedirectToAction("RequestScreen", "Patient");
            }
            else
            {
                return View(patientInfoModel);
            }
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


        //public IActionResult PatientDashboard()
        //{
        //    me
        //    return View();
        //}
     
        public IActionResult PatientDashboard(User user)
        {
            
            var infos = _patientService.GetMedicalHistory(user);
            var viewmodel = new MedicalHistoryList { medicalHistoriesList = infos };
            return View(viewmodel);
        }
        
        public IActionResult GetDcoumentsById(int requestId) 
        {
            var list = _patientService.GetAllDocById(requestId);
            return PartialView("_DocumentList",list.ToList());
        }

        public IActionResult Edit(MedicalHistoryList medicalHistoryList)
        {
            MedicalHistory medicalHistory = medicalHistoryList.medicalHistoriesList[0];
            bool isEdited = _patientService.EditProfile(medicalHistory);
            if (isEdited)
            {
                _notyf.Success("Profile Edited Successfully");
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                _notyf.Error("Profile Edited Failed");
                return RedirectToAction("PatientDashboard");
            }
            
        }
        public IActionResult SubmitMeInfo()
        {
            return View();
        }

    }
}
