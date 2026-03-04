using EmployeeManagementSystem;

internal class Program
{
    private static void Main(string[] args)
    {
        EmployeeManager manager = new EmployeeManager();
        manager.AddEmployee(new Employee { Id = 1, Name = "Alice", Description = "Senior SE", Salary = 90000, JobType = JobType.SoftwareEngineer });
        manager.AddEmployee(new Employee { Id = 2, Name = "Bob", Description = "PM", Salary = 90000, JobType = JobType.ProductManager });
        manager.AddEmployee(new Employee { Id = 3, Name = "Charlie", Description = "Designer", Salary = 70000, JobType = JobType.Designer });
        bool running = true;

        Console.WriteLine("=== Welcome to Employee Management System ===");

        while (running)
        {
            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("1- Add | 2- Display Info | 3- Update | 4- Remove | 5- Clear");
            Console.WriteLine("6- Count | 7- Search | 8- Display All | 9- Sort | 10- Filter | 11- Exit");
            Console.Write("Enter Your Request: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter ID: "); int id = int.Parse(Console.ReadLine());
                    Console.Write("Enter Name: "); string name = Console.ReadLine();
                    Console.Write("Enter Description: "); string desc = Console.ReadLine();
                    Console.Write("Enter Salary: "); int salary = int.Parse(Console.ReadLine());
                    Console.Write("Enter Job Type (0:SE, 1:PM, 2:Des, 3:DS): ");
                    int titleIdx = int.Parse(Console.ReadLine());

                    manager.AddEmployee(new Employee
                    {
                        Id = id,
                        Name = name,
                        Description = desc,
                        Salary = salary,
                        JobType = (JobType)titleIdx
                    });
                    Console.WriteLine("Employee Added Successfully!");
                    break;

                case "2":
                  //implement display info logic here
                 
                case "3":
                    //implement update logic here
                    break;

                case "4":
                    //implement remove logic here
                    break;

                case "5":
                    manager.Employees.Clear();
                    Console.WriteLine("Cleared.");
                    break;

                case "6":
                    Console.WriteLine($"Total: {manager.Employees.Count}");
                    break;

                case "7":
                    Console.Write("Search by (id/name/salary): ");
                    string sBy = Console.ReadLine();
                    Console.Write("Enter value: ");
                    string sVal = Console.ReadLine();
                    DisplayList(manager.SearchBy(sBy, sVal));
                    break;

                case "8":
                    DisplayList(manager.Employees);
                    break;

                case "9":
                    Console.Write("Sort by (name/salary): ");
                    DisplayList(manager.GetSortedList(Console.ReadLine()));
                    break;

                case "10":
                    Console.Write("Enter Title ID (0:SE, 1:PM, 2:Des, 3:DS): ");
                    int fId = int.Parse(Console.ReadLine());
                    DisplayList(manager.FilterByTitle((JobType)fId));
                    break;

                case "11":
                    running = false;
                    break;
            }

        }
        static void DisplayList(List<Employee> list)
        {
            if (list.Count == 0) Console.WriteLine("No records found.");
            else list.ForEach(e => Console.WriteLine(e));
        }

    }
}