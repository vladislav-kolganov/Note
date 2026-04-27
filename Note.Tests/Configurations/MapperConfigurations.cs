using AutoMapper;
using Note.Application.Mapping;

namespace Note.Tests.Configurations;

public static class MapperConfigurations
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ReportMapping());
        });

        return mockMapper.CreateMapper();
    }
}