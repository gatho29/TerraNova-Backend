using AutoMapper;
using FluentAssertions;
using Moq;
using PropertyAPI.Application.DTOs;
using PropertyAPI.Application.Services;
using PropertyAPI.Domain.Entities;
using PropertyAPI.Domain.Interfaces;
using Xunit;

namespace PropertyAPI.Tests.Services;

public class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PropertyService _service;

    public PropertyServiceTests()
    {
        _repositoryMock = new Mock<IPropertyRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new PropertyService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetPropertiesAsync_ShouldReturnMappedProperties()
    {
        // Arrange
        var filtersDto = new PropertyFilterDto { Name = "Casa" };
        var domainFilter = new PropertyFilter { Name = "Casa" };
        var properties = new List<Property>
        {
            new Property { Id = "1", Name = "Casa Test", Price = 100000 }
        };
        var propertyDtos = new List<PropertyDto>
        {
            new PropertyDto { Id = "1", Name = "Casa Test", Price = 100000 }
        };

        _repositoryMock.Setup(r => r.GetPropertiesAsync(
            It.Is<PropertyFilter>(f => f.Name == "Casa"), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);
        _mapperMock.Setup(m => m.Map<IEnumerable<PropertyDto>>(properties))
            .Returns(propertyDtos);

        // Act
        var result = await _service.GetPropertiesAsync(filtersDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Casa Test");
        _repositoryMock.Verify(r => r.GetPropertiesAsync(
            It.Is<PropertyFilter>(f => f.Name == "Casa"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPropertyByIdAsync_WithValidId_ShouldReturnProperty()
    {
        // Arrange
        var id = "1";
        var property = new Property { Id = id, Name = "Test Property" };
        var propertyDto = new PropertyDto { Id = id, Name = "Test Property" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);
        _mapperMock.Setup(m => m.Map<PropertyDto>(property))
            .Returns(propertyDto);

        // Act
        var result = await _service.GetPropertyByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Test Property");
    }

    [Fact]
    public async Task GetPropertyByIdAsync_WithInvalidId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidId = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetPropertyByIdAsync(invalidId));
    }

    [Fact]
    public async Task GetPropertyByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var id = "999";
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property?)null);

        // Act
        var result = await _service.GetPropertyByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }
}

