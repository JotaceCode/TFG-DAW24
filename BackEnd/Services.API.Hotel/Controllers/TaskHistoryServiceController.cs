using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.API.Hotel.Core;
using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities;
using Services.API.Hotel.Repository;

namespace Services.API.Hotel.Controllers
{
    // Controller for the Task API
    [Route("api/[controller]")]
    [ApiController]
    public class TaskHistoryServiceController : ControllerBase
    {
        private readonly IMongoRepository<TaskHistoryEntity> _taskRepository;

        private readonly IMongoRepository<RoomEntity> _roomRepository;


        public TaskHistoryServiceController(IMongoRepository<TaskHistoryEntity> taskGenericRepository, IMongoRepository<RoomEntity> roomGenericRepository)
        {
            _taskRepository = taskGenericRepository;
            _roomRepository = roomGenericRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskHistoryEntity>>> GetAll()
        {
            return Ok(await _taskRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskHistoryEntity>> GetById(string Id)
        {
            var task = await _taskRepository.GetById(Id);
            if (task == null)
            {
                return NotFound("Esta tarea no existe.");
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(TaskHistoryEntity task)
        {
            await _taskRepository.InsertDocument(task);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
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
            return Ok("Tarea eliminada correctamente del histórico.");
        }

        [HttpDelete("deleteHistory")]
        public async Task<IActionResult> DeleteHistory()
        {
            await _taskRepository.DeleteAll();
            return Ok("Histórico vaciado correctamente");
        }

        [HttpPost("pagination")]
        public async Task<ActionResult<PaginationDto<TaskHistoryEntity>>> Pagination(PaginationDto<TaskHistoryEntity> pagination)
        {
            var resultados = await _taskRepository.PaginationBy(pagination);
            return Ok(resultados);
        }
    }
}
