using Bitacora.Domain.Entities;
using Microsoft.Win32;

namespace Bitacora.Application.Interfaces;

public interface IRegistroRepository
{
    Task<IEnumerable<Registro>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<Registro?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);
    Task CrearAsync(Registro registro, CancellationToken cancellationToken);
    Task ActualizarAsync(Registro registro, CancellationToken cancellationToken);
    Task EliminarAsync(Registro registro, CancellationToken cancellationToken);
}