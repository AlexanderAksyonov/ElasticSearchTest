using ElasticSearchTest.Models;
using ElasticSearchTest.Repository.Contracts;
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

namespace ElasticSearchTest.Repository
{
    public class PersonRepository : IRepository<Person>
    {
        /*  i use this reqest for create mapping for Person in Kibana:
        put est
        {
              "mappings": {
              "Person": {
                "properties": {
                  "DepartmentID": {
                    "type": "keyword"
                  },
                  "Id": {
                    "type": "long"
                  },
                  "Name": {
                    "type": "keyword"
                  },
                  "Surname": {
                    "type": "keyword"
                  }
                }
              }
            }
        }
         */

        private static PersonRepository repo = new PersonRepository();
        public static IRepository<Person> getRepository()
        {
            return repo;
        }


        private static Uri uri = new Uri("http://localhost:9200");

        private async Task<IEnumerable<Person>> GetPersonFromElastic (string requestText = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = uri;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string requestString = "est/Person/_search" + (requestText.Length > 1 ? String.Format("?q={0}", requestText) : "");
                    HttpResponseMessage response = await client.GetAsync (requestString, HttpCompletionOption.ResponseContentRead);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> respContent = response.Content.ReadAsStringAsync();
                        respContent.Wait();

                        JObject jObject = JObject.Parse(respContent.Result);

                        List<Person> persons = new List<Person>();

                        foreach (JToken tok in jObject.SelectTokens("$.._source"))
                        {
                            persons.Add(tok.ToObject<Person>());
                        }
                        return persons;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: "+e.Message);
            }
            return null;
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            return await GetPersonFromElastic();
        }
        public async Task<Person> Get(int id)
        {
            return (await GetPersonFromElastic(String.Format("Id:{0}", id))).FirstOrDefault();
        }

        public async Task<IEnumerable<Person>> GetForDepartment(int DepId)
        {
            return (await GetPersonFromElastic(String.Format("DepartmentID:{0}", DepId)));
        }

        public async Task<Person> Add(Person item)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = uri;

                    var q = new { size = 0, aggs = new { max_id = new { max = new { field = "Id"} } } };
                    string content = JsonConvert.SerializeObject(q);

                    HttpContent query = new StringContent(content, Encoding.UTF8, "application/json");
                    HttpResponseMessage maxIDresponse = await client.PostAsync("est/Person/_search", query);
                    int maxID = 0;
                    if (maxIDresponse.IsSuccessStatusCode)
                    {
                        Task<string> respContent = maxIDresponse.Content.ReadAsStringAsync();
                        respContent.Wait();

                        JObject jObject = JObject.Parse(respContent.Result);

                        maxID = jObject.SelectToken("$..value").ToObject<int>();

                    }

                    item.Id = maxID+1;
                    HttpResponseMessage response = client.PostAsJsonAsync("est/Person/", item).Result;
                    return item;
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }
        public async Task Remove(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = uri;
                    var q = new { query = new { term = new { Id = id } } };
                    string content = JsonConvert.SerializeObject(q);

                    HttpContent query = new StringContent(content, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("est/Person/_delete_by_query", query);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public async Task<bool> Update(Person item)
        {
            try
            {
                var pers = await GetPersonFromElastic(String.Format("Id:{0}", item.Id));
                if (!pers.Any())
                {
                    return false;
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = uri;

                    var q = new { query = new { term = new { Id = item.Id } },
                                    script = new
                                    {
                                        inline = String.Format("ctx._source.Id = {0}; ctx._source.Name = '{1}';ctx._source.Surname = '{2}';ctx._source.DepartmentID = {3}",
                                                    item.Id, item.Name, item.Surname, item.DepartmentID),
                                        lang = "painless"
                                    }
                                };
                    string content = JsonConvert.SerializeObject(q);

                    HttpContent query = new StringContent(content, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("est/Person/_update_by_query", query);
                    return response.StatusCode==System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }
    }
}