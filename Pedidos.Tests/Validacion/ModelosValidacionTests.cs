using Pedidos.BL.Exceptions;
using Pedidos.BL.Validation;
using Pedidos.Entities.Models;

namespace Pedidos.Tests.Validacion
{
    public class ModelosValidacionTests
    {
        [Theory]
        [InlineData("0.01", 0)]
        [InlineData("1500.50", 1)]
        [InlineData("1000000000", 100)]
        public void Producto_PrecioYStockValidos_NoLanzaExcepcion(string precio, int stock)
        {
            var producto = CrearProducto(decimal.Parse(precio), stock);

            var excepcion = Record.Exception(() => ModeloValidator.Validar(producto));

            Assert.Null(excepcion);
        }

        [Theory]
        [InlineData("0", 10, "Precio")]
        [InlineData("-0.01", 10, "Precio")]
        [InlineData("1000000001", 10, "Precio")]
        [InlineData("10", -1, "Stock")]
        public void Producto_PrecioOStockFueraDeRango_LanzaValidacionException(string precio, int stock, string campoEsperado)
        {
            var producto = CrearProducto(decimal.Parse(precio), stock);

            var excepcion = Assert.Throws<ValidacionException>(() => ModeloValidator.Validar(producto));

            Assert.True(excepcion.Errores.ContainsKey(campoEsperado));
        }

        [Fact]
        public void Cliente_DatosValidos_NoLanzaExcepcion()
        {
            var cliente = new Cliente { Nombre = "Luis", Apellido = "Pérez", Email = "luis.perez@ejemplo.com" };

            var excepcion = Record.Exception(() => ModeloValidator.Validar(cliente));

            Assert.Null(excepcion);
        }

        [Fact]
        public void Cliente_SinCamposObligatorios_DevuelveErrorPorCadaCampo()
        {
            var cliente = new Cliente();

            var excepcion = Assert.Throws<ValidacionException>(() => ModeloValidator.Validar(cliente));

            Assert.True(excepcion.Errores.ContainsKey(nameof(Cliente.Nombre)));
            Assert.True(excepcion.Errores.ContainsKey(nameof(Cliente.Apellido)));
            Assert.True(excepcion.Errores.ContainsKey(nameof(Cliente.Email)));
        }

        private static Producto CrearProducto(decimal precio, int stock) => new()
        {
            Nombre = "Monitor",
            Descripcion = "Monitor de 27 pulgadas",
            Precio = precio,
            Stock = stock
        };
    }
}
