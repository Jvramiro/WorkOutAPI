using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using WorkOutAPI.Controllers;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;
using WorkOutAPI.Services;

namespace WorkOutAPI.Tests.Controllers;

public class LoginControllerTests
{
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly TokenService tokenService;
    private readonly LoginController controller;
    
    private readonly User mockUser;

    public LoginControllerTests()
    {
        mockUserRepository = new Mock<IUserRepository>();
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockConfiguration = new Mock<IConfiguration>();

        mockConfiguration.Setup(c => c["Security:JwtKey"]).Returns("ThisIsAVerySecretKeyThatIsAtLeast32BytesLong!!");

        tokenService = new TokenService(mockConfiguration.Object);

        controller = new LoginController(mockUserRepository.Object, mockUnitOfWork.Object, tokenService);

        mockUser = new User()
        {
            Id = 1,
            Username = "John Doe",
            Email = "test@test.com",
            PasswordHash = PasswordHashService.HashPassword("password123"),
            Role = Enums.Role.Default,
            RefreshToken = "valid_refresh_token"
        };
    }

    void SetAuthenticatedUserClaim(string type, string value)
    {
        var claims = new List<Claim> {
            new Claim(type, value)
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
    public async Task Login_ShouldReturnNotFound_WhenEmailInvalid()
    {
        //Arrange
        mockUserRepository.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync((User?)null);

        //Act
        var result = await controller.Login(new UserLoginDTO("invalid@test.com", "password123"));

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnNotFound_WhenPasswordInvalid()
    {
        //Arrange
        mockUserRepository.Setup(r => r.GetByEmail(mockUser.Email)).ReturnsAsync(mockUser);

        //Act
        var result = await controller.Login(new UserLoginDTO(mockUser.Email, "wrongpassword"));

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsValid()
    {
        //Arrange
        mockUserRepository.Setup(r => r.GetByEmail(mockUser.Email)).ReturnsAsync(mockUser);
        mockUserRepository.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Login(new UserLoginDTO(mockUser.Email, "password123"));

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.Email, "invalid@test.com");
        mockUserRepository.Setup(r => r.GetByEmail("invalid@test.com")).ReturnsAsync((User?)null);

        //Act
        var result = await controller.Refresh("any_token");

        //Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_ShouldReturnUnauthorized_WhenRefreshTokenInvalid()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.Email, mockUser.Email);
        mockUserRepository.Setup(r => r.GetByEmail(mockUser.Email)).ReturnsAsync(mockUser);

        //Act
        var result = await controller.Refresh("invalid_refresh_token");

        //Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Refresh_ShouldReturnOk_WhenRefreshTokenValid()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.Email, mockUser.Email);
        mockUserRepository.Setup(r => r.GetByEmail(mockUser.Email)).ReturnsAsync(mockUser);
        mockUserRepository.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Refresh(mockUser.RefreshToken!);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
