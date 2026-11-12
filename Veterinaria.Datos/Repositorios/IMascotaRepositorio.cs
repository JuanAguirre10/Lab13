using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepositorio
{
    Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, string? buscar);
    Task<Mascota?> ObtenerAsync(int id);
    Task<Mascota?> ObtenerParaEditarAsync(int id);
    Task<bool> ExisteNombreAsync(int propietarioId, string nombre, int excluirId = 0);
    Task AgregarAsync(Mascota mascota);
    Task EliminarAsync(Mascota mascota);
    Task GuardarAsync();
}
