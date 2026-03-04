using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    public class EmployeeManager
    {
        public List<Employee> Employees;
        public EmployeeManager()
        {
            Employees = new List<Employee>();
        }


        public void AddEmployee(Employee emp)
        {
<<<<<<< HEAD
            Employees.Add(emp);

            Console.WriteLine("Employee added successfully");
=======
               Employees.Add(emp);
        }
        public void UpdateEmployee(int id, string name, string desc, JobType type, int salary)
        {
            //implement
>>>>>>> 7ad65a4682c0733fa782a8609ef21a063fca0356
        }

        public List<Employee> SearchBy(string searchBy, string value)
        {
<<<<<<< HEAD
            Console.WriteLine(" Employees List");

            if (employees.Count == 0)
            {
                Console.WriteLine("No employees found ");
                return;
            }

            foreach (var emp in employees)
            {
                Console.WriteLine(emp);
            }

            Console.WriteLine("---------------------------------");
=======
            return searchBy.ToLower() switch
            {
                "id" => Employees.Where(e => e.Id.ToString() == value).ToList(),
                "name" => Employees.Where(e => e.Name.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList(),
                "salary" => Employees.Where(e => e.Salary.ToString() == value).ToList(),
                _ => new List<Employee>()
            };
>>>>>>> 7ad65a4682c0733fa782a8609ef21a063fca0356
        }

        public List<Employee> FilterByTitle(JobType title)
        {
            //implement
        }

        public List<Employee> GetSortedList(string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "name" => Employees.OrderBy(e => e.Name).ToList(),
                "salary" => Employees.OrderByDescending(e => e.Salary).ToList(),
                _ => Employees.ToList()
            };

        }
    }
}
