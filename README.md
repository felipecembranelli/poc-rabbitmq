# Poc-rabbitmq

Demo code using .NET and Rabbitmq basic functions

# Creating local Rabbitmq instance

docker run -d --hostname rabbit-local --name testes-rabbitmq -p 6672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=testes -e RABBITMQ_DEFAULT_PASS=Testes2018! rabbitmq:3-management-alpine

# Run Message Loader

```
dotnet run messageloader
```

# Run Producer
```
dotnet run exchangequeue 100
```

```
dotnet run basicqueue 10
```

```
dotnet run exchangetopicqueue 10
```

# Run Consumer

```
dotnet run consumerexchangequeue
```

# Reference

https://medium.com/nerd-for-tech/dead-letter-exchanges-at-rabbitmq-net-core-b6348122460d

https://www.rabbitmq.com/docs/dlx#using-optional-queue-arguments

