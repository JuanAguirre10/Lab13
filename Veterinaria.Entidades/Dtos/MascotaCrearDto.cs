using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

public class MascotaCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, ErrorMessage = "La especie no puede tener más de 30 caracteres.")]
    public string Especie { get; set; } = null!;

    [StringLength(50, ErrorMessage = "La raza no puede tener más de 50 caracteres.")]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly? FechaNacimiento { get; set; }

    [Range(0.01, 200, ErrorMessage = "El peso debe estar entre 0.01 y 200 kg.")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El id del propietario debe ser mayor que 0.")]
    public int PropietarioId { get; set; }
}
