using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IPropietarioRepositorio
{
    Task<List<Propietario>> ListarAsync(string? buscar);
    Task<Propietario?> ObtenerAsync(int id);
    Task<Propietario?> ObtenerParaEditarAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteDniAsync(string dni);
    Task<bool> TieneMascotasAsync(int id);
    Task AgregarAsync(Propietario propietario);
    Task EliminarAsync(Propietario propietario);
    Task GuardarAsync();
}
