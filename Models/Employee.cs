using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace MVCrestAPI.Models
{
    public class Employee
    {
        
        public string Id { get; set; }
        
        public string EmployeeName { get; set; }
       
        public string StarTimeUtc { get; set; }
        
        public string EndTimeUtc { get; set; }
        
        public string EntryNotes { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? DeletedOn { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int DaysWorked { get; set; }
    }

    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set;}
    }
        

}