
using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using BusinessLogic.Services;

namespace HalloDoc.mvc.Controllers
{
   
   public class PatientController : Controller
    {

        private readonly ILogger<PatientController> _logger;
        private readonly ILoginService _loginService;
        private readonly IPatientService _patientService;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _db;
       


        public PatientController(ILogger<PatientController> logger , ILoginService loginService,IPatientService patientService , INotyfService notyf,ApplicationDbContext db)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _notyf = notyf;
            _db = db;
          
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

                //var userId = user.Userid;
                HttpContext.Session.SetInt32("UserId", user.Userid);

                //the above data is coming from user table and storing in user object
                if (user != null)
                {
                    TempData["username"] = user.Firstname;
                    TempData["id"] = user.Lastname;
                    _notyf.Success("Logged In Successfully !!");
                    return RedirectToAction("PatientDashboard","Patient");
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
                if(patientInfoModel.password != null)
                {
                    patientInfoModel.password = GenerateSHA256(patientInfoModel.password);
                }
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
            if (ModelState.IsValid)
            {
                _patientService.AddFamilyReq(familyReqModel);
                _notyf.Success("Submit Successfully !!");
                return RedirectToAction("RequestScreen", "Patient");
            }
            else
            {
                return View(familyReqModel);
            }
        }

        [HttpPost]
        public IActionResult AddConciergeRequest(ConciergeReqModel conciergeReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddConciergeReq(conciergeReqModel);
                _notyf.Success("Submit Successfully !!");
                return RedirectToAction("RequestScreen", "Patient");
            }
            else
            {
                return View(conciergeReqModel);
            }
        }

        [HttpPost]
        public IActionResult AddBusinessRequest(BusinessReqModel businessReqModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddBusinessReq(businessReqModel);
                _notyf.Success("Submit Successfully !!");
                return RedirectToAction("RequestScreen", "Patient");
            }
            else
            {
                return View(businessReqModel) ;
            }
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

        [HttpPost]
        public IActionResult CreateAccount(CreateAccountModel createAccountModel)
        {
            if (ModelState.IsValid)
            {              
                _notyf.Success("Registered Successfully !!");
                return RedirectToAction("Login", "Patient");
            }
            else
            {
                return View(createAccountModel);
            }
        }
















        public IActionResult PatientResetPasswordEmail(Aspnetuser user)

        {

            var usr = _db.Aspnetusers.FirstOrDefault(x => x.Email == user.Email);
            if (usr != null)
            {
                string Id = _db.Aspnetusers.FirstOrDefault(x => x.Email == user.Email).Id;
                string resetPasswordUrl = GenerateResetPasswordUrl(Id);
                SendEmail(user.Email, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");
            }
            else
            {
                _notyf.Error("Email Id Not Registered");
                return RedirectToAction("ForgotPassword", "Patient");
            }


            return RedirectToAction("Login");
        }

        private string GenerateResetPasswordUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("ResetPassword", new { id = userId });
            return baseUrl + resetPasswordPath;
        }

        private Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.vatsalgadoya@outlook.com";
            var password = "VatsalTatva@2024";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        // Handle the reset password URL in the same controller or in a separate one
        public IActionResult ResetPassword(string id)
        {
            var aspuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == id);
            return View(aspuser);
        }

        [HttpPost]
        public IActionResult ResetPassword(Aspnetuser aspnetuser)
        {
            var aspuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuser.Id);
            aspuser.Passwordhash = aspnetuser.Passwordhash;
            _db.Aspnetusers.Update(aspuser);
            _db.SaveChanges();
            return RedirectToAction("Login");
        }








        public IActionResult PatientDashboard()
        {
            int? userid = HttpContext.Session.GetInt32("UserId");

            var infos = _patientService.GetMedicalHistory((int)userid);

            return View(infos);
        }

        public IActionResult DocumentList(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var y = _patientService.GetAllDocById(reqId);
            return View(y);
        }

        [HttpPost]
        public IActionResult UploadDocuments(DocumentModel model)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            if (model.uploadedFiles == null)
            {
                _notyf.Error("First Upload Files");
                return RedirectToAction("DocumentList", "Patient", new { reqId = rid });
            }
            bool isUploaded = _patientService.UploadDocuments(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("DocumentList", "Patient", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("DocumentList", "Patient", new { reqId = rid });
            }
        }



        public IActionResult ShowProfile(int userid)
        {
            HttpContext.Session.SetInt32("EditUserId", userid);
            var profile = _patientService.GetProfile(userid);
            return PartialView("_Profile", profile);
        }

        public IActionResult SaveEditProfile(Profile profile)
        {
            int EditUserId = (int)HttpContext.Session.GetInt32("EditUserId");
            profile.userId = EditUserId;
            bool isEdited = _patientService.EditProfile(profile);
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
