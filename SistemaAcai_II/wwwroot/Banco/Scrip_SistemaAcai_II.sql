drop database if exists SistemaAcai_II;
create database SistemaAcai_II;
use SistemaAcai_II;

create table Colaborador(
Id int auto_increment primary key,
Nome Varchar(50) not null,
Email Varchar(50) not null,
Senha VARCHAR(255) not null,
Tipo Varchar(8) not null,
Situacao char(1) not null
);

select * FROM CLIENTE;
/* insert into	Colaborador (Nome,Email,Senha,Tipo) 
values("Nilson - Admin", "admin@sistemaacai.com",SHA2('123456', 256),"G");
*/
insert into	Colaborador (Nome,Email,Senha,Tipo, Situacao) 
values("Admin - Admin", "admin@sistemaacai.com",'123456',"G", "A");
select * from colaborador;
-- Tabela de Cliente
create table Cliente(
Id int auto_increment primary key,
Nome Varchar(50) not null,
Nascimento DateTime not null,
Sexo char(1),
CPF Varchar(11) not null,
Telefone Varchar(14) not null,
Email Varchar(50) not null,
Senha Varchar(255) not null,
Situacao char(1) not null
);
-- Tabela de Filiais
CREATE TABLE Filiais (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RazaoSocial VARCHAR(100),
    NomeFantasia VARCHAR(100),
    Email VARCHAR(100),
    CNPJ VARCHAR(14),
    telefone VARCHAR(20),    
    status ENUM('Ativa', 'Inativa') NOT NULL DEFAULT 'Ativa'
);
-- Tabela de Endereco
create table Endereco(
Id int auto_increment primary key,
IdCli int,
IdColab int,
CEP varchar(10) not null,
Estado varchar(70) not null,
Cidade varchar(70) not null,
Bairro varchar(70),
Endereco varchar(150) not null,
Complemento varchar(150), 
Numero varchar(15) not null,
foreign key (IdCli) references Cliente(Id),
foreign key (IdColab) references Colaborador(Id)
);
create table Categoria(
Id int auto_increment primary key,
Nome varchar(10) not null,
Descricao varchar(70) not null
);
