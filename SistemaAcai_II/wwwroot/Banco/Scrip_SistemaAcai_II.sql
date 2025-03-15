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
select * from colaborador;

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
select * from cliente;

-- Tabela de Filiais
CREATE TABLE Filiais (
    Idfilial INT AUTO_INCREMENT PRIMARY KEY,
    RazaoSocial VARCHAR(100) not null,
    NomeFantasia VARCHAR(100),
    Email VARCHAR(100) not null,
    CNPJ VARCHAR(14) not null,
    InscricaEstadual VARCHAR(17),
    telefone VARCHAR(20) not null,    
    status ENUM('Ativa', 'Inativa') NOT NULL DEFAULT 'Ativa'
);

select * from Filiais;

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
select * from endereco;

-- Tabela de Categoria
create table Categoria(
Id int auto_increment primary key,
Nome varchar(10) not null,
Descricao varchar(70) not null
);

create table ProdutoSimples(
IdProd int primary key auto_increment,
Descricao varchar(75) not null,
PrecoUn decimal(10,2)
); 

Select * from filiais as t1 inner join  endereco as t2 on t1.Idfilial = t2.Idfilial where t1.Idfilial = t1.Idfilial;
