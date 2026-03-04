using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Salary { get; set; }
        public JobType JobType { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}\nName: {Name}\nSalary: {Salary}\nJob type: {JobType}\nDescription: {Description}";
        }
    }
}
