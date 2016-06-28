namespace RabbitMQ.Consumer.Interface {

    internal interface ICalculate {

        // Operações matemáticas
        int Addition(int valorX, int valorY);

        int Subtraction(int valorX, int valorY);

        int Multiplication(int valorX, int valorY);

        int Division(int valorX, int valorY);
    }
}