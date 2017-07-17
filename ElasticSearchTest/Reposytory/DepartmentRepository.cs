using ElasticSearchTest.Models;
using ElasticSearchTest.Repository.Contracts;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ElasticSearchTest.Reposytory
{
    public class DepartmentRepository
    {
        /* i use this reqest for create mapping for Department in Kibana:
        put est/_mapping/Department
        {
                "properties": {
                    "ID": {
                    "type": "long"
                    },
                    "ParentId": {
                    "type": "keyword"
                    },
                    "Name": {
                    "type": "keyword"
                    }
                }
        }
*/
        private static Uri uri = new Uri("http://localhost:9200");

        private async Task<IEnumerable<Department>> GetDepartmentFromElastic(QueryContainerDescriptor<Department> query = null)
        {
            try
            {
                var settings = new ConnectionSettings(uri).InferMappingFor<Department>(i => i.IndexName("est").TypeName("Department")).EnableDebugMode();


                var client = new ElasticClient(settings);

                var asyncResp = await client.SearchAsync<Department>(s => s.Query(q => q = query)); // = query));

                var rezult = asyncResp.Documents;
                return rezult;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
            return null;
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            return await GetDepartmentFromElastic();
        }
        public async Task<Department> Get(int id)
        {
            QueryContainerDescriptor<Department> descript = new QueryContainerDescriptor<Department>();
            descript.Match(m => m.Field(f => f.ID).Query(id.ToString()));
            return (await GetDepartmentFromElastic(descript)).FirstOrDefault();
        }

        public async Task<IEnumerable<Department>> GetChildDepartment(int DepId)
        {
            QueryContainerDescriptor<Department> descript = new QueryContainerDescriptor<Department>();
            descript.Match(m => m.Field(f => f.ParentId).Query(DepId.ToString()));
            return (await GetDepartmentFromElastic(descript));
        }

        public async Task<IEnumerable<Department>> GetRootDepartment()
        {
            QueryContainerDescriptor<Department> descript = new QueryContainerDescriptor<Department>();
            //ParentId = 0 or ParentId is not exist
            descript.Bool(b => b.Should(s => s.Match(m => m.Field(f => f.ParentId).Query("0")), s=>s.Bool(q => q.MustNot(m => m.Exists(e => e.Field(f => f.ParentId))))));
            return (await GetDepartmentFromElastic(descript)); ;
        }

        public async Task<bool> Add(Department item)
        {
            try
            {
                var settings = new ConnectionSettings(uri).InferMappingFor<Department>(i => i.IndexName("est").TypeName("Department")).EnableDebugMode();

                var client = new ElasticClient(settings);

                var asyncResp = await client.SearchAsync<Department>(s => s.Size(0).Aggregations(aggs => aggs.Max("max_id", avg => avg.Field(p => p.ID))));
                int maxID = (int)asyncResp.Aggs.Max("max_id").Value;
                item.ID = maxID + 1;
                var resp2 = client.IndexAsync(item, i => i.Index("est"));
                return resp2.Result.Created;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public async Task Remove(int id)
        {
            try
            {
                var settings = new ConnectionSettings(uri).InferMappingFor<Department>(i => i.IndexName("est").TypeName("Department")).EnableDebugMode();

                var client = new ElasticClient(settings);

                var asyncResp = client.DeleteByQuery<Department>(d => d.Query(q => q.Term(t => t.Field("ID").Value(id))));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}