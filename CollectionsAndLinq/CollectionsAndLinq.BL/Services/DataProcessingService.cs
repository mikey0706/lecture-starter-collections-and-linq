using AutoMapper;
using CollectionsAndLinq.BL.Context;
using CollectionsAndLinq.BL.Entities;
using CollectionsAndLinq.BL.Interfaces;
using CollectionsAndLinq.BL.MappingProfiles;
using CollectionsAndLinq.BL.Models;
using CollectionsAndLinq.BL.Models.Projects;
using CollectionsAndLinq.BL.Models.Tasks;
using CollectionsAndLinq.BL.Models.Teams;
using CollectionsAndLinq.BL.Models.Users;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace CollectionsAndLinq.BL.Services;

// Add implementations to the methods and constructor. You can also add new members to the class.
public class DataProcessingService : IDataProcessingService
{
    private readonly IDataProvider _dataProvider;
    private readonly IMapper _mapper;

    public DataProcessingService()
    {
        _dataProvider = new DataProvider();

        var config = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new BusionessLayerProfile());
        });

        _mapper = new Mapper(config);
    }

    public async Task<Dictionary<string, int>> GetTasksCountInProjectsByUserIdAsync(int userId)
    {
        var data = await _dataProvider.GetTasksAsync();
        var projects = await _dataProvider.GetProjectsAsync();
        var user = await _dataProvider.GetUsersAsync();

        return (user.FirstOrDefault(u => u.Id == userId) != null) ? data.Where(t => t.PerformerId == userId).ToDictionary(
                x => $"{x.ProjectId}: {projects.FirstOrDefault(p => p.Id == x.ProjectId).Name}",
                x => data.Where(d => d.ProjectId == x.ProjectId).Count()) 
            : new Dictionary<string, int>();
    }

    public async Task<List<TaskDto>> GetCapitalTasksByUserIdAsync(int userId)
    {
        var data = await _dataProvider.GetTasksAsync();
        var user = await _dataProvider.GetUsersAsync();

        return (user.FirstOrDefault(u => u.Id == userId) != null) ? _mapper.Map<List<TaskDto>>(
            data.Where(d => d.PerformerId == userId & char.IsUpper(d.Name[0]))) 
            : new List<TaskDto>();
    }

    public async Task<List<(int Id, string Name)>> GetProjectsByTeamSizeAsync(int teamSize)
    {
        var usersData = await _dataProvider.GetUsersAsync();
        var projectsData = await _dataProvider.GetProjectsAsync();
        var list = new List<(int, string)>();

        var teams = usersData.GroupBy(t => t.TeamId).Where(i => i.Count() > teamSize).Select(u => u.FirstOrDefault().TeamId).ToList();
        
        if (teams != null)
        {
            foreach (var t in teams)
            {
                var project = projectsData.FirstOrDefault(p => p.TeamId == t);
                if (project == null) continue;
                list.Add((project.Id, project.Name));
            }
        }
        return list;
    }

    public async Task<List<TeamWithMembersDto>> GetSortedTeamByMembersWithYearAsync(int year)
    {
        var data = await _dataProvider.GetUsersAsync();
        var teams = await _dataProvider.GetTeamsAsync();

       var result = teams.Select(i => new TeamWithMembersDto(
            i.Id, 
            i.Name,
            _mapper.Map<List<UserDto>>(data.Where(u => u.BirthDay.Date.Year < year & u.TeamId == i.Id).OrderByDescending(r => r.RegisteredAt).ThenBy(n => n.FirstName))
            )).ToList();

        if (result != null) 
        {
            return result;
        }

        return new List<TeamWithMembersDto>();

    }

    public async Task<List<UserWithTasksDto>> GetSortedUsersWithSortedTasksAsync()
    {
        var users = await _dataProvider.GetUsersAsync();
        var tasks = await _dataProvider.GetTasksAsync();

        if (users.Count() <= 0) return new List<UserWithTasksDto>();
        
        return users.OrderBy(u => u.FirstName).Select(t => new UserWithTasksDto(
            t.Id,
            t.FirstName,
            t.LastName,
            t.Email,
            t.RegisteredAt,
            t.BirthDay,
            _mapper.Map<List<TaskDto>>(tasks.OrderByDescending(n => n.Name.Length).Where(p => p.PerformerId == t.Id))
        )).ToList();
    }

    public async Task<UserInfoDto> GetUserInfoAsync(int userId)
    {
        var users = await _dataProvider.GetUsersAsync();
        var projects = await _dataProvider.GetProjectsAsync();
        var tasks = await _dataProvider.GetTasksAsync();

        var user = users.FirstOrDefault(u => u.Id == userId);

        if (user != null)
        {
            var project = projects.Where(p => p.TeamId == user.TeamId).Last();
            project = (project == null) ? null : project;

            var userLastTasks = (project == null) ? null : tasks.Where(t => t.ProjectId == project.Id);
            var tasksCount = (project == null || userLastTasks == null) ? 0 : userLastTasks.Count(); 

            var unfinishedTasksCount = tasks.Where(t => !t.FinishedAt.HasValue & t.PerformerId == user.Id).ToList().Count();

            return new UserInfoDto(
                _mapper.Map<UserDto>(user),
                _mapper.Map<ProjectDto>(project),
                tasksCount, 
                unfinishedTasksCount,
                _mapper.Map<TaskDto>(tasks.OrderBy(t => DateTime.Compare((t.FinishedAt == null ) ? DateTime.Now : (DateTime)t.FinishedAt, t.CreatedAt)).Last())
                );
        }
        return null;
    }

    public async Task<List<ProjectInfoDto>> GetProjectsInfoAsync()
    {
        var users = await _dataProvider.GetUsersAsync();
        var projects = await _dataProvider.GetProjectsAsync();
        var tasks = await _dataProvider.GetTasksAsync();

        return projects.Select(d => new ProjectInfoDto(
            _mapper.Map<ProjectDto>(d),
            _mapper.Map<TaskDto>(tasks.OrderBy(d => d.Description.Length).Where(t => t.ProjectId == d.Id).LastOrDefault()),
            _mapper.Map<TaskDto>(tasks.OrderByDescending(n => n.Name.Length).Where(t => t.ProjectId == d.Id).LastOrDefault()),
            (d.Description.Length > 20 & tasks.Where(p => p.ProjectId == d.Id).Count() < 3) ? users.Where(t => t.TeamId == d.TeamId).Count() : null
            )).ToList();
    }

    public async Task<PagedList<FullProjectDto>> GetSortedFilteredPageOfProjectsAsync(PageModel pageModel, FilterModel filterModel, SortingModel sortingModel)
    {
        var users = await _dataProvider.GetUsersAsync();
        var projects = await _dataProvider.GetProjectsAsync();
        var tasks = await _dataProvider.GetTasksAsync();
        var teams = await _dataProvider.GetTeamsAsync();

        var data = new PagedList<FullProjectDto>(projects.Select(d => new FullProjectDto(
            d.Id,
            d.Name,
            d.Description,
            d.CreatedAt,
            d.Deadline,

            tasks.Where(p => p.ProjectId == d.Id).Select(d => new TaskWithPerformerDto(
                d.Id, 
                d.Name, 
                d.Description, 
                d.State.ToString(), 
                d.CreatedAt, 
                d.FinishedAt, 
                _mapper.Map<UserDto>(users.FirstOrDefault(u => u.Id == d.PerformerId)))).ToList(),

            _mapper.Map<UserDto>(users.FirstOrDefault(u => u.Id == d.AuthorId)),
            _mapper.Map<TeamDto>(teams.FirstOrDefault(t => t.Id == d.TeamId))
            )).ToList(),
            projects.Count
            );

        var list = SortList(data, sortingModel);

        var result = ListFilter(list, filterModel, pageModel);
        return result;

    }

    private PagedList<FullProjectDto> SortList(PagedList<FullProjectDto> list, SortingModel sortingModel) 
    {
        var (Items, ToTalCount) = list;
        switch (sortingModel.Property) 
        {
            case SortingProperty.Name: 
                {
                    Items = Items.OrderBy(n => n.Name).ToList();
                    break; 
                }
            case SortingProperty.Description: 
                {
                    Items = Items.OrderBy(d => d.Description).ToList();
                    break;
                }
            case  SortingProperty.Deadline: 
                {
                    Items = Items.OrderBy(d => d.Deadline).ToList();
                    break;
                }
            case SortingProperty.CreatedAt: 
                {
                    Items = Items.OrderBy(t => t.CreatedAt).ToList();
                    break; 
                }
            case SortingProperty.TasksCount: 
                {
                    Items = Items.OrderBy(t => t.Tasks.Count).ToList();
                    break; 
                }
            case SortingProperty.AuthorFirstName: 
                {
                    Items = Items.OrderBy(a => a.Author.FirstName).ToList();
                    break; 
                }
            case SortingProperty.AuthorLastName: 
                {
                    Items = Items.OrderBy(a => a.Author.LastName).ToList();
                    break; 
                }
            case SortingProperty.TeamName: 
                {
                    Items = Items.OrderBy(t => t.Team.Name).ToList();
                    break; 
                }
        }

        if (sortingModel != null)
        {
            switch (sortingModel.Order)
            {
                case SortingOrder.Descending:
                    {
                        Items.Reverse();
                        break;
                    }
            }
        }
        return new PagedList<FullProjectDto>(Items, ToTalCount); 
    }

    private PagedList<FullProjectDto> ListFilter(PagedList<FullProjectDto> list, FilterModel filterModel, PageModel pageModel) 
    {
        var(Items, _) = list;

        Items = (!string.IsNullOrEmpty(filterModel.Name) & !string.IsNullOrEmpty(filterModel.Description)) ? list.Items.Where(
            i => i.Name.Contains(filterModel.Name) && i.Description.Contains(filterModel.Description)).ToList() : Items;

        Items = (!string.IsNullOrEmpty(filterModel.AuthorFirstName) & !string.IsNullOrEmpty(filterModel.AuthorLastName)) ? list.Items.Where(
            i => i.Author.FirstName.Contains(filterModel.AuthorFirstName) && i.Author.LastName.Contains(filterModel.AuthorLastName)).ToList() : Items;

        Items = (!string.IsNullOrEmpty(filterModel.TeamName)) ? Items = list.Items.Where(i => i.Team.Name.Contains(filterModel.TeamName)).ToList() : Items;

        return new PagedList<FullProjectDto>((pageModel == null) ? Items : Items.Skip((pageModel.PageNumber - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList(), Items.Count);
    }
}
