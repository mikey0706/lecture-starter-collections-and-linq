using AutoMapper;
using CollectionsAndLinq.BL.Context;
using CollectionsAndLinq.BL.Entities;
using CollectionsAndLinq.BL.Models.Projects;
using CollectionsAndLinq.BL.Models.Tasks;
using CollectionsAndLinq.BL.Models.Teams;
using CollectionsAndLinq.BL.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsAndLinq.BL.MappingProfiles
{
    public sealed class BusionessLayerProfile : Profile
    {

        public BusionessLayerProfile() 
        {
            CreateMap<TaskDto, CollectionsAndLinq.BL.Entities.Task>().ForMember(d => d.State, o => o.MapFrom(s => s.State.ToString()));
            CreateMap<CollectionsAndLinq.BL.Entities.Task, TaskDto>();
            CreateMap<User, UserDto>();
            CreateMap<Project, ProjectDto>();
            CreateMap<Team, TeamDto>();
        }
    }
}
