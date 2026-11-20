using System;
using System.Collections.Generic;

namespace CartDbFirstApp.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;
}
