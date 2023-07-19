using CollectionsAndLinq.BL.Models.Projects;
using CollectionsAndLinq.VL;
using CollectionsAndLinq.VL.Controllers;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace Program
{
    class Program
    {
        private static TaskController _taskController;
        private static ProjectController _projectController;
        private static TeamController _teamController;
        private static UsersController _usersController;

        static async Task Main(string[] args)
        {
            int opt = 100;

            while (opt != 0)
            {
                Console.WriteLine("Select your action by printing specific number.");
                Console.WriteLine("1 - Projects controller;");
                Console.WriteLine("2 - Tasks controller;");
                Console.WriteLine("3 - Teams controller;");
                Console.WriteLine("4 - Users controller;");
                Console.WriteLine("0 - Exit;");

                opt = Convert.ToInt32(Console.ReadLine());

                switch (opt) 
                {
                    case 1: 
                        {
                            await ViewProjectInfo();
                            break;
                        }
                    case 2: 
                        {
                            await ViewTasksInfo(); 
                            break;
                        }
                    case 3: 
                        {
                            await ViewTeamInfo();
                            break;
                        }
                    case 4: 
                        {
                            await ViewUserInfo();
                            break;
                        }
                    case 0: 
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }

            Console.ReadLine();
        }
        public static async Task ViewTeamInfo() 
        {
            _teamController = new TeamController();
            Console.WriteLine("Enter year");
            int year = Convert.ToInt32(Console.ReadLine());
            await _teamController.ShowSortedTeamByMembersWithYearAsync(year);
        }
        public static async Task ViewTasksInfo() 
        {
            _taskController = new TaskController();

            int opt = 100;

            while (opt != 0)
            {
                Console.WriteLine("Select your action by printing specific number.");
                Console.WriteLine("1 - Tasks Count In Projects By User Id;");
                Console.WriteLine("2 - Capital Tasks By User Id;");
                Console.WriteLine("0 - Exit;");

                opt = Convert.ToInt32(Console.ReadLine());
                switch (opt) 
                {
                    case 1: 
                        {
                            Console.WriteLine("Enter user's Id");
                            int id = Convert.ToInt32(Console.ReadLine());
                            await _taskController.ShowTasksCountInProjectsByUserIdAsync(id);
                            break;
                        }
                    case 2: 
                        {
                            Console.WriteLine("Enter user's Id");
                            int id = Convert.ToInt32(Console.ReadLine());
                            await _taskController.ShowCapitalTasksByUserIdAsync(id);
                            break;
                        }
                    case 0:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }
        }

        public static async Task ViewUserInfo()
        {
            _usersController = new UsersController();
            int opt = 100;

            while (opt != 0)
            {
                Console.WriteLine("Select your action by printing specific number.");
                Console.WriteLine("1 - Sorted Users With Sorted Tasks;");
                Console.WriteLine("2 - User Info;");
                Console.WriteLine("0 - Exit;");

                opt = Convert.ToInt32(Console.ReadLine());
                switch (opt)
                {
                    case 1:
                        {
                            await _usersController.ShowSortedUsersWithSortedTasksAsync();
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Enter user's Id");
                            int id = Convert.ToInt32(Console.ReadLine());
                            await _usersController.ShowUserInfoAsync(id);
                            break;
                        }
                    case 0:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
            }
        }

        public static async Task ViewProjectInfo() 
        {
            _projectController = new ProjectController();

            int opt = 100;

            while (opt != 0) 
            {
                Console.WriteLine("Select your action by printing specific number.");
                Console.WriteLine("1 - Projects By Team Size;");
                Console.WriteLine("2 - Projects Info;");
                Console.WriteLine("3 - Sort And Filter Page Of Projects;");
                Console.WriteLine("0 - Exit;");

                opt = Convert.ToInt32(Console.ReadLine());

                if (opt == 1)
                {
                    Console.WriteLine("Enter the size of a team");
                    int size = Convert.ToInt32(Console.ReadLine());
                    await _projectController.ShowProjectsByTeamSizeAsync(size);
                }
                else
                if (opt == 2)
                {
                    await _projectController.ShowProjectsInfoAsync();
                }
                else
                if (opt == 3)
                {
                    Console.WriteLine("Enter the size of a page");
                    int size = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter page number");
                    int number = Convert.ToInt32(Console.ReadLine());

                    var pages = (number == 0 & size == 0) ? null : new PageModel(size,number);

                    Console.WriteLine("Enter Name");
                    string name = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Enter Description");
                    string desc = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Enter Author First Name");
                    string firstName = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Enter Author Last Name");
                    string lastName = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Enter Team Name");
                    string teamName = Convert.ToString(Console.ReadLine());

                    var filter = new FilterModel(name,desc,firstName,lastName,teamName);

                    Console.WriteLine("Chose sorting property:\n" +
                        "0- By Name;\n" +
                        "1- By Description\n" +
                        "2- By Deadline\n" +
                        "3- By Created At\n" +
                        "4- By Task Count\n" +
                        "5- By Author First Name\n" +
                        "6- By Author Last Name\n" +
                        "7-By Team Name ");
                    
                    int sortProperty = Convert.ToInt32(Console.ReadLine());
                    var sorting = new SortingProperty();

                    switch (sortProperty) 
                    {
                        case 0: 
                            {
                                sorting = SortingProperty.Name; 
                                break;
                            }
                        case 1: 
                            {
                                sorting = SortingProperty.Description; 
                                break;
                            }
                        case 2: 
                            {
                                sorting = SortingProperty.Deadline;
                                break;
                            }
                        case 3: 
                            {
                                sorting = SortingProperty.CreatedAt;
                                break;
                            }
                        case 4: 
                            {
                                sorting = SortingProperty.TasksCount;
                                break;
                            }
                        case 5: 
                            {
                                sorting = SortingProperty.AuthorFirstName;
                                break;
                            }
                        case 6: 
                            {
                                sorting = SortingProperty.AuthorLastName;
                                break;
                            }
                        case 7: 
                            {
                                sorting = SortingProperty.TeamName;
                                break;
                            }
                    }
                    Console.WriteLine("Chose the Sorting Order:\n" +
                        "1- Ascending\n" +
                        "2- Descending");
                    int sortOrder = Convert.ToInt32(Console.ReadLine());
                    var order = new SortingOrder();
                    switch (sortOrder) 
                    {
                        case 1: 
                            {
                                order = SortingOrder.Ascending; 
                                break;
                            }
                        case 2: 
                            {
                                order = SortingOrder.Descending;
                                break;
                            }
                    }
                    await _projectController.ShowSortedFilteredPageOfProjectsAsync(pages, filter, new SortingModel(sorting, order));
                    
                }
                else
                if (opt == 0)
                {
                    Environment.Exit(0);
                }
            }
        }

    }
}
