using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities;
using Services.API.Hotel.Repository;
using System.Text.Json;

namespace Services.API.Hotel.Controllers
{
    // Controller for the Task API
    [Route("api/[controller]")]
    [ApiController]
    public class TaskServiceController : ControllerBase
    {
        private readonly IMongoRepository<TaskEntity> _taskRepository;

        private readonly IMongoRepository<RoomEntity> _roomRepository;

        public TaskServiceController(IMongoRepository<TaskEntity> taskGenericRepository, IMongoRepository<RoomEntity> roomGenericRepository)
        {
            _taskRepository = taskGenericRepository;
            _roomRepository = roomGenericRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetAll()
        {
            return Ok(await _taskRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskEntity>> GetById(string Id)
        {
            var task = await _taskRepository.GetById(Id);
            if (task == null)
            {
                return NotFound("Esta tarea no existe.");
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(TaskEntity task)
        {
            var room = await _roomRepository.GetById(task.Room.Id);
            if (room == null)
            {
                return NotFound("Esta Habitación no existe.");
            }

            task.Room = room;

            // Check if user exists
            var httpClient = new HttpClient();
            string urlUser = "http://security:3000/api/UserService/" + task.User.Id;
            try
            {
                var response = await httpClient.GetAsync(urlUser);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return NotFound(content);
                }
                var jsonString = await response.Content.ReadAsStringAsync();

                var userValid = JsonSerializer.Deserialize<RegisteredUserDto>(jsonString);

                task.User = userValid;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await _taskRepository.InsertDocument(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string Id, TaskEntity newtask)
        {
            var task = await _taskRepository.GetById(Id);
            if (task == null)
            {
                return NotFound("Esta tarea no existe.");
            }

            if (Id != newtask.Id)
            {
                return BadRequest("Los Identificadores no coinciden.");
            }

            var room = await _roomRepository.GetById(newtask.Room.Id);
            if (room == null)
            {
                return NotFound("Esta Habitación no existe.");
            }

            newtask.Room = room;

            // Check if user exists
            var httpClient = new HttpClient();
            string urlUser = "http://security:3000/api/UserService/" + newtask.User.Id;
            try
            {
                var response = await httpClient.GetAsync(urlUser);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return NotFound(content);
                }
                var jsonString = await response.Content.ReadAsStringAsync();

                var userValid = JsonSerializer.Deserialize<RegisteredUserDto>(jsonString);

                newtask.User = userValid;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await _taskRepository.UpdateDocument(newtask);
            return CreatedAtAction(nameof(GetById), new { id = newtask.Id }, newtask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            var task = await _taskRepository.GetById(Id);
            if (task == null)
            {
                return NotFound("Esta tarea no existe.");
            }

            await _taskRepository.DeleteById(Id);
            return Ok("Tarea eliminada correctamente.");
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            await _taskRepository.DeleteAll();
            return Ok("Tareas eliminadas correctamente");
        }

        [HttpPost("pagination")]
        public async Task<ActionResult<PaginationDto<TaskEntity>>> Pagination(PaginationDto<TaskEntity> pagination)
        {
            var resultados = await _taskRepository.PaginationBy(pagination);
            return Ok(resultados);
        }
    }
}
