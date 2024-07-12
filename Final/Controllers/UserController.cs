using Final.Models;
using Final.Packages;
using Final.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IPKG_USERS _package;
        private readonly ITokenService _tokenService;




        public UserController(IPKG_USERS package, ITokenService tokenService)
        {
            _package = package;
            _tokenService = tokenService;
   
        }



        [HttpPost("register_user")]
        public async Task<IActionResult> Register_user( RegisterUser user)
        {

            try
            {

                _package.register_user(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("logIn")]
        public IActionResult Log_in(LogIn user)
        {

            LogInResponse logInResponse = _package.Login_user(user);
            if (logInResponse == null)
            {
                return Unauthorized("Invalid credentials");
            }

           var token = _tokenService.CreateToken(logInResponse.Id, logInResponse.Role);

            return Ok(new
            {
                token,
                logInResponse.Id
            });

        }
        
        
        [HttpGet("get_user_by_id"), Authorize]
        public IActionResult get_user_by_id(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (id.ToString() != userIdClaim)
            {
                return Forbid();
            }


            return Ok(_package.get_user_by_id(id));

        }
        [HttpGet("get_users")]
        public IActionResult get_users()
        {

            return Ok(_package.get_all_user());

        }
        [HttpGet("get_answers_by_full_name")]
        public IActionResult get_answers_by_full_name(int id)
        {
           


            return Ok(_package.get_answers_by_full_name(id));

        }

        [HttpPost("add_question")]
        public async Task<IActionResult> add_question(addQuestion newQuestion)
        {
    
            try
            {

                _package.add_question(newQuestion);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }



        }
        [HttpPut("update_questions")]
        public async Task<IActionResult> update_questions(int id,addQuestion newQuestion)
        {

            try
            {

                _package.update_questions(id, newQuestion);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }



        }
        [HttpPost("parse_json_answers")]
        public async Task<IActionResult> parse_json_answers(List<userForm> userForm)
        {
            try
            {

                _package.parse_json_answers(userForm);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("get_questions")]
        public IActionResult get_questions()
        {

            return Ok(_package.get_questions());

        }


    }
}
