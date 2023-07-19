using CollectionsAndLinq.BL.Entities;
using CollectionsAndLinq.BL.Interfaces;
using Newtonsoft.Json;


namespace CollectionsAndLinq.BL.Context
{
    public class DataProvider : IDataProvider
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await GetDataAsync<Project>("projects");
        }

        public async Task<List<Entities.Task>> GetTasksAsync()
        {
            return await GetDataAsync<Entities.Task>("tasks");
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            return await GetDataAsync<Team>("teams");
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await GetDataAsync<User>("users");
        }

        private async Task<List<T>> GetDataAsync<T>(string routeIndex) 
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"https://bsa-dotnet.azurewebsites.net/api/{routeIndex}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<T>>(responseBody);
            }
            catch (HttpRequestException e) 
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
