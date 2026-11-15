using System;
using System.Collections.Generic;

namespace AppointmentDbFirst.Domain.Entities;

public partial class Doctor
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public DateTime WorkingHoursStart { get; set; }

    public DateTime WorkingHoursEnd { get; set; }

    public int SlotDurationMinutes { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
