namespace Pedidos.BL.Exceptions
{
    public class ValidacionException(IReadOnlyDictionary<string, string[]> errores)
        : Exception("Los datos proporcionados no son válidos.")
    {
        public IReadOnlyDictionary<string, string[]> Errores { get; } = errores;
    }
}
