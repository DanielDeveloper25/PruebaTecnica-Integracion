using System;
using System.Collections.Generic;

namespace PruebaTecnicaAPI.Models;

public partial class Phone
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Number { get; set; } = null!;

    public string CityCode { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
