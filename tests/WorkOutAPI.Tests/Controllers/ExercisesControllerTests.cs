using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkOutAPI.Controllers;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Tests.Controllers;

public class ExercisesControllerTests
{
    private readonly Mock<IExerciseRepository> mockExerciseRepository;
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly ExercisesController controller;
    
    private readonly Exercise mockExercise;
    private readonly List<Exercise> mockExercises;

    public ExercisesControllerTests()
    {
        mockExerciseRepository = new Mock<IExerciseRepository>();
        mockUnitOfWork = new Mock<IUnitOfWork>();

        controller = new ExercisesController(mockExerciseRepository.Object, mockUnitOfWork.Object);

        mockExercise = new Exercise()
        {
            Id = 1,
            Name = "Abdominal",
            Group = Enums.MuscleGroup.Abs
        };
        mockExercises = new List<Exercise>
        {
            mockExercise
        };
    }


    [Fact]
    public async Task GetList_ShouldReturnOk_WhenValidParameters()
    {
        //Arrange
        mockExerciseRepository.Setup(r => r.GetList(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockExercises);

        //Act
        var result = await controller.GetList(1, 10);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 1000)]
    public async Task GetList_ShouldReturnBadRequest_WhenInvalidPageOrSize(int page, int size)
    {
        //Act
        var result = await controller.GetList(page, size);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenExerciseFound()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync(mockExercise);

        //Act
        var result = await controller.GetById(exerciseId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenExerciseNotFound()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync((Exercise?) null);

        //Act
        var result = await controller.GetById(exerciseId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        //Arrange
        controller.ModelState.AddModelError("Name", "Required");

        //Act
        var result = await controller.Create(new ExerciseCreateDTO("", Enums.MuscleGroup.Chest));

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenModelValid()
    {
        //Arrange
        mockExerciseRepository.Setup(r => r.Add(It.IsAny<Exercise>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Create(new ExerciseCreateDTO("Pull Up", Enums.MuscleGroup.Back));

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        //Arrange
        controller.ModelState.AddModelError("Name", "Required");

        //Act
        var result = await controller.Update(1, new ExerciseUpdateDTO(null, null));

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenExerciseNotFound()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync((Exercise?)null);

        //Act
        var result = await controller.Update(exerciseId, new ExerciseUpdateDTO("Pull Up", null));

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenModelValid()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync(mockExercise);
        mockExerciseRepository.Setup(r => r.Update(It.IsAny<Exercise>())).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Update(exerciseId, new ExerciseUpdateDTO("Updated Name", null));

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFound_WhenExerciseNotFound()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync((Exercise?)null);

        //Act
        var result = await controller.Remove(exerciseId);

        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ShouldReturnOk_WhenExerciseFound()
    {
        //Arrange
        var exerciseId = 1;
        mockExerciseRepository.Setup(r => r.GetById(exerciseId)).ReturnsAsync(mockExercise);
        mockExerciseRepository.Setup(r => r.Delete(exerciseId)).Returns(Task.CompletedTask);
        mockUnitOfWork.Setup(u => u.Commit()).Returns(Task.CompletedTask);

        //Act
        var result = await controller.Remove(exerciseId);

        //Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
