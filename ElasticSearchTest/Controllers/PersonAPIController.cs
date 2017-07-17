using ElasticSearchTest.Models;
using ElasticSearchTest.Repository;
using ElasticSearchTest.Repository.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace ElasticSearchTest.Controllers
{
    public class PersonAPIController : ApiController
    {
        private IRepository<Person> repo = PersonRepository.getRepository();

        public async Task<IEnumerable<Person>> GetAllPerson()
        {
            return await repo.GetAll();
        }
        public async Task<Person> GetPerson(int id)
        {

            return await repo.Get(id);
        }

        [HttpGet]
        [ActionName("GetForDepartment")]
        public async Task<IEnumerable<Person>> GetForDepartment(int parametr)
        {

            return await repo.GetForDepartment(parametr);
        }

        [HttpPost]
        public async Task<Person> CreatePerson(Person item)
        {
            return await repo.Add(item);
        }
        [HttpPut]
        public async Task<bool> UpdatePerson(Person item)
        {
            return await repo.Update(item);
        }
        public void DeletePerson(int id)
        {
            repo.Remove(id);
        }
    }
}