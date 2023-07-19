using CollectionsAndLinq.BL.Interfaces;
using CollectionsAndLinq.BL.Models.Projects;
using CollectionsAndLinq.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsAndLinq.VL.Controllers
{
    public class ProjectController
    {
        private readonly IDataProcessingService _dataProcessingService;
        public ProjectController()
        {
            _dataProcessingService = new DataProcessingService();
        }

        public async Task ShowProjectsByTeamSizeAsync(int teamSize) 
        {
            var data = await _dataProcessingService.GetProjectsByTeamSizeAsync(teamSize);

            foreach (var item in data)
            {
                Console.WriteLine("{0} - {1}\n", item.Id, item.Name);
            }
        }
        public async Task ShowProjectsInfoAsync() 
        {
            var data = await _dataProcessingService.GetProjectsInfoAsync();
            foreach (var item in data) 
            {
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"Project name: {item.Project.Name}\n" +
                    $"TeamMembersCount: {item.TeamMembersCount}\n");
                if (item.ShortestTaskByName != null & item.LongestTaskByDescription != null) 
                {
                    Console.WriteLine($"Longest task description: {item.LongestTaskByDescription.Description}\n" +
                    $"Shortest task name: {item.ShortestTaskByName.Name}\n");
                    continue;
                }
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------");
            }
        }
        public async Task ShowSortedFilteredPageOfProjectsAsync(PageModel pageModel, FilterModel filterModel, SortingModel sortingModel) 
        {
            var data = await _dataProcessingService.GetSortedFilteredPageOfProjectsAsync(pageModel, filterModel, sortingModel);
            foreach (var item in data.Items) 
            {
                Console.WriteLine();
                Console.WriteLine("-------------------------------- GENERAL INFO -------------------------------------");

                Console.WriteLine($"About Project\n" +
                    $"Project id: {item.Id}\n" +
                    $"Project name: {item.Name}\n" +
                    $"Description: {item.Description}\n" +
                    $"Deadline: {item.Deadline}\n" +
                    $"Created at: {item.CreatedAt}");

                Console.WriteLine($"\nAbout Author\n" +
                    $"Author id: {item.Author.Id}\n" +
                    $"Full name: {item.Author.FirstName} - {item.Author.LastName}\n" +
                    $"Email: {item.Author.Email}\n" +
                    $"Date of birth: {item.Author.BirthDay}\n" +
                    $"Registered at: {item.Author.RegisteredAt}");

                Console.WriteLine($"About Team\n" +
                    $"Team id: {item.Team.Id} \n" +
                    $"Team name: {item.Team.Name}\n" +
                    $"Created at: {item.Team.CreatedAt}");

                Console.WriteLine("--------------------------------TASKS INFO-----------------------------------------");

                foreach (var task in item.Tasks) 
                {
                    Console.WriteLine($"-------About Task-------------\n" +
                        $"Task id: {task.Id}\n" +
                        $"Task name: {task.Name}\n" +
                        $"Description: {task.Description}\n" +
                        $"Created at: {task.CreatedAt}\n" +
                        $"Finished at: {task.FinishedAt}\n" +
                        $"Task state: {task.State}");
                    
                    Console.WriteLine($"-------About Performer--------\n" +
                        $"Performer id: {task.Performer.Id}\n" +
                        $"Full performer name: {task.Performer.FirstName} - {task.Performer.LastName}\n" +
                        $"Performer email: {task.Performer.Email}\n" +
                        $"Performer's date of birth: {task.Performer.BirthDay}\n" +
                        $"Registered at: {task.Performer.RegisteredAt}");
                    Console.WriteLine("--------------------------------");
                }
            }
        }
    }
}
