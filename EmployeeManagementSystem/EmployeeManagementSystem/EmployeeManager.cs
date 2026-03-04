using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    public class EmployeeManager
    {
        private List<Employee> employees = new List<Employee>();

        public void AddEmployee(Employee emp)
        {
            employees.Add(emp);

            Console.WriteLine("Employee added successfully");
        }

        public void ViewAllEmployees()
        {
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
        }

        public void SearchEmployee(int empId)
        {
           
        }
        
        public void FilterEmployees()
        {
           
        }
    }
}
