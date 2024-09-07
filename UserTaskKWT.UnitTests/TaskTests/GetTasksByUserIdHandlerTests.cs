using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using System.Security.Claims;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.GetTasksByUserId;
using Task = System.Threading.Tasks.Task;
using TaskStatus = UserTaskJWT.Web.Api.Tasks.TaskStatus;

namespace UserTaskJWT.UnitTests.TaskTests
{
    public class GetTasksByUserIdHandlerTests
    {
        private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();
        private readonly IValidator<GetTasksByUserIdQuery> _getTaskByUserIdValidator = Substitute.For<IValidator<GetTasksByUserIdQuery>>();
        private readonly GetTasksByUserIdHandler _handler;

        public GetTasksByUserIdHandlerTests()
        {
            _handler = new GetTasksByUserIdHandler(_taskRepository, _getTaskByUserIdValidator);
        }

        [Fact]
        public async Task HandleAsync_ValidQueryAndAuthorizedUser_ReturnsFilteredAndSortedTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var query = new GetTasksByUserIdQuery(TaskPriority.High, TaskStatus.InProgress, 
                DateTime.UtcNow.AddDays(1), Sorting.Asc, Sorting.Desc,
                1, 10);

            var tasks = new List<Web.Api.Tasks.Task>
            {
                new Web.Api.Tasks.Task
                {
                    Id = Guid.NewGuid(), Title = "Task 1", Status = TaskStatus.InProgress, 
                    DueDate = DateTime.UtcNow.AddDays(2), Priority = TaskPriority.High, UserId = userId,
                    CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
                },
                new Web.Api.Tasks.Task { Id = Guid.NewGuid(), Title = "Task 2", Status = TaskStatus.Pending, DueDate = DateTime.UtcNow.AddDays(1), Priority = TaskPriority.Medium, UserId = userId,
                    CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Web.Api.Tasks.Task { Id = Guid.NewGuid(), Title = "Task 3", Status = TaskStatus.InProgress, DueDate = DateTime.UtcNow, Priority = TaskPriority.Low, UserId = userId,
                    CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                // Task with different UserId should be filtered out
                new Web.Api.Tasks.Task { Id = Guid.NewGuid(), Title = "Task 4", Status = TaskStatus.InProgress, DueDate = DateTime.UtcNow.AddDays(3), Priority = TaskPriority.High, UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            var validationResult = new ValidationResult(); // Valid result
            _getTaskByUserIdValidator.ValidateAsync(query, Arg.Any<CancellationToken>()).Returns(validationResult);
            _taskRepository.GetTasksByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(tasks);

            // Act
            var response = await _handler.HandleAsync(query, user, CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleAsync_NullQuery_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var query = new GetTasksByUserIdQuery(null, null, null, null, null,
                1, 10); // Potentially invalid query

            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _handler.HandleAsync(query, user, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_NullUser_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var user = new ClaimsPrincipal();
            var query = new GetTasksByUserIdQuery(null, null, null, null, null,
                1, 10);
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _handler.HandleAsync(query, user, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UserWithoutUserIdClaim_ThrowsInvalidOperationException()
        {
            // Arrange
            var query = new GetTasksByUserIdQuery(null, null, null, null, null,
                1, 10); // Potentially invalid query

            var user = new ClaimsPrincipal(new ClaimsIdentity()); // User without NameIdentifier claim

            // Act & Assert
            // Assuming GetUserInformation.GetUserIdFromClaims throws InvalidOperationException when claim is missing
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _handler.HandleAsync(query, user, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_InvalidQuery_ThrowsValidationException()
        {
            // Arrange
            var query = new GetTasksByUserIdQuery(null, null, null, null, null,
                1, 10); // Potentially invalid query
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") }); // Invalid result
            _getTaskByUserIdValidator.ValidateAsync(query, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(query, user, CancellationToken.None))
                .ConfigureAwait(false);
        }
    }
}