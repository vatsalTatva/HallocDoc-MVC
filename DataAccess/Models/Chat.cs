using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public string? Message { get; set; }

    public int? AdminId { get; set; }

    public int? PhyscianId { get; set; }

    public int? RequestId { get; set; }

    public DateTime? SentDate { get; set; }

    public int? SentBy { get; set; }

    public int? ChatType { get; set; }
}
