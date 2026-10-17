namespace Pedidos.BL.Exceptions
{
    public class EntidadNoEncontradaException(string entidad, int id)
        : Exception($"{entidad} con id {id} no fue encontrado.")
    {
        public string Entidad { get; } = entidad;

        public int Id { get; } = id;
    }
}
