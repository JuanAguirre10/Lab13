IF OBJECT_ID('Mascotas', 'U') IS NOT NULL DROP TABLE Mascotas;
IF OBJECT_ID('Propietarios', 'U') IS NOT NULL DROP TABLE Propietarios;
GO

CREATE TABLE Propietarios (
    Id       INT IDENTITY(1,1) CONSTRAINT PK_Propietarios PRIMARY KEY,
    Nombres  NVARCHAR(100) NOT NULL,
    Dni      CHAR(8) NOT NULL CONSTRAINT UQ_Propietarios_Dni UNIQUE,
    Telefono NVARCHAR(9) NULL,
    Correo   NVARCHAR(100) NULL
);

CREATE TABLE Mascotas (
    Id              INT IDENTITY(1,1) CONSTRAINT PK_Mascotas PRIMARY KEY,
    Nombre          NVARCHAR(50) NOT NULL,
    Especie         NVARCHAR(30) NOT NULL,
    Raza            NVARCHAR(50) NULL,
    FechaNacimiento DATE NOT NULL,
    Peso            DECIMAL(5,2) NOT NULL,
    PropietarioId   INT NOT NULL,
    CONSTRAINT FK_Mascotas_Propietarios FOREIGN KEY (PropietarioId) REFERENCES Propietarios (Id),
    CONSTRAINT UQ_Mascotas_PropietarioId_Nombre UNIQUE (PropietarioId, Nombre)
);
GO

INSERT INTO Propietarios (Nombres, Dni, Telefono, Correo) VALUES
    (N'Ana Lucía Paredes Quispe',     '46281937', '987452136', N'ana.paredes@gmail.com'),
    (N'José Antonio Huamán Torres',   '41753928', '954187362', NULL),
    (N'Milagros Rojas Ccori',         '72619483', NULL,        N'milagros.rojas@hotmail.com'),
    (N'Ricardo Mendoza Salazar',      '43098215', '912634870', N'rmendoza@outlook.com'),
    (N'Lucía Fernanda Chávez Pinedo', '70345862', '998215347', NULL);

INSERT INTO Mascotas (Nombre, Especie, Raza, FechaNacimiento, Peso, PropietarioId) VALUES
    (N'Firulais', N'Perro',   N'Mestizo',               '2019-03-14', 18.50, 1),
    (N'Michi',    N'Gato',    N'Siamés',                '2021-07-02',  4.20, 1),
    (N'Copito',   N'Conejo',  N'Cabeza de león',        '2023-01-20',  1.80, 1),
    (N'Rocky',    N'Perro',   N'Pastor alemán',         '2018-11-05', 32.40, 2),
    (N'Pelusa',   N'Gato',    N'Persa',                 '2020-05-17',  3.90, 2),
    (N'Paco',     N'Ave',     N'Periquito australiano', '2022-09-10',  0.04, 3),
    (N'Luna',     N'Perro',   N'Shih tzu',              '2022-02-28',  6.30, 4),
    (N'Tito',     N'Hámster', NULL,                     '2025-04-12',  0.12, 4);
GO

SELECT * FROM Propietarios;
SELECT * FROM Mascotas;
