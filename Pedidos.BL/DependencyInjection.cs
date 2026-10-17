using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pedidos.BL.Interfaces;
using Pedidos.BL.Services;

namespace Pedidos.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLogicaDeNegocio(this IServiceCollection services)
        {
            services.TryAddSingleton(TimeProvider.System);

            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IPedidoService, PedidoService>();

            return services;
        }
    }
}
