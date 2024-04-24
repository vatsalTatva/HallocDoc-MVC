using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class LibraryModel
    {
        public int? BookId { get; set; }
        public string? BookName { get; set; }
        public string? Author { get; set; }
        public string? BorrowerName { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? City { get; set; }
        public string? Genere { get; set; }
    }

    public class LibraryModel2
    {
        public int? TotalPage { get; set; }

        public int? CurrentPage { get; set; }
        public string? Search { get; set; }
        public List<LibraryModel>? LibraryList { get; set; }
    }

    public class BookFormModel
    {
        public int? BookId { get; set; }
        [Required(ErrorMessage ="Book Name is required")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Book Name should not contain null entry")]
        public string? BookName { get; set; }
        [Required(ErrorMessage = "Author is required")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Author should not contain null entry")]
        public string? Author { get; set; }
        [Required(ErrorMessage = "Borrower Name is required")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Borrower should not contain null entry")]
        public string? BorrowerName { get; set; }
        [Required(ErrorMessage =("Date Of Issue is required"))]
        public DateTime? DateOfIssue { get; set; }
        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "City should contain only letters")]
        public string? City { get; set; }
        [Required(ErrorMessage ="Genere is required")]
        public string? Genere { get; set; }
    }
}