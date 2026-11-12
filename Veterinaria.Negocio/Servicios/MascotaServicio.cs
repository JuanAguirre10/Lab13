using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public class MascotaServicio : IMascotaServicio
{
    private readonly IMascotaRepositorio _repo;
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMapper _mapper;

    public MascotaServicio(IMascotaRepositorio repo, IPropietarioRepositorio propietarios, IMapper mapper)
    {
        _repo = repo;
        _propietarios = propietarios;
        _mapper = mapper;
    }

    public async Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId, string? buscar)
    {
        var mascotas = await _repo.ListarAsync(especie, propietarioId, buscar);
        return _mapper.Map<List<MascotaDto>>(mascotas);
    }

    public async Task<MascotaDto?> ObtenerAsync(int id)
    {
        var mascota = await _repo.ObtenerAsync(id);
        return mascota == null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        if (!await _propietarios.ExisteAsync(dto.PropietarioId))
            throw new DatoInvalidoException(nameof(dto.PropietarioId), "El propietario indicado no existe.");

        ValidarFecha(dto.FechaNacimiento);

        if (await _repo.ExisteNombreAsync(dto.PropietarioId, dto.Nombre))
            throw new ReglaNegocioException($"El propietario ya tiene una mascota llamada {dto.Nombre}.");

        var mascota = _mapper.Map<Mascota>(dto);
        await _repo.AgregarAsync(mascota);
        await _repo.GuardarAsync();

        return _mapper.Map<MascotaDto>(await _repo.ObtenerAsync(mascota.Id));
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _repo.ObtenerParaEditarAsync(id);
        if (mascota == null) return false;

        ValidarFecha(dto.FechaNacimiento);

        if (await _repo.ExisteNombreAsync(mascota.PropietarioId, dto.Nombre, id))
            throw new ReglaNegocioException($"El propietario ya tiene una mascota llamada {dto.Nombre}.");

        _mapper.Map(dto, mascota);
        await _repo.GuardarAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _repo.ObtenerParaEditarAsync(id);
        if (mascota == null) return false;

        await _repo.EliminarAsync(mascota);
        await _repo.GuardarAsync();
        return true;
    }

    private static void ValidarFecha(DateOnly? fechaNacimiento)
    {
        if (fechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new DatoInvalidoException(nameof(MascotaCrearDto.FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
    }
}
