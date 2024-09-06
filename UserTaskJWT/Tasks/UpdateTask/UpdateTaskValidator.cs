using FluentValidation;

namespace UserTaskJWT.Web.Api.Tasks.UpdateTask
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                    .WithMessage("Title is required");

            RuleFor(x => x.DueDate)
                .Must(date => date != default(DateTime))
                    .WithMessage("DueDate must be in DateTime format");

            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.Priority)
                .IsInEnum();
        }
    }
}
