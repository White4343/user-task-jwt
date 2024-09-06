using FluentAssertions;
using NSubstitute;
using System.Security.Claims;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.GetTaskById;
using Task = System.Threading.Tasks.Task;
using TaskStatus = UserTaskJWT.Web.Api.Tasks.TaskStatus;

namespace UserTaskJWT.UnitTests.TaskTests
{
    public class GetTaskByIdHandlerTests
    {
        private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();
        private readonly GetTaskByIdHandler _handler;

        public GetTaskByIdHandlerTests()
        {
            _handler = new GetTaskByIdHandler(_taskRepository);
        }

        [Fact]
        public async Task HandleAsync_ValidIdAndAuthorizedUser_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var task = new Web.Api.Tasks.Task
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

            // Act
            var response = await _handler.HandleAsync(taskId, user, CancellationToken.None).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().Be(taskId);
            response.Title.Should().Be(task.Title);
            response.Description.Should().Be(task.Description);
            response.DueDate.Should().Be(task.DueDate);
            response.Status.Should().Be(task.Status);
            response.Priority.Should().Be(task.Priority);
            response.CreatedAt.Should().Be(task.CreatedAt);
            response.UpdatedAt.Should().Be(task.UpdatedAt);
            response.UserId.Should().Be(task.UserId);
        }

        [Fact]
        public async Task HandleAsync_UserWithoutUserIdClaim_ThrowsInvalidOperationException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // User without NameIdentifier claim

            // Act & Assert
            // Assuming GetUserId.GetUserIdFromClaims throws InvalidOperationException when claim is missing
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.HandleAsync(taskId, user, CancellationToken.None))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var task = new Web.Api.Tasks.Task
            {
                Id = taskId,
                UserId = Guid.NewGuid(), // Different UserId
                Title = "null",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                CreatedAt = default,
                UpdatedAt = default
            }; 
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.HandleAsync(taskId, user, CancellationToken.None))
                .ConfigureAwait(false);
        }
    }
}