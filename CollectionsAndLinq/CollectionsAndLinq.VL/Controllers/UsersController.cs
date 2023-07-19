using CollectionsAndLinq.BL.Interfaces;
using CollectionsAndLinq.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsAndLinq.VL.Controllers
{
    public class UsersController
    {
        private readonly IDataProcessingService _dataProcessingService;
        public UsersController()
        {
            _dataProcessingService = new DataProcessingService();
        }

        public async Task ShowSortedUsersWithSortedTasksAsync() 
        {
            var data = await _dataProcessingService.GetSortedUsersWithSortedTasksAsync();
            foreach (var user in data) 
            {
                Console.WriteLine($"User id: {user.Id}\n " +
                    $"Full user name: {user.FirstName} - {user.LastName}\n" +
                    $"Email: {user.Email}\n" +
                    $"Birthday: {user.BirthDay}\n");
                foreach (var task in user.Tasks) 
                {
                    Console.WriteLine($"Task id: {task.Id}\n" +
                        $"Task name: {task.Name}\n" +
                        $"Task description: {task.Description}\n" +
                        $"Created at: {task.CreatedAt}\nFinished at:{task.FinishedAt}\n" +
                        $"Task state: {task.State}");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

        }

        public async Task ShowUserInfoAsync(int userId) 
        {
            var user = await _dataProcessingService.GetUserInfoAsync(userId);
            Console.WriteLine($"---------------------The {user.User.LastName} {user.User.FirstName} Info -----------------------");
            
            Console.WriteLine($"Id: {user.User.Id}\n" +
                $"Email: {user.User.Email}\n" +
                $"Date of birth: {user.User.BirthDay}\n" +
                $"Registered at: {user.User.RegisteredAt}\n");
            
            Console.WriteLine($"--------------------The Last project: {user.LastProject.Name} Info -----------------------------");
            
            Console.WriteLine($"Project Id: {user.LastProject.Id}\n" +
                $"Description: {user.LastProject.Description}\n" +
                $"Created at: {user.LastProject.CreatedAt} \n" +
                $"Deadline: {user.LastProject.Deadline}\n" +
                $"Last project tasks count: {user.LastProjectTasksCount}\n");
            
            Console.WriteLine($"--------------------The longest {user.LongestTask.Name} Info ------------------------");

            Console.WriteLine($"Tast id: {user.LongestTask.Id}\n" +
                $"Task Name: {user.LongestTask.Name}\n" +
                $"Description: {user.LongestTask.Description}\n" +
                $"Task State: {user.LongestTask.State}\n" +
                $"Created at: {user.LongestTask.CreatedAt}\n" +
                $"Finished at: {user.LongestTask.FinishedAt}\n");

            Console.WriteLine($"Unfinished tasks count: {user.NotFinishedOrCanceledTasksCount}");
            
        }
    }
}
