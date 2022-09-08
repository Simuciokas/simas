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
    public abstract class BackgroundJobService : IHostedService, IDisposable
    {
        private Timer _timer = null!;

        protected readonly IServiceProvider services;

        public BackgroundJobService(IServiceProvider services)
        {
            this.services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(ExecuteAsync, null, 0, 7200000);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public abstract void ExecuteAsync(object state);


        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}