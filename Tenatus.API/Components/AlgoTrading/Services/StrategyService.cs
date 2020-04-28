using System;
using AutoMapper;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services
{
    public class StrategyService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public StrategyService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddStrategy(ApplicationUser user, StrategyModel request)
        {
            ValidateRequest(request);
            Strategy strategy;
            switch (request.Type)
            {
                case AppConstants.StrategyTypePercent:
                    strategy = _mapper.Map<StrategyModel, PercentStrategy>(request);
                    break;
                case AppConstants.StrategyTypeRange:
                    strategy = _mapper.Map<StrategyModel, RangeStrategy>(request);
                    break;
                default:
                    throw new Exception($"Unknown type: {request.Type}");
            }

            strategy.UserId = user.Id;
            _dbContext.Strategies.Add(strategy);
        }

        public void ValidateRequest(StrategyModel request)
        {
            if (string.IsNullOrEmpty(request.Type))
                throw new Exception("type is empty.");
            var percentStrategy = request.Type.EqualsIgnoreCase(AppConstants.StrategyTypePercent);
            var rangeStrategy = request.Type.EqualsIgnoreCase(AppConstants.StrategyTypeRange);
            if (percentStrategy && request.Percent <= 0)
                throw new Exception(
                    $"Strategy is of type {AppConstants.StrategyTypePercent} and the {nameof(request.Percent)} is {request.Percent}");

            if (rangeStrategy && request.Minimum < 0)
                throw new Exception(
                    $"Strategy is of type {AppConstants.StrategyTypeRange} and the {nameof(request.Minimum)} is {request.Minimum}");

            if (rangeStrategy && request.Maximum < 0)
                throw new Exception(
                    $"Strategy is of type {AppConstants.StrategyTypeRange} and the {nameof(request.Maximum)} is {request.Maximum}");
        }
    }
}