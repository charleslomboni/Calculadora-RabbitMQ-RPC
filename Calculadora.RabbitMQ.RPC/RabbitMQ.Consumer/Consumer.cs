using System;

namespace RabbitMQ.Consumer {

    internal class Consumer {

        private static void Main(string[] args) {
            Receive.Consumer();
            Console.Read();
        }
    }
}