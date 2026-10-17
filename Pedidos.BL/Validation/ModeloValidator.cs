using System.ComponentModel.DataAnnotations;
using Pedidos.BL.Exceptions;

namespace Pedidos.BL.Validation
{
    public static class ModeloValidator
    {
        public static void Validar<T>(T modelo) where T : class
        {
            var resultados = new List<ValidationResult>();

            if (Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, validateAllProperties: true))
            {
                return;
            }

            var errores = resultados
                .SelectMany(r => r.MemberNames.DefaultIfEmpty(string.Empty)
                    .Select(campo => (Campo: campo, Mensaje: r.ErrorMessage ?? "Valor no válido.")))
                .GroupBy(e => e.Campo)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Mensaje).ToArray());

            throw new ValidacionException(errores);
        }
    }
}
