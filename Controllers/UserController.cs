using AutoMapper;
using BackTareas.Data;
using BackTareas.Models;
using BackTareas.Models.DTO;
using BackTareas.Repository;
using BackTareas.Repository.IRepository;
using BackTareas.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Telegram.Bot;

namespace BackTareas.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController:ControllerBase
    {
        private readonly  IUserRepository _userRepository;
        private readonly ApiResponse response;
        private readonly IMapper _mapper;
        private readonly ITelegramBotClient _telegramBotClient;
        public UserController(IUserRepository userRepository, IMapper mapper, ITelegramBotClient telegramBotClient)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _telegramBotClient = telegramBotClient;
            response = new();   
        }

        [HttpGet("get-all-users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllUsers()
        {
            try
            {
                var list = await _userRepository.Getall();

                if (list != null&& list.Any())
                {
                    var listDTO = _mapper.Map<List<UserDTO>>(list);
                    response.Result = listDTO;
                    response.StatusCode = HttpStatusCode.OK;

                    return Ok(response);
                }

                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    "No se encontraron usuarios"
                };

                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    ex.ToString()
                };

                return  response;
            }
        }

        [HttpGet("{id:int}",Name ="get-user-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetUserById([FromRoute]int id)
        {
            try
            {
                if(id < 0)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages = new List<string>
                    {
                        "Id no encontrado o incorrecto"
                    };
                }

                var user = await _userRepository.GetById(id);

                if (user==null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages = new List<string>
                    {
                        "Id no encontrado o incorrecto"
                    };
                }

                var userDTO = _mapper.Map<UserWithWorksDTO>(user);

                response.StatusCode = HttpStatusCode.OK;
                response.Result = userDTO;

                return Ok(response);
            }
            catch (Exception)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    "Algo ocurrio"
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("register-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Register([FromBody]UserCreationDTO userDTO)
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

                var verificationEmail = await _userRepository.GetByEmail(userDTO.Email);

                if(verificationEmail != null) 
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages = new List<string>
                    {
                        "Correo ya registrado con anterioridad"
                    };

                    return BadRequest(response);
                }
                var userNew = _mapper.Map<User>(userDTO);

                var createdUser = await _userRepository.Create(userNew);

                response.Result = createdUser;
                response.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("get-user-by-id", new { id = createdUser.Id }, response);

            }
            catch (Exception)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string>
                {
                    "Algo ocurrio"
                };

                return StatusCode(500, response);
            }

        }

        [HttpPost("login-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Login([FromBody]UserLoginDTO userDTO)
        {
            try
            {
                if(userDTO == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>
                    {
                        "El usuario y contraseña son necesarios"
                    };

                    return BadRequest(response);
                }

                var user = await _userRepository.GetUserByEmailAndPasswordAsync(userDTO.Email, userDTO.Password);

                if(user == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages = new List<string>
                    {
                        "Usuario y/o contraseña incorrectos"
                    };

                    return NotFound(response);
                }
                var userLogin = _mapper.Map<User>(userDTO);

                response.StatusCode = HttpStatusCode.OK;
                response.Result = await _userRepository.Login(userLogin);

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

                return StatusCode(500,response);

            }

        }


    }
}
