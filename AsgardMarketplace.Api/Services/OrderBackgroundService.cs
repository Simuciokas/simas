using System;
using System.Threading;
using System.Threading.Tasks;
using AsgardMarketplace.Domain.Models;
using AsgardMarketplace.Domain.Repositories;
using AsgardMarketplace.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AsgardMarketplace.Api.Services
{
    public class OrderBackgroundService : BackgroundJobService
    {
        public OrderBackgroundService(IServiceProvider services) : base(services)
        {
        }

        public override async void ExecuteAsync(object state)
        {
            using (var scope = services.CreateScope())
            {
                var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();

                await orderService.removeOrders();
            }
        }
    }
}