using RabbitMQ.Consumer.Interface;

namespace RabbitMQ.Consumer {

    internal class Calculate : ICalculate {

        public int Addition(int valorX, int valorY) {
            return valorX + valorY;
        }

        public int Subtraction(int valorX, int valorY) {
            return valorX - valorY;
        }

        public int Multiplication(int valorX, int valorY) {
            return valorX * valorY;
        }

        public int Division(int valorX, int valorY) {
            return valorX / valorY;
        }
    }
}