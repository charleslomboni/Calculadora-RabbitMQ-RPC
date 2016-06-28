using System.Web.Http;
using WebAPI.DataContracts.Request;
using WebAPI.Queue;

namespace WebAPI.Controllers {

    [RoutePrefix("Calculator")]
    public class CalculatorController : ApiController {

        public CalculatorController() {
        }

        [HttpPost]
        [Route("Calculate")]
        public IHttpActionResult Calculate([FromBody]CalculateRequest calculateRequest) {
            // Pego a informação do request
            // Envio para a fila
            // Espero o cálculo do consumer
            // Pego a resposta
            // Envio para a tela

            if (ModelState.IsValid == false) {
                return BadRequest("The are invalid parameters.");
            }

            var result = QueueManager.SendToQueue(calculateRequest);

            return Ok(result);
        }
    }
}