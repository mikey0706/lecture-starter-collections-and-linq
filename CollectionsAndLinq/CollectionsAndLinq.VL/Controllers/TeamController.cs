using CollectionsAndLinq.BL.Interfaces;
using CollectionsAndLinq.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsAndLinq.VL.Controllers
{
    public class TeamController
    {
        private readonly IDataProcessingService _dataProcessingService;
        public TeamController()
        {
            _dataProcessingService = new DataProcessingService();
        }

        public async Task ShowSortedTeamByMembersWithYearAsync(int year) 
        {
            var data = await _dataProcessingService.GetSortedTeamByMembersWithYearAsync(year);
            foreach (var item in data) 
            {
            Console.WriteLine($"TeamId: {item.Id}\nTeam Name: {item.Name}\n");
                foreach (var user in item.Members)
                {
                    Console.WriteLine($"Id: {user.Id}\nFirst Name: {user.FirstName}\nLast Name: {user.LastName}\nEmail: {user.Email}\nBirthday: {user.BirthDay}\nRegistration Date: {user.RegisteredAt}\n");
                }
            }
        }
    }
}
