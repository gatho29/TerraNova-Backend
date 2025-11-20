using AutoMapper;
using PropertyAPI.Application.DTOs;
using PropertyAPI.Domain.Entities;

namespace PropertyAPI.Application.Mappings;


public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<Property, PropertyDto>();
        CreateMap<Owner, OwnerDto>();
        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<PropertyTrace, PropertyTraceDto>();
    }
}

