using AutoMapper;
using Delivery.Api.Dtos;
using Delivery.Api.Models;


namespace Delivery.Api.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<MenuItem, MenuItemDto>().ReverseMap();
            CreateMap<Delivery.Api.Models.Restaurant, RestaurantDto>()
               .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName));

            CreateMap<City, CityDto>();


        }


    }


}
