using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Services.API.Hotel.Controllers;
using Services.API.Hotel.Core;
using Services.API.Hotel.Core.Entities;
using Services.API.Hotel.Repository;

namespace Hotel
{
    // Room Tests
    public class RoomsTests
    {
        private readonly Mock<IMongoRepository<RoomEntity>> _mockRoomRepository;
        private readonly RoomServiceController _roomController;

        public RoomsTests()
        {
            _mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            _roomController = new RoomServiceController(_mockRoomRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfRooms()
        {
            var rooms = new List<RoomEntity> {
            new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" },
            new RoomEntity { RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Vacia" }
            };
            _mockRoomRepository.Setup(repo => repo.GetAll()).ReturnsAsync(rooms);

            var result = await _roomController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomEntity>>(okResult.Value);
            Assert.Equal(rooms.Count, returnedRooms.Count());
        }

        [Fact]
        public async Task GetById_ExistingRoom_ReturnsRoom()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.GetById(roomId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoom = Assert.IsAssignableFrom<RoomEntity>(okResult.Value);
            Assert.Equal(room, returnedRoom);
        }

        [Fact]
        public async Task GetById_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.GetById(roomId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidRoom_ReturnsCreatedAtAction()
        {
            var room = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var result = await _roomController.Insert(room);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(room.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Insert_InvalidRoom_ReturnsBadRequest()
        {
            var room = new RoomEntity { RoomNumber = null };

            var result = await _roomController.Insert(room);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingRoom_ReturnsCreatedAtAction()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Update(roomId, updatedRoom);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(updatedRoom.Id, createdAtActionResult.RouteValues["id"]);
            Assert.NotEqual(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.Update(roomId, updatedRoom);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingRoom_ReturnsBadRequest()
        {
            // URL Id and Room Id must be the same (and not null, obviously)
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = "2", RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Update(roomId, updatedRoom);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingRoom_ReturnsOk()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Delete(roomId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingRoom_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.Delete(roomId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }

    // Users Tests (in progress)
    public class UsersTests
    {
        private readonly Mock<IMongoRepository<RoomEntity>> _mockRoomRepository;
        private readonly RoomServiceController _roomController;

        public UsersTests()
        {
            _mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            _roomController = new RoomServiceController(_mockRoomRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfUsers()
        {
            var rooms = new List<RoomEntity> {
            new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" },
            new RoomEntity { RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Vacia" }
            };
            _mockRoomRepository.Setup(repo => repo.GetAll()).ReturnsAsync(rooms);

            var result = await _roomController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomEntity>>(okResult.Value);
            Assert.Equal(rooms.Count, returnedRooms.Count());
        }

        [Fact]
        public async Task GetById_ExistingUser_ReturnsUser()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.GetById(roomId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoom = Assert.IsAssignableFrom<RoomEntity>(okResult.Value);
            Assert.Equal(room, returnedRoom);
        }

        [Fact]
        public async Task GetById_NonExistingUser_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.GetById(roomId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidUser_ReturnsCreatedAtAction()
        {
            var room = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var result = await _roomController.Insert(room);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(room.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Insert_InvalidUser_ReturnsBadRequest()
        {
            var room = new RoomEntity { RoomNumber = null };

            var result = await _roomController.Insert(room);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingUser_ReturnsCreatedAtAction()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Update(roomId, updatedRoom);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(RoomServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(updatedRoom.Id, createdAtActionResult.RouteValues["id"]);
            Assert.NotEqual(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_NonExistingUser_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            var updatedRoom = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.Update(roomId, updatedRoom);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingUser_ReturnsBadRequest()
        {
            // URL Id and Room Id must be the same (and not null, obviously)
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            var updatedRoom = new RoomEntity { Id = "2", RoomNumber = "2", Floor = "2", Type = "Matrimonio", Status = "Disponible" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Update(roomId, updatedRoom);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingUser_ReturnsOk()
        {
            var roomId = "1";
            var room = new RoomEntity { Id = roomId, RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Ocupada" };
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync(room);

            var result = await _roomController.Delete(roomId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingUser_ReturnsNotFound()
        {
            var roomId = "nonexistent";
            _mockRoomRepository.Setup(repo => repo.GetById(roomId)).ReturnsAsync((RoomEntity)null);

            var result = await _roomController.Delete(roomId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }

    // Tasks Tests
    public class TasksTests
    {
        private readonly Mock<IMongoRepository<TaskEntity>> _mockTaskRepository;
        private readonly Mock<IMongoRepository<RoomEntity>> _mockRoomRepository;

        private readonly TaskServiceController _taskController;

        public TasksTests()
        {
            _mockTaskRepository = new Mock<IMongoRepository<TaskEntity>>();
            _mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            _taskController = new TaskServiceController(
                taskGenericRepository: _mockTaskRepository.Object,
                roomGenericRepository: _mockRoomRepository.Object
            );
        }

        [Fact]
        public async Task GetAll_ReturnsListOfTasks()
        {
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1", Name = "1", Surname = "1", Email = "1", RoleAdmin = false
                },
                    Room = new RoomEntity { RoomNumber = "1" }, Priority = "Baja", Observations = "Cambiar colchón" },
                new TaskEntity { User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1", Name = "1", Surname = "1", Email = "1", RoleAdmin = false
                },
                    Room = new RoomEntity { RoomNumber = "1" }, Priority = "Baja", Observations = "Cambiar colchón" }
            };
            _mockTaskRepository.Setup(repo => repo.GetAll()).ReturnsAsync(tasks);

            var result = await _taskController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskEntity>>(okResult.Value);
            Assert.Equal(tasks.Count, returnedTasks.Count());
        }

        [Fact]
        public async Task GetById_ExistingTask_ReturnsTask()
        {
            var taskId = "1";
            var task = new TaskEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(task);

            var result = await _taskController.GetById(taskId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTask = Assert.IsAssignableFrom<TaskEntity>(okResult.Value);
            Assert.Equal(task, returnedTask);
        }

        [Fact]
        public async Task GetById_NonExistingTask_ReturnsNotFound()
        {
            var taskId = "nonexistent";
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync((TaskEntity)null);

            var result = await _taskController.GetById(taskId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsCreatedAtAction()
        {
            // Mock Room
            var mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            mockRoomRepository.Setup(repo => repo.GetById(It.IsAny<string>())).ReturnsAsync
                (new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" }
                );

            // Mock Controller
            var taskController = new TaskServiceController(
                taskGenericRepository: _mockTaskRepository.Object,
                roomGenericRepository: mockRoomRepository.Object
            );

            var taskId = "1";
            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };
            var task = new TaskEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };

            var result = await taskController.Insert(task);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TaskServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(task.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(task, createdAtActionResult.Value);
        }


        [Fact]
        public async Task Insert_InvalidTask_ReturnsNotFound()
        {
            // Creamos una tarea con una habitación que no existe (ID nulo)
            var task = new TaskEntity { User = null, Room = new RoomEntity { Id = null, RoomNumber = null } };

            // Simulamos el comportamiento del método GetById del repositorio de habitaciones
            _mockRoomRepository.Setup(repo => repo.GetById(null)).ReturnsAsync((RoomEntity)null);

            // Ejecutamos el método Insert del controlador
            var result = await _taskController.Insert(task);

            // Verificamos que se devuelva un NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingTask_ReturnsOkObjectResult()
        {
            var taskId = "1";

            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var task = new TaskEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };

            var updatedUserRoom = new Services.API.Hotel.Core.Dto.RegisteredUserDto
            {
                Id = "1",
                Name = "1",
                Surname = "1",
                Email = "1",
                RoleAdmin = false
            };
            var updatedTaskRoom = new RoomEntity { RoomNumber = "2", Floor = "2", Type = "Individual", Status = "Disponible" };
            var updatedTask = new TaskEntity { Id = taskId, User = updatedUserRoom, Room = updatedTaskRoom, Priority = "Baja", Observations = "Cambiar colchón" };
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(task);

            var result = await _taskController.Update(taskId, updatedTask);

            var okResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedTask = Assert.IsAssignableFrom<TaskEntity>(okResult.Value);
            Assert.Equal(updatedTask, returnedTask);
        }

        [Fact]
        public async Task Update_NonExistingTask_ReturnsNotFound()
        {
            var taskId = "nonexistent";
            var taskUser = new Services.API.Hotel.Core.Dto.RegisteredUserDto
            {
                Id = "1",
                Name = "1",
                Surname = "1",
                Email = "1",
                RoleAdmin = false
            };
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var task = new TaskEntity { Id = taskId, User = taskUser, Room = taskRoom, Priority = "Baja", Observations = "Cambiar colchón" };
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync((TaskEntity)null);

            var result = await _taskController.Update(taskId, task);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ExistingTask_ReturnsBadRequest()
        {
            var taskId = "nonexistent";
            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var task = new TaskEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };

            var updatedUserRoom = new Services.API.Hotel.Core.Dto.RegisteredUserDto
            {
                Id = "1",
                Name = "1",
                Surname = "1",
                Email = "1",
                RoleAdmin = false
            };
            var updatedTaskRoom = new RoomEntity { RoomNumber = "2", Floor = "2", Type = "Individual", Status = "Disponible" };
            var updatedTask = new TaskEntity { User = updatedUserRoom, Room = updatedTaskRoom, Priority = "Baja", Observations = "Cambiar colchón" };

            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(updatedTask);

            var result = await _taskController.Update(taskId, updatedTask);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingTask_ReturnsOk()
        {
            var taskId = "1";
            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var task = new TaskEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(task);

            var result = await _taskController.Delete(taskId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingTask_ReturnsNotFound()
        {
            var taskId = "nonexistent";
            _mockTaskRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync((TaskEntity)null);

            var result = await _taskController.Delete(taskId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
    // Tasks History Tests
    public class TasksHistoryTests
    {
        private readonly Mock<IMongoRepository<TaskHistoryEntity>> _mockTaskHistoryRepository;
        private readonly Mock<IMongoRepository<RoomEntity>> _mockRoomRepository;
        private readonly NotificationHub notificationHub;
        private readonly TaskHistoryServiceController _taskController;

        public TasksHistoryTests()
        {
            _mockTaskHistoryRepository = new Mock<IMongoRepository<TaskHistoryEntity>>();
            _mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            _taskController = new TaskHistoryServiceController(
                taskGenericRepository: _mockTaskHistoryRepository.Object,
                roomGenericRepository: _mockRoomRepository.Object,
                notificationHub: notificationHub
            );
        }

        [Fact]
        public async Task GetAll_ReturnsListOfTasks()
        {
            var tasks = new List<TaskHistoryEntity>
            {
                new TaskHistoryEntity { User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1", Name = "1", Surname = "1", Email = "1", RoleAdmin = false
                },
                    Room = new RoomEntity { RoomNumber = "1" }, Priority = "Baja", Observations = "Cambiar colchón" },
                new TaskHistoryEntity { User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1", Name = "1", Surname = "1", Email = "1", RoleAdmin = false
                },
                    Room = new RoomEntity { RoomNumber = "1" }, Priority = "Baja", Observations = "Cambiar colchón" }
            };
            _mockTaskHistoryRepository.Setup(repo => repo.GetAll()).ReturnsAsync(tasks);

            var result = await _taskController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskHistoryEntity>>(okResult.Value);
            Assert.Equal(tasks.Count, returnedTasks.Count());
        }

        [Fact]
        public async Task GetById_ExistingTask_ReturnsTask()
        {
            var taskId = "1";
            var task = new TaskHistoryEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };
            _mockTaskHistoryRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(task);

            var result = await _taskController.GetById(taskId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTask = Assert.IsAssignableFrom<TaskHistoryEntity>(okResult.Value);
            Assert.Equal(task, returnedTask);
        }

        [Fact]
        public async Task GetById_NonExistingTask_ReturnsNotFound()
        {
            var taskId = "nonexistent";
            _mockTaskHistoryRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync((TaskHistoryEntity)null);

            var result = await _taskController.GetById(taskId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Insert_ValidTask_ReturnsCreatedAtAction()
        {
            // Mock Room
            var mockRoomRepository = new Mock<IMongoRepository<RoomEntity>>();
            mockRoomRepository.Setup(repo => repo.GetById(It.IsAny<string>())).ReturnsAsync
                (new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" }
                );

            // Mock Controller
            var taskController = new TaskHistoryServiceController(
                taskGenericRepository: _mockTaskHistoryRepository.Object,
                roomGenericRepository: mockRoomRepository.Object,
                notificationHub: notificationHub
            );

            var taskId = "1";
            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };
            var task = new TaskHistoryEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };

            var result = await taskController.Insert(task);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TaskServiceController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(task.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(task, createdAtActionResult.Value);
        }


        [Fact]
        public async Task Insert_InvalidTask_ReturnsNotFound()
        {
            // Creamos una tarea con una habitación que no existe (ID nulo)
            var task = new TaskHistoryEntity { User = null, Room = new RoomEntity { Id = null, RoomNumber = null } };

            // Simulamos el comportamiento del método GetById del repositorio de habitaciones
            _mockRoomRepository.Setup(repo => repo.GetById(null)).ReturnsAsync((RoomEntity)null);

            // Ejecutamos el método Insert del controlador
            var result = await _taskController.Insert(task);

            // Verificamos que se devuelva un NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingTask_ReturnsOk()
        {
            var taskId = "1";
            var taskUser = "1";
            var taskRoom = new RoomEntity { RoomNumber = "1", Floor = "1", Type = "Individual", Status = "Disponible" };

            var task = new TaskHistoryEntity
            {
                User = new Services.API.Hotel.Core.Dto.RegisteredUserDto
                {
                    Id = "1",
                    Name = "1",
                    Surname = "1",
                    Email = "1",
                    RoleAdmin = false
                },
                Room = new RoomEntity { RoomNumber = "1" },
                Priority = "Baja",
                Observations = "Cambiar colchón"
            };
            _mockTaskHistoryRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync(task);

            var result = await _taskController.Delete(taskId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingTask_ReturnsNotFound()
        {
            var taskId = "nonexistent";
            _mockTaskHistoryRepository.Setup(repo => repo.GetById(taskId)).ReturnsAsync((TaskHistoryEntity)null);

            var result = await _taskController.Delete(taskId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
    // ErrorHandlerMiddleware Tests
    public class ErrorHandlerMiddlewareTests
    {
        [Fact]
        public async Task Invoke_Returns_404_NotFound_For_Invalid_URL()
        {
            var middleware = new ErrorHandlerMiddleware((innerHttpContext) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Request.Path = "/invalid/path";
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("La URL solicitada no es válida.", await GetResponseBody(context.Response));
        }

        [Fact]
        public async Task Invoke_Returns_405_MethodNotAllowed_For_Invalid_Method()
        {
            var middleware = new ErrorHandlerMiddleware((innerHttpContext) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/RoomService";
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            await middleware.Invoke(context);

            Assert.Equal("Método no permitido para esta URL.", await GetResponseBody(context.Response));
        }

        private async Task<string> GetResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(response.Body))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}