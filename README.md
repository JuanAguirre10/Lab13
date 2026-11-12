# Veterinaria

Web API REST para registrar a los propietarios de una veterinaria y a sus mascotas. Está hecha con ASP.NET Core (.NET 10), Entity Framework Core con **base de datos primero** y AutoMapper. La base `VeterinariaDB` se crea con el script `db/VeterinariaDB.sql` y el modelo se genera con Scaffold.

La solución está separada en cuatro proyectos. Los controladores solo trabajan con servicios y DTOs; nunca usan el `DbContext` ni las entidades.

## Capas

| Proyecto | Contenido | Referencia a |
|---|---|---|
| `Veterinaria.Entidades` | Entidades generadas con Scaffold (`Propietario`, `Mascota`) y los DTOs en `Dtos/` | — |
| `Veterinaria.Datos` | `VeterinariaContext` y los repositorios con sus interfaces | Entidades |
| `Veterinaria.Negocio` | Servicios con las reglas de negocio, `MappingProfile` de AutoMapper y las excepciones | Datos |
| `Veterinaria.Api` | Controladores, `Program.cs`, Swagger y las pruebas `.http` | Negocio |

Los repositorios devuelven entidades y no saben nada de DTOs ni de HTTP. Los servicios validan las reglas, convierten entre DTOs y entidades con AutoMapper y devuelven DTOs. Todos los métodos son asíncronos y las consultas se hacen con LINQ.

## Entidades

| Entidad | Campos |
|---|---|
| `Propietario` | Id, Nombres (máx. 100), Dni (8 dígitos, único), Telefono (opcional, 9 dígitos que empiezan con 9), Correo (opcional, máx. 100) |
| `Mascota` | Id, Nombre (máx. 50), Especie (máx. 30), Raza (opcional, máx. 50), FechaNacimiento, Peso (decimal(5,2)), PropietarioId |

Relación 1:N: un propietario tiene muchas mascotas y cada mascota pertenece a un propietario (`FK_Mascotas_Propietarios`). En la base, un mismo propietario no puede tener dos mascotas con el mismo nombre (`UQ_Mascotas_PropietarioId_Nombre`).

Las entidades no tienen `[JsonIgnore]` ni Data Annotations porque nunca salen de la API: lo que se recibe y se devuelve son DTOs.

## DTOs

| DTO | Uso | Detalle |
|---|---|---|
| `PropietarioDto` | Respuesta | Incluye `CantidadMascotas` (`MapFrom` de `Mascotas.Count`) |
| `PropietarioCrearDto` | POST | Nombres, Dni, Telefono, Correo |
| `PropietarioActualizarDto` | PUT | Igual que el de crear, pero sin Dni porque el DNI no cambia |
| `MascotaDto` | Respuesta | Incluye `Propietario` con el nombre del dueño (`MapFrom` de `Propietario.Nombres`) |
| `MascotaCrearDto` | POST | Nombre, Especie, Raza, FechaNacimiento, Peso, PropietarioId |
| `MascotaActualizarDto` | PUT | Igual que el de crear, pero sin PropietarioId: una mascota no cambia de dueño |

Ningún DTO de entrada lleva Id; el id va en la ruta. Las validaciones son Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]`) con mensajes en español. Con `[ApiController]`, si los datos no son válidos la API responde 400 automáticamente.

Todos los mapeos están en `Veterinaria.Negocio/MappingProfile.cs`. No se usa `ReverseMap` porque cada mapeo se usa en un solo sentido. En Swagger, la sección Schemas muestra solo los 6 DTOs.

## Reglas de negocio (409)

- No se puede registrar un propietario con un DNI que ya existe.
- No se puede eliminar un propietario que todavía tiene mascotas.
- Un propietario no puede tener dos mascotas con el mismo nombre (POST y PUT de mascotas).

Los servicios lanzan `ReglaNegocioException` y el controlador la convierte en un 409 con `ProblemDetails`.

También responden 400, con el error en el campo que corresponde:

- POST de mascota con un `propietarioId` que no existe (`PropietarioId`).
- POST o PUT de mascota con fecha de nacimiento futura (`FechaNacimiento`).

## Endpoints

| Verbo | Ruta | Respuesta |
|---|---|---|
| GET | `/api/propietarios?buscar=` | 200 |
| GET | `/api/propietarios/{id}` | 200, 404 |
| GET | `/api/propietarios/{id}/mascotas` | 200, 404 |
| POST | `/api/propietarios` | 201, 400, 409 |
| PUT | `/api/propietarios/{id}` | 204, 400, 404 |
| DELETE | `/api/propietarios/{id}` | 204, 404, 409 |
| GET | `/api/mascotas?especie=&propietarioId=&buscar=` | 200 |
| GET | `/api/mascotas/{id}` | 200, 404 |
| POST | `/api/mascotas` | 201, 400, 409 |
| PUT | `/api/mascotas/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/mascotas/{id}` | 204, 404 |

En propietarios, `buscar` filtra por nombres o DNI. En mascotas, `buscar` filtra por nombre o raza, y los tres filtros se pueden combinar. Las pruebas de cada endpoint y de cada código están en `Veterinaria.Api/Veterinaria.Api.http`; se ejecutan en orden sobre la base recién cargada.

## Ejecutar en local (Visual Studio)

1. En SSMS, conectarse a `.\SQLEXPRESS`, crear la base `VeterinariaDB` y ejecutar `db/VeterinariaDB.sql` sobre ella.
2. Abrir `Veterinaria.slnx`.
3. Clic derecho en `Veterinaria.Api` > **Administrar secretos de usuario** y pegar:

   ```json
   {
     "ConnectionStrings": {
       "Veterinaria": "Server=.\\SQLEXPRESS;Database=VeterinariaDB;Integrated Security=True;TrustServerCertificate=True"
     }
   }
   ```

4. Dejar `Veterinaria.Api` como proyecto de inicio y ejecutar con F5 (perfil `http`). Se abre Swagger en http://localhost:5213/swagger.

Desde la terminal, en la carpeta de la solución:

```bash
sqlcmd -S ".\SQLEXPRESS" -E -C -Q "CREATE DATABASE VeterinariaDB"
sqlcmd -S ".\SQLEXPRESS" -E -C -d VeterinariaDB -i db/VeterinariaDB.sql
dotnet user-secrets set "ConnectionStrings:Veterinaria" "Server=.\SQLEXPRESS;Database=VeterinariaDB;Integrated Security=True;TrustServerCertificate=True" --project Veterinaria.Api
dotnet run --project Veterinaria.Api --launch-profile http
```

El script borra y vuelve a crear las tablas, así que también sirve para regresar a los datos iniciales. La cadena de conexión no está en `appsettings.json`; se lee de los secretos de usuario.

## Scaffold

Se ejecuta sobre `Veterinaria.Datos`, con `Veterinaria.Api` como proyecto de inicio para que lea la cadena de los secretos de usuario. `dotnet-ef` está en el manifiesto local `dotnet-tools.json`:

```bash
dotnet tool restore
dotnet ef dbcontext scaffold "Name=ConnectionStrings:Veterinaria" Microsoft.EntityFrameworkCore.SqlServer --project Veterinaria.Datos --startup-project Veterinaria.Api --output-dir Models --context VeterinariaContext --no-onconfiguring --force
```

Consola del Administrador de paquetes:

```
Scaffold-DbContext "Name=ConnectionStrings:Veterinaria" Microsoft.EntityFrameworkCore.SqlServer -Project Veterinaria.Datos -StartupProject Veterinaria.Api -OutputDir Models -Context VeterinariaContext -NoOnConfiguring -Force
```

El Scaffold deja todo en `Veterinaria.Datos/Models`. Después se movieron `Propietario.cs` y `Mascota.cs` a `Veterinaria.Entidades` (namespace `Veterinaria.Entidades`) y `VeterinariaContext.cs` a la raíz de `Veterinaria.Datos` (namespace `Veterinaria.Datos`, con `using Veterinaria.Entidades;`), y se borró la carpeta `Models`. Además, el Scaffold nombra la colección de `Propietario` como `Mascota`, en singular, así que se renombró a `Mascotas` en la entidad y en el `WithMany` del contexto. Si se vuelve a generar, hay que repetir estos pasos.

## Estructura

```
Lab13/
├── Veterinaria.slnx
├── dotnet-tools.json
├── db/VeterinariaDB.sql
├── Veterinaria.Entidades/
│   ├── Propietario.cs, Mascota.cs
│   └── Dtos/          PropietarioDto, PropietarioCrearDto, PropietarioActualizarDto,
│                      MascotaDto, MascotaCrearDto, MascotaActualizarDto
├── Veterinaria.Datos/
│   ├── VeterinariaContext.cs
│   └── Repositorios/  IPropietarioRepositorio, PropietarioRepositorio,
│                      IMascotaRepositorio, MascotaRepositorio
├── Veterinaria.Negocio/
│   ├── MappingProfile.cs
│   ├── ReglaNegocioException.cs, DatoInvalidoException.cs
│   └── Servicios/     IPropietarioServicio, PropietarioServicio,
│                      IMascotaServicio, MascotaServicio
└── Veterinaria.Api/
    ├── Controllers/   PropietariosController, MascotasController
    ├── Program.cs     EF Core, repositorios, servicios, AutoMapper, Swagger y CORS
    └── Veterinaria.Api.http
```

CORS solo permite el origen `http://localhost:5173`.
