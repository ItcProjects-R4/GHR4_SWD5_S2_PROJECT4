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
            Console.WriteLine("1- Add | 2- Display All | 3- Update | 4- Search | 5- Filter");
            Console.WriteLine("6- Sort | 7- Count Employees  | 8- Clear  | 9- Exit");
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
                    List<Employee> employees = manager.GetAllEmployees();
                    if (!employees.Any())
                        Console.WriteLine("No employees found.");

                    employees.ForEach(e => Console.WriteLine(e + "\n-------------------\n"));
                    break;

                case "3":
                    Console.Write("Enter ID of employee to update: ");
                    int uId = int.Parse(Console.ReadLine());
                    Console.Write("Enter new Name: ");
                    string uName = Console.ReadLine();
                    Console.Write("Enter new Description: ");
                    string uDesc = Console.ReadLine();
                    Console.Write("Enter new Salary: ");
                    int uSalary = int.Parse(Console.ReadLine());

                    int uType;

                    while (true)
                    {
                        Console.Write("Enter new Job Type (0:SE,1:PM,2:Des,3:DS): ");
                        uType = int.Parse(Console.ReadLine());

                        if (Enum.IsDefined(typeof(JobType), uType))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Job Type, please enter again.");
                        }
                    }

                    manager.UpdateEmployee(uId, uName, uDesc, (JobType)uType, uSalary);
                    Console.WriteLine("Employee updated successfully");

                    break;

                case "4":
                    Console.Write("Search by (id/name/salary): ");
                    string sBy = Console.ReadLine();

                    Console.Write("Enter value: ");
                    string sVal = Console.ReadLine();

                    var searchResults = manager.SearchBy(sBy, sVal);

                    if (searchResults.Count == 0)
                        Console.WriteLine("No records found.");
                    else
                        foreach (var e in searchResults)
                            Console.WriteLine(e);

                    break;


                case "5":
                    Console.Write("Enter Title ID (0:SE, 1:PM, 2:Des, 3:DS): ");
                    int fId = int.Parse(Console.ReadLine());

                    var filtered = manager.FilterByTitle((JobType)fId);

                    if (filtered.Count == 0)
                        Console.WriteLine("No records found.");
                    else
                        foreach (var e in filtered)
                            Console.WriteLine(e);

                    break;

                case "6":
                    Console.Write("Sort by (name/salary): ");
                    string sortBy = Console.ReadLine();

                    var sorted = manager.GetSortedList(sortBy);

                    if (sorted.Count == 0)
                        Console.WriteLine("No records found.");
                    else
                        foreach (var e in sorted)
                            Console.WriteLine(e);

                    break;

                case "7":
                    Console.WriteLine("Total number of employees is: " + manager.CountEmployees());
                    break;

                case "8":
                    manager.Clear();
                    Console.WriteLine("Employees cleared successfully.");
                    break;

                case "9":
                    running = false;
                    break;
            }

        }

    }
}