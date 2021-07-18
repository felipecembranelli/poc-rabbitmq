# Poc-rabbitmq

Demo code using .NET and Rabbitmq basic functions

# Creating local Rabbitmq instance

docker run -d --hostname rabbit-local --name testes-rabbitmq -p 6672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=testes -e RABBITMQ_DEFAULT_PASS=Testes2018! rabbitmq:3-management-alpine
