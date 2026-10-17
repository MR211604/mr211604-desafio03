using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pedidos.DAL.Data;
using Pedidos.DAL.Interfaces;
using Pedidos.DAL.Repositories;

namespace Pedidos.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAccesoADatos(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PedidosDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
