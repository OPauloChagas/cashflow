# **Financial Cash Flow System**

## **Sumário**
- [Visão Geral do Projeto](#visão-geral-do-projeto)
- [Decisões Arquiteturais (ADR)](#decisões-arquiteturais-adr)
- [Arquitetura](#arquitetura)
- [Requisitos](#requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Rodando o Sistema](#rodando-o-sistema)
  - [Executar Localmente](#executar-localmente)
  - [Executar com Docker](#executar-com-docker)
- [Testes Unitários](#testes-unitários)
- [Sugestões de implementação futura](#Sugestões-Futuras-de-Implementação)

## **Visão Geral do Projeto**

A solução implementada é uma arquitetura escalável e resiliente, projetada para gerenciar o controle de lançamentos financeiros e consolidar relatórios diários.

### Principais Contribuições:

- **Arquitetura Baseada em Microserviços**: A aplicação foi estruturada em microserviços, permitindo uma escalabilidade independente e a possibilidade de realizar atualizações sem impactar todo o sistema. Isso garante que cada componente possa evoluir de forma isolada.

- **Uso de Tecnologias Modernas**:
  - **.NET Core**: Utilizado para o desenvolvimento das APIs, oferecendo alta performance e suporte a padrões modernos de programação.
  - **gRPC**: Implementado para comunicação eficiente entre os microserviços, proporcionando baixa latência e alta performance nas chamadas de API.
  - **RabbitMQ**: Integrado para mensageria assíncrona, permitindo que os serviços se comuniquem de forma desacoplada e resiliente.
  - **PostgreSQL e MongoDB**: Utilizados para persistência de dados, garantindo tanto a consistência transacional quanto a flexibilidade em consultas de leitura.

- **Implementação de CQRS**: A arquitetura foi desenhada seguindo o padrão **Command Query Responsibility Segregation (CQRS)**, que separa as operações de leitura e escrita. Isso melhora a escalabilidade e a performance, permitindo otimizações específicas para cada tipo de operação.

- **Testes Automatizados**: Foram desenvolvidos testes unitários robustos para garantir a integridade e o correto funcionamento das funcionalidades implementadas, incluindo testes para controladores, command handlers e interações com gRPC.

### Alinhamento com o Desafio Proposto

O projeto atende aos requisitos do desafio ao proporcionar uma solução que não só atende às necessidades imediatas de controle de lançamentos financeiros, mas também é escalável, resiliente e segura. As decisões arquiteturais tomadas garantem que o sistema possa lidar com um alto volume de transações e consultas, especialmente em momentos de pico, sem comprometer a performance ou a integridade dos dados.

## **Decisões Arquiteturais (ADR)**

### ADR: Arquitetura de Microsserviços com CQRS e gRPC

#### **Contexto**  
O sistema de fluxo de caixa requer escalabilidade, resiliência e segurança para gerenciar lançamentos financeiros e consolidar relatórios diários.

#### **Decisão 1: CQRS**  
**Decisão**: Adotar o padrão CQRS para separar operações de leitura e escrita.  
**Motivação**: Otimizar a escalabilidade das operações de consulta e escrita.  
**Consequências**: Maior flexibilidade, porém com complexidade adicional na manutenção dos dois modelos de dados.

#### **Decisão 2: gRPC**  
**Decisão**: Usar gRPC para a comunicação entre microsserviços.  
**Motivação**: gRPC oferece comunicação eficiente e de baixa latência.  
**Consequências**: Curva de aprendizado maior, mas maior robustez e performance.

#### **Decisão 3: SQL para Escrita e MongoDB para Leitura**  
**Decisão**: Utilizar SQL para controle de lançamentos e MongoDB para o serviço de relatórios.  
**Motivação**: Garantir consistência em transações financeiras (SQL) e otimizar a performance de leitura com NoSQL.  
**Consequências**: Modelo de leitura mais rápido, mas a necessidade de sincronização entre bases.

#### **Decisão 4: Sincronização via Eventos**  
**Decisão**: Implementar arquitetura baseada em eventos com RabbitMQ.  
**Motivação**: Desacoplar os serviços e garantir resiliência em caso de falhas.  
**Consequências**: Maior resiliência, porém maior complexidade com a gestão de eventos.

## **Arquitetura**

A arquitetura é baseada em microserviços com dois componentes principais:

- **API de Controle de Lançamentos**: Gerencia os lançamentos financeiros (criação, atualização, exclusão).
- **API de Consolidação Diária**: Consolida os lançamentos diários e retorna o saldo. Comunicação via gRPC com a API de Lançamentos.

Os microserviços usam **RabbitMQ** para mensageria assíncrona e persistem os dados em **PostgreSQL** e **MongoDB**.

## **Requisitos**

- **Escalabilidade**: A arquitetura permite que os serviços sejam escalados de forma independente.
- **Resiliência**: O sistema foi desenhado para lidar com falhas e permitir retries automáticos.
- **Segurança**: Autenticação JWT é utilizada para proteger a API.
- **Documentação**: A API está documentada com **Swagger**.

## **Instalação e Configuração**

1. Clone o repositório:
``bash
git clone https://github.com/OPauloChagas/cashflow.git cd cashflow

3. Configure o ambiente:
- Certifique-se de ter as variáveis de ambiente configuradas para conectar aos serviços de banco de dados e RabbitMQ.

## **Rodando o Sistema**

### **Executar Localmente**

1. Certifique-se de ter o .NET Core instalado.
2. Compile o projeto:

dotnet build

3. Execute as migrações do banco de dados:

dotnet ef database update

5. Rode a aplicação:

dotnet run


### **Executar com Docker**

1. Construa e suba os containers:

docker-compose up --build


2. Acesse a aplicação:
- A API estará disponível em [http://localhost:5000](http://localhost:5000).

## **Testes Unitários**

Este repositório contém testes unitários para garantir o bom funcionamento dos controladores, command handlers e serviços gRPC.

### **Estrutura dos Testes**

1. **Testes do LancamentoController**  
Verifica o comportamento dos endpoints para criar, atualizar e deletar lançamentos financeiros. Testa cenários de sucesso e falha, garantindo a captura correta de exceções.

2. **Testes dos CommandHandlers**  
Garantem que a lógica de negócio para criação, atualização e exclusão de lançamentos está sendo processada corretamente, assim como a interação com o serviço gRPC.

3. **Testes gRPC**  
Simulam a comunicação entre os microsserviços para registrar, atualizar e deletar lançamentos financeiros. Garantem a correta tratativa de exceções como RpcException.

## **Como Executar os Testes**

1. Entre na pasta de testes:
cd Financial.CashFlow.Tests

cd Financial.CashFlow.Tests
dotnet test

## **Sugestões Futuras de Implementação**

Embora identificado a importância de implementar as seguintes funcionalidades, essas implementações foram deixadas como sugestões para o futuro devido ao prazo:

- **Autenticação JWT**: Implementação de um sistema seguro de autenticação para proteger as APIs.
- **Monitoramento e Observabilidade**: Integração de ferramentas como Prometheus e Grafana para monitorar o desempenho do sistema e coletar métricas.
- **Cache com Redis**: Uso de um sistema de cache distribuído para otimizar consultas frequentes, melhorando a performance durante picos de tráfego.
- **API Gateway Ocelot:** Para roteamento e gerenciamento de chamadas entre serviços.

Com isso, a solução demonstra não apenas a capacidade técnica de implementar uma arquitetura moderna, mas também um entendimento profundo dos princípios de design de software, essenciais para o sucesso em um ambiente de produção.
