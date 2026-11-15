using System;
using System.Collections.Generic;

namespace AppointmentDbFirst.Domain.Entities;

public partial class Patient
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
