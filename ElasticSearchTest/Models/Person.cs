using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticSearchTest.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int DepartmentID { get; set; }
    }
}