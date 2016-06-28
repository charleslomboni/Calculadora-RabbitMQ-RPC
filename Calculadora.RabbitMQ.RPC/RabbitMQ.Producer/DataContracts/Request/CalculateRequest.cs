namespace RabbitMQ.Producer.DataContracts.Request {

    public class CalculateRequest {

        // Valores para as operações matemáticas
        public int ValorX { get; set; }

        public int ValorY { get; set; }

        // Operação
        // +, -, *, /
        public char Operation { get; set; }
    }
}