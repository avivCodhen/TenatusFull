using System;
using System.Linq;
using AutoMapper;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.Dashboard.Models;
using Tenatus.API.Data;
using Tenatus.API.Util;

namespace Tenatus.API.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Strategy, StrategyModel>()
                .Include<PercentStrategy, StrategyModel>()
                .Include<RangeStrategy, StrategyModel>();

            CreateMap<PercentStrategy, StrategyModel>()
                .ForMember(dest => dest.Type,
                    src=> src.MapFrom(x=> AppConstants.StrategyTypePercent));
            CreateMap<RangeStrategy, StrategyModel>()
                .ForMember(dest => dest.Type,
                    src=> src.MapFrom(x=> AppConstants.StrategyTypeRange));
            CreateMap<UserOrder, UserOrderModel>();
        }
    }
}