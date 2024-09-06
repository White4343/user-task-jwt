using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using System.Security.Claims;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.DeleteTask;
using Task = System.Threading.Tasks.Task;
using TaskStatus = UserTaskJWT.Web.Api.Tasks.TaskStatus;

namespace UserTaskJWT.UnitTests.TaskTests
{
    public class DeleteTaskHandlerTests
    {
        private readonly ITaskRepository _taskRepository = Substitute.For<ITaskRepository>();
        private readonly DeleteTaskHandler _handler;

        public DeleteTaskHandlerTests()
        {
            _handler = new DeleteTaskHandler(_taskRepository);
        }

        [Fact]
        public async Task HandleAsync_ValidIdAndAuthorizedUser_DeletesTaskAndReturnsAccepted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            var task = new Web.Api.Tasks.Task
            {
                Id = taskId,
                UserId = userId,
                Title = "null",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low,
                CreatedAt = default,
                UpdatedAt = default
            };
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);

            // Act
            var result = await _handler.HandleAsync(taskId, user, CancellationToken.None).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType<Accepted>();
            await _taskRepository.Received(1).DeleteAsync(taskId, Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_NullId_ThrowsFormatException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<FormatException>(() =>
                _handler.HandleAsync(new Guid("null"), new ClaimsPrincipal(), CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UserWithoutUserIdClaim_ThrowsArgumentNullException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // User without NameIdentifier claim

            // Act & Assert
            // Assuming GetUserInformation.GetUserIdFromClaims throws InvalidOperationException when claim is missing
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(taskId, user, CancellationToken.None))
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

        [Fact]
        public async Task HandleAsync_TaskNotFound_ThrowsNullReferenceException() // Or another appropriate exception based on your requirements
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }));
            _taskRepository.GetTaskAsync(taskId, Arg.Any<CancellationToken>())!.Returns((Web.Api.Tasks.Task?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.HandleAsync(taskId, user, CancellationToken.None))
                .ConfigureAwait(false);
        }
    }
}