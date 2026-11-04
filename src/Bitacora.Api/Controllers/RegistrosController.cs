using Bitacora.Application.DTOs;
using Bitacora.Application.Interfaces;
using Bitacora.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bitacora.Api.Controllers;

[ApiController]
[Route("api/registros")]
public sealed class RegistrosController(IRegistroRepository repository) : ControllerBase
{
    // GET: /api/registros (Listar registros)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistroResponse>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var registros = await repository.ObtenerTodosAsync(cancellationToken);

        var response = registros.Select(r => new RegistroResponse(
            r.Id,
            r.Titulo,
            r.Descripcion,
            !string.IsNullOrEmpty(r.FotoBase64),
            r.FotoBase64,
            r.FechaCreacion
        ));

        return Ok(response);
    }

    // GET: /api/registros/{id} (Consultar un registro con su foto)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistroResponse>> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var r = await repository.ObtenerPorIdAsync(id, cancellationToken);

        if (r == null)
        {
            return NotFound();
        }

        var response = new RegistroResponse(
            r.Id,
            r.Titulo,
            r.Descripcion,
            !string.IsNullOrEmpty(r.FotoBase64),
            r.FotoBase64, // Aquí sí se incluye la foto completa
            r.FechaCreacion
        );

        return Ok(response);
    }

    // POST: /api/registros (Crear un registro)
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearRegistroRequest request, CancellationToken cancellationToken)
    {
        var registro = new Registro
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            FotoBase64 = request.FotoBase64,
            FechaCreacion = DateTimeOffset.UtcNow // Asignada por el servidor
        };

        await repository.CrearAsync(registro, cancellationToken);

        var response = new RegistroResponse(
            registro.Id,
            registro.Titulo,
            registro.Descripcion,
            !string.IsNullOrEmpty(registro.FotoBase64),
            registro.FotoBase64,
            registro.FechaCreacion
        );

        return CreatedAtAction(nameof(ObtenerPorId), new { id = registro.Id }, response);
    }

    // PUT: /api/registros/{id} (Actualizar un registro)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarRegistroRequest request, CancellationToken cancellationToken)
    {
        var existente = await repository.ObtenerPorIdAsync(id, cancellationToken);
        if (existente == null)
        {
            return NotFound();
        }

        existente.Titulo = request.Titulo;
        existente.Descripcion = request.Descripcion;
        existente.FotoBase64 = request.FotoBase64;

        await repository.ActualizarAsync(existente, cancellationToken);

        return NoContent(); // 204 No Content o 200 OK
    }

    // DELETE: /api/registros/{id} (Eliminar un registro)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var existente = await repository.ObtenerPorIdAsync(id, cancellationToken);
        if (existente == null)
        {
            return NotFound();
        }

        await repository.EliminarAsync(existente, cancellationToken);

        return NoContent(); // 204 No Content
    }
}
