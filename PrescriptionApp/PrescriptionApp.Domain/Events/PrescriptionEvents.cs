using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Domain.Events
{
    public record PrescriptionCreatedEvent(
        int PrescriptionId,
        string PrescriptionCode,
        int PatientId,
        string PatientEmail,
        DateTime ExpiresAt);

    public record PrescriptionDispensedEvent(
        int PrescriptionId,
        string PrescriptionCode,
        int PatientId,
        DateTime DispensedAt);
}
