# Estacionamento Top

Sistema de controle de estacionamento que permite registrar entrada e saída de veículos, calcular tempo de permanência e valor cobrado com base em tabela de preços configurável.

## Tecnologias utilizadas

- **Linguagem:** C#  
- **Framework:** ASP.NET Core MVC  
- **Banco de Dados:** MySQL  
- **ORM/Driver:** MySql.Data (MySQL Connector for .NET)  
- **Frontend:** Razor Views + TailwindCSS  

## Instalação e uso

 1. Pré-requisitos
- .NET 6 ou superior instalado  
- MySQL Server rodando localmente ou em servidor acessível  
- MySQL Workbench ou terminal para executar scripts SQL  

 2. Configuração do Banco de Dados
Execute o script SQL fornecido (`database.sql`) para criar o banco e tabelas:
///sql
CREATE DATABASE EstacionamentoTop;
USE EstacionamentoTop;

CREATE TABLE veiculos (
    placa VARCHAR(10) PRIMARY KEY,
    data_hora_entrada DATETIME NOT NULL,
    data_hora_saida DATETIME,
    valor_cobrado DECIMAL(10,2),
    tempo DECIMAL(10,2)
);

CREATE TABLE tabela_precos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    valor_hora_inicial DECIMAL(10,2) NOT NULL,
    valor_hora_adicional DECIMAL(10,2) NOT NULL
);///

3. Configuração da conexão:
No arquivo Connection.cs que está no EstacionamentoDatabase, ajuste a string de conexão conforme seu ambiente:
 
public const string CONNECTION_STRING = "Server=localhost;Port=3306;Database=EstacionamentoTop;Uid=root;Pwd=root;";

