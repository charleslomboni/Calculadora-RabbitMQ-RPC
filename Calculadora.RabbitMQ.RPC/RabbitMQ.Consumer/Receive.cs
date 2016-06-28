using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Consumer {

    internal static class Receive {

        // Pega da fila
        // Faz o cálculo
        // Devolve para a API
        public static void Consumer() {
            // Host
            var factory = new ConnectionFactory() { HostName = "localhost" };

            // Criando conexão específica para o endpoint
            using (var connection = factory.CreateConnection()) {
                // Abrindo um canal e criando a fila
                using (var channel = connection.CreateModel()) {
                    // Declarando a fila
                    channel.QueueDeclare(queue: "input_api",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    // Fair Dispatch
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    // Criando um consumer
                    //var consumer = new QueueingBasicConsumer(channel);
                    var consumer = new EventingBasicConsumer(channel);

                    // Inicia um consumer básico
                    channel.BasicConsume(queue: "input_api",
                        noAck: false,
                        consumer: consumer);

                    // Retorno da msg
                    var response = string.Empty;

                    Console.WriteLine("[x] INICIANDO EVENTO..");

                    // Cria um evento que será disparado quando tiver algum item para ser recebido
                    consumer.Received += (model, ea) => {
                        // Pega a mensagem pela rota
                        var body = ea.Body;

                        // Propriedades básicas
                        var props = ea.BasicProperties;

                        // Para vou enviar
                        var replyPros = channel.CreateBasicProperties();
                        replyPros.ReplyTo = props.ReplyTo;

                        try {
                            Console.WriteLine("[x] TRY..");
                            // Recupera a msg
                            var operation = Encoding.UTF8.GetString(body);
                            var valorX = 0;
                            var valorY = 0;

                            ValidateHeader(props.Headers, ref valorX, ref valorY);
                            // Faz o cálculo
                            // Fazer o cálculo aqui
                            response = Calculate(valorX, valorY, operation);
                            // Acertar aqui
                            // operation + valor1 e valor2
                        } catch (Exception ex) {
                            Console.WriteLine("Some error was occured on Consumer.. " + ex.Message);
                            response = string.Empty;
                        } finally {
                            Console.WriteLine("[x] DEVOLVENDO MSG {0}..", response);
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            // Publica na fila
                            channel.BasicPublish(exchange: "",
                                routingKey: props.ReplyTo,
                                basicProperties: replyPros,
                                body: responseBytes);

                            // Reconhecimento de msg
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            //return 0;
                        }

                        #region [ Usando com QueueingBasicConsumer (que é obsoleto) ]

                        //while (true) {
                        //    // Cria um evento que será disparado quando tiver algum item para ser recebido
                        //    //consumer.Received += (model, ea) => {
                        //    var ea = consumer.Queue.Dequeue();

                        //    // Pega a mensagem pela rota
                        //    var body = ea.Body;

                        //    // Propriedades básicas
                        //    var props = ea.BasicProperties;

                        //    // Para vou enviar
                        //    var replyPros = channel.CreateBasicProperties();
                        //    replyPros.ReplyTo = props.ReplyTo;

                        //    try {
                        //        Console.WriteLine("[x] TRY..");
                        //        // Recupera a msg
                        //        var operation = Encoding.UTF8.GetString(body);
                        //        var valorX = 0;
                        //        var valorY = 0;

                        //        ValidateHeader(props.Headers, ref valorX, ref valorY);
                        //        // Faz o cálculo
                        //        // Fazer o cálculo aqui
                        //        response = Calculate(valorX, valorY, operation);
                        //        // Acertar aqui
                        //        // operation + valor1 e valor2
                        //    } catch (Exception ex) {
                        //        throw new Exception(
                        //            "Some error was occured on Consumer.. " + ex.Message);
                        //    } finally {
                        //        Console.WriteLine("[x] PUBLICANDO MSG..");
                        //        var responseBytes = Encoding.UTF8.GetBytes(response);
                        //        // Publica na fila
                        //        channel.BasicPublish(exchange: "",
                        //            routingKey: props.ReplyTo,
                        //            basicProperties: replyPros,
                        //            body: responseBytes);

                        //        // Reconhecimento de msg
                        //        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        //        Console.WriteLine("[x] SAIU..");
                        //        //return 0;
                        //    }
                        //}

                        #endregion [ Usando com QueueingBasicConsumer (que é obsoleto) ]
                    }; // Event

                    Console.WriteLine("[x] PRESSIONE [ENTER] PARA SAIR..");
                    Console.Read();
                }
            }
        }

        //return
        //}

        private static string Calculate(int valorX, int valorY, string operation) {
            var result = string.Empty;
            switch (operation) {
                case "+":
                    result = new Calculate().Addition(valorX, valorY).ToString();
                    break;

                case "-":
                    result = new Calculate().Subtraction(valorX, valorY).ToString();
                    break;

                case "*":
                    result = new Calculate().Multiplication(valorX, valorY).ToString();
                    break;

                case "/":
                    result = new Calculate().Division(valorX, valorY).ToString();
                    break;

                default:
                    result = "Wrong operation.";
                    break;
            }

            return result;
        }

        private static void ValidateHeader(IDictionary<string, object> header, ref int valorX, ref int valorY) {
            foreach (KeyValuePair<string, object> item in header) {
                Console.WriteLine("Item.Key: {0} - Item.Value: {1}", item.Key, item.Value);
                valorX = Convert.ToInt32(item.Key);
                valorY = Convert.ToInt32(item.Value);
            }
        }
    }
}