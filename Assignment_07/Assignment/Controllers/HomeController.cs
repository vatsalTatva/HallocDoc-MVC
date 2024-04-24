using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Assignment.Models;
using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly ILibraryService _libraryService;
        private readonly INotyfService _notyf;


        public HomeController(ILogger<HomeController> logger, ILibraryService libraryService, INotyfService notyfService)
        {
            _logger = logger;
            _libraryService = libraryService;
            _notyf = notyfService;

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

        [HttpGet]
        public IActionResult Library()
        {
            var model = _libraryService.GetLibraryData();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            return PartialView("_BookForm");
        }

        [HttpPost]
        public IActionResult AddBook(BookFormModel model)
        {
            bool isAdded = _libraryService.AddEditBook(model);  
            if(isAdded)
            {

                _notyf.Success("Added successfully");
            }
            else
            {
                _notyf.Error("Something went wrong");
            }
            return RedirectToAction("Library");
        }

        [HttpGet]
        public IActionResult EditBook(int bookId)
        {
            var model = _libraryService.GetBookData(bookId);
            return PartialView("_EditBookForm",model);
        }
         [HttpPost]
        public IActionResult EditBook(BookFormModel model)
        {
            bool isEdited = _libraryService.AddEditBook(model);
            if (isEdited)
            {

                _notyf.Success("Edited successfully");
            }
            else
            {
                _notyf.Error("Something went wrong");
            }
            return RedirectToAction("Library");
        }


        public IActionResult DeleteBook(int bookId)
        {
            var isDeleted = _libraryService.DeleteBook(bookId);
            return Json(new { isDeleted });
        }





    }
}