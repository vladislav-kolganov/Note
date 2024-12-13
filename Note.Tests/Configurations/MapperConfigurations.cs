using AutoMapper;
using Note.Application.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Tests.Configurations
{
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
}
