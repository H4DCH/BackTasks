using AutoMapper;
using AutoMapper.Configuration.Annotations;
using BackTareas.Models;
using BackTareas.Models.DTO;
using BackTareas.Repository.IRepository;
using BackTareas.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net;

namespace BackTareas.Controllers
{
    [ApiController]
    [Route("api/work")]
    public class WorkController : ControllerBase
    {
        private readonly IWorkRepository _workRepository;
        private readonly IUserReminderRepository _userReminderRepository;
        private readonly ReminderService _reminderRepository;
        private readonly IMapper _mapper;
        private readonly ApiResponse response;
        public WorkController(IWorkRepository workRepository, IMapper mapper, IUserReminderRepository userReminderRepository,
            ReminderService reminderRepository)
        {
            _workRepository = workRepository;
            _mapper = mapper;
            _userReminderRepository = userReminderRepository;
            response = new();
            _reminderRepository = reminderRepository;
        }

        [HttpGet("get-all-works")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllWorks()
        {
            try
            {
                var works = await _workRepository.Getall();
                if (works == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>
                    {
                        "No se encontraron tareas"
                    };
                    return NotFound(response);
                }

                var workDTOs = _mapper.Map<List<WorkDTO>>(works);

                response.StatusCode = HttpStatusCode.OK;
                response.Result = workDTOs;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return StatusCode(500, response);
            }
        }
        [HttpGet("{id:int}",Name ="get-work-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetWorkById([FromRoute]int id)
        {
            try
            {
                if (id <= 0)
                {
                    response.ErrorMessages = new List<string> { "El id debe ser mayor a cero" };
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var work = await _workRepository.GetById(id);

                if (work == null)
                {
                    response.ErrorMessages = new List<string> { $"El id: {id} no se encontro" };
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
                var workDTO = _mapper.Map<WorkWithUser>(work);

                response.Result = workDTO;
                response.StatusCode = HttpStatusCode.OK;

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return StatusCode(500, response);
            }

        }

        [HttpPost("create-work")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateWork([FromBody] WorkCreationDTO creationDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(response);
                }

                var workNew = _mapper.Map<Work>(creationDTO);

                workNew.Status = WorkState.Pendiente;

                var chatId =await _userReminderRepository.getUserChat(workNew.UserId);
                

                var createWork = await _workRepository.Create(workNew);

                if (createWork != null)
                {
                    await _reminderRepository.ScheduleReminder(chatId.ChatId, $"Hola {chatId.Name}, recuerda esto:{createWork.Title}", createWork.DateTime);
                    response.StatusCode = HttpStatusCode.Created;
                    response.Result = createWork;

                    return CreatedAtRoute("get-work-by-id", new { id = createWork.Id }, response);
                }

                response.ErrorMessages = new List<string> { "Algo ocurrio al crear el trabajo" };
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;

                return response;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return StatusCode(500, response);
            }

        }
        [HttpDelete("{id:int}", Name = "delete-work")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ApiResponse>> DeleteWork([FromRoute] int id)
        {
            try
            {
                if (id <= 0)
                {
                    response.ErrorMessages = new List<string> { "El id debe ser mayoy a 0" };
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(response);
                }

                var exist = await _workRepository.GetById(id);

                if (exist == null)
                {
                    response.ErrorMessages = new List<string> { "No se encontro algun trabajo" };
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(response);
                }

                await _workRepository.Delete(id);
                response.StatusCode = HttpStatusCode.NoContent;

                return Ok(response);
                
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return StatusCode(500, response);
            }
        }

        [HttpPut("{id:int}",Name ="update-work")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateWork([FromRoute]int id, [FromBody] WorkUpdateDTO workUpdate)
        {
            try
            {
            if (!ModelState.IsValid || id<=0)
            {
                response.ErrorMessages = new List<string> { "Todos los campos son necesarios y/o el id es invalido" };
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;

                return BadRequest(response);
            }

            var model = await _workRepository.GetById(id);

            if (model == null)
            {
                response.ErrorMessages = new List<string> { "No se encontraron registros" };
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;

                return NotFound(response);
            }

                _mapper.Map(workUpdate,model);

                var modelUpdate = _workRepository.Update(model);
                response.StatusCode = HttpStatusCode.OK;
                response.Result = _mapper.Map<WorkDTO>(model);

                 return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return StatusCode(500, response);
            }


        }

    }
}
