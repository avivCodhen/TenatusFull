using System.Linq;
using AutoMapper;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Data;

namespace Tenatus.API.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TraderSetting, TraderSettingModel>().ForMember(dest => dest.Stocks,
                src => src.MapFrom(x => x.Stocks.Select(s => s.Name)));
        }
    }
}