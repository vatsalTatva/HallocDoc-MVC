using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Book
{
    public int Id { get; set; }

    public string? Bookname { get; set; }

    public string? Author { get; set; }

    public int? Borrowerid { get; set; }

    public string? Borrowername { get; set; }

    public DateTime? Dateofissue { get; set; }

    public string? City { get; set; }

    public string? Genere { get; set; }

    public virtual Borrower? Borrower { get; set; }
}
