using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkOutAPI.Controllers;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Tests.Controllers;

public class RegisterControllerTests
{
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly RegisterController controller;

    public RegisterControllerTests()
    {
        mockUserRepository = new Mock<IUserRepository>();
        mockUnitOfWork = new Mock<IUnitOfWork>();

        controller = new RegisterController(mockUserRepository.Object, mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        //Arrange
        controller.ModelState.AddModelError("Email", "Required");

        //Act
        var result = await controller.Register(new UserRegisterDTO());

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyRegistered()
    {
        //Arrange
        var email = "test@test.com";
        var mockUser = new User { Email = email };
        mockUserRepository.Setup(r => r.GetByEmail(email)).ReturnsAsync(mockUser);

        //Act
        var result = await controller.Register(new UserRegisterDTO { Email = email, Password = "password123", Username = "TestUser" });

        //Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenModelValid()
    {
        //Arrange
        var email = "new@test.com";
        mockUserRepository.Setup(r => r.GetByEmail(email)).ReturnsAsync((User?)null);
        mockUserRepository.Setup(r => r.Add(It.IsAny<User>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Register(new UserRegisterDTO { Email = email, Password = "password123", Username = "NewUser" });

        //Assert
        Assert.IsType<CreatedResult>(result);
    }
}
