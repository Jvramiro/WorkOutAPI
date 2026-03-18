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

public class CheckInControllerTests
{

    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<ICheckInRepository> mockCheckInRepository;
    private readonly Mock<IUnityOfWork> mockUnityOfWork;
    private readonly CheckInController controller;
    
    private readonly User mockUser;
    private readonly List<CheckIn> mockCheckIns;
    private readonly string mockEmail = "test@test.com";

    public CheckInControllerTests()
    {
        mockUserRepository = new Mock<IUserRepository>();
        mockCheckInRepository = new Mock<ICheckInRepository>();
        mockUnityOfWork = new Mock<IUnityOfWork>();

        controller = new CheckInController(mockUserRepository.Object, mockCheckInRepository.Object, mockUnityOfWork.Object);

        mockUser = new User()
        {
            Id = 0,
            Username = "John Doe",

        };
        mockCheckIns = new List<CheckIn>
        {
            new CheckIn
            {
                Id = 1,
                UserId = 0,
                Date = DateTime.UtcNow

            }
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
    public async Task GetListByUserId_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange
        var userId = 0;
        mockCheckInRepository.Setup(r => r.GetListByUserId(userId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockCheckIns);

        //Act
        var result = await controller.GetListByUserId(userId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 1000)]
    public async Task GetListByUserId_ShouldReturnBadRequest_WhenInvalidPageOrSize(int page, int size)
    {
        //Arrange
        var userId = 0;

        //Act
        var result = await controller.GetListByUserId(userId, page, size);

        //Asset
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetListByUserId_ShouldReturnOk_WhenUserFound()
    {
        //Arrange
        var userId = 0;

        mockUserRepository.Setup(r => r.GetById(userId)).ReturnsAsync(mockUser);
        mockCheckInRepository.Setup(r => r.GetListByUserId(userId)).ReturnsAsync(mockCheckIns);

        //Act
        var result = await controller.GetListByUserId(userId);
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 1000)]
    public async Task GetListBySelfId_ShouldReturnBadRequest_WhenInvalidPageOrSize(int page, int size)
    {
        //Act
        var result = await controller.GetListBySelfId(page, size);

        //Asset
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetListBySelfId_ShouldReturnNotFound_WhenUserNotFound()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.Email, mockEmail);
        mockUserRepository.Setup(r => r.GetByEmail(mockEmail)).ReturnsAsync((User?) null);

        //Act
        var result = await controller.GetListBySelfId();

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetListBySelfId_ShouldReturnOk_WhenUserFound()
    {
        //Arrange
        var userId = 0;
        SetAuthenticatedUserClaim(ClaimTypes.Email, mockEmail);
        mockUserRepository.Setup(r => r.GetByEmail(mockEmail)).ReturnsAsync(mockUser);
        mockCheckInRepository.Setup(r => r.GetListByUserId(userId)).ReturnsAsync(mockCheckIns);

        //Act
        var result = await controller.GetListBySelfId();

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        //Arrange
        controller.ModelState.AddModelError("UserId", "Required");

        //Act
        var result = await controller.Create(new CheckInCreateDTO());

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIdentifierInvalid()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "invalidNameIdentifier");
        
        //Act
        var result = await controller.Create(new CheckInCreateDTO());

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenModelStateValid()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "0");
        mockCheckInRepository.Setup(r => r.Add(It.IsAny<CheckIn>())).Returns(Task.CompletedTask);
        mockUnityOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        
        //Act
        var result = await controller.Create(new CheckInCreateDTO());

        //Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFound_WhenIdNotFound()
    {
        //Arrange
        var checkInId = 0;
        mockCheckInRepository.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((CheckIn?) null);
        
        //Act
        var result = await controller.Remove(checkInId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnForbid_WhenNotAuthorized()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "0");
        var checkInId = 0;
        var mockCheckIn = new CheckIn()
        {
            Id = checkInId,
            UserId = 1
        };

        mockCheckInRepository.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(mockCheckIn);
        
        //Act
        var result = await controller.Remove(checkInId);

        //Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnOk_WhenValidParameters()
    {
        //Arrange
        SetAuthenticatedUserClaim(ClaimTypes.NameIdentifier, "0");
        var checkInId = 0;
        var mockCheckIn = new CheckIn()
        {
            Id = checkInId,
            UserId = 0
        };

        mockCheckInRepository.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync(mockCheckIn);
        mockCheckInRepository.Setup(r => r.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);
        mockUnityOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        
        //Act
        var result = await controller.Remove(checkInId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

}