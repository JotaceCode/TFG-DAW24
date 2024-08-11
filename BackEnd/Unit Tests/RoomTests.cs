using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.API.Hotel.Controllers;
using Services.API.Hotel.Core.Entities;
using Services.API.Hotel.Repository;

namespace Hotel
{
    public class RoomTests
    {
        private readonly Mock<IMongoRepository<RoomEntity>> _mockRepository;
        private readonly RoomServiceController _controller;

        public RoomTests()
        {
            _mockRepository = new Mock<IMongoRepository<RoomEntity>>();
            _controller = new RoomServiceController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfRooms()
        {
            var rooms = new List<RoomEntity> {
            new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" },
            new RoomEntity { RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Vacia" }
        };
            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(rooms);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomEntity>>(okResult.Value);
            Assert.Equal(rooms.Count, returnedRooms.Count());
        }

        [Fact]
        public async Task GetById_ExistingRoom_ReturnsRoom()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _controller.GetById(roomId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoom = Assert.IsAssignableFrom<RoomEntity>(okResult.Value);
            Assert.Equal(room, returnedRoom);
        }

        [Fact]
        public async Task GetById_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _controller.GetById(roomId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidRoom_ReturnsCreatedAtAction()
        {
            var room = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var result = await _controller.Insert(room);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(room.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Insert_InvalidRoom_ReturnsBadRequest()
        {
            var room = new RoomEntity { RoomNumber = null };

            var result = await _controller.Insert(room);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingRoom_ReturnsCreatedAtAction()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _controller.Update(roomId, updatedRoom);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.NotEqual(updatedRoom.RoomNumber, createdAtActionResult.RouteValues["RoomNumber"]);
            Assert.NotEqual(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _controller.Update(roomId, updatedRoom);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingRoom_ReturnsBadRequest()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = "2", RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _controller.Update(roomId, updatedRoom);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingRoom_ReturnsOk()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _controller.Delete(roomId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _controller.Delete(roomId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}