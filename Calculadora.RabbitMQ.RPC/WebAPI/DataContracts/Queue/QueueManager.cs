using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.DataContracts.Request;
using WebAPI.DataContracts.Response;

namespace WebAPI.DataContracts.Queue {

    public static class QueueManager {

        // Passar para o projeto do producer depois
        // E refatorar o código
        public static CalculateResponse SendToQueue(CalculateRequest calculateRequest) {
            var result = new CalculateResponse();

            // Host
            var factory = new ConnectionFactory() { HostName = "localhost" };

            // Cria uma conxão específica para o endpoint
            using (var connection = factory.CreateConnection()) {
                // Abre um canal e cria a fila
                using (var channel = connection.CreateModel()) {
                    // Nome da fila temporária
                    var queueName = Guid.NewGuid().ToString();
                    var replyToQueueName = channel.QueueDeclare(queue: queueName,
                                                                durable: false,
                                                                exclusive: false,
                                                                autoDelete: true,
                                                                arguments: null).QueueName;

                    // Configura as propriedades do model
                    var consumer = new QueueingBasicConsumer(channel);

                    // Consumo básico com reconhecimento de mensagem
                    channel.BasicConsume(queue: replyToQueueName,
                                        noAck: true,
                                        consumer: consumer);

                    // Preparando o envio
                    var props = channel.CreateBasicProperties();
                    props.ReplyTo = replyToQueueName;
                    //props.CorrelationId = correlationId;

                    var headerDictionary = new Dictionary<string, object>();
                    headerDictionary.Add(calculateRequest.ValorX.ToString(), calculateRequest.ValorY);

                    props.Headers = headerDictionary;                         // operação e valor

                    // Mensage a ser passada
                    var messageBytes = Encoding.UTF8.GetBytes(calculateRequest.Operation.ToString());

                    // Publicando msg
                    channel.BasicPublish(exchange: "",
                                        routingKey: "input_api",
                                        basicProperties: props,
                                        body: messageBytes);

                    while (true) {
                        var ea = consumer.Queue.Dequeue();
                        // Se a fila (com GUID) que criei, for igual a que o consumer respondeu
                        if (ea.BasicProperties.ReplyTo == replyToQueueName) {
                            result.Result = Convert.ToInt32(Encoding.UTF8.GetString(ea.Body));
                            return result;
                        }
                    }
                }
            }
        }
    }
}