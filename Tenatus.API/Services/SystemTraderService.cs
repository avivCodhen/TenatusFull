using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders;
using Tenatus.API.Data;
using Tenatus.API.Util;

namespace Tenatus.API.Services
{
    public class SystemTraderService : IHostedService
    {
        private readonly TraderManager _traderManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SystemTraderService> _logger;
        private Thread _workThread;

        public SystemTraderService(TraderManager traderManager, IServiceProvider serviceProvider,
            ILogger<SystemTraderService> logger)
        {
            _traderManager = traderManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _workThread = new Thread(Start) {IsBackground = true};
            _workThread.Start();
            return Task.CompletedTask;
        }

        private void Start()
        {
            while (true)
            {
                try
                {
                    if (MarketHelper.IsMarketOpen())
                        StartTraders();
                    else
                        StopTraders();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }

        private void StopTraders()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var usersWithActiveStrategies = dbContext.ApplicationUsers.Where(x => x.Strategies.Any(s => s.Active));

            foreach (var user in usersWithActiveStrategies)
            {
                _traderManager.StopTrader(user);
            }

            dbContext.SaveChanges();
        }

        private void StartTraders()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var usersWithActiveStrategies =
                dbContext.ApplicationUsers.Where(x => x.IsTraderOn && x.Strategies.Any(s => s.Active));
            foreach (var user in usersWithActiveStrategies)
            {
                if (!_traderManager.IsOnForUser(user))
                    _traderManager.ManageTrader(user);
            }

            dbContext.SaveChanges();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void Sleep()
        {
            var (open, close) = MarketHelper.GetOpenAndClose();
            var et = MarketHelper.GetEasternTime();
            if (open > et)
                Thread.Sleep((open - et).Milliseconds);
            if (et > close)
                Thread.Sleep((open.AddDays(1) - et).Milliseconds);
            if (et > open && et < close)
                Thread.Sleep((close - et).Milliseconds);
        }
    }
}