using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bitacora.Application.DTOs;

public sealed class ActualizarRegistroRequest : IValidatableObject
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres.")]
    public required string Titulo { get; init; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public required string Descripcion { get; init; }

    public string? FotoBase64 { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(FotoBase64))
        {
            bool formatoInvalido = false;
            bool excedeLimite = false;

            try
            {
                var base64Data = FotoBase64;
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex >= 0)
                {
                    base64Data = base64Data[(commaIndex + 1)..];
                }

                byte[] bytes = Convert.FromBase64String(base64Data);
                const int maxBytes = 2 * 1024 * 1024; // 2 MB
                if (bytes.Length > maxBytes)
                {
                    excedeLimite = true;
                }
            }
            catch (FormatException)
            {
                formatoInvalido = true;
            }

            if (formatoInvalido)
            {
                yield return new ValidationResult(
                    "El formato de la fotografía en Base64 no es válido.",
                    new string[] { nameof(FotoBase64) }
                );
            }
            else if (excedeLimite)
            {
                yield return new ValidationResult(
                    "La fotografía no puede superar los 2 MB.",
                    new string[] { nameof(FotoBase64) }
                );
            }
        }
    }
}