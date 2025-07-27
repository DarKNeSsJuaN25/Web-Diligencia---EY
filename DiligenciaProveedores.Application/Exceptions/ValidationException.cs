namespace DiligenciaProveedores.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("Uno o más errores de validación ocurrieron.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Uno o más errores de validación ocurrieron.")
        {
            Errors = errors;
        }

        public ValidationException(string propertyName, string errorMessage)
            : base("Uno o más errores de validación ocurrieron.")
        {
            Errors = new Dictionary<string, string[]>
            {
                { propertyName, new [] { errorMessage } }
            };
        }
    }
}