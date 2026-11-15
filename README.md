# InterviewProjects

dotnet tool install --global dotnet-ef

dotnet ef --version

dotnet tool install --global dotnet-ef --version 8.*

Scaffold-DbContext "server=localhost:5442;username=postgres;password=postgres;database=DoctorAppointmentBooking;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir ..\AppointmentDbFirst.Domain\Entities -ContextDir Contexts -Context AppointmentDbFirstContext -Namespace AppointmentDbFirst.Domain.Entities -ContextNamespace AppointmentDbFirst.Persistence.Contexts -Force

dotnet ef dbcontext scaffold "server=localhost:5442;username=postgres;password=postgres;database=DoctorAppointmentBooking;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL --project AppointmentDbFirst.Persistence --startup-project AppointmentDbFirst.Api --output-dir ..\AppointmentDbFirst.Domain\Entities --context-dir Contexts --context AppointmentDbFirstContext --namespace AppointmentDbFirst.Domain.Entities --context-namespace AppointmentDbFirst.Persistence.Contexts --force

dotnet ef dbcontext scaffold "server=localhost:5442;username=postgres;password=postgres;database=DoctorAppointmentBooking;Pooling=true;" Npgsql.EntityFrameworkCore.PostgreSQL --project AppointmentDbFirst\AppointmentDbFirst.Persistence --startup-project AppointmentDbFirst\AppointmentDbFirst.Api --output-dir ..\AppointmentDbFirst.Domain\Entities --context-dir Contexts --context AppointmentDbFirstContext --namespace AppointmentDbFirst.Domain.Entities --context-namespace AppointmentDbFirst.Persistence.Contexts --force

(Not: Connection String içindeki veritabanı adı, kullanıcı adı ve şifreyi kendi ortamına göre güncellemeyi unutma; komut çalıştıktan sonra üretilen AppointmentDbFirstContext.cs içindeki OnConfiguring metodunu silip bağlantı cümlesini appsettings.json dosyasına alabilirsin).

