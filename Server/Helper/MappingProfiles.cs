using AutoMapper;
using BusinessObjects.Entities;
using Server.Dto;

namespace Server.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CourseDto, Course>();
            CreateMap<FileDto, BusinessObjects.Entities.File>();
            CreateMap<ResourceDto, Resource>();
        }
    }
}
