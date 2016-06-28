namespace RabbitMQ.Producer.DataContracts.Response {

    public class CalculateResponse {

        public CalculateResponse() {
        }

        // Resultado do cálculo
        public int Result { get; set; }

        public string Error { get; set; }
    }
}