using System;

namespace RabbitMQ.Consumer {

    internal class RPCServer {

        private static void Main(string[] args) {
            Receive.Consumer();
            Console.Read();
        }
    }
}