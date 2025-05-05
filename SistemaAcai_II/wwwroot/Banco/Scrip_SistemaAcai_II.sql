drop database if exists SistemaAcai_II;
create database SistemaAcai_II;
use SistemaAcai_II;

-- Tabela de Colaborador
create table Colaborador(
IdColab int auto_increment primary key,
Nome Varchar(50) not null,
Email Varchar(50) not null,
Senha VARCHAR(255) not null,
Tipo Varchar(8) not null,
Situacao char(1) not null
);

/* 
	insert into	Colaborador (Nome,Email,Senha,Tipo, Situacao) 
	values("Nilson - Admin", "admin@sistemaacai.com",SHA2('123456', 256),"G", "A");
*/
insert into	Colaborador (Nome,Email,Senha,Tipo, Situacao) 
values("Admin - Admin", "admin@sistemaacai.com",'123456',"G", "A");
-- select * from colaborador;

-- Tabela de Cliente
create table Cliente(
IdCli int auto_increment primary key,
Nome Varchar(50) not null,
Nascimento DateTime not null,
Sexo char(1),
CPF Varchar(11) not null,
Telefone Varchar(14) not null,
Email Varchar(50) not null,
Senha Varchar(255) not null,
Situacao char(1) not null
);
-- select * from cliente;
-- Tabela de Caixa
CREATE TABLE Caixa (
    IdCaixa INT AUTO_INCREMENT PRIMARY KEY,
    Situacao char(1) default 'A' not null, 
    StatusEmail char(1) not null,
    DataAbertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataFechamento DATETIME,
    ValorInicial DECIMAL(10,2)
);
-- Tabela de Filiais
CREATE TABLE Filiais (
    Idfilial INT AUTO_INCREMENT PRIMARY KEY,
    RazaoSocial VARCHAR(100) not null,
    NomeFantasia VARCHAR(100),
    Email VARCHAR(100) not null,
    CNPJ VARCHAR(14) not null,
    InscricaEstadual VARCHAR(17),
    telefone VARCHAR(20) not null,    
    StatusFiliais ENUM('Ativa', 'Inativa') NOT NULL DEFAULT 'Ativa'
);
-- select * from Filiais;

-- Tabela de Endereco
create table Endereco(
IdEnd int auto_increment primary key,
IdCli int,
IdColab int,
Idfilial int,
CEP varchar(10) not null,
Estado varchar(70) not null,
Cidade varchar(70) not null,
Bairro varchar(70),
Endereco varchar(150) not null,
Complemento varchar(150), 
Numero varchar(15) not null,
foreign key (IdCli) references Cliente(IdCli),
foreign key (IdColab) references Colaborador(IdColab),
foreign key (Idfilial) references filiais(Idfilial)
);
-- select * from endereco;
create table ProdutoSimples(
IdProd int primary key auto_increment,
Descricao varchar(75) not null,
PrecoUn decimal(10,2)
); 
Insert into ProdutoSimples (Descricao, PrecoUn ) values('Açai / Sorvete / Opcionais','59.99');

-- Select * from ProdutoSimples;
-- Select * from filiais as t1 inner join  endereco as t2 on t1.Idfilial = t2.Idfilial where t1.Idfilial = t1.Idfilial;

-- Tabela forma de pagamento
CREATE TABLE FormaPagamento (
    IdForma INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(30) NOT NULL
);
/*
INSERT INTO FormaPagamento (Nome) VALUES 
('Dinheiro'),
('Cartão Crédito'),
('Cartão Débito'),
('Pix'),
('Vale Alimentação (VA)'),
('Vale Refeição (VR)');

ALTER TABLE Comanda
ADD COLUMN IdForma INT;

ALTER TABLE Comanda
ADD CONSTRAINT FK_Comanda_FormaPagamento
FOREIGN KEY (IdForma) REFERENCES FormaPagamento(IdForma);
*/

select * from FormaPagamento;
-- Tabela de Comanda (Nova)
CREATE TABLE Comanda (
    IdComanda INT AUTO_INCREMENT PRIMARY KEY,
    IdColab int,
    IdForma int,
    NomeCliente varchar(100) NOT NULL,    
    DataAbertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataFechamento TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    ValorTotal DECIMAL(10,2),
    Desconto varchar(50),
    Situacao char(1) default 'A' not null,  
    foreign key (IdColab) references Colaborador(IdColab),
    foreign key (IdForma) references FormaPagamento(IdForma)
);
SELECT * FROM Comanda WHERE SITUACAO = 'A';
-- Tabela de Itens da Comanda (Nova)
CREATE TABLE ItemComanda (
    IdItem INT AUTO_INCREMENT PRIMARY KEY,
    IdComanda INT NOT NULL,
    IdProd INT NOT NULL,
    Peso float,
    Quantidade int,
    Subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdComanda) REFERENCES Comanda(IdComanda) ON DELETE CASCADE,
    FOREIGN KEY (IdProd) REFERENCES ProdutoSimples(IdProd) ON DELETE CASCADE
);