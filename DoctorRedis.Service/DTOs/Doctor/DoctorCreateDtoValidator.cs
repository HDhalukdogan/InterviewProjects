using FluentValidation;

namespace DoctorRedis.Service.DTOs
{
    public class DoctorCreateDtoValidator : AbstractValidator<DoctorCreateDto>
    {
        public DoctorCreateDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Doktor adı soyadı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Doktor adı soyadı en az 3 karakter olmalıdır.")
                .MaximumLength(150).WithMessage("Doktor adı soyadı en fazla 150 karakter olabilir.");

            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Uzmanlık alanı (branş) boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Uzmanlık alanı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Uzmanlık alanı en fazla 100 karakter olabilir.");
        }
    }
}
