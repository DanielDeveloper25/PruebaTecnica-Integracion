using System;
using System.Collections.Generic;

namespace PruebaTecnicaAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    public DateTime LastLogin { get; set; }

    public string Token { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
