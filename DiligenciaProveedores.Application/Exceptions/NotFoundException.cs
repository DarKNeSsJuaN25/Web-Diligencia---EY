using System;

namespace DiligenciaProveedores.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string name, object key)
            : base($"El recurso '{name}' con identificador '{key}' no fue encontrado.")
        {
        }
    }
}