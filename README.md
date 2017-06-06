# Calculadora-RabbitMQ-RPC

<strong>Master branch</strong><br />
[![Build status](https://ci.appveyor.com/api/projects/status/k8se4np19v25sme7/branch/master?svg=true)](https://ci.appveyor.com/project/charleslomboni/calculadora-rabbitmq-rpc/branch/master)

Exemplo de calculadora de operações básicas como: 
- Adição, subtração, multiplicação e divisão, usando webapi e rabbitmq com rpc.


**PS¹ .: Exemplo precisa ser refatorado, código de exemplo apenas para estudo!** :smile:

**PS² .: Caso não saiba mexer com o RabbitMQ, aqui tem um [EXEMPLO](https://github.com/charleslomboni/Exemplos-RabbitMQ) bem explicativo que fiz!** :blush:


### Chamando a API
Use o [postman](https://www.getpostman.com/) ou outro client para enviar a chamada para a API pela URL:
```
http://localhost:[porta]/Calculator/Calculate
```

Passando os valores:
```
{
  "ValorX": 6,
  "ValorY": 2,
  "Operation": "/"
}
```

Onde..
- ValorX e ValorY são os valores para o cálculo
- Operation são operações do tipo: +, -, *, /


### Resultado esperado
```
{
  "Result": 3,
  "Error": null
}
```

### Resultado com erro
```
{
  "Result": 0,
  "Error": "Wrong operation."
}
```

### Como funciona?
O client chama a api passando o request (valorX, valorY e Operation), a api envia para a fila e fica aguardando a resposta, o consumer fica esperando chegar a resposta na fila (no nosso caso: input_api). Após pegar os valores passados, ele faz o cálculo e retorna para uma fila especificada pela api (no campo replyTo), a api que já estava aguardando alguma mensagem nessa fila, recebe a mensagem e mostra para o client que a chamou, e fim! Todos felizes! :joy:
