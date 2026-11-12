using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class MascotaRepositorio : IMascotaRepositorio
{
    private readonly VeterinariaContext _context;

    public MascotaRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, string? buscar)
    {
        var consulta = _context.Mascotas
            .Include(m => m.Propietario)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);

        if (propietarioId.HasValue)
            consulta = consulta.Where(m => m.PropietarioId == propietarioId);

        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(m => m.Nombre.Contains(buscar) || (m.Raza != null && m.Raza.Contains(buscar)));

        return await consulta.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<Mascota?> ObtenerAsync(int id) =>
        await _context.Mascotas
            .Include(m => m.Propietario)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Mascota?> ObtenerParaEditarAsync(int id) =>
        await _context.Mascotas.FirstOrDefaultAsync(m => m.Id == id);

    public async Task<bool> ExisteNombreAsync(int propietarioId, string nombre, int excluirId = 0) =>
        await _context.Mascotas.AnyAsync(m => m.PropietarioId == propietarioId && m.Nombre == nombre && m.Id != excluirId);

    public async Task AgregarAsync(Mascota mascota) =>
        await _context.Mascotas.AddAsync(mascota);

    public Task EliminarAsync(Mascota mascota)
    {
        _context.Mascotas.Remove(mascota);
        return Task.CompletedTask;
    }

    public Task GuardarAsync() => _context.SaveChangesAsync();
}
