using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkOutAPI.Controllers;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IExerciseRepository> mockExerciseRepository;
    private readonly Mock<IUnityOfWork> mockUnityOfWork;
    private readonly UsersController controller;
    
    private readonly User mockUser;
    private readonly List<User> mockUsers;

    public UsersControllerTests()
    {
        mockUserRepository = new Mock<IUserRepository>();
        mockExerciseRepository = new Mock<IExerciseRepository>();
        mockUnityOfWork = new Mock<IUnityOfWork>();

        controller = new UsersController(mockUserRepository.Object, mockExerciseRepository.Object, mockUnityOfWork.Object);

        mockUser = new User()
        {
            Id = 1,
            Username = "John Doe",
            Email = "test@test.com",
            Role = Enums.Role.Default,
            Schedule = new List<Exercise>()
        };

        mockUsers = new List<User>
        {
            mockUser
        };
    }

    void SetAuthenticatedUserClaim(string type, string value, string role = "Default")
    {
        var claims = new List<Claim> {
            new Claim(type, value),
            new Claim(ClaimTypes.Role, role)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext {
                User = claimsPrincipal
            }
        };
    }

    [Fact]
    public async Task GetList_ShouldReturnOk_WhenValidParameters()
    {
        //Arrange
        mockUserRepository.Setup(r => r.GetList(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockUsers);

        //Act
        var result = await controller.GetList(1, 10);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    public async Task GetList_ShouldReturnBadRequest_WhenInvalidPageOrSize(int page, int size)
    {
        //Act
        var result = await controller.GetList(page, size);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetList_ShouldReturnBadRequest_WhenSizeTooLarge()
    {
        //Act
        var result = await controller.GetList(1, 150);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenUserFound()
    {
        //Arrange
        var userId = 1;
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync(mockUser);

        //Act
        var result = await controller.GetById(userId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange
        var userId = 1;
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        //Act
        var result = await controller.GetById(userId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        //Arrange
        controller.ModelState.AddModelError("Username", "Required");

        //Act
        var result = await controller.Update(1, new UserUpdateDTO(null, null));

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnForbid_WhenNotOwnerOrAdmin()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "2"); // User mapped to id 1, but authenticated as 2
        var userId = 1;

        //Act
        var result = await controller.Update(userId, new UserUpdateDTO(null, null));

        //Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange
        var userId = 1;
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, userId.ToString());
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        //Act
        var result = await controller.Update(userId, new UserUpdateDTO(null, null));

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenScheduleExerciseNotFound()
    {
        //Arrange
        var userId = 1;
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, userId.ToString());
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync(mockUser);
        mockExerciseRepository.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Exercise?)null);

        var model = new UserUpdateDTO(null, new List<int> { 99 });

        //Act
        var result = await controller.Update(userId, model);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenValidAndOwner()
    {
        //Arrange
        var userId = 1;
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, userId.ToString());
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync(mockUser);
        
        var exercise = new Exercise { Id = 1, Name = "Test" };
        mockExerciseRepository.Setup(r => r.GetById(1)).ReturnsAsync(exercise);

        mockUserRepository.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        mockUnityOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        var model = new UserUpdateDTO("UpdatedName", new List<int> { 1 });

        //Act
        var result = await controller.Update(userId, model);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnForbid_WhenNotOwnerOrAdmin()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "2"); // Authenticated as 2, trying to delete 1
        var userId = 1;

        //Act
        var result = await controller.Remove(userId);

        //Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange
        var userId = 1;
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, userId.ToString());
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        //Act
        var result = await controller.Remove(userId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnOk_WhenValidAndOwner()
    {
        //Arrange
        var userId = 1;
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, userId.ToString());
        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync(mockUser);
        mockUserRepository.Setup(r => r.Delete(userId)).Returns(Task.CompletedTask);
        mockUnityOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Remove(userId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
