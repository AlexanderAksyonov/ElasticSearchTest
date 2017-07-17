using ElasticSearchTest.Models;
using ElasticSearchTest.Repository.Contracts;
using ElasticSearchTest.Reposytory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ElasticSearchTest.Controllers
{
    public class DepartmentAPIController : ApiController
    {

        public async Task<IEnumerable<Department>> GetAll()
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.GetAll();
        }

        public async Task<Department> GetDepartmrnt(int id)
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.Get(id);
        }

        [HttpGet]
        [ActionName("GetChildDepartment")]
        public async Task<IEnumerable<Department>> GetChildDepartment(int parametr)
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.GetChildDepartment(parametr);
        }

        [HttpGet]
        [ActionName("GetRootDepartment")]
        public async Task<IEnumerable<Department>> GetRootDepartment(int parametr = 0) //убрать parametr и сделать вменяемый роутинг...
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.GetRootDepartment();
        }

        [HttpPost]
        public async Task<bool> CreateDepartment(Department item)
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.Add(item);
        }
    /*    [HttpPut]
        public async Task<bool> UpdateDepartment(Department item)
        {
            DepartmentRepository repo = new DepartmentRepository();
            return await repo.Update(item);
        }*/
        public void DeleteDepartment(int id)
        {
            DepartmentRepository repo = new DepartmentRepository();
            repo.Remove(id);
        }
    }
}
