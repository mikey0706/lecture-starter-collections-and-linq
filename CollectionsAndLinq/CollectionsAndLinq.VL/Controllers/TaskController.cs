using AutoMapper;
using CollectionsAndLinq.BL.Context;
using CollectionsAndLinq.BL.Interfaces;
using CollectionsAndLinq.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsAndLinq.VL.Controllers
{
    public class TaskController
    {
        private readonly IDataProcessingService _dataProcessingService;
        public TaskController()
        {
            _dataProcessingService = new DataProcessingService();
        }

        public async Task ShowTasksCountInProjectsByUserIdAsync(int userId) 
        {
            var output = await _dataProcessingService.GetTasksCountInProjectsByUserIdAsync(userId);

            foreach (var item in output) 
            {
            Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }

        public async Task ShowCapitalTasksByUserIdAsync(int userId) 
        {
            Console.WriteLine("-----------------------------------Capital Tasks By User Id-------------------------------------------");
            var output = await _dataProcessingService.GetCapitalTasksByUserIdAsync(userId);
            foreach (var item in output) 
            {
                Console.WriteLine($"--------------------About Task------------------------\n" +
                    $"UserId:Task id: {item.Id}\n" +
                    $"Task name: {item.Name}\n" +
                    $"Description: {item.Description}\n" +
                    $"State: {item.State}\n" +
                    $"Created at:{item.CreatedAt}\n" +
                    $"Finished at: {item.FinishedAt}");
                Console.WriteLine("--------------------------------------------------------");
            }
        }
    }
}
