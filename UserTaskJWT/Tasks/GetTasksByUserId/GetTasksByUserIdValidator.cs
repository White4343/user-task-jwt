using FluentValidation;

namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public class GetTasksByUserIdValidator : AbstractValidator<GetTasksByUserIdQuery>
    {
        public GetTasksByUserIdValidator()
        {
            RuleFor(x => x.DueDate)
                .Must(date => date != default(DateTime))
                    .WithMessage("DueDate must be in DateTime format");

            RuleFor(x => x.Status)
                .IsInEnum();

            RuleFor(x => x.Priority)
                .IsInEnum();

            RuleFor(x => x.PrioritySorting)
                .IsInEnum();

            RuleFor(x => x.DueDateSorting)
                .IsInEnum();
        }
    }
}