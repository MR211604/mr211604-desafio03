using System.ComponentModel.DataAnnotations;

namespace Pedidos.Entities.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public DateTime FechaPedido { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un cliente válido.")]
        public int ClienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un producto válido.")]
        public int ProductoId { get; set; }

        public Cliente Cliente { get; set; } = null!;

        public Producto Producto { get; set; } = null!;
    }
}
