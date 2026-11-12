namespace Veterinaria.Entidades.Dtos;

public class PropietarioDto
{
    public int Id { get; set; }
    public string Nombres { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public int CantidadMascotas { get; set; }
}
