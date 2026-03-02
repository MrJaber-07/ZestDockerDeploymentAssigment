using API.Controllers;
using Application.Services;
using Application.ViewModels.Product;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace APIs.tests.Controller
{
    public class ProductControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _fixture = new Fixture();
            _productServiceMock = new Mock<IProductService>();
            _controller = new ProductController(
                _productServiceMock.Object,
                Mock.Of<ILogger<ProductController>>()
            );
        }

        #region GET Tests

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithData()
        {
            // Arrange
            var products = _fixture.CreateMany<ProductResponse>(3);
            _productServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                               .ReturnsAsync((ProductResponse)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var request = _fixture.Create<ProductRequest>();
            var response = _fixture.Create<ProductResponse>();
            _productServiceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task Update_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<ProductRequest>();
            var response = _fixture.Create<ProductResponse>();
            _productServiceMock.Setup(s => s.UpdateAsync(id, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            var id = 1;
            var request = _fixture.Create<ProductRequest>();
            _productServiceMock.Setup(s => s.UpdateAsync(id, request))
                               .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            _productServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        #endregion
    }
}