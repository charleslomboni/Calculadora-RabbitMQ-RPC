using RabbitMQ.Producer.DataContracts.Request;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAPI.Controllers {

    [RoutePrefix("Calculator")]
    public class CalculatorController : ApiController {

        public CalculatorController() {
        }

        [HttpPost]
        [Route("Calculate")]
        public async Task<IHttpActionResult> Calculate([FromBody]CalculateRequest calculateRequest) {
            // Pego a informação do request
            // Envio para a fila
            // Espero o cálculo do consumer
            // Pego a resposta
            // Envio para a tela

            if (ModelState.IsValid == false) {
                return BadRequest("The are invalid parameters.");
            }

            var result = await new RabbitMQ.Producer.RPCCLient().SendToQueue(calculateRequest);
            //new QueueManager().ResetEvent();

            return Ok(result);
        }
    }
}