using ElasticSearchTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTest.Repository.Contracts
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(int id);
        Task<IEnumerable<T>> GetForDepartment(int DepId);
        Task<T> Add(T item);
        Task Remove(int id);
        Task<bool> Update(T item);
    }
}
