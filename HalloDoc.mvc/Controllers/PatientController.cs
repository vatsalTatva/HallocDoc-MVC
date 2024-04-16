
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
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDoc.mvc.Controllers
{
   
   public class PatientController : Controller
    {

        private readonly ILogger<PatientController> _logger;
        private readonly ILoginService _loginService;
        private readonly IPatientService _patientService;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _db;
        private readonly IJwtService _jwtService;
       


        public PatientController(ILogger<PatientController> logger , ILoginService loginService,IPatientService patientService , INotyfService notyf,ApplicationDbContext db,IJwtService jwtService
            )
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _notyf = notyf;
            _db = db;
            _jwtService = jwtService;
          
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

        //[HttpPost]
        //public IActionResult Login(LoginModel loginModel)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        string passwordhash = GenerateSHA256(loginModel.password);
        //        loginModel.password = passwordhash;
        //        var user = _loginService.Login(loginModel);

        //        //var userId = user.Userid;
        //        HttpContext.Session.SetInt32("UserId", user.Userid);

        //        //the above data is coming from user table and storing in user object
        //        if (user != null)
        //        {
        //            //TempData["username"] = user.Firstname;
        //            //TempData["id"] = user.Lastname;
                  
        //            _notyf.Success("Logged In Successfully !!");
        //            return RedirectToAction("PatientDashboard","Patient");
        //        }
        //        else
        //        {
        //            _notyf.Error("Invalid Credentials");

        //            //ViewBag.AuthFailedMessage = "Please enter valid username and password !!";
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        return View(loginModel);
        //    }
        //}


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _patientService.GetAspnetuser(model.email);
                if (aspnetuser != null)
                {
                    model.password = GenerateSHA256(model.password);
                    if (aspnetuser.Passwordhash == model.password)
                    {
                        var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                        Response.Cookies.Append("PatientJwt", jwtToken);
                       
                            _notyf.Success("Logged in Successfully");
                            return RedirectToAction("PatientDashboard", "Patient");
                      
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
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("PatientJwt");
            return RedirectToAction("Login", "Patient");
        }

        public string GetTokenEmail()
        {
            var token = HttpContext.Request.Cookies["PatientJwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            return emailClaim.Value;
        }
        public string GetLoginId()
        {
            var token = HttpContext.Request.Cookies["PatientJwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return "";
            }
            var loginId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "aspNetUserId");
            return loginId.Value;
        }
        [HttpGet]
        public IActionResult CreatePatientReq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePatientReq(PatientInfoModel patientInfoModel)
        {

            
            if(patientInfoModel.password != null)
            {
                patientInfoModel.password = GenerateSHA256(patientInfoModel.password);
            }
            bool isValid = _patientService.AddPatientInfo(patientInfoModel);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(patientInfoModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("RequestScreen", "Patient");
            
        }


        [HttpGet]
        public IActionResult CheckEmailExists( string email)
            {
            var emailExists =  _patientService.IsEmailExists(email);
            return Json(new { emailExists });
        }
        [HttpGet]
        public IActionResult CheckPasswordExists( string email)
            {
            var passwordExists =  _patientService.IsPasswordExists(email);
            return Json(new { passwordExists });
        }
        [HttpGet]
        public IActionResult CreateFamilyFrndReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateFamilyFrndReq(FamilyReqModel familyReqModel)
        {
            //if (ModelState.IsValid)
            //{
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("CreateAccount", "Patient");
            bool isValid = _patientService.AddFamilyReq(familyReqModel,createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(familyReqModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("RequestScreen", "Patient");
            //}
            //else
            //{
            //    return View(familyReqModel);
            //}
        }

        [HttpGet]
        public IActionResult CreateConciergeReq()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateConciergeReq(ConciergeReqModel conciergeReqModel)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("CreateAccount", "Patient");
            bool isValid = _patientService.AddConciergeReq(conciergeReqModel, createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(conciergeReqModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("RequestScreen", "Patient");
        }

        [HttpGet]
        public IActionResult CreateBusinessPartnerReq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBusinessPartnerReq(BusinessReqModel businessReqModel)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string createAccountLink = baseUrl + Url.Action("CreateAccount", "Patient");
            bool isValid = _patientService.AddBusinessReq(businessReqModel, createAccountLink);
            if (!isValid)
            {
                _notyf.Error("Service is not available in entered Region");
                return View(businessReqModel);
            }
            _notyf.Success("Submit Successfully !!");
            return RedirectToAction("RequestScreen", "Patient");
        }

        
        public IActionResult RequestScreen()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(CreateAccountModel createAccountModel)
        {
            if (ModelState.IsValid)
            {
                bool isCreated = _patientService.CreateAccount(createAccountModel);
                if (isCreated)
                {
                    _notyf.Success("Account Created Successfully !!");
                    return RedirectToAction("Login", "Patient");
                }
                else
                {
                    _notyf.Error("Something went wrong !!");
                    return RedirectToAction("CreateAccount");
                }
                
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
            var email = GetTokenEmail();
            if(email == "")
            {
                return RedirectToAction("Login");
            }
            var infos = _patientService.GetMedicalHistory(email);

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
            var email = GetTokenEmail();
            PatientInfoModel Reqobj = _patientService.FetchData(email);

            return View(Reqobj);
        }
        public IActionResult SubmitElseInfo()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult SubmitElseInfo(FamilyReqModel familyFriendRequestForm)
        //{

        //    //try
        //    //{
        //    //    _patientService.ReqforSomeoneElse(familyFriendRequestForm, userid);
        //    //    return RedirectToAction("patientdashboard");
        //    //}
        //    //catch { return View(); }

        //}
    }
}
