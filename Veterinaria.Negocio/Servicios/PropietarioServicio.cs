using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public class PropietarioServicio : IPropietarioServicio
{
    private readonly IPropietarioRepositorio _repo;
    private readonly IMascotaRepositorio _mascotas;
    private readonly IMapper _mapper;

    public PropietarioServicio(IPropietarioRepositorio repo, IMascotaRepositorio mascotas, IMapper mapper)
    {
        _repo = repo;
        _mascotas = mascotas;
        _mapper = mapper;
    }

    public async Task<List<PropietarioDto>> ListarAsync(string? buscar)
    {
        var propietarios = await _repo.ListarAsync(buscar);
        return _mapper.Map<List<PropietarioDto>>(propietarios);
    }

    public async Task<PropietarioDto?> ObtenerAsync(int id)
    {
        var propietario = await _repo.ObtenerAsync(id);
        return propietario == null ? null : _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<List<MascotaDto>?> ListarMascotasAsync(int id)
    {
        if (!await _repo.ExisteAsync(id)) return null;

        var mascotas = await _mascotas.ListarAsync(null, id, null);
        return _mapper.Map<List<MascotaDto>>(mascotas);
    }

    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        if (await _repo.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un propietario con el DNI {dto.Dni}.");

        var propietario = _mapper.Map<Propietario>(dto);
        await _repo.AgregarAsync(propietario);
        await _repo.GuardarAsync();

        return _mapper.Map<PropietarioDto>(await _repo.ObtenerAsync(propietario.Id));
    }

    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var propietario = await _repo.ObtenerParaEditarAsync(id);
        if (propietario == null) return false;

        _mapper.Map(dto, propietario);
        await _repo.GuardarAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var propietario = await _repo.ObtenerParaEditarAsync(id);
        if (propietario == null) return false;

        if (await _repo.TieneMascotasAsync(id))
            throw new ReglaNegocioException("No se puede eliminar al propietario porque tiene mascotas registradas.");

        await _repo.EliminarAsync(propietario);
        await _repo.GuardarAsync();
        return true;
    }
}
