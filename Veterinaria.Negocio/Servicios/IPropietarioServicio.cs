using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public interface IPropietarioServicio
{
    Task<List<PropietarioDto>> ListarAsync(string? buscar);
    Task<PropietarioDto?> ObtenerAsync(int id);
    Task<List<MascotaDto>?> ListarMascotasAsync(int id);
    Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto);
    Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
