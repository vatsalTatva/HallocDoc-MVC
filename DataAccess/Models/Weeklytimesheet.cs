using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Weeklytimesheet
{
    public int Timesheetid { get; set; }

    public DateOnly Startdate { get; set; }

    public DateOnly Enddate { get; set; }

    public int? Status { get; set; }

    public DateTime? Createddate { get; set; }

    public int Physicianid { get; set; }

    public int? Payrateid { get; set; }

    public int? Adminid { get; set; }

    public bool? Isfinalized { get; set; }

    public string? Adminnote { get; set; }

    public int? Bonusamount { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Payrate? Payrate { get; set; }

    public virtual Physician Physician { get; set; } = null!;

    public virtual ICollection<Weeklytimesheetdetail> Weeklytimesheetdetails { get; set; } = new List<Weeklytimesheetdetail>();
}
