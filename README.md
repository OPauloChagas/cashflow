# Testes Unitários para LancamentoController, CommandHandlers e gRPC

## Descrição

Este repositório contém os testes unitários do **LancamentoController**, **CommandHandlers**, e interações com serviços **gRPC** no contexto de um sistema de controle de lançamentos financeiros. Os testes foram desenvolvidos utilizando o **xUnit** como framework de testes e **Moq** para a criação de mocks e simulação de dependências.

## Objetivo

O objetivo deste conjunto de testes é garantir que as funcionalidades de criação, atualização, e exclusão de lançamentos estejam funcionando corretamente, tanto em termos de lógica de negócio quanto na comunicação com o serviço gRPC. Os testes cobrem cenários de sucesso, falha, e tratamento de exceções.

## Estrutura dos Testes

### 1. Testes do LancamentoController

Os testes para o `LancamentoController` verificam o comportamento dos endpoints da API para criar, atualizar, e deletar lançamentos.

- **Método CriarLancamento**:
  - Verifica se o lançamento é criado com sucesso.
  - Lida com comandos de lançamento nulos e lança `BadRequest` se o comando for inválido.
  - Verifica o retorno de erros inesperados com status `500` (Internal Server Error).

- **Método DeletarLancamento**:
  - Verifica o comportamento quando um lançamento é deletado com sucesso.
  - Lança `BadRequest` se o ID fornecido for vazio ou inválido.
  - Retorna `NotFound` se o lançamento não for encontrado.
  - Captura e trata exceções de sistema, retornando erro `500`.

- **Método AtualizarLancamento**:
  - Verifica se o lançamento é atualizado corretamente.
  - Lança `BadRequest` se o ID da URL não corresponder ao ID do comando.
  - Retorna `NotFound` se o lançamento não for encontrado para atualização.
  - Captura exceções inesperadas e retorna `500`.

### 2. Testes dos CommandHandlers

Os command handlers tratam a lógica de negócios relacionada a criação, atualização, e exclusão de lançamentos. Os testes garantem que os handlers estão interagindo corretamente com o serviço gRPC.

- **DeletarLancamentoCommandHandler**:
  - Verifica se o lançamento é deletado com sucesso através do serviço gRPC.
  - Testa a captura de `RpcException` (exceção de comunicação com gRPC) e a subsequente propagação de `ApplicationException`.
  - Lida com exceções genéricas inesperadas e garante que o erro é registrado corretamente.

- **AtualizarLancamentoCommandHandler**:
  - Garante que o lançamento é atualizado corretamente através do serviço gRPC.
  - Testa cenários onde o serviço gRPC lança exceções, como `RpcException`, e garante que essas exceções são tratadas e propagadas corretamente.
  - Verifica a resposta correta para lançamentos não encontrados ou mal formatados.

### 3. Testes gRPC

Os testes simulam o comportamento do cliente gRPC (`LauchClient`) para garantir que a comunicação entre o sistema e o serviço de lançamentos via gRPC ocorra corretamente.

- **RegistrarLancamentoAsync**:
  - Testa a comunicação bem-sucedida com o serviço gRPC, garantindo que o lançamento é registrado com sucesso.
  - Simula falhas no serviço gRPC, como a exceção `RpcException`, e verifica o tratamento e log correto das exceções.
  
- **AtualizarLancamentoAsync e DeletarLancamentoAsync**:
  - Verifica a comunicação para atualização e exclusão de lançamentos, cobrindo tanto o sucesso da operação quanto falhas devido a exceções gRPC.
  
## Tecnologias Utilizadas

- **xUnit**: Framework de testes para realizar asserções e validar o comportamento do código.
- **Moq**: Biblioteca de mocks para simular dependências como `ISender`, `ILauchClient`, e `ILogger`.
- **gRPC**: Para comunicação remota com o serviço de controle de lançamentos.
- **.NET Core**: Plataforma usada para o desenvolvimento da API e dos testes.

## Estrutura dos Arquivos de Teste

- **LancamentoControllerTests.cs**: Testes para o controller que lida com as operações CRUD de lançamentos.
- **DeletarLancamentoCommandHandlerTests.cs**: Testes para o handler responsável por deletar lançamentos.
- **AtualizarLancamentoCommandHandlerTests.cs**: Testes para o handler que lida com a atualização de lançamentos.
- **LancamentoClientTests.cs**: Testes para o cliente gRPC que se comunica com o serviço de lançamentos.

## Como Executar os Testes

1. Clone o repositório:
   ```bash
   git clone https://github.com/OPauloChagas/cashflow.git
2. Clone o repositório:
   ```bash
   cd Financial.CashFlow.Tests
3. Execute os testes usando o dotnet test:
  ```bash
  dotnet test

