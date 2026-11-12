using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class PropietarioRepositorio : IPropietarioRepositorio
{
    private readonly VeterinariaContext _context;

    public PropietarioRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Propietario>> ListarAsync(string? buscar)
    {
        var consulta = _context.Propietarios
            .Include(p => p.Mascotas)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(p => p.Nombres.Contains(buscar) || p.Dni.Contains(buscar));

        return await consulta.OrderBy(p => p.Nombres).ToListAsync();
    }

    public async Task<Propietario?> ObtenerAsync(int id) =>
        await _context.Propietarios
            .Include(p => p.Mascotas)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Propietario?> ObtenerParaEditarAsync(int id) =>
        await _context.Propietarios.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExisteAsync(int id) =>
        await _context.Propietarios.AnyAsync(p => p.Id == id);

    public async Task<bool> ExisteDniAsync(string dni) =>
        await _context.Propietarios.AnyAsync(p => p.Dni == dni);

    public async Task<bool> TieneMascotasAsync(int id) =>
        await _context.Mascotas.AnyAsync(m => m.PropietarioId == id);

    public async Task AgregarAsync(Propietario propietario) =>
        await _context.Propietarios.AddAsync(propietario);

    public Task EliminarAsync(Propietario propietario)
    {
        _context.Propietarios.Remove(propietario);
        return Task.CompletedTask;
    }

    public Task GuardarAsync() => _context.SaveChangesAsync();
}
