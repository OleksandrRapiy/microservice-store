using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpc.Models;
using ProductGrpc.Protos;

namespace ProductGrpc.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(
                    des => des.CreatedTime,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedTime))
                );

            CreateMap<ProductModel, Product>()
                .ForMember(
                    des => des.CreatedTime,
                    opt => opt.MapFrom(src => src.CreatedTime.ToDateTime())
                );
        }
    }
}
