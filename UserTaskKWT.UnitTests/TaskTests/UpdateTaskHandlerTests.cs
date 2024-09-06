using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using System.Security.Claims;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.UpdateTask;
using UserTaskJWT.Web.Api.Users;
using Task = System.Threading.Tasks.Task;
using TaskStatus = UserTaskJWT.Web.Api.Tasks.TaskStatus;

namespace UserTaskJWT.UnitTests.TaskTests
{
    public class UpdateTaskHandlerTests
    {
        private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();
        private readonly IValidator<UpdateTaskCommand> _updateTaskValidator = Substitute.For<IValidator<UpdateTaskCommand>>();
        private readonly UpdateTaskHandler _handler;

        public UpdateTaskHandlerTests()
        {
            _handler = new UpdateTaskHandler(_taskRepository, _updateTaskValidator);
        }

        [Fact]
        public async Task HandleAsync_ValidCommandAndAuthorizedUser_UpdatesTaskAndReturnsResponse()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateTaskCommand("Updated Task", "Updated Description", DateTime.UtcNow.AddDays(2), TaskStatus.InProgress, TaskPriority.High);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var existingTask = new Web.Api.Tasks.Task
            {
                Id = taskId,
                Title = "Original Task",
                Description = "Original Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            var validationResult = new ValidationResult(); // Valid result
            _updateTaskValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(existingTask);

            // Act
            var response = await _handler.HandleAsync(taskId, command, user, CancellationToken.None).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(taskId);
            response.Title.Should().Be(command.Title);
            response.Description.Should().Be(command.Description);
            response.DueDate.Should().Be(command.DueDate);
            response.Status.Should().Be(command.Status);
            response.Priority.Should().Be(command.Priority);
            response.CreatedAt.Should().Be(existingTask.CreatedAt); // CreatedAt should remain unchanged
            response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            response.UserId.Should().Be(userId);

            await _taskRepository.Received(1).UpdateAsync(Arg.Is<Web.Api.Tasks.Task>(t =>
                t.Id == taskId &&
                t.Title == command.Title &&
                t.Description == command.Description &&
                t.DueDate == command.DueDate &&
                t.Status == command.Status &&
                t.Priority == command.Priority &&
                t.CreatedAt == existingTask.CreatedAt &&
                t.UserId == userId
            ), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_NullCommand_ThrowsArgumentNullException()
        {
            var command = new UpdateTaskCommand("Test", "Test", DateTime.UtcNow, TaskStatus.Pending, TaskPriority.Medium);

            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.HandleAsync(Guid.NewGuid(), command, new ClaimsPrincipal(), CancellationToken.None))
                    .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_NullUser_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var command = new UpdateTaskCommand("Test", "Test", DateTime.UtcNow, TaskStatus.Pending, TaskPriority.Medium);

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.HandleAsync(Guid.NewGuid(), command, user, CancellationToken.None))
                    .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateTaskCommand("null", null, DateTime.UtcNow, TaskStatus.Pending, TaskPriority.Medium); // Invalid command
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var existingTask = new Web.Api.Tasks.Task
            {
                Id = taskId,
                UserId = userId,
                Title = "null",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                CreatedAt = default,
                UpdatedAt = default
            };
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") }); // Invalid result
            _updateTaskValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(existingTask);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(taskId, command, user, CancellationToken.None))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateTaskCommand("Updated Task", "Updated Description", DateTime.UtcNow.AddDays(2), TaskStatus.InProgress, TaskPriority.High);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var existingTask = new Web.Api.Tasks.Task
            {
                Id = taskId,
                UserId = Guid.NewGuid(), // Different UserId
                Title = "null",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                CreatedAt = default,
                UpdatedAt = default
            };
            var validationResult = new ValidationResult(); // Valid result
            _updateTaskValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(existingTask);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(taskId, command, user, CancellationToken.None))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_TaskNotFound_ThrowsArgumentNullException() // Or another appropriate exception based on your requirements
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateTaskCommand("Updated Task", "Updated Description", DateTime.UtcNow.AddDays(2), TaskStatus.InProgress, TaskPriority.High);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));

            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>())!.Returns((Web.Api.Tasks.Task?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(taskId, command, user, CancellationToken.None))
                .ConfigureAwait(false);
        }
    }
}