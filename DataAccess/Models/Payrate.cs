using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Payrate
{
    public int Payrateid { get; set; }

    public int Physicianid { get; set; }

    public int? Nightshiftweekend { get; set; }

    public int? Shift { get; set; }

    public int? Housecallnightweekend { get; set; }

    public int? Phoneconsult { get; set; }

    public int? Phoneconsultnightweekend { get; set; }

    public int? Batchtesting { get; set; }

    public int? Housecall { get; set; }

    public DateTime Createddate { get; set; }

    public virtual Physician Physician { get; set; } = null!;

    public virtual ICollection<Weeklytimesheet> Weeklytimesheets { get; set; } = new List<Weeklytimesheet>();
}
