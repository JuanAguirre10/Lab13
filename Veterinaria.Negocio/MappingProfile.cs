using AutoMapper;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascotas.Count));
        CreateMap<PropietarioCrearDto, Propietario>();
        CreateMap<PropietarioActualizarDto, Propietario>();

        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.Propietario, o => o.MapFrom(s => s.Propietario.Nombres));
        CreateMap<MascotaCrearDto, Mascota>();
        CreateMap<MascotaActualizarDto, Mascota>();
    }
}
