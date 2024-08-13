using Final.Models;
using Final.Packages;
using Final.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IPKG_SUBJECTS _package;


        public SubjectsController(IPKG_SUBJECTS package)
        {
            _package = package;

        }
        [HttpPost("add_subject")]
        public async Task<IActionResult> add_subject(string newSubect)
        {

            try
            {

                _package.add_subject(newSubect);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpGet("get_subjects")]
        public IActionResult get_questions()
        {

            return Ok(_package.get_questions());

        }
    }
}
