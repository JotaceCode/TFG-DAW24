using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.API.Hotel.Core.Dto;
using Services.API.Hotel.Core.Entities;
using Services.API.Hotel.Repository;
using System.Threading.Tasks;

namespace Services.API.Hotel.Controllers
{
    // Controller for the Room API
    [Route("api/[controller]")]
    [ApiController]
    public class RoomServiceController : ControllerBase
    {
        private readonly IMongoRepository<RoomEntity> _roomRepository;

        public RoomServiceController(IMongoRepository<RoomEntity> roomGenericRepository)
        {
            _roomRepository = roomGenericRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomEntity>>> GetAll()
        {
            return Ok(await _roomRepository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomEntity>> GetById(string Id)
        {
            var room = await _roomRepository.GetById(Id);
            if (room == null)
            {
                return NotFound("Esta habitación no existe.");
            }

            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(RoomEntity room)
        {
            if (string.IsNullOrEmpty(room.RoomNumber))
            {
                return BadRequest("'RoomNumber' es requerido, no puede ir vacio.");
            }

            await _roomRepository.InsertDocument(room);
            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string Id, RoomEntity newRoom)
        {
            var room = await _roomRepository.GetById(Id);
            if (room == null)
            {
                return NotFound("Esta habitación no existe.");
            }

            if (Id != newRoom.Id)
            {
                return BadRequest("Los Identificadores no coinciden.");
            }

            if (room.Id != newRoom.Id)
            {
                return BadRequest("El id de la habitación no se puede modificar");
            }

            await _roomRepository.UpdateDocument(newRoom);
            return CreatedAtAction(nameof(GetById), new { id = newRoom.Id }, newRoom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            var room = await _roomRepository.GetById(Id);
            if (room == null)
            {
                return NotFound("Esta habitación no existe.");
            }

            await _roomRepository.DeleteById(Id);
            return Ok("Habitación eliminada correctamente.");
        }

        [HttpPost("pagination")]
        public async Task<ActionResult<PaginationDto<RoomEntity>>> Pagination(PaginationDto<RoomEntity> pagination)
        {
            var resultados = await _roomRepository.PaginationBy(pagination);
            return Ok(resultados);
        }
    }
}
