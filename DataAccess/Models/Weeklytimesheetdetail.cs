using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Weeklytimesheetdetail
{
    public int Timesheetdetailid { get; set; }

    public DateOnly Date { get; set; }

    public int? Numberofshifts { get; set; }

    public int? Nightshiftweekend { get; set; }

    public int? Housecall { get; set; }

    public int? Housecallnightweekend { get; set; }

    public int? Phoneconsult { get; set; }

    public int? Phoneconsultnightweekend { get; set; }

    public int? Batchtesting { get; set; }

    public string? Item { get; set; }

    public int? Amount { get; set; }

    public string? Bill { get; set; }

    public int? Totalamount { get; set; }

    public int? Bonusamount { get; set; }

    public int? Timesheetid { get; set; }

    public int? Oncallhours { get; set; }

    public int? Totalhours { get; set; }

    public bool? Isweekendholiday { get; set; }

    public virtual Weeklytimesheet? Timesheet { get; set; }
}
