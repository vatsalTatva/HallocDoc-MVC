using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ILibraryService
    {
        LibraryModel2 GetLibraryData();
        bool AddEditBook(BookFormModel model);

        BookFormModel GetBookData(int bookId);

        bool DeleteBook(int bookId);
    }
}
