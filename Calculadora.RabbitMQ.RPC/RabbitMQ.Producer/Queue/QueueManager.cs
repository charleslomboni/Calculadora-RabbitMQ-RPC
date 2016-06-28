using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Producer.DataContracts.Request;
using RabbitMQ.Producer.DataContracts.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Producer.Queue {

    public class QueueManager {

        // used to pass messages back to UI for processing
        public delegate void OnReceiveMessage(byte[] message, int size, IBasicProperties properties);

        public event OnReceiveMessage OnMessageReceived;

        // Passar para o projeto do producer depois
        // E refatorar o código
        public async Task<CalculateResponse> SendToQueue(CalculateRequest calculateRequest) {
            var resultResponse = new CalculateResponse();

            var taskCompletionSource = new TaskCompletionSource<bool>();

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

                    // Fair Dispatch
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

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

                    // =========================================== CONSUMER

                    //await Task.Factory.StartNew(() => {
                    // Configura as propriedades do model
                    var consumer = new EventingBasicConsumer(channel);

                    // Consumo básico com reconhecimento de mensagem
                    channel.BasicConsume(queue: replyToQueueName,
                                        noAck: true,
                                        consumer: consumer);

                    //while (true) {
                    //var ea = consumer.Queue.Dequeue();
                    consumer.Received += (model, ea) => {
                        OnMessageReceived?.Invoke(ea.Body, ea.Body.Length, ea.BasicProperties);

                        // Se a fila (com GUID) que criei, for igual a que o consumer respondeu
                        //if (ea.BasicProperties.ReplyTo == replyToQueueName) {
                        // Msg de retorno
                        var eaBody = Encoding.UTF8.GetString(ea.Body);

                        // Se deu algum erro no Cálculo
                        if (eaBody.ToLower().Contains("wrong")) {
                            resultResponse.Error = eaBody;
                        } else {
                            resultResponse.Result = Convert.ToInt32(eaBody);
                            taskCompletionSource.SetResult(true);
                        }
                        //return result;
                        //}
                    };
                    //});// task

                    //}
                }
            }
            await taskCompletionSource.Task;
            return resultResponse;
        }
    }
}