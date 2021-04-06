using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProcessadorMensagens
{
    class Program
    {
        private static IConfiguration _configuration;
        private static IModel channel;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            // Para este exemplo foi criado um container Docker baseado
            // em uma imagem do RabbitMQ. Segue o comando para geração
            // desta estrutura:
            // docker run -d --hostname rabbit-local --name testes-rabbitmq -p 6672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=testes -e RABBITMQ_DEFAULT_PASS=Testes2018! rabbitmq:3-management-alpine
            var rabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                _configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(rabbitMQConfigurations);

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfigurations.HostName,
                Port = rabbitMQConfigurations.Port,
                UserName = rabbitMQConfigurations.UserName,
                Password = rabbitMQConfigurations.Password
            };

            using (var connection = factory.CreateConnection())

            using (channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TestesASPNETCore",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // worker (1)
                var consumer = new EventingBasicConsumer(channel);
                
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queue: "TestesASPNETCore",
                     autoAck: true,
                     consumer: consumer);

                // worker (2)
                var consumer2 = new EventingBasicConsumer(channel);
                consumer2.Received += Consumer_Received;
                channel.BasicConsume(queue: "TestesASPNETCoreExchange",
                     autoAck: true,
                     consumer: consumer2);            
                
                // worker (3)
                var consumer3 = new EventingBasicConsumer(channel);
                consumer3.Received += Consumer_Received_with_ack;

                channel.BasicConsume(queue: "CriticalQueue2",
                     autoAck: false,
                     consumer: consumer3);                                 

                Console.WriteLine("Aguardando mensagens para processamento");
                Console.WriteLine("Pressione uma tecla para encerrar...");
                Console.ReadKey();
            }
        }


        private static void Consumer_Received(
            object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(Environment.NewLine +
                "[Nova mensagem recebida] " + message);
        }

        private static void Consumer_Received_with_ack(
            object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);

            Console.WriteLine(Environment.NewLine +
                "[Nova mensagem recebida] " + message);

            //pretend that message cannot be processed and must be rejected
            if (message.Contains("10"))
                channel.BasicReject(deliveryTag:e.DeliveryTag,false);
            else
                channel.BasicAck(deliveryTag:e.DeliveryTag, multiple:false);
        }
    }
}