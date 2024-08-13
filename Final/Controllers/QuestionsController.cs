using Final.Models;
using Final.Packages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IPKG_QUESTIONS _package;


        public QuestionsController(IPKG_QUESTIONS package)
        {
            _package = package;

        }
        [HttpPost("update_question")]
        public async Task<IActionResult> update_question(int id, List<questionJSONmodel> question)
        {

            try
            {
                _package.update_question(id, question);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }



        }
        [HttpGet("read_questions")]
        public IActionResult read_questions(int subjectID)
        {

            return Ok(_package.read_questions(subjectID));

        }
        [HttpPost("parse_json_question")]
        public async Task<IActionResult> parse_json_question(List<questionJSONmodel> question)
        {
            try
            {


                return Ok(_package.parse_json_question(question));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
