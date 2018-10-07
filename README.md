# Sobre o projeto

Este é um projeto sobre um sistema de Log de aplicações. Resumidamente ele inclui:

* Uma API capaz de enviar logs para um serviço de mensageria baseada em filas por requisições REST;
* Um consumidor que recebe mensagens desta fila e as publica em um índice no Elasticsearch;
* Um servidor com Kibana para exibir um dashboard com as 10 aplicações com tempo médio de resposta mais altos.

## Ambiente de implementação

Este projeto foi desenvolvido em uma máquina única, com todos serviços usando "localhost" como endereço básico. Principais tecnologias usadas:

* Windows Subservice For Linux usando Ubuntu 18.04 (provavelmente deve funcionar em um Ubuntu real sem problemas);
* [Ansible](https://www.ansible.com) para automação de criação de infra;
* C# e Dotnet framework para criação de serviços de criação e consumo de mensagens de Log;
* RabbitMQ como sistema de mensageria;
* Elasticsearch + Kibana para armazenar e explorar logs.

## Criação de Infra e execução de serviços

Criei diferentes playbooks do Ansible para automatizar a instalação das ferramentas necessárias para rodar este projeto. Para os usar você precisará de:

* Ansible instalado na máquina que rodará os comandos;
* Python 2.x instalado na máquina alvo.

Se for instalar a infra em uma máquina remota, altere o arquivo /Ansible/hosts.ini para usar o IP que você quiser. Depois rode o seguinte comando (os dois último parâmetros só são necessários se a máquina alvo é remota, como uma máquina na AWS):

```
ansible-playbook /repoPath/Ansible/playbooks/hello-world.yml -i /repoPath/Ansible/hosts.ini -v --ask-become-pass -u ubuntu --private-key=~/.ssh/RabbitMqChallenge.pem
```

Caso funcione, execute o mesmo comando, mas referenciando o seguinte playbook que inclui todos os arquivos necessários:

```
ansible-playbook /repoPath/Ansible/playbooks/install-all.yml -i /repoPath/Ansible/hosts.ini -v --ask-become-pass -u ubuntu --private-key=~/.ssh/RabbitMqChallenge.pem
```

Após isto, tudo deve estar instalado e os três serviços de dados rodando:

* RabbitMQ:
** Comandos de controle: "sudo rabbitmq-server" e "sudo rabbitmqctl stop"
* Kibana;
** Comandos de controle: "sudo -i service kibana start" e "sudo -i service kibana stop"
* Elasticsearch
** Comandos de controle: "sudo -i service elasticsearch start e sudo -i service elasticsearch stop"

Agora você pode iniciar os serviços de consumo e publicação. Para isto, na máquina alvo:

```
cd ~/rabbitMqChallenge
dotnet clean
dotnet restore
dotnet build
```

Este comandos garantirão que os projetos foram compilados com sucesso, a próxima seção falará mais sobre eles.



## Descrição de projetos

O projeto está dividido em 4 principais projetos, todos implementados usando o framework DotNet com C#. Cada um tem um projeto relacionado para execução de testes unitários. Caso queira executar este testes, basta entrar na pasta deles e rodar o comando de testes, por exemplo:

```
cd ConsumerToDb.Tests
dotnet test
...
```

![Alt text](imgs/test.jpg?raw=true "Resultados de execução de testes com sucesso, incluindo tratamento de erros.")

Foto de exemplo aqui

Os serviços de publicação e consumo de mensagens de Logs são desacoplados entre si, usando apenas o Broker de mensagens do RabbitMQ para se comunicarem entre si, assim eles continuam funcionando se um deles forem encerrados, mas o fluxo de mensagem se interroperá. Além disto, eles já criam as estruturas de dados necessárias nos bancos de dados para seu uso (como novos índices ou filas). Para executá-los basta rodar o comando "dotnet run" da pasta "ProducerRestApi" e "ConsumerToDb".

### ProducerRestApi

Um serviço REST capaz de receber uma requisição POST com uma lista de Logs para uma fila de mensagens. Ele possui duas URLs principais:

* http://localhost:5000/api/logs/50 - Gera um pedido para criar 50 mensagens de Logs com conteúdo aleatório. Nelas os processos são indexados de 1 a 20, onde quanto maior o número, maior o tempo de resposta gerado;

* http://localhost:5000/api/logs - Gera um pedido para criar uma lista baseada no conteúdo de um JSON enviado no corpo da requisição POST, entradas inválidas são ignoradas e indicadas no resultado da requisição.

PS: Adicione no header da requisição (Content-Type: application/json) antes de usar este serviço.

![Alt text](imgs/producer.jpg?raw=true "Serviço rodando após receber requisição para gerar 100 mensagens de log.")

### ConsumerToDb

Um serviço que fica escutando a lista que recebe as mensagens gerados pelo ProducerRestApi, reagindo a cada mensagem escrita lá. Ele checa a integridade de cada mensagem de log e publica somente as entradas válidas no Elasticsearch.

![Alt text](imgs/consumer.jpg?raw=true "Consumidor rodando após receber notificação de um conjunto de novas mensagens de log.")

### JsonHelper

Poderia usar algum sistema de serialização de dados como o [Avro](https://avro.apache.org/) mas preferi criar este pequeno projeto para definir o esquema de dado dos logs por causa de restrições de tempo. Esta classe permite a verificação da integridade de cada Log e a adição de novos campos ao usar o tipo Dynamic na hora da Desserialização.


### QueueDatabase

Classe para abstrair a conexão a um sistema de mensageria baseado em filas. Contém uma implementação genérica de acesso à filas para testes e uma específica para uso do RabbitMQ. Acredito que esta arquitetura facilita uma fácil manutenção e troca de tecnologias, por exemplo para Kafka, caso desejado.


## Elasticsearch e Kibana

Enquanto o RabbitMQ fica por trás das cortinas transmitindo mensagens entre diferentes serviços. Usamos a combinação do Elasticsearch e do Kibana para visualizar estas mensagens de forma prática. Além do sistema de queries que a ferrramenta proporciona, criei um histograma para mostrar os 10 processos com tempo de resposta mais altos. Use a URL a seguir para explorar o resultado dos serviços descritos anteriormente:

```
127.0.0.1:5601/app/kibana
```

![Alt text](imgs/kibana.jpg?raw=true "Histograma das 10 aplicações mais lentas no Kibana.")

## Críticas e Trabalhos futuros

### Melhorar sistema de serialização e validação de dados

Devido a restrições de tempo, não validei todos os campos da mensagem de Logs em relação à regras de negócio, apenas a parte da existência deles. Coloquei um exemplo de validação de formato de número lá que acredito demonstrar bem a ideia e o local que esta parte deveria ser implementada.

Também acho que seria uma boa reportar o campo problemático em uma string JSON na hora da desserialização dela. Talvez fazer o método retornar uma exceção mais descritiva ao invés de apenas retornar NULL.

Além disto, do jeito que foi implementado, aplicar esta validação é bem trabalhoso pois deixei acoplado com a parte de desserialização a parte da verificação (veja o arquivo LogEntryList.cs do Projeto JsonHelper por exemplo). Acredito que criar uma função à parte para fazer a verificação, indepentente de anotações para garantir a existência de campos seria ótimo para melhorar a criação de código e o desempenho do sistema.

### Instalar sistema em múltiplas máquinas

Tentei imaginar como preparar a arquitetura para ser configurada para rodar em múltiplas máquinas, acredito que alguns passos extras seriam necessários como:

* Modificar /etc/hosts para criar apelidos para máquinas de acordo com seus IPs;
```
111.111.111.11 rabbitMqServer
111.111.111.12 elkServer
...
```

* Colocar no arquivo hosts.ini endereços diferentes para cada máquina;
* Adicionar apelidos nos lugares necessários nas configurações do Kibana para referenciar o Elasticsearch;
* Adicionar apelidos na criação de conexões no código.

### Controle de fluxo de dados

O serviço que escreve os logs no Elasticsearch recebe uma lista de logs mas os escreve de 1 em 1 no Elasticsearch. Enquanto isso o produtor recebe uma lista de logs e escreve ela inteira no RabbitMQ. Talvez seria útil quebrar esta lista em pacotes de um tamanho máximo, mas para isto seria necessário um estudo da capacidade de cada máquina e da rede disponível, o que não se aplica neste projeto demonstrativo.

### Melhoria na automatização

Existem algumas melhoras que poderiam ser feitas nos playbooks, como colocar a pasta destino, IPs de máquinas e outras configurações em um arquivo específico no lugar de deixar tudo hardcoded nos arquivos .yml.

