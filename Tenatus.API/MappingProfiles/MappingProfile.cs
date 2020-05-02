using System;
using System.Linq;
using AutoMapper;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.Dashboard.Models;
using Tenatus.API.Components.SignalR.Models;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Util;

namespace Tenatus.API.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StrategyModel, Strategy>().ForMember(dest => dest.Created, src => src.Ignore())
                .ForMember(dest => dest.LastActive, src => src.Ignore());

            CreateMap<Strategy, StrategyModel>()
                .ForMember(dest => dest.Created, src => src.MapFrom(x => x.Created.FormatDate()))
                .ForMember(dest => dest.LastActive, src => src.MapFrom(x => x.LastActive.FormatDate()))
                .Include<PercentStrategy, StrategyModel>()
                .Include<RangeStrategy, StrategyModel>();

            CreateMap<PercentStrategy, StrategyModel>().ForMember(src => src.Type,
                dest => dest.MapFrom(x => AppConstants.StrategyTypePercent));
            CreateMap<StrategyModel, PercentStrategy>().ForMember(dest => dest.Created, src => src.Ignore())
                .ForMember(dest => dest.LastActive, src => src.Ignore());
            CreateMap<RangeStrategy, StrategyModel>().ForMember(src => src.Type,
                dest => dest.MapFrom(x => AppConstants.StrategyTypeRange));
            CreateMap<StrategyModel, RangeStrategy>().ForMember(dest => dest.Created, src => src.Ignore())
                .ForMember(dest => dest.LastActive, src => src.Ignore());
            CreateMap<UserOrder, UserOrderModel>().ForMember(dest => dest.Created, src=> src.MapFrom(x=>x.Created.FormatDateTime()));

            CreateMap<StockData, StockDataModel>();
        }
    }
}