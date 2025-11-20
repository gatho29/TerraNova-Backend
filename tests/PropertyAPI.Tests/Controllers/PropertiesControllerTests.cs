using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PropertyAPI.Application.DTOs;
using PropertyAPI.Application.Interfaces;
using PropertyAPI.Controllers;
using Xunit;

namespace PropertyAPI.Tests.Controllers;

public class PropertiesControllerTests
{
    private readonly Mock<IPropertyService> _serviceMock;
    private readonly Mock<ILogger<PropertiesController>> _loggerMock;
    private readonly PropertiesController _controller;

    public PropertiesControllerTests()
    {
        _serviceMock = new Mock<IPropertyService>();
        _loggerMock = new Mock<ILogger<PropertiesController>>();
        _controller = new PropertiesController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetProperties_WithoutFilters_ShouldReturnAllProperties()
    {
        // Arrange
        var properties = new List<PropertyDto>
        {
            new PropertyDto { Id = "1", Name = "Property 1", Price = 100000 },
            new PropertyDto { Id = "2", Name = "Property 2", Price = 200000 }
        };

        _serviceMock.Setup(s => s.GetPropertiesAsync(
            It.Is<PropertyFilterDto>(f => f.Name == null && f.Address == null && !f.MinPrice.HasValue && !f.MaxPrice.HasValue),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _controller.GetProperties();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProperties = okResult.Value.Should().BeAssignableTo<IEnumerable<PropertyDto>>().Subject;
        returnedProperties.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProperties_WithNameFilter_ShouldReturnFilteredProperties()
    {
        // Arrange
        var properties = new List<PropertyDto>
        {
            new PropertyDto { Id = "1", Name = "Casa Test", Price = 100000 }
        };

        _serviceMock.Setup(s => s.GetPropertiesAsync(
            It.Is<PropertyFilterDto>(f => f.Name == "Casa"),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _controller.GetProperties(name: "Casa");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProperties = okResult.Value.Should().BeAssignableTo<IEnumerable<PropertyDto>>().Subject;
        returnedProperties.Should().HaveCount(1);
        returnedProperties.First().Name.Should().Contain("Casa");
    }

    [Fact]
    public async Task GetProperties_WithInvalidPriceRange_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.GetProperties(minPrice: 200000, maxPrice: 100000);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
    }

    [Fact]
    public async Task GetPropertyById_WithValidId_ShouldReturnProperty()
    {
        // Arrange
        var id = "1";
        var property = new PropertyDto { Id = id, Name = "Test Property", Price = 100000 };

        _serviceMock.Setup(s => s.GetPropertyByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _controller.GetPropertyById(id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProperty = okResult.Value.Should().BeOfType<PropertyDto>().Subject;
        returnedProperty.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetPropertyById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var id = "999";
        _serviceMock.Setup(s => s.GetPropertyByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyDto?)null);

        // Act
        var result = await _controller.GetPropertyById(id);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
    }

    [Fact]
    public async Task GetPropertyById_WithEmptyId_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.GetPropertyById("");

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
    }
}

