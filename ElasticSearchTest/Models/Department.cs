using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nest;

namespace ElasticSearchTest.Models
{
    public class Department
    {
        [Nested(Name = "ID")]
        public int ID { get; set; }
        [Nested(Name = "Name")]
        public string Name { get; set; }
        [Nested(Name = "ParentId")]
        public int ParentId { get; set; }
    }
}