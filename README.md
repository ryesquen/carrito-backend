# carrito-backend

script de base de datos
CREATE  DATABASE Carrito;
Go
USE Carrito;

CREATE TABLE Clientes (
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Direccion VARCHAR(100) NOT NULL
);

CREATE TABLE Tiendas (
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Sucursal VARCHAR(50) NOT NULL,
    Direccion VARCHAR(100) NOT NULL
);

CREATE TABLE Articulos (
    Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL,
    Descripcion VARCHAR(100) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    Imagen VARBINARY(MAX) NOT NULL,
    Stock INT NOT NULL
);

CREATE TABLE ArticulosTiendas (
    ArticuloId INT NOT NULL,
    TiendaId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    PRIMARY KEY (ArticuloId, TiendaId),
    FOREIGN KEY (ArticuloId) REFERENCES Articulos(Id),
    FOREIGN KEY (TiendaId) REFERENCES Tiendas(Id)
);

CREATE TABLE ClientesArticulos (
Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    ClienteId INT NOT NULL,
    ArticuloId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    FOREIGN KEY (ArticuloId) REFERENCES Articulos(Id)
);

para la autenticación y autorización
#Add-Migration InitialIdentity -c IdentityContext
#Update-database -context IdentityContext -migration InitialIdentity
