using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

public class PropietarioCrearDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden tener más de 100 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
    public string Dni { get; set; } = null!;

    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe tener 9 dígitos y empezar con 9.")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El correo no puede tener más de 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    public string? Correo { get; set; }
}
