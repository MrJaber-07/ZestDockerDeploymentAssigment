using API.Controllers;
using Application.DTOs.Item;
using Application.Services;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace APIs.tests.Controller
{
    public class ItemControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IItemService> _itemServiceMock;
        private readonly ItemController _controller;

        public ItemControllerTests()
        {
            _fixture = new Fixture();
            _itemServiceMock = new Mock<IItemService>();
            _controller = new ItemController(
                Mock.Of<ILogger<ItemController>>(),
                _itemServiceMock.Object
            );
        }

        #region Get Methods

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var itemResponse = _fixture.Create<ItemResponse>();
            _itemServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(itemResponse);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(itemResponse);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            int id = 99;
            _itemServiceMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ItemResponse)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Create Method

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var request = _fixture.Create<ItemRequest>();
            var response = _fixture.Create<ItemResponse>();
            _itemServiceMock.Setup(x => x.CreateAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(response.Id);
            createdResult.Value.Should().BeEquivalentTo(response);
        }

        #endregion

        #region Update Method

        [Fact]
        public async Task UpdateItem_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var id = _fixture.Create<int>();
            var request = _fixture.Create<ItemRequest>();
            var response = _fixture.Create<ItemResponse>();
            _itemServiceMock.Setup(x => x.UpdateAsync(id, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateItem(id, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _itemServiceMock.Verify(x => x.UpdateAsync(id, request), Times.Once);
        }

        #endregion

        #region Delete Method

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _itemServiceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _itemServiceMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion
    }
}