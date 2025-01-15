using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using APIMensagens.Models;
using System.Collections;
using System.Collections.Generic;

namespace APIMensagens.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensagensController : ControllerBase
    {
        private static Contador _CONTADOR = new Contador();

        [HttpGet]
        public object Get()
        {
            return new
            {
                QtdMensagensEnviadas = _CONTADOR.ValorAtual
            };
        }

        [HttpPost("BasicQueue")]
        public object BasicQueue(
            [FromServices]RabbitMQConfigurations configurations,
            [FromBody]MessageContent conteudo)
        {
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();

                var factory = new ConnectionFactory()
                {
                    HostName = configurations.HostName,
                    Port = configurations.Port,
                    UserName = configurations.UserName,
                    Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TestesASPNETCore",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Conteúdo da Mensagem: {conteudo.Message}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "TestesASPNETCore",
                                         basicProperties: null,
                                         body: body);

                }

                return new
                {
                    Resultado = "Mensagem encaminhada com sucesso"
                };
            }
        }

        [HttpPost("ExchangeQueue")]
        public object ExchangeQueue(
            [FromServices]RabbitMQConfigurations configurations,
            [FromBody]MessageContent conteudo)
        {
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();

                var factory = new ConnectionFactory()
                {
                    HostName = configurations.HostName,
                    Port = configurations.Port,
                    UserName = configurations.UserName,
                    Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    string exchangeName = "exchange1";
                    string queueName = "TestesASPNETCoreExchange";
                    string routingKey = "routing1";

                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                    //channel.QueueDeclare(queueName, false, false, false, null);
                   

                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.QueueBind(queueName, exchangeName,routingKey, null);                                         

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Conteúdo da Mensagem: {conteudo.Message}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);

                }

                return new
                {
                    Resultado = "Mensagem encaminhada com sucesso"
                };
            }
        }
    
    [HttpPost("ExchangeTopicQueue")]
        public object ExchangeTopicQueue(
            [FromServices]RabbitMQConfigurations configurations,
            [FromBody]MessageContent conteudo)
        {
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();

                var factory = new ConnectionFactory()
                {
                    HostName = configurations.HostName,
                    Port = configurations.Port,
                    UserName = configurations.UserName,
                    Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    string exchangeName = "exchangeSeverityTopic";

                    // routingkey = <source>.<severity>

                    // Queue (1) : critical messages
                    string queueName = "CriticalQueue2";
                    string routingKey = "*.critical";

                    channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                    Dictionary<string,object> args = new Dictionary<string, object>();


                    args.Add("x-dead-letter-exchange", "my-dead-letter-exchange");


                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: args);

                    channel.QueueBind(queueName, exchangeName,routingKey, null);                                         

                    // Queue (2) : warning messages
                    string queueName2 = "WarningQueue";
                    string routingKey2 = "*.warning";

                    channel.QueueDeclare(queue: queueName2,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.QueueBind(queueName2, exchangeName,routingKey2, null);                                         

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Conteúdo da Mensagem: {conteudo.Message} - " + 
                        $"Source: {conteudo.Source} - " + 
                        $"Severidade da Mensagem: {conteudo.Severity}";

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: conteudo.Source + "." + conteudo.Severity,
                                         basicProperties: null,
                                         body: body);

                }

                return new
                {
                    Resultado = "Mensagem encaminhada com sucesso"
                };
            }
        }



        [HttpPost("DeadLetterTest")]
        public object DeadLetterTest(
           [FromServices] RabbitMQConfigurations configurations,
           [FromBody] MessageContent conteudo)
        {
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();

                var factory = new ConnectionFactory()
                {
                    HostName = configurations.HostName,
                    Port = configurations.Port,
                    UserName = configurations.UserName,
                    Password = configurations.Password
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //channel.QueueDeclare(queue: "TestesASPNETCore",
                    //                     durable: false,
                    //                     exclusive: false,
                    //                     autoDelete: false,
                    //                     arguments: null);

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Conteúdo da Mensagem: {conteudo.Message}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "amq.direct",
                                         routingKey: "demo-queue",
                                         basicProperties: null,
                                         body: body);

                }

                return new
                {
                    Resultado = "Mensagem encaminhada com sucesso"
                };
            }


        }

    
    }
}