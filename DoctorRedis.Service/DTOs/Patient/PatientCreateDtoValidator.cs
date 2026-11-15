using FluentValidation;

namespace DoctorRedis.Service.DTOs
{
    public class PatientCreateDtoValidator : AbstractValidator<PatientCreateDto>
    {
        public PatientCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Doktor adı soyadı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Doktor adı soyadı en az 3 karakter olmalıdır.")
                .MaximumLength(150).WithMessage("Doktor adı soyadı en fazla 150 karakter olabilir.");

            RuleFor(x => x.IdentityNumber)
                .NotEmpty().WithMessage("Tckn alanı (branş) boş bırakılamaz.")
                .Length(11).WithMessage("11 haneli olmalı"); ;
        }
    }
}
