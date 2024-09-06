using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using System.Security.Claims;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.CreateTask;
using Task = System.Threading.Tasks.Task;
using TaskStatus = UserTaskJWT.Web.Api.Tasks.TaskStatus;

namespace UserTaskJWT.UnitTests
{
    public class CreateTaskHandlerTests
    {
        private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();
        private readonly IValidator<CreateTaskCommand> _createTaskValidator = Substitute.For<IValidator<CreateTaskCommand>>();
        private readonly CreateTaskHandler _handler;

        public CreateTaskHandlerTests()
        {
            _handler = new CreateTaskHandler(_taskRepository, _createTaskValidator);
        }

        [Fact]
        public async Task HandleAsync_ValidCommandAndUser_CreatesTaskAndReturnsIt()
        {
            // Arrange
            var command = new CreateTaskCommand("Test Task", "Test Description", DateTime.UtcNow.AddDays(1), TaskStatus.Pending, TaskPriority.Medium);
            var userId = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            var validationResult = new ValidationResult(); // Valid result
            _createTaskValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act
            var task = await _handler.HandleAsync(command, user, CancellationToken.None).ConfigureAwait(false);

            // Assert
            task.Should().NotBeNull();
            task.Id.Should().NotBeEmpty();
            task.Title.Should().Be(command.Title);
            task.Description.Should().Be(command.Description);
            task.DueDate.Should().Be(command.DueDate);
            task.Status.Should().Be(command.Status);
            task.Priority.Should().Be(command.Priority);
            task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            task.UserId.Should().Be(new Guid(userId));

            await _taskRepository.Received(1).CreateAsync(task, Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UserWithoutUserIdClaim_ThrowsArgumentNullException()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // User without NameIdentifier claim

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.HandleAsync(new CreateTaskCommand("Test", "Test", 
                    DateTime.UtcNow, TaskStatus.Pending, TaskPriority.Medium), user, CancellationToken.None))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var command = new CreateTaskCommand("", null, DateTime.UtcNow, TaskStatus.Pending, TaskPriority.Medium); // Invalid command
            var userId = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") }); // Invalid result
            _createTaskValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => 
                _handler.HandleAsync(command, user, CancellationToken.None)).ConfigureAwait(false);
        }
    }
}