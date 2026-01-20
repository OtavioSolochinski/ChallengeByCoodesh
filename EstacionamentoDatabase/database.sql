Create Database EstacionamentoTop;

Use EstacionamentoTop;

CREATE TABLE veiculos (
    placa VARCHAR(10) PRIMARY KEY,
    data_hora_entrada DATETIME NOT NULL,
    data_hora_saida DATETIME,
    valor_cobrado DECIMAL(10,2)
    tempo Decimal(10,2)
);

CREATE TABLE tabela_precos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    valor_hora_inicial DECIMAL(10,2) NOT NULL,
    valor_hora_adicional DECIMAL(10,2) NOT NULL
);
