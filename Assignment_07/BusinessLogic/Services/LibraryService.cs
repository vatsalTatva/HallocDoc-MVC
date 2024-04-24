using BusinessLogic.Interfaces;
using DataAccess.CustomModels;
using DataAccess.Data;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BusinessLogic.Services
{

    public class LibraryService : ILibraryService
    {
        private readonly ApplicationDbContext _db;

        public LibraryService(ApplicationDbContext db)
        {
            _db = db;
        }


        public LibraryModel2 GetLibraryData()
        {
            LibraryModel2 model = new();

            var data = from b in _db.Books
                       select new LibraryModel
                       {
                           BookId = b.Id,
                           BookName = b.Bookname,
                           Author = b.Author,
                           BorrowerName = b.Borrowername,
                           DateOfIssue = b.Dateofissue,
                           City = b.City,
                           Genere = b.Genere,
                       };

            var result = data.ToList();
            //int count = result.Count();
            //int TotalPage = (int)Math.Ceiling(count / (double)5);
            //result = result.Skip((CurrentPage - 1) * 5).Take(5).ToList();
            //model.TotalPage = TotalPage;
            //model.CurrentPage = CurrentPage;
            model.LibraryList = result;

            return model;
        }

        public bool AddEditBook(BookFormModel model)
        {

            try
            {
                var record = _db.Books.Where(x=>x.Id ==model.BookId).Select(x=>x).FirstOrDefault();
                var borrowerId = _db.Borrowers.Where(x => x.City.Trim().ToLower() == model.City.Trim().ToLower()).Select(x => x.Id).FirstOrDefault();

                if (record == null)
                {

                    Book book = new Book();
                    book.Borrowername = model.BorrowerName;
                    book.Author = model.Author;
                    book.Bookname = model.BookName;
                    book.Dateofissue = model.DateOfIssue;
                    book.City = model.City;
                    book.Genere = model.Genere;
                    if (borrowerId != 0)
                    {
                        book.Borrowerid = borrowerId;
                    }
                    _db.Books.Add(book);
                    _db.SaveChanges();
                }
                else
                {
                    record.Borrowername = model.BorrowerName;
                    record.Author = model.Author;
                    record.Bookname = model.BookName;
                    record.Dateofissue = model.DateOfIssue;
                    record.City = model.City;
                    record.Genere = model.Genere;
                    if (borrowerId != 0)
                    {
                        record.Borrowerid = borrowerId;
                    }

                    _db.Books.Update(record);
                    _db.SaveChanges();
                }
          

                return true;

            }
            catch { return false; } 
        }

        public BookFormModel GetBookData(int bookId)
        {
            var book = _db.Books.Where(x=>x.Id==bookId).Select(x=>x).FirstOrDefault();
            BookFormModel model = new();

            model.BorrowerName = book.Borrowername;
            model.Author = book.Author;
            model.BookName = book.Bookname;
            model.DateOfIssue = book.Dateofissue;
            model.City = book.City;
            model.Genere = book.Genere;
            model.BookId = book.Id;
            
            return model;
        }

        public bool DeleteBook(int bookId)
        {
            try
            {
                var book = _db.Books.Where(x => x.Id == bookId).Select(x => x).FirstOrDefault();
                _db.Books.Remove(book);
                _db.SaveChanges();
                return true;
            }catch {
            
            return false;   
            }
        }
    }
}
