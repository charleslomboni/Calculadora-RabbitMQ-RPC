# Calculadora-RabbitMQ-RPC


Exemplo de calculadora de operações básicas como: 
- Adição, subtração, multiplicação e divisão, usando webapi e rabbitmq com rpc.


PS .: Exemplo precisa ser refatorado, código de exemplo apenas para estudo! :smile:


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
