using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Borrower
{
    public int Id { get; set; }

    public string? City { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
