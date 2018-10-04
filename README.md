# Sobre o projeto

Este é um projeto sobre um sistema de Log de aplicações. Resumidamente ele inclui:

* Uma API capaz de enviar logs para um serviço de mensageria baseada em filas por requisições REST;
* Um consumidor que recebe mensagens desta fila e as publica em um índice no Elasticsearch;
* Um servidor com Kibana para exibir um dashboard com as 10 aplicações com tempo médio de resposta mais altos.

## Ambiente de implementação

Este projeto foi desenvolvido em uma máquina única, com todos serviços usando "localhost" como endereço básico. Principais tecnologias usadas:

* Windows Subservice For Linux usando Ubuntu 18.04;
* Ansible para automação de criação de infra;
* C# Dotnet para criação de serviços de criação e consumo de mensagens de Log;
* RabbitMQ como sistema de mensageria;
* Elasticsearch + Kibana para armazenar e explorar logs.