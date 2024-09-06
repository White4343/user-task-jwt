using FluentValidation;

namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator()
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